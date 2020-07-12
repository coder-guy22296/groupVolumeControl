using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GlobalHotkeys;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace VolumeControlUtility
{
    static class Program
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
        public static SecureJsonSerializer<ProgramGroupManagerData> pgmDataFile = new SecureJsonSerializer<ProgramGroupManagerData>("pgmSave.json");

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
                    foreach (AudioSession session in ASM.getActiveAudioSessions())
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
                }
                else if (message.Msg == 42)
                {
                    Console.WriteLine("set hotkeys - Roger Roger Bravo Leader");
                    IntPtr parameter = message.WParam;
                    GCHandle handle2 = (GCHandle)parameter;
                    HotkeyPayload payload = (handle2.Target as HotkeyPayload?) ?? new HotkeyPayload("");

                    ProgramGroup pg = PGM.getProgramGroup(payload.resourceId);
                    pg.setVolumeHotkeys(payload.mods, payload.volumeUp, payload.volumeDown, null);
                }
                else if (message.Msg == 69) // remove program group
                {
                    Console.WriteLine("remove group - Roger Roger Bravo Leader");
                    IntPtr parameter = message.WParam;
                    GCHandle handle2 = (GCHandle) parameter;
                    Int32 index = (Int32) handle2.Target;

                    PGM.removeProgramGroup(index);
                }
                else if (message.Msg == WM_TIMER)
                {
                    System.Environment.Exit(0);
                }
            }
        }

        delegate void HandleMessageDelegate();

        [STAThread]
        static void Main()
        {

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);

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
            HandleNetworkDelegate m2 = new HandleNetworkDelegate(handle_network_requests);
            IAsyncResult ar2 = m2.BeginInvoke(Process.GetCurrentProcess().Threads[0].Id, null, null);
            
            //HandleMessageDelegate m = new HandleMessageDelegate(handle_messages);
            //IAsyncResult ar = m.BeginInvoke(null, null);

            handle_messages();

            Thread.Sleep(999999999);
            //ar.AsyncWaitHandle.WaitOne();
            //m.EndInvoke(ar);
           //m2.EndInvoke(ar2);

            //Application.Run(new MainUI());

            //Application.Run(new MainUI());
            /*
            string proceed = "y";
            do
            {
                ConsoleManager.Show();
                Console.Clear();
                Console.WriteLine("0) Create a program group");
                Console.WriteLine("1) Delete a program group");
                Console.WriteLine("2) Add a program to a group");
                Console.WriteLine("3) Show Programs in a group");
                Console.WriteLine("4) Set Volume of a group");
                Console.WriteLine("5) Set the volume of a particular Application");
                Console.WriteLine("6) Quit");
                Console.Write("Enter your selection: ");
                int ans;
                if (!(int.TryParse(Console.ReadLine(), out ans)))
                {
                    ans = 99999;
                }

                switch (ans)
                {
                    case 0:
                        Console.Clear();
                        Console.Write("Enter a name for the Program Group: ");
                        string groupName = Console.ReadLine();
                        PGM.addProgramGroup(groupName);
                        break;
                    case 1:
                        PGM.displayProgramGroups();
                        if (PGM.getNumOfProgramGroups() == 0)
                        {
                            Console.Write("press Enter to continue...");
                            Console.ReadLine();
                            break;
                        }

                        int toRemove;
                        do
                        {
                            Console.WriteLine("Select a Program Group from above to remove: ");
                        } while (!(int.TryParse(Console.ReadLine(), out toRemove)));
                        PGM.removeProgramGroup(toRemove);
                        break;
                    case 2:
                        PGM.displayProgramGroups();
                        if (PGM.getNumOfProgramGroups() == 0)
                        {
                            Console.Write("press Enter to continue...");
                            Console.ReadLine();
                            break;
                        }
                        int groupNum;
                        do
                        {
                            Console.WriteLine("Select a Program Group to add a program to: ");
                        } while (!(int.TryParse(Console.ReadLine(), out groupNum)));
                        ProgramGroup toBeUpdated = PGM.getProgramGroup(groupNum);
                        ASM.updateActiveAudioSessions();
                        ASM.displayActiveAudioSessions();
                        int ASIndex;
                        do
                        {
                            Console.Write("enter application number from above list to add to '"
                                + toBeUpdated.getName() + "' Program Group: ");
                        } while (!(int.TryParse(Console.ReadLine(), out ASIndex)));
                        toBeUpdated.addAudioSession(ASM.getAudioSession(ASIndex));
                        Console.WriteLine(ASM.getAudioSession(ASIndex).Process.ProcessName + " has been added to " + toBeUpdated.getName());
                        Console.Write("press Enter to continue...");
                        Console.ReadLine();
                        break;
                    case 3:
                        PGM.displayProgramGroups();
                        if (PGM.getNumOfProgramGroups() == 0)
                        {
                            Console.Write("press Enter to continue...");
                            Console.ReadLine();
                            break;
                        }
                        do
                        {
                            Console.WriteLine("Select a Program Group to show the programs it contains: ");
                        } while (!(int.TryParse(Console.ReadLine(), out groupNum)));
                        PGM.getProgramGroup(groupNum).displayAudioSessions();
                        Console.Write("press Enter to continue...");
                        Console.ReadLine();
                        break;
                    case 4:
                        PGM.displayProgramGroups();
                        if (PGM.getNumOfProgramGroups() == 0)
                        {
                            Console.Write("press Enter to continue...");
                            Console.ReadLine();
                            break;
                        }
                        do
                        {
                            Console.WriteLine("Select a Program Group to adjust the volume: ");
                        } while (!(int.TryParse(Console.ReadLine(), out groupNum)));
                        PGM.getProgramGroup(groupNum).displayAudioSessions();
                        ProgramGroup toAdjustVol = PGM.getProgramGroup(groupNum);
                        Console.WriteLine("current volume: " + toAdjustVol.getVolume());
                        int vol;
                        do
                        {
                            Console.Write("enter the new volume percentage(0-100): ");
                        } while (!(int.TryParse(Console.ReadLine(), out vol)));
                        toAdjustVol.setVolume(vol);
                        Console.WriteLine("Volume has been Set!");
                        Console.Write("press Enter to continue...");
                        Console.ReadLine();
                        break;
                    case 5:
                        Console.Clear();
                        singleAppVolumeControl();
                        break;
                    case 6:
                        proceed = "n";
                        pgmDataFile.Save(PGM.generateProgramGroupManagerData());
                        break;
                    default:
                        Console.WriteLine("Please enter a valid number from the options above");
                        Console.Write("press Enter to continue...");
                        Console.ReadLine();
                        break;
                }
            } while (proceed != "n");
            */
        }

        public static void singleAppVolumeControl()
        {
            AudioSessionManager ASM = new AudioSessionManager();
            int ans = 99;
            int tPid = 99;
            string proceed = "n";

            ASM.updateActiveAudioSessions();
            ASM.displayActiveAudioSessions();
            do
            {
                Console.Write("enter application number from above list: ");
            } while (!(int.TryParse(Console.ReadLine(), out ans)));
            tPid = ASM.getActiveAudioSessions().ElementAt(ans).ProcessId;
            float vol;
            do
            {
                Console.Write("enter the volume as a percentage(0-100): ");
            } while (!(float.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out vol)));
            VolumeMixer.SetApplicationVolume(tPid, vol);
            Console.WriteLine("Volume has been Set!");
            Console.Write("Press Enter to continue......");
            proceed = Console.ReadLine();
        }
    }
}
