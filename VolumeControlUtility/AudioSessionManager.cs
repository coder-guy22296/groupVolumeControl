using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace VolumeControlUtility
{
    [Serializable]
    public class AudioSessionManager
    {
        public List<AudioSessionV2> activeAudioSessions = new List<AudioSessionV2>();

        public AudioSessionManager()
        {
            updateActiveAudioSessions();
        }

        public void updateActiveAudioSessions()
        {
            activeAudioSessions.Clear();

            foreach (AudioSessionV2 session in AudioUtilitiesV2.GetAllSessions())
            {
                if (session.Process != null)
                {
                    activeAudioSessions.Add(session);
                }
            }

        }

        public void displayActiveAudioSessions()
        {
            updateActiveAudioSessions();
            Console.Clear();
            Console.WriteLine("======[Audio Sessions]======");
            // dump all audio sessions
            for (int i = 0; i < activeAudioSessions.Count; i++)
            {
                AudioSessionV2 session = activeAudioSessions.ElementAt(i);
                if (session.Process != null)
                {
                    // only the one associated with a defined process
                    Console.WriteLine(i + ") " + session.Process.ProcessName + " - " + session.State);
                }
                else
                {
                    Console.WriteLine(i + ") " + session.DisplayName + " - " + session.State);
                }
            }
            Console.WriteLine("======[END]======");
        }

        public List<AudioSessionV2> getActiveAudioSessions()
        {
            lock (activeAudioSessions) {
                updateActiveAudioSessions();
                return activeAudioSessions;
            }
        }

        public AudioSessionV2 getAudioSession(int index)
        {
            return activeAudioSessions.ElementAt(index);
        }
        public AudioSessionV2 getAudioSession(string name)
        {
            //updateActiveAudioSessions();
            foreach (AudioSessionV2 session in getActiveAudioSessions().ToList())
            {
                if (session != null && session.Process.ProcessName == name)
                {
                    return session;
                }
            }
            return null;
        }
    }
}
