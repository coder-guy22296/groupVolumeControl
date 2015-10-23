using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolumeControlUtility;
using GlobalHotkeys;
using System.Windows.Forms;

namespace VolumeControlUtility
{
    class ProgramGroup
    {
        public string groupName = "[Default]";
        public int volAsPercent = 0;
        public int numOfSessions = 0;
        public List<AudioSession> audioSessions = new List<AudioSession>();
        public List<string> nonLoadedAudioSessions = new List<string>();

        //hotkey data
        public bool hasHotkey = false;
        public List<string> mods = new List<string>();
        public string volumeUp;
        public string volumeDown;
        //
        public GlobalHotkey hotkeyVolUp = null;//volume up
        public GlobalHotkey hotkeyVolDown = null;//volume down

        public ProgramGroup(string name, int volume)
        {
            groupName = name;
            setVolume(volume);
            updateVolume();
            hasHotkey = false;
        }
        //unused
        public ProgramGroup(string name, int volume, Modifiers mods, int volUp, int volDown)
        {
            groupName = name;
            hotkeyVolUp = new GlobalHotkey(mods, (Keys)volUp, Form1.ActiveForm, false);
            hotkeyVolDown = new GlobalHotkey(mods, (Keys)volDown, Form1.ActiveForm, false);
            setVolume(volume);
            updateVolume();
            hasHotkey = true;
        }
        //
        public ProgramGroup(string name, int volume, List<string> mods, string volUp, string volDown)
        {
            groupName = name;
            this.volumeUp = volUp;
            this.volumeDown = volDown;
            this.mods = mods;
            setVolume(volume);
            updateVolume();
            hasHotkey = true;
        }
        public void isThisYourKotkey(HotkeyInfo hotkeyInfo)
        {
            if (!hasHotkey)
            {
                return;
            }
            if (hotkeyInfo.Key == (Keys)hotkeyVolUp.Key &&
                hotkeyInfo.Modifiers == (Modifiers)hotkeyVolUp.Modifier)
            {
                volAsPercent += 5;
                
            }

            if (hotkeyInfo.Key == (Keys)hotkeyVolDown.Key &&
                hotkeyInfo.Modifiers == (Modifiers)hotkeyVolDown.Modifier)
            {
                volAsPercent -= 5;
            }
            updateVolume();
        }
        public void setVolumeHotkeys(List<string> modifiers, string volumeUp, string volumeDown,  IWin32Window window)
        {
            if (hasHotkey)
            {
                hotkeyVolUp.Unregister();
                hotkeyVolDown.Unregister();
            }
            this.volumeUp = volumeUp;
            this.volumeDown = volumeDown;
            this.mods = modifiers;
            this.hasHotkey = true;
            registerHotkey(window);
        }
        public void setVolumeHotkeys(GlobalHotkey volumeUp, GlobalHotkey volumeDown)
        {
            if (hotkeyVolUp != null)
            {
                hotkeyVolUp.Unregister();
            }
            else if(hotkeyVolDown != null){
                hotkeyVolDown.Unregister();
            }
            hotkeyVolUp = volumeUp;
            hotkeyVolDown = volumeDown;
            hasHotkey = true;
        }
        public void setVolume(int inputVolume)
        {
            updateActiveSessions();
            if (inputVolume <= 0)
            {
                volAsPercent = 0;
            }
            else if (inputVolume >= 100)
            {
                volAsPercent = 100;
            }
            else
            {
                volAsPercent = inputVolume;
            }
            updateVolume();
        }
        public int getVolume()
        {
            updateActiveSessions(); 
            return volAsPercent;
        }
        public string getName()
        {
            return groupName;
        }
        public void rename(string newName)
        {
            groupName = newName;
        }
        public void addAudioSession(AudioSession Session)
        {
            audioSessions.Add(Session);
            numOfSessions = audioSessions.Count;
            updateVolume();
        }
        public void removeAudioSession(AudioSession Session)
        {
            updateActiveSessions(); 
            audioSessions.Remove(Session);
            numOfSessions = audioSessions.Count;
        }
        public void removeAudioSession(string Session)
        {
            updateActiveSessions();
            AudioSession targetSession = null;
            foreach (AudioSession currentAS in audioSessions)
            {
                if(currentAS.Process.ProcessName == Session){
                    targetSession = currentAS;
                }
            }
            audioSessions.Remove(targetSession);
            numOfSessions = audioSessions.Count;
        }
        public void updateVolume()
        {
            if (audioSessions != null)
            {
                foreach (AudioSession currentAS in audioSessions)
                {
                    VolumeMixer.SetApplicationVolume(currentAS.ProcessId, (float)volAsPercent);
                }
            }
            
        }
        public int getNumOfPrograms()
        {
            return numOfSessions;
        }

        public void displayAudioSessions()
        {
            updateActiveSessions();
            Console.Clear();
            Console.WriteLine("======[Audio Sessions]======");
            if (numOfSessions > 0)
            {
                for (int i = 0; i < audioSessions.Count; i++)
                {
                    AudioSession session = audioSessions.ElementAt(i);
                    if (session.Process != null)
                    {
                        Console.WriteLine(i + ") " + session.Process.ProcessName);
                    }
                }
            }
            else
            {
                Console.WriteLine("No Audio Sessions Within this Program Group!");
            }

            Console.WriteLine("======[END]======");
        }
        //create a striped down object that can be saved to a json file
        public ProgramGroupData generateProgramGroupData()
        {
            ProgramGroupData outPGdata = new ProgramGroupData();
            //add audio sessions for programs that were running when THIS program started running
            foreach(AudioSession aSession in audioSessions.Distinct()){
                outPGdata.audioSessions.Add(aSession.Process.ProcessName);
                outPGdata.numOfSessions++;
                
            }
            //add audio sessions for programs that were not running when THIS program started running
            foreach (string aSession in nonLoadedAudioSessions)
            {
                
                outPGdata.audioSessions.Add(aSession);
                outPGdata.numOfSessions++;
            }
            outPGdata.volAsPercent = volAsPercent;
            outPGdata.groupName = groupName;
            outPGdata.audioSessions = outPGdata.audioSessions.Distinct().ToList();
            outPGdata.numOfSessions = outPGdata.audioSessions.Count();

            //hotkey saveData
            outPGdata.hasHotkey = hasHotkey;
            outPGdata.mods = this.mods;
            outPGdata.volumeUp = this.volumeUp;
            outPGdata.volumeDown = this.volumeDown;

            return outPGdata;
        }

        public void updateActiveSessions()
        {
            if (nonLoadedAudioSessions.Count > 0)
            {
                string strSession = nonLoadedAudioSessions.ElementAt(0);
                AudioSession aSession = Program.ASM.getAudioSession(strSession);
                if (aSession == null)
                {
                    //ignore
                }
                else
                {
                    addAudioSession(aSession);
                    numOfSessions = audioSessions.Count;
                    nonLoadedAudioSessions.Remove(strSession);
                    updateActiveSessions();
                }
            }
        }

        public void registerHotkey(IWin32Window window)
        {

            if (!hasHotkey)
            {
                return;
            }
            //convert the Keys and Modifiers into a useable form
            Keys volUpKey = (Keys)Enum.Parse(typeof(Keys), volumeUp);
            Keys volDownKey = (Keys)Enum.Parse(typeof(Keys), volumeDown);
            Modifiers modifiers = Modifiers.NoMod;
            foreach(string keyModifier in mods){
                modifiers |= (Modifiers)Enum.Parse(typeof(Modifiers), keyModifier); 
            }
            //create the hotkeys from hotkey data and register them with the system
            try
            {
                hotkeyVolUp = new GlobalHotkey(modifiers, volUpKey, window, true);
                hotkeyVolDown = new GlobalHotkey(modifiers, volDownKey, window, true);
            }
            catch (GlobalHotkeyException exc)
            {
                MessageBox.Show(exc.Message);
            }
            Console.WriteLine(groupName+" HotkeysRegistered");
        }
    }
}
