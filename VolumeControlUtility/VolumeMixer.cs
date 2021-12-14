using System;
using System.Diagnostics;
using Microsoft.Windows.Sdk;

namespace VolumeControlUtility
{
    public class VolumeMixer
    {
        unsafe public static float? GetApplicationVolume(int pid)
        {
            ISimpleAudioVolume* volume = GetVolumeControl(pid);
            if (volume == null)
                return null;

            float level;
            volume->GetMasterVolume(out level);
            volume->Release();
            return level * 100;
        }

        unsafe public static bool? GetApplicationMute(int pid)
        {
            ISimpleAudioVolume* volume = GetVolumeControl(pid);
            if (volume == null)
                return null;

            BOOL mute;
            volume->GetMute(out mute);
            volume->Release();
            return mute;
        }

        unsafe public static void SetApplicationVolume(int pid, float level)
        {
            ISimpleAudioVolume* volume = GetVolumeControl(pid);
            //Console.WriteLine("vol object pid " + pid + " = "+ volume);
            if (volume == null) { 
                //ConsoleManager.Show();
                Console.WriteLine("processId "+pid+" is not associated with an audio session, Volume not changed!");
                return;
            }
            Guid guid = Guid.Empty;
            volume->SetMasterVolume(level / 100, ref guid);
            volume->Release();
        }

        unsafe public static void SetApplicationMute(int pid, bool mute)
        {
            ISimpleAudioVolume* volume = GetVolumeControl(pid);
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume->SetMute(mute, ref guid);
            volume->Release();
        }

        unsafe private static ISimpleAudioVolume* GetVolumeControl(int pid)
        {
            //CLSCTX CLSCTX_ALL = CLSCTX.CLSCTX_INPROC_SERVER
            //    | CLSCTX.CLSCTX_INPROC_HANDLER
            //    | CLSCTX.CLSCTX_LOCAL_SERVER
            //    | CLSCTX.CLSCTX_REMOTE_SERVER;

            //Guid CLSID_MMDeviceEnumerator = typeof(MMDeviceEnumerator).GUID;
            //Guid IID_IMMDeviceEnumerator = typeof(IMMDeviceEnumerator).GUID;
            //Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            //Guid IID_IAudioSessionControl2 = typeof(IAudioSessionControl2).GUID;
            //Guid IID_ISimpleAudioVolume = typeof(ISimpleAudioVolume).GUID;

            //IMMDeviceEnumerator* deviceEnumerator;
            //IMMDevice* speakers;
            IAudioSessionManager2* audioSessionManager2;
            IAudioSessionEnumerator* sessionEnumerator;
            ISimpleAudioVolume* volumeControl = null;
            int audioSessionCount;

            //void* deviceEnumeratorPtr;
            //PInvoke.CoCreateInstance(
            //    CLSID_MMDeviceEnumerator,
            //    null,
            //    (uint)CLSCTX_ALL,
            //    IID_IMMDeviceEnumerator,
            //    out deviceEnumeratorPtr
            //).ThrowOnFailure();
            //deviceEnumerator = (IMMDeviceEnumerator*) deviceEnumeratorPtr;

            //deviceEnumerator->GetDefaultAudioEndpoint(
            //    EDataFlow.eRender,
            //    ERole.eMultimedia,
            //    out speakers
            //).ThrowOnFailure();

            //void* audioSessionManager2Ptr;
            //speakers->Activate(
            //    IID_IAudioSessionManager2,
            //    (uint)CLSCTX_ALL,
            //    null,
            //    out audioSessionManager2Ptr
            //);
            //audioSessionManager2 = (IAudioSessionManager2*) audioSessionManager2Ptr;
            IMMDevice* speaker = AudioUtilitiesV2.GetDefaultSpeaker();

            audioSessionManager2 = AudioUtilitiesV2.GetAudioSessionManager(speaker);

            audioSessionManager2->GetSessionEnumerator(out sessionEnumerator).ThrowOnFailure();
            sessionEnumerator->GetCount(out audioSessionCount);

            for (int index = 0; index < audioSessionCount; index++)
            {
                IAudioSessionControl* ctl = null;
                IAudioSessionControl2* ctl2 = null;

                sessionEnumerator->GetSession(index, out ctl).ThrowOnFailure();
                if (ctl == null)
                {
                    Console.WriteLine("GetVolumeControl: ctl is null");
                    continue;
                }

                void* ctl2Ptr;
                ctl->QueryInterface(AudioUtilitiesV2.IID_IAudioSessionControl2, out ctl2Ptr).ThrowOnFailure();
                ctl2 = (IAudioSessionControl2*) ctl2Ptr;
                if (ctl2 == null)
                {
                    Console.WriteLine("GetVolumeControl: ctl2 is null");
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

                if (checked((int)processId) == pid)
                {
                    Console.WriteLine("Found Audio Session: ");
                    Console.WriteLine("======================================================");
                    Console.WriteLine("Audio Session Display Name: " + displayName);
                    Console.WriteLine("Audio Session Process Name: " + Process.GetProcessById(checked((int)processId)).ProcessName);
                    Console.WriteLine("Audio Session Process Id: " + pid);
                    Console.WriteLine("Audio Session Session Id: " + sessionId);
                    Console.WriteLine("Audio Session Session Instance Id: " + sessionInstanceId);
                    Console.WriteLine("Audio Session State: " + state);
                    Console.WriteLine("======================================================");


                    void* simpleAudioVolumePtr;
                    ctl->QueryInterface(AudioUtilitiesV2.IID_ISimpleAudioVolume, out simpleAudioVolumePtr).ThrowOnFailure();
                    volumeControl = (ISimpleAudioVolume*) simpleAudioVolumePtr;

                    ctl->Release();
                    ctl2->Release();
                    break;
                }
                ctl->Release();
                ctl2->Release();
            }
            //deviceEnumerator->Release();
            //speakers->Release();
            speaker->Release();
            audioSessionManager2->Release();
            sessionEnumerator->Release();
            return volumeControl;

        }
    }
}
