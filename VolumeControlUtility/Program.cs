using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GlobalHotkeys;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Microsoft.Extensions.Logging;
using Microsoft.Windows.Sdk;


namespace VCU_TEMP
{
    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    partial interface IMMDevice
    {
        void Activate(Guid iid, uint dwClsCtx, in PROPVARIANT pActivationParams, out object ppInterface);
        void OpenPropertyStore(uint stgmAccess, out IPropertyStore ppProperties);
        unsafe void GetId(PWSTR* ppstrId);
        unsafe void GetState(uint* pdwState);
    }
}


namespace VolumeControlUtility
{

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [System.Runtime.InteropServices.DllImport("user32")]
        public static extern
            bool GetMessage(ref Message lpMsg, IntPtr handle, uint mMsgFilterInMain, uint mMsgFilterMax);

        [System.Runtime.InteropServices.DllImport("user32")]
        public static extern
            bool PostThreadMessage(int threadId, int msg, IntPtr wParam, IntPtr lParam);


        public static ProgramGroupManager PGM = new ProgramGroupManager();
        public static AudioSessionManager ASM = new AudioSessionManager();
        private static SecureJsonSerializer<ProgramGroupManagerData> pgmDataFile = new SecureJsonSerializer<ProgramGroupManagerData>("pgmSave.json");

        internal static SecureJsonSerializer<ProgramGroupManagerData> PgmDataFile { get => pgmDataFile; set => pgmDataFile = value; }

        public static WebSocket ws;

        static string getGroups()
        {
            List<ProgramGroupData> groupNames = new List<ProgramGroupData>();
            foreach (ProgramGroup group in PGM.programGroups)
            {
                groupNames.Add(group.generateProgramGroupData());
            }
            return JsonConvert.SerializeObject(groupNames);
        }
        static string createGroup(string newGroup)
        {
            PGM.createProgramGroup(newGroup);
            return newGroup;
        }
        static string deleteGroup(int index)
        {
            PGM.removeProgramGroup(index);
            return "";
        }

        static string handleGroups(HttpListenerRequest request, string resourceId, int parentThreadId)
        {
            JObject json = new JObject();
            if (request.HttpMethod == "PUT" || request.HttpMethod == "POST")
            {
                StreamReader contentStream = new StreamReader(request.InputStream);
                string jsonString = contentStream.ReadToEnd();
                json = JsonConvert.DeserializeObject<JObject>(jsonString);
            }

            switch (request.HttpMethod)
            {
                case "GET":
                    return getGroups();
                case "PUT":
                    Int32 index = Int32.Parse(resourceId);
                    ProgramGroup pg = PGM.getProgramGroup(index);

                    pg.rename(json["newName"].ToString());
                    return "name updated";
                case "POST":
                    return createGroup(json["groupName"].ToString());
                case "DELETE":
                    // send to main thread
                    Console.WriteLine("deleting group, sending to thread " + parentThreadId + "(main thread)");
                    Int32 payload = Int32.Parse(resourceId);
                    GCHandle payload_handle = GCHandle.Alloc(payload);
                    IntPtr parameter = (IntPtr)payload_handle;
                    PostThreadMessage(parentThreadId, 69, parameter, IntPtr.Zero);
                    return "";
            }
            return "not handled";
        }

        static string getPrograms(string groupName)
        {
            ProgramGroup pg = PGM.getProgramGroup(groupName);
            return JsonConvert.SerializeObject(pg.audioSessions);
        }

        static string addProgram(string groupName, string programName)
        {
            ProgramGroup pg = PGM.getProgramGroup(groupName);
            pg.audioSessions.Add(programName);
            pg.nonLoadedAudioSessions.Add(programName);
            pg.updateActiveSessions();
            return "";
        }

        static string removeProgram(string groupName, string programName)
        {
            ProgramGroup pg = PGM.getProgramGroup(groupName);
            pg.removeAudioSession(programName);
            return "";
        }

        static string handleGroupPrograms(HttpListenerRequest request, string resourceId, string subResourceId)
        {
            StreamReader contentStream = new StreamReader(request.InputStream);
            string jsonString = contentStream.ReadToEnd();
            JObject json = JsonConvert.DeserializeObject<JObject>(jsonString);

            switch (request.HttpMethod)
            {
                case "GET":
                    return getPrograms(resourceId);
                case "POST":
                    return addProgram(resourceId, json["programName"].ToString());
                case "DELETE":
                    return removeProgram(resourceId, subResourceId);
            }
            return "not handled";
        }

        static string handleGroupVolume(HttpListenerRequest request, string resourceId)
        {
            ProgramGroup pg = PGM.getProgramGroup(resourceId);

            StreamReader contentStream = new StreamReader(request.InputStream);
            string jsonString = contentStream.ReadToEnd();
            JObject json = JsonConvert.DeserializeObject<JObject>(jsonString);
               

            switch (request.HttpMethod)
            {
                case "GET":
                    return JsonConvert.SerializeObject(pg.getVolume()); ;
                case "PUT":
                    pg.setVolume(json["volume"].ToObject<int>());
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(getGroups());
                    ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    return "volume set";
            }
            return "not handled";
        }

        static string handleGroupShortcuts(HttpListenerRequest request, string resourceId, int parentThreadId)
        {
            ProgramGroup pg = PGM.getProgramGroup(resourceId);

            StreamReader contentStream = new StreamReader(request.InputStream);
            string jsonString = contentStream.ReadToEnd();
            JObject json = JsonConvert.DeserializeObject<JObject>(jsonString);


            switch (request.HttpMethod)
            {
                case "GET":
                    JObject shortcuts = new JObject();
                    shortcuts["volumeUp"] = pg.getVolumeUpHotkey();
                    shortcuts["volumeDown"] = pg.getVolumeDownHotkey();
                    shortcuts["mods"] = JsonConvert.SerializeObject(pg.mods);
                    return JsonConvert.SerializeObject(shortcuts);
                case "PUT":
                    Console.WriteLine("setting hotkey");
                    HotkeyPayload payload = new HotkeyPayload(resourceId, json["volumeUp"].ToObject<string>(), json["volumeDown"].ToObject<string>(), json["mods"].ToObject<List<string>>());
                    GCHandle payload_handle = GCHandle.Alloc(payload);
                    IntPtr parameter = (IntPtr)payload_handle;

                    PostThreadMessage(parentThreadId, 42, parameter, IntPtr.Zero);
                    return "shortcut set";
            }
            return "not handled";
        }

        static string handleSystem(HttpListenerRequest request, string resourceId)
        {
            StreamReader contentStream = new StreamReader(request.InputStream);
            string jsonString = contentStream.ReadToEnd();
            JObject json = JsonConvert.DeserializeObject<JObject>(jsonString);


            switch (request.HttpMethod)
            {
                case "GET":
                    List<string> programs = new List<string>();
                    foreach (AudioSessionV2 session in ASM.getActiveAudioSessions())
                    {
                        programs.Add(session.Process.ProcessName);
                    }
                    return JsonConvert.SerializeObject(programs);
                case "POST":
                    if (json["action"].ToObject<string>() == "save")
                    {
                        SecureJsonSerializer<ProgramGroupManagerData> pgmDataFile = new SecureJsonSerializer<ProgramGroupManagerData>("pgmSave.json");
                        pgmDataFile.Save(PGM.generateProgramGroupManagerData());
                        return "saved";
                    }
                    else if (json["action"].ToObject<string>() == "default_volume")
                    {
                        foreach (ProgramGroup group in PGM.programGroups)
                        {
                            group.setVolume(100);
                        }
                        return "group volumes reset";
                    }
                    break;
            }
            return "not handled";
        }
        static void HotkeyProc(HotkeyInfo hotkeyInfo)
        {
            //Console.WriteLine("{0} : Hotkey Proc! {1}, {2}{3}", DateTime.Now.ToString("hh:MM:ss.fff"),
            //                                hotkeyInfo.Key, hotkeyInfo.Modifiers, Environment.NewLine);
            foreach (ProgramGroup group in PGM.programGroups)
            {
                group.isThisYourKotkey(hotkeyInfo);
            }
        }

        static void handle_network_requests(int parentThreadId)
        {
            try
            {
                Console.WriteLine("handling network requests for thread " + parentThreadId);
                if (HttpListener.IsSupported)
                {
                    Console.WriteLine("sweet, it works");
                }
                else
                {
                    Console.WriteLine("func, that sucks");
                }    // URI prefixes are required,
                     // for example "http://contoso.com:8080/index/".
                String[] prefixes = { "http://*:4000/api/" };
                if (prefixes == null || prefixes.Length == 0)
                    throw new ArgumentException("prefixes");

                // Create a listener.
                HttpListener listener = new HttpListener();
                // Add the prefixes.
                foreach (string s in prefixes)
                {
                    listener.Prefixes.Add(s);
                }
                listener.Start();
                while (true)
                {
                    Console.WriteLine("Listening...");
                    // Note: The GetContext method blocks while waiting for a request. 
                    //await Task.Yield();
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerRequest request = context.Request;

                    // Obtain a response object.
                    HttpListenerResponse response = context.Response;
                    // Construct a response.
                    byte[] buffer;
                    System.IO.Stream output;
                    if (request.HttpMethod == "OPTIONS")
                    {
                        response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
                        response.AddHeader("Access-Control-Allow-Methods", "GET, PUT, POST, DELETE");
                        response.AddHeader("Access-Control-Max-Age", "1728000");
                        response.AddHeader("Access-Control-Allow-Origin", "*");

                        buffer = System.Text.Encoding.UTF8.GetBytes("");
                        // Get a response stream and write the response to it.
                        response.ContentLength64 = buffer.Length;
                        response.ContentType = "application/json";
                        output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        // You must close the output stream.
                        output.Close();
                        continue;
                    }
                    string responseString = "not implemented";
                    string path = request.RawUrl.Split('?')[0];

                    string[] pathArr = path.Split('/');
                    string apiVersion = pathArr[2];

                    string resource = pathArr.Length > 3 ? pathArr[3] : null;
                    string resourceId = pathArr.Length > 4 ? pathArr[4] : null;

                    string subResource = pathArr.Length > 5 ? pathArr[5] : null;
                    string subResourceId = pathArr.Length > 6 ? pathArr[6] : null;

                    //StreamReader contentStream = new StreamReader(request.InputStream);
                    //string jsonString = contentStream.ReadToEnd();
                    //JObject jsonContent = JsonConvert.DeserializeObject<JObject>(jsonString);
                    if (resource == "groups")
                    {
                        if (subResource == "programs")
                        {
                            responseString = handleGroupPrograms(request, resourceId, subResourceId);
                        }
                        else if (subResource == "volume")
                        {
                            responseString = handleGroupVolume(request, resourceId);
                        }
                        else if (subResource == "shortcuts")
                        {
                            responseString = handleGroupShortcuts(request, resourceId, parentThreadId);
                        }
                        else
                        {
                            responseString = handleGroups(request, resourceId, parentThreadId);
                        }
                    }
                    else if (resource == "system")
                    {
                        responseString = handleSystem(request, resourceId);
                    }

                    // send update after every operation
                    byte[] wsBuffer = System.Text.Encoding.UTF8.GetBytes(getGroups());
                    ws.SendAsync(new ArraySegment<byte>(wsBuffer), WebSocketMessageType.Text, true, CancellationToken.None);

                    buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    // Get a response stream and write the response to it.
                    response.ContentLength64 = buffer.Length;
                    response.ContentType = "application/json";
                    response.AddHeader("Access-Control-Allow-Origin", "*");
                    output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    // You must close the output stream.
                    output.Close();
                }
                listener.Stop();
            }
            catch (Exception exception)
            {
                Console.WriteLine("http task error: " + exception.ToString());
            }
        }

        delegate void HandleNetworkDelegate(int parentThreadId);

        static void handle_messages()
        {
            Console.WriteLine("handling messages");
            Message message = new Message();

            while (GetMessage(ref message, IntPtr.Zero, 0, 0))
            {
                //await Task.Yield();
                Console.WriteLine("msg received");
                int WM_TIMER = 275;
                if (message.Msg == Win32.WM_HOTKEY_MSG_ID)
                {
                    //Console.WriteLine(m.ToString());

                    var hotkeyInfo = HotkeyInfo.GetFromMessage(message);
                    if (hotkeyInfo != null) HotkeyProc(hotkeyInfo);
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(getGroups());
                    ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if (message.Msg == 42)
                {
                    Console.WriteLine("set hotkeys - Roger Roger Bravo Leader");
                    IntPtr parameter = message.WParam;
                    GCHandle handle2 = (GCHandle)parameter;
                    HotkeyPayload payload = (handle2.Target as HotkeyPayload?) ?? new HotkeyPayload("");

                    ProgramGroup pg = PGM.getProgramGroup(payload.resourceId);
                    pg.setVolumeHotkeys(payload.mods, payload.volumeUp, payload.volumeDown, null);
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(getGroups());
                    ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if (message.Msg == 69) // remove program group
                {
                    Console.WriteLine("remove group - Roger Roger Bravo Leader");
                    IntPtr parameter = message.WParam;
                    GCHandle handle2 = (GCHandle) parameter;
                    Int32 index = (Int32) handle2.Target;

                    PGM.removeProgramGroup(index);
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(getGroups());
                    ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if (message.Msg == WM_TIMER)
                {
                    System.Environment.Exit(0);
                }
            }
        }


        static void handle_websocket()
        {
            try
            {
                // wait for websocket connection
                HttpListener listener = new HttpListener();
                listener.Prefixes.Add("http://*:4000/ws/");
                listener.Start();


                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    HttpListenerWebSocketContext wsContext = context.AcceptWebSocketAsync(null).GetAwaiter().GetResult();
                    ws = wsContext.WebSocket;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("websocket task error: " + exception.Message);
            }
        }

        delegate void ManageWebSocketDelegate();


        unsafe public static void DebugMain()
        {
            //Debugger.Launch();
            //Debugger.Break();
            CLSCTX CLSCTX_ALL = CLSCTX.CLSCTX_INPROC_SERVER
                | CLSCTX.CLSCTX_INPROC_HANDLER
                | CLSCTX.CLSCTX_LOCAL_SERVER
                | CLSCTX.CLSCTX_REMOTE_SERVER;

            void* deviceEnumeratorObj;
            PInvoke.CoCreateInstance(
                typeof(MMDeviceEnumerator).GUID,
                null,
                (uint)CLSCTX_ALL,
                typeof(IMMDeviceEnumerator).GUID,
                out deviceEnumeratorObj
            ).ThrowOnFailure();
            IMMDeviceEnumerator* deviceEnumerator = (IMMDeviceEnumerator*)deviceEnumeratorObj;

            IMMDevice* speakers;
            deviceEnumerator->GetDefaultAudioEndpoint(
                EDataFlow.eRender,
                ERole.eMultimedia,
                out speakers
            ).ThrowOnFailure();

            void* audioSessionManager2ptr;
            speakers->Activate(
                typeof(IAudioSessionManager2).GUID,
                (uint)CLSCTX_ALL,
                null,
                out audioSessionManager2ptr
            );
            IAudioSessionManager2* audioSessionManager2 = (IAudioSessionManager2*)audioSessionManager2ptr;

            Guid IID_IAudioSessionManager = typeof(IAudioSessionManager).GUID;

            IAudioSessionEnumerator* sessionEnumerator;
            int count;
            audioSessionManager2->GetSessionEnumerator(out sessionEnumerator).ThrowOnFailure();
            sessionEnumerator->GetCount(out count);
            List<AudioSessionV2> list = new List<AudioSessionV2>();
            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl* ctl = null;

                Guid IID_IAudioSessionControl2 = typeof(IAudioSessionControl2).GUID;
                int result = sessionEnumerator->GetSession(i, out ctl).ThrowOnFailure();
                if (ctl == null)
                    continue;


                void* ctl2Ptr = null;
                ctl->QueryInterface(IID_IAudioSessionControl2, out ctl2Ptr).ThrowOnFailure();
                IAudioSessionControl2* ctl2 = (IAudioSessionControl2*)ctl2Ptr;
                if (ctl2 == null)
                    ctl->Release();
                    continue;

                PWSTR displayName;
                AudioSessionState state;
                uint pid = 0;
                PWSTR sessionId;
                PWSTR sessionInstanceId;

                ctl2->GetDisplayName(out displayName).ThrowOnFailure();
                ctl2->GetState(out state).ThrowOnFailure();
                ctl2->GetProcessId(out pid).ThrowOnFailure();
                ctl2->GetSessionIdentifier(out sessionId).ThrowOnFailure();
                ctl2->GetSessionInstanceIdentifier(out sessionInstanceId).ThrowOnFailure();

                Console.WriteLine("======================================================");
                Console.WriteLine("Audio Session Display Name: " + displayName);
                Console.WriteLine("Audio Session Process Name: " + Process.GetProcessById(checked((int)pid)).ProcessName);
                Console.WriteLine("Audio Session Process Id: " + pid);
                Console.WriteLine("Audio Session Session Id: " + sessionId);
                Console.WriteLine("Audio Session Session Instance Id: " + sessionInstanceId);
                Console.WriteLine("Audio Session State: " + state);

                list.Add(new AudioSessionV2(ctl2));
                ctl->Release();
                ctl2->Release();
            }
            Console.WriteLine("======================================================");
            Console.WriteLine("did a thing");
            deviceEnumerator->Release();
            speakers->Release();
            audioSessionManager2->Release();
            sessionEnumerator->Release();
        }

        //public static void ServiceMain(ILogger eventLog)
        //{
        //    //DebugMain();
        //    eventLog.LogInformation("Service Main Executed");
        //    foreach (AudioDevice device in AudioUtilities.GetAllDevices())
        //    {
        //        eventLog.LogInformation("AudioDevice: " + device.Id + " - " + device.State);
        //    }

        //    foreach (AudioSession session in AudioUtilities.GetAllSessions())
        //    {
        //        if (session.Process != null)
        //        {
        //            eventLog.LogInformation("AudioSessionA: " + session.Process.ProcessName + " - " + session.State);
        //        }
        //        else
        //        {
        //            eventLog.LogInformation("AudioSessionB: " + session.DisplayName + " - " + session.State);
        //        }
        //    }

        //    Console.WriteLine("====================================");
        //    Console.WriteLine("initializing PGM");
        //    Console.WriteLine("====================================");
        //    bool saveFileExists = File.Exists("pgmSave.json");
        //    eventLog.LogInformation("save file found:" + saveFileExists);

        //    if (saveFileExists)
        //    {
        //        eventLog.LogInformation("Initializing PGM - from save file");
        //        PGM = (new ProgramGroupManagerFactory().getProgramGroupManager(pgmDataFile.Load()));
        //        PGM.setLogger(eventLog);
        //        eventLog.LogInformation("Initializing PGM done");
        //    }
        //    else
        //    {
        //        eventLog.LogInformation("Initializing PGM - new instance");
        //        PGM = new ProgramGroupManager();
        //        PGM.setLogger(eventLog);
        //        eventLog.LogInformation("Initializing PGM done");

        //        eventLog.LogInformation("Creating save file...");
        //        SecureJsonSerializer<ProgramGroupManagerData> pgmDataFile = new SecureJsonSerializer<ProgramGroupManagerData>("pgmSave.json");
        //        pgmDataFile.Save(PGM.generateProgramGroupManagerData());
        //        eventLog.LogInformation("Done!");
        //    }
        //    Console.WriteLine("====================================");
        //    Console.WriteLine("initializing PGM done");
        //    Console.WriteLine("====================================");

        //    ManageWebSocketDelegate websocket_delegate = new ManageWebSocketDelegate(handle_websocket);
        //    Task websockets_task = Task.Run(() => websocket_delegate.Invoke());

        //    HandleNetworkDelegate http_requests_delegate = new HandleNetworkDelegate(handle_network_requests);
        //    Task http_requests_task = Task.Run(() => http_requests_delegate.Invoke(Process.GetCurrentProcess().Threads[0].Id));

        //    //HandleMessageDelegate m = new HandleMessageDelegate(handle_messages);
        //    //IAsyncResult ar = m.BeginInvoke(null, null);

        //    handle_messages();

        //    websockets_task.Wait();
        //    http_requests_task.Wait();
        //}


        [STAThread]
        static void Main()
        {
            Console.WriteLine("====================================");
            Console.WriteLine("initializing PGM");
            Console.WriteLine("====================================");
            bool saveFileExists = File.Exists("pgmSave.json");

            if (saveFileExists)
            {
                PGM = (new ProgramGroupManagerFactory().getProgramGroupManager(pgmDataFile.Load()));
            }
            Console.WriteLine("====================================");
            Console.WriteLine("initializing PGM done");
            Console.WriteLine("====================================");

            ManageWebSocketDelegate websocket_delegate = new ManageWebSocketDelegate(handle_websocket);
            Task websockets_task = Task.Run(() => websocket_delegate.Invoke());

            HandleNetworkDelegate http_requests_delegate = new HandleNetworkDelegate(handle_network_requests);
            Task http_requests_task = Task.Run(() => http_requests_delegate.Invoke(Process.GetCurrentProcess().Threads[0].Id));
            
            //HandleMessageDelegate m = new HandleMessageDelegate(handle_messages);
            //IAsyncResult ar = m.BeginInvoke(null, null);

            handle_messages();

            websockets_task.Wait();
            http_requests_task.Wait();
        }
    }
}
