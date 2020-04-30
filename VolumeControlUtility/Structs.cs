using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace VolumeControlUtility
{
    struct HotkeyPayload
    {
        public string resourceId;
        public string volumeUp;
        public string volumeDown;
        public List<string> mods;
        public HotkeyPayload(string _)
        {
            this.resourceId = "";
            this.volumeUp = "";
            this.volumeDown = "";
            this.mods = new List<string>();
        }
        public HotkeyPayload(string resourceId, string volumeUp, string volumeDown, List<string> mods)
        {
            this.resourceId = resourceId;
            this.volumeUp = volumeUp;
            this.volumeDown = volumeDown;
            this.mods = mods;
        }
    }
}
