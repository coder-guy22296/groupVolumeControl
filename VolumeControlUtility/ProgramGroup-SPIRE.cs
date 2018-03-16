using System;
using System.Collections.Generic;
using System.Linq;
using GlobalHotkeys;
using System.Windows.Forms;

namespace VolumeControlUtility
{
    class ProgramGroup
    {
        public string groupName = "[Default]";
        public int volAsPercent = 0;
        public int numOfSessions = 0;
        RgbStrip rgbStrip = new RgbStrip();
        public List<string> audioSessions = new List<string>();
        public List<AudioSession> loadedAudioSessions = new List<AudioSession>();
        public List<string> nonLoadedAudioSessions = new List<string>();

        //hotkey data
        public bool hasHotkey = false;
        public List<string> mods = new List<string>();
        public string volumeUp;
        public string volumeDown;
        //
        public GlobalHotkey hotkeyVolUp = null;//volume up
        public GlobalHotkey hotkeyVolDown = null;//volume down
        private bool hotkeyPendingRegistration;

        /*
            Constructor used when creating a new program group from the UI
            */
        public ProgramGroup(string name, int volume)
        {
            groupName = name;
            setVolume(volume);
            updateVolume();
            hasHotkey = false;
            rgbStrip.barGraphVisual(100);
        }

        /*
            Constructor used when loading from file
            */
        public ProgramGroup(string name, int volume, List<string> mods, string volUp, string volDown)
        {
            groupName = name;
            this.volumeUp = volUp;
            this.volumeDown = volDown;
            this.mods = mods;
            setVolume(volume);
            updateVolume();
            if (!mods.Any() || volUp != "None" || volDown != "None")
                hasHotkey = true;
            else
                hasHotkey = false;
        }

        /*
            This function is called with info on what hotkey was pressed,
            if the hotkey that was pressed belongs to this group then it
            performs the appropriate task
            */
        public void isThisYourKotkey(HotkeyInfo hotkeyInfo)
        {
            if (!hasHotkey)
            {
                return;
            }
            if (hotkeyInfo.Key == (Keys)hotkeyVolUp.Key &&
                hotkeyInfo.Modifiers == (Modifiers)hotkeyVolUp.Modifier)
            {
                //volAsPercent += 5;
                setVolume(volAsPercent + 5);

            }

            if (hotkeyInfo.Key == (Keys)hotkeyVolDown.Key &&
                hotkeyInfo.Modifiers == (Modifiers)hotkeyVolDown.Modifier)
            {
                //volAsPercent -= 5;
                setVolume(volAsPercent - 5);
            }
            updateVolume();
        }

        /*
            Sets the hotkeys based on selections from the UI *Depreicated*
            */
        public void setVolumeHotkeys(List<string> modifiers, string volumeUp, string volumeDown,  IWin32Window window)
        {
            if (hasHotkey)
            {
                unregisterHotkeys();
            }
            this.volumeUp = volumeUp;
            this.volumeDown = volumeDown;
            this.mods = modifiers;
            hotkeyPendingRegistration = true;
            registerHotkey(window);
        }

        /*
            Sets the volume up hotkey for the program group without
            registering the hotkey
            */
        public void setVolumeUpHotkey(string volumeUp)
        {
            if (hasHotkey)
            {
                unregisterHotkeys();
            }
            this.volumeUp = volumeUp;
            hotkeyPendingRegistration = true;
        }

        /*
            Sets the volume down hotkey for the program group without
            registering the hotkey
            */
        public void setVolumeDownHotkey(string volumeDown)
        {
            if (hasHotkey)
            {
                unregisterHotkeys();
            }
            this.volumeDown = volumeDown;
            hotkeyPendingRegistration = true;
        }

        /*
            Sets the modifiers for the program group's hotkeys without
            registering the hotkey
            */
        public void setVolumeHotkeyModifiers(List<string> modifiers)
        {
            if (hasHotkey)
            {
                unregisterHotkeys();
            }
            this.mods = modifiers;
            hotkeyPendingRegistration = true;
        }

        /*
            Gets the name of the hotkey assigned to increase the volume for 
            the program group
            */
        public string getVolumeUpHotkey()
        {
            return volumeUp;
        }

        /*
            Gets the name of the hotkey assigned to decrease the volume for 
            the program group
            */
        public string getVolumeDownHotkey()
        {
            return volumeDown;
        }

        /*
            Sets the hotkeys using global hotkey objects *Depreciated*
            */
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

        /*
            Set the volume for this group of programs
            */
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
            //rgbStrip.barGraphVisual(volAsPercent);
            updateVolume();
        }

        /*
            Get the current volume of the program group
            */
        public int getVolume()
        {
            updateActiveSessions();
            return volAsPercent;
        }

        /*
            Gets the name that has been assigned to this program
            group
            */
        public string getName()
        {
            return groupName;
        }

        /*
            Sets a new name for the program group
            */
        public void rename(string newName)
        {
            groupName = newName;
        }

        /*
            Gets all the audio programs assigned to 
            this group
            */
        public List<string> getAudioSessions()
        {
                return audioSessions;
        }

        /*
            Adds an audio program to the Collection of active audio programs
            */
        public void addAudioSession(AudioSession Session, Boolean rip)
        {
                loadedAudioSessions.Add(Session);
                if(!rip)
                {
                    audioSessions.Add(Session.Process.ProcessName);
                }
                numOfSessions = loadedAudioSessions.Count;
                updateVolume();
        }

        /*
            Remove an audio program from the program group
            */
        public void removeAudioSession(AudioSession Session)
        {
            lock (loadedAudioSessions)
            {
                updateActiveSessions();
                loadedAudioSessions.Remove(Session);
                getAudioSessions().Remove(Session.Process.ProcessName);
                numOfSessions = loadedAudioSessions.Count;
            }
        }

        /*
            Remove an audio program from the program group based on the 
            name of the Process
            */
        public void removeAudioSession(string Session)
        {
            lock (loadedAudioSessions) {
                updateActiveSessions();
                AudioSession targetSession = null;
                foreach (AudioSession currentAS in loadedAudioSessions.ToList())
                {
                    if(currentAS.Process.ProcessName == Session){
                        targetSession = currentAS;
                    }
                }
                loadedAudioSessions.Remove(targetSession);
                getAudioSessions().Remove(Session);
                numOfSessions = loadedAudioSessions.Count;
            }
        }

        /*
            Updates the volume of all the running programs in the
            program group
            */
        public void updateVolume()
        {
            lock(loadedAudioSessions) {
                if (loadedAudioSessions != null)
                {
                    foreach (AudioSession currentAS in loadedAudioSessions.ToList())
                    {
                        //AudioSession ash = Program.ASM.getAudioSession(currentAS.Process.ProcessName);
                        VolumeMixer.SetApplicationVolume(currentAS.ProcessId, (float)volAsPercent);
                    }
                }
            }
            
        }

        /*
            Gets the number of programs from the group that are currently running
            */
        public int getNumOfPrograms()
        {
            return numOfSessions;
        }

        /*
            Displays the process names of the running audio programs in the group
            */
        public void displayAudioSessions()
        {
            updateActiveSessions();
            Console.Clear();
            Console.WriteLine("======[Audio Sessions]======");
            if (numOfSessions > 0)
            {
                for (int i = 0; i < loadedAudioSessions.Count; i++)
                {
                    AudioSession session = loadedAudioSessions.ElementAt(i);
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


        //create a serializable object that can be saved to a json file
        public ProgramGroupData generateProgramGroupData()
        {
            ProgramGroupData outPGdata = new ProgramGroupData();
            //add audio sessions for programs that were running when THIS program started running
            foreach(AudioSession aSession in loadedAudioSessions.Distinct())
            {
                outPGdata.audioSessions.Add(aSession.Process.ProcessName);
                outPGdata.numOfSessions++;
                
            }
            //add audio sessions for programs that were not running when THIS program started running
            foreach (string aSession in nonLoadedAudioSessions.ToList())
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

        /*
        * checks if any of the audio programs that have been assigned to this program group is running.
        * if so, then loads that audio session into the list of active sessions
        */
        public void updateActiveSessions()
        {
            List<string> sessionsTmp;

            sessionsTmp = getAudioSessions().ToList();
            loadedAudioSessions.Clear();
            nonLoadedAudioSessions.Clear();
            this.numOfSessions = 0;
            for (int i = sessionsTmp.Count; i > 0; i--)
            {
                string strSession = sessionsTmp.ElementAt(i - 1);
                AudioSession aSession = Program.ASM.getAudioSession(strSession);
                if (aSession == null)
                {
                    Console.WriteLine(strSession + " is not running, so it will not be loaded into an Program Group");
                    nonLoadedAudioSessions.Add(strSession);
                }
                else
                {
                    addAudioSession(aSession, true);
                }
            }
        }

        /*
            Registers the hotkeys assigned to this group with the operating system
            and associates them with the programs main window
            */
        public void registerHotkey(IWin32Window window)
        {

            if (!hasHotkey && !hotkeyPendingRegistration)
            {
                return;
            }

            Keys volUpKey = Keys.None;
            Keys volDownKey = Keys.None;
            //convert the Keys and Modifiers into a useable form
            if (volumeUp != "None" && volumeUp != null)
            {
                volUpKey = (Keys)Enum.Parse(typeof(Keys), volumeUp);
            }
            if (volumeDown != "None" && volumeDown != null)
            {
                volDownKey = (Keys)Enum.Parse(typeof(Keys), volumeDown);
            }
                
            Modifiers modifiers = Modifiers.NoMod;
            foreach(string keyModifier in mods){
                modifiers |= (Modifiers)Enum.Parse(typeof(Modifiers), keyModifier); 
            }
            //create the hotkeys from hotkey data and register them with the system
            try
            {
                if (volumeUp != "None")
                {
                    hotkeyVolUp = new GlobalHotkey(modifiers, volUpKey, window, true);
                }
                if (volumeDown != "None")
                {
                    hotkeyVolDown = new GlobalHotkey(modifiers, volDownKey, window, true);
                }
                hasHotkey = true;
                hotkeyPendingRegistration = false;
            }
            catch (GlobalHotkeyException exc)
            {
                MessageBox.Show(exc.Message);
            }
            Console.WriteLine(groupName+" HotkeysRegistered");
        }

        /*
            Unregisters the hotkeys that have been registed by this group
            */
        public void unregisterHotkeys()
        {

            if (!hasHotkey)
            {
                return;
            }
            try
            {
                hotkeyVolUp.Dispose();
                hotkeyVolDown.Dispose();
                hasHotkey = false;
            }
            catch (GlobalHotkeyException exc)
            {
                MessageBox.Show(exc.Message);
            }
            Console.WriteLine(groupName + " HotkeysUNRegistered");
        }
    }
}

