using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Windows.Sdk;

namespace VolumeControlUtility
{
    class AudioUtilitiesV2
    {
        public static CLSCTX CLSCTX_ALL = CLSCTX.CLSCTX_INPROC_SERVER
            | CLSCTX.CLSCTX_INPROC_HANDLER
            | CLSCTX.CLSCTX_LOCAL_SERVER
            | CLSCTX.CLSCTX_REMOTE_SERVER;

        public static Guid CLSID_MMDeviceEnumerator = typeof(MMDeviceEnumerator).GUID;
        public static Guid IID_IMMDeviceEnumerator = typeof(IMMDeviceEnumerator).GUID;
        public static Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
        public static Guid IID_IAudioSessionControl2 = typeof(IAudioSessionControl2).GUID;
        public static Guid IID_ISimpleAudioVolume = typeof(ISimpleAudioVolume).GUID;

        unsafe internal static IMMDevice* GetDefaultSpeaker()
        {
            IMMDeviceEnumerator* deviceEnumerator;
            IMMDevice* speakers;

            void* deviceEnumeratorPtr;
            PInvoke.CoCreateInstance(
                CLSID_MMDeviceEnumerator,
                null,
                (uint)CLSCTX_ALL,
                IID_IMMDeviceEnumerator,
                out deviceEnumeratorPtr
            ).ThrowOnFailure();
            deviceEnumerator = (IMMDeviceEnumerator*)deviceEnumeratorPtr;

            deviceEnumerator->GetDefaultAudioEndpoint(
                EDataFlow.eRender,
                ERole.eMultimedia,
                out speakers
            ).ThrowOnFailure();

            deviceEnumerator->Release();
            return speakers;
        }
        unsafe internal static IAudioSessionManager2* GetAudioSessionManager(IMMDevice* speaker)
        {
            IAudioSessionManager2* audioSessionManager2;

            void* audioSessionManager2Ptr;
            speaker->Activate(
                IID_IAudioSessionManager2,
                (uint)CLSCTX_ALL,
                null,
                out audioSessionManager2Ptr
            );
            audioSessionManager2 = (IAudioSessionManager2*)audioSessionManager2Ptr;

            return audioSessionManager2;
        }

        unsafe public static System.Collections.Generic.List<AudioSessionV2> GetAllSessions()
        {
            IMMDevice* speaker;
            IAudioSessionManager2* audioSessionManager2;
            IAudioSessionEnumerator* sessionEnumerator;
            int audioSessionCount;
            List<AudioSessionV2> list;


            speaker = GetDefaultSpeaker();
            audioSessionManager2 = GetAudioSessionManager(speaker);

            audioSessionManager2->GetSessionEnumerator(out sessionEnumerator).ThrowOnFailure();
            sessionEnumerator->GetCount(out audioSessionCount);

            list = new List<AudioSessionV2>();
            Console.WriteLine("All Audio Sessions: ");
            for (int index = 0; index < audioSessionCount; index++)
            {
                IAudioSessionControl* ctl = null;
                IAudioSessionControl2* ctl2 = null;

                int result = sessionEnumerator->GetSession(index, out ctl).ThrowOnFailure();
                if (ctl == null)
                    continue;

                void* ctl2Ptr;
                ctl->QueryInterface(IID_IAudioSessionControl2, out ctl2Ptr).ThrowOnFailure();
                ctl2 = (IAudioSessionControl2*)ctl2Ptr;
                if (ctl2 == null)
                {
                    ctl->Release();
                    continue;
                }

                PWSTR displayName;
                AudioSessionState state;
                uint processId = 0;
                PWSTR sessionId;
                PWSTR sessionInstanceId;

                ctl2->GetDisplayName(out displayName).ThrowOnFailure();
                ctl2->GetState(out state).ThrowOnFailure();
                ctl2->GetProcessId(out processId).ThrowOnFailure();
                ctl2->GetSessionIdentifier(out sessionId).ThrowOnFailure();
                ctl2->GetSessionInstanceIdentifier(out sessionInstanceId).ThrowOnFailure();

                Console.WriteLine("======================================================");
                Console.WriteLine("Audio Session Display Name: " + displayName);
                Console.WriteLine("Audio Session Process Name: " + Process.GetProcessById(checked((int)processId)).ProcessName);
                Console.WriteLine("Audio Session Process Id: " + processId);
                Console.WriteLine("Audio Session Session Id: " + sessionId);
                Console.WriteLine("Audio Session Session Instance Id: " + sessionInstanceId);
                Console.WriteLine("Audio Session State: " + state);

                list.Add(new AudioSessionV2(ctl2));
                ctl->Release();
                ctl2->Release();
            }
            speaker->Release();
            audioSessionManager2->Release();
            sessionEnumerator->Release();
            Console.WriteLine("======================================================");
            Console.WriteLine("Done listing all audio sessions");
            return list;
        }
    }
}
