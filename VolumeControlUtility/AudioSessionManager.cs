using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace VolumeControlUtility
{
    [Serializable]
    class AudioSessionManager
    {
        public List<AudioSession> activeAudioSessions = new List<AudioSession>();

        public AudioSessionManager()
        {
            updateActiveAudioSessions();
        }

        public void updateActiveAudioSessions()
        {
            activeAudioSessions.Clear();

            foreach (AudioSession session in AudioUtilities.GetAllSessions())
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
                AudioSession session = activeAudioSessions.ElementAt(i);
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

        public List<AudioSession> getActiveAudioSessions()
        {
            lock (activeAudioSessions) {
                updateActiveAudioSessions();
                return activeAudioSessions;
            }
        }

        public AudioSession getAudioSession(int index)
        {
            return activeAudioSessions.ElementAt(index);
        }
        public AudioSession getAudioSession(string name)
        {
            //updateActiveAudioSessions();
            foreach (AudioSession session in getActiveAudioSessions().ToList())
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
