using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.Windows.Sdk;

namespace VolumeControlUtility
{
    public sealed class AudioSessionV2 : IDisposable
    {
        public int ProcessId;
        public string Identifier;
        public string InstanceIdentifier;
        public string State;
        public string DisplayName;

        [JsonProperty]
        private Process _process;

        public AudioSessionV2()
        {

        }

        unsafe internal AudioSessionV2(IAudioSessionControl2* audioSessionControl2)
        {
            PWSTR displayName;
            AudioSessionState state;
            uint pid = 0;
            PWSTR sessionId;
            PWSTR sessionInstanceId;

            audioSessionControl2->GetDisplayName(out displayName).ThrowOnFailure();
            audioSessionControl2->GetState(out state).ThrowOnFailure();
            audioSessionControl2->GetProcessId(out pid).ThrowOnFailure();
            audioSessionControl2->GetSessionIdentifier(out sessionId).ThrowOnFailure();
            audioSessionControl2->GetSessionInstanceIdentifier(out sessionInstanceId).ThrowOnFailure();

            ProcessId = checked((int)pid);
            Identifier = sessionId.ToString();
            InstanceIdentifier = sessionInstanceId.ToString();
            State = state.ToString();
            DisplayName = displayName.ToString();
        }

        public Process Process
        {
            get
            {
                if (_process == null && ProcessId != 0)
                {
                    try
                    {
                        _process = Process.GetProcessById(ProcessId);
                    }
                    catch
                    {
                        // do nothing
                        Console.WriteLine("Failed to get process");
                    }
                }
                return _process;
            }
        }

        public override string ToString()
        {
            Console.WriteLine("AudioSessionV2.toString() called");

            string s = DisplayName;
            if (!string.IsNullOrEmpty(s))
                return "DisplayName: " + s;

            if (Process != null)
                return "Process: " + Process.ProcessName;

            return "Pid: " + ProcessId;
        }

        public void Dispose()
        {
        }
    }
}
