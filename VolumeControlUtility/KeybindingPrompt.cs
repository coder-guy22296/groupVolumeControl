using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GlobalHotkeys;

namespace VolumeControlUtility
{
    partial class KeybindingPrompt : Form
    {
        private ProgramGroup group;
        private String volUpDown;
        public KeybindingPrompt()
        {
            InitializeComponent();
        }
        public KeybindingPrompt(ProgramGroup group, String volUpDown)
        {
            InitializeComponent();
            this.group = group;
            this.volUpDown = volUpDown;
            String messageToUser = "press the key you wish to bind to " + group.getName() + " group's volume " + volUpDown;
            this.message1.Text = messageToUser;
            
        }

        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        /*
        private void KeybindingPrompt_KeyPress(object sender, KeyPressEventArgs e)
        {
            Console.WriteLine("A KEY WAS PRESSED!!!!! :   "+ e.KeyChar+ " string: "+ e.ToString() );
            //this.Dispose();
        }*/

        protected override void WndProc(ref Message m)
        {
            Console.WriteLine("something happened!");
            if (m.Msg == 0x101)
            {
                m.LParam = (IntPtr)((int)m.WParam * (int)Math.Pow(16,4));
                Console.WriteLine(m.WParam); 
                Console.WriteLine(m.LParam);
                m.Msg = Win32.WM_HOTKEY_MSG_ID; 
            }
            var hotkeyInfo = HotkeyInfo.GetFromMessage(m);
            if (m.Msg == 0x101 || m.Msg == Win32.WM_HOTKEY_MSG_ID)
            {
                Console.WriteLine(m.ToString());
                Console.WriteLine("Detected!");
            }
            if (hotkeyInfo != null) HotkeyProc(hotkeyInfo);
            base.WndProc(ref m);
        }

        private void HotkeyProc(HotkeyInfo hotkeyInfo)
        {
            Console.WriteLine("KBW- {0} : Hotkey Proc! {1}, {2}{3}", DateTime.Now.ToString("hh:MM:ss.fff"),
                                             hotkeyInfo.Key, hotkeyInfo.Modifiers, Environment.NewLine);
            if (volUpDown == "Up")
                group.setVolumeUpHotkey(hotkeyInfo.Key.ToString());
            else
                group.setVolumeDownHotkey(hotkeyInfo.Key.ToString());

            this.Close();

        }
    }
}
