using GlobalHotkeys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeControlUtility
{
    class ProgramGroupData
    {
        public string groupName = "[Default]";
        public int volAsPercent = 0;
        public int numOfSessions = 0;
        public List<string> audioSessions = new List<string>();

        //hotkey data
        public bool hasHotkey = false;
        public List<string> mods = new List<string>();
        public string volumeUp;
        public string volumeDown;
    }
}
