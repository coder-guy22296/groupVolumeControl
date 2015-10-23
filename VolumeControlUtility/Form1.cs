using VolumeControlUtility;
using GlobalHotkeys;
using GroupVolumeControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.UI.WebControls;

namespace VolumeControlUtility
{
    public partial class Form1 : Form
    {
        private List<GlobalHotkey> hotkeys = new List<GlobalHotkey>();

        public Form1()
        {
            InitializeComponent();

            foreach (ProgramGroup group in Program.PGM.programGroups)
            {
                programGroupList.Items.Add(group.getName());
            }
            foreach (AudioSession session in Program.ASM.getActiveAudioSessions())
            {
                AudioSessionList.Items.Add(session.Process.ProcessName + " - " + session.State);
            }
            //add all the hotkeys to the drop down
            Array itemValues = System.Enum.GetValues(typeof(Keys));
            Array itemNames = System.Enum.GetNames(typeof(Keys));
            for (int i = 0; i <= itemNames.Length - 1; i++)
            {
                ListItem item = new ListItem(itemNames.GetValue(i).ToString(), itemValues.GetValue(i).ToString());
                volUpKeystrokeDropDown.Items.Add(item);
            }
            for (int i = 0; i <= itemNames.Length - 1; i++)
            {
                ListItem item = new ListItem(itemNames.GetValue(i).ToString(), itemValues.GetValue(i).ToString());
                volDownKeystrokeDropDown.Items.Add(item);
            }
            registerHotkeys();
            Closing += Form1Closing;
        }

        void Form1Closing(object sender, CancelEventArgs e)
        {
            foreach (GlobalHotkey ghk in hotkeys)
            {
                ghk.Dispose();
            }
        }

        protected override void WndProc(ref Message m)
        {
            var hotkeyInfo = HotkeyInfo.GetFromMessage(m);
            if (hotkeyInfo != null) HotkeyProc(hotkeyInfo);
            base.WndProc(ref m);
        }

        private void HotkeyProc(HotkeyInfo hotkeyInfo)
        {
            ConsoleManager.Show();
            Console.WriteLine("{0} : Hotkey Proc! {1}, {2}{3}", DateTime.Now.ToString("hh:MM:ss.fff"),
                                             hotkeyInfo.Key, hotkeyInfo.Modifiers, Environment.NewLine);
            foreach(ProgramGroup group in Program.PGM.programGroups){
                group.isThisYourKotkey(hotkeyInfo);
            }
        }
        private void registerHotkeys()
        {
            foreach (ProgramGroup group in Program.PGM.programGroups)
            {
                group.registerHotkey(this);
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void addButton_Click(object sender, EventArgs e)
        {
            if (programGroupList.SelectedItem != null &&
                AudioSessionList.SelectedItem != null)
            {
                ProgramGroup targetGroup = Program.PGM.programGroups.ElementAt(
                    programGroupList.SelectedIndex);
                targetGroup.addAudioSession(Program.ASM.activeAudioSessions.ElementAt(
                    AudioSessionList.SelectedIndex));
            }
            programGroupList_SelectedIndexChanged(-99, null);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (programsInGroupList.SelectedItem != null)
            {
                string targetSession = programsInGroupList.SelectedItem.ToString();
                Program.PGM.programGroups.ElementAt(
                    programGroupList.SelectedIndex).removeAudioSession(targetSession);
            }
            programGroupList_SelectedIndexChanged(null, null);
        }

        private void programGroupList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (programGroupList.SelectedItem != null)
            {
                programBox.Name = programGroupList.SelectedItem.ToString();
                programBox.Text = programGroupList.SelectedItem.ToString();

                programsInGroupList.Items.Clear();
                foreach (ProgramGroup group in Program.PGM.programGroups)
                {
                    group.updateActiveSessions();
                }
                foreach (AudioSession session in Program.PGM.programGroups.ElementAt(programGroupList.SelectedIndex).audioSessions)
                {
                    programsInGroupList.Items.Add(session.Process.ProcessName);
                }
            }
                //*****display hotkey Box

                //display currently binded hotkeys
                ProgramGroup target = Program.PGM.getProgramGroup(programGroupList.SelectedIndex);

                checkBoxCTRL.Checked = false;
                checkBoxALT.Checked = false;
                checkBoxWIN.Checked = false;
                if (target.hasHotkey)
                {
                    foreach (string modifiers in target.mods)
                    {
                        switch (modifiers)
                        {
                            case "Ctrl":
                                checkBoxCTRL.Checked = true;
                                break;
                            case "Alt":
                                checkBoxALT.Checked = true;
                                break;
                            case "Win":
                                checkBoxWIN.Checked = true;
                                break;
                        }
                    }
                    volUpKeystrokeDropDown.Text = target.volumeUp;
                    volDownKeystrokeDropDown.Text = target.volumeDown;
                }
                else
                {
                    volUpKeystrokeDropDown.Text = "None Binded";
                    volDownKeystrokeDropDown.Text = "None Binded";
                }
            
        }

        private void programsInGroupList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void AudioSessionList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void AudioSessionList_Enter(object sender, EventArgs e)
        {
            AudioSessionList.Items.Clear();
            foreach (AudioSession session in Program.ASM.getActiveAudioSessions())
            {
                AudioSessionList.Items.Add(session.Process.ProcessName + " - " + session.State);
            }
        }

        private void AudioSessionList_MouseHover(object sender, EventArgs e)
        {
            AudioSessionList.Items.Clear();
            foreach (AudioSession session in Program.ASM.getActiveAudioSessions())
            {
                AudioSessionList.Items.Add(session.Process.ProcessName + " - " + session.State);
            }
        }

        private void addGroupButton_Click(object sender, EventArgs e)
        {

            Program.PGM.addProgramGroup(addGroupTextBox.Text);
            addGroupTextBox.Clear();
            programGroupList.Items.Clear();
            foreach (ProgramGroup group in Program.PGM.programGroups)
            {
                programGroupList.Items.Add(group.getName());
            }
        }

        private void removeGroupButton_Click(object sender, EventArgs e)
        {
            int toRemove = programGroupList.SelectedIndex;
            Program.PGM.removeProgramGroup(toRemove);
            programGroupList.Items.Clear();
            foreach (ProgramGroup group in Program.PGM.programGroups)
            {
                programGroupList.Items.Add(group.getName());
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SecureJsonSerializer<ProgramGroupManagerData> pgmDataFile = new SecureJsonSerializer<ProgramGroupManagerData>("pgmSave.json");
            pgmDataFile.Save(Program.PGM.generateProgramGroupManagerData());
        }

        private void renameGroupButton_Click(object sender, EventArgs e)
        {
            int toRename = programGroupList.SelectedIndex;
            Program.PGM.getProgramGroup(toRename).rename(addGroupTextBox.Text);
            addGroupTextBox.Clear();
            programGroupList.Items.Clear();
            foreach (ProgramGroup group in Program.PGM.programGroups)
            {
                programGroupList.Items.Add(group.getName());
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void infoBox_Enter(object sender, EventArgs e)
        {

        }

        private void saveKbButton_Click(object sender, EventArgs e)
        {
            try
            {
                if(volUpKeystrokeDropDown.SelectedItem.ToString() == null || volDownKeystrokeDropDown.SelectedItem.ToString() == null) return;
            }catch(NullReferenceException exception){
                return;
            }
            
            ProgramGroup targetGroup = Program.PGM.getProgramGroup(programGroupList.SelectedIndex);

            //hotkey data
            List<string> keyModifiers = new List<string>();

            string volUp = volUpKeystrokeDropDown.SelectedItem.ToString();
            string volDown = volDownKeystrokeDropDown.SelectedItem.ToString();

            //store the keystroke modifiers
            if(checkBoxCTRL.Checked){
                keyModifiers.Add(Modifiers.Ctrl.ToString());
            }
            if (checkBoxALT.Checked)
            {
                keyModifiers.Add(Modifiers.Alt.ToString());
            }
            if (checkBoxWIN.Checked)
            {
                keyModifiers.Add(Modifiers.Win.ToString());
            }
            targetGroup.setVolumeHotkeys(keyModifiers, volUp, volDown, this);
        }

        private void checkBoxCTRL_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBoxALT_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void checkBoxWIN_CheckedChanged(object sender, EventArgs e)
        {
            
        }
    }
}
