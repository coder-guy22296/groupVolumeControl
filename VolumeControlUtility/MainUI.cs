using GlobalHotkeys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Web.UI.WebControls;
using System.Management;
using System.Threading;

namespace VolumeControlUtility
{
    public partial class MainUI : Form
    {
        private List<GlobalHotkey> hotkeys = new List<GlobalHotkey>();
        private ManagementEventWatcher MEW = new ManagementEventWatcher("SELECT TargetInstance FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'");

        public MainUI()
        {
            InitializeComponent();

            WqlEventQuery query =
            new WqlEventQuery("__InstanceCreationEvent",
            new TimeSpan(0, 0, 1),
            "TargetInstance ISA \"Win32_Process\"");

            MEW.EventArrived += new EventArrivedEventHandler(this.OnProcessStart);
            MEW.Start();

            hotkeys.Add(new GlobalHotkey(Modifiers.NoMod, Keys.F5, this, true));

            foreach (ProgramGroup group in Program.PGM.programGroups)
            {
                programGroupList.Items.Add(group.getName());
            }
            foreach (AudioSession session in Program.ASM.getActiveAudioSessions())
            {
                AudioSessionList.Items.Add(session.Process.ProcessName + " - " + session.State);
            }/*
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
            }*/
            registerHotkeys();
            Closing += Form1Closing;
        }

        private void OnProcessStart(object sender, EventArrivedEventArgs e)
        {
            //ConsoleManager.Show();
            Console.WriteLine("Process started!!!!");
            rebuildGroups();

        }

        void Form1Closing(object sender, CancelEventArgs e)
        {
            MEW.Stop();
            SecureJsonSerializer<ProgramGroupManagerData> pgmDataFile = new SecureJsonSerializer<ProgramGroupManagerData>("pgmSave.json");
            pgmDataFile.Save(Program.PGM.generateProgramGroupManagerData());
            unregisterHotkeys();
            foreach (GlobalHotkey ghk in hotkeys)
            {
                ghk.Dispose();
            }

        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Win32.WM_HOTKEY_MSG_ID)
            {
                Console.WriteLine(m.ToString());
            }
            var hotkeyInfo = HotkeyInfo.GetFromMessage(m);
            if (hotkeyInfo != null) HotkeyProc(hotkeyInfo);
            base.WndProc(ref m);
        }

        private void HotkeyProc(HotkeyInfo hotkeyInfo)
        {
            Console.WriteLine("{0} : Hotkey Proc! {1}, {2}{3}", DateTime.Now.ToString("hh:MM:ss.fff"),
                                             hotkeyInfo.Key, hotkeyInfo.Modifiers, Environment.NewLine);
            if(hotkeyInfo.Key == Keys.F5)
            {
                rebuildGroups();

            }
            else
            {
                foreach (ProgramGroup group in Program.PGM.programGroups)
                {
                    group.isThisYourKotkey(hotkeyInfo);
                }

            }
            updateDisplayedVolume();

        }

        private void updateDisplayedVolume()
        {
            if (programGroupList.SelectedItem != null)
            {
                ProgramGroup targetGroup = Program.PGM.getProgramGroup(programGroupList.SelectedIndex);
                this.curVolume.Text = targetGroup.getVolume().ToString() + "%";
            }
        }

        private void registerHotkeys()
        {
            foreach (ProgramGroup group in Program.PGM.programGroups)
            {
                group.registerHotkey(this);
            }
        }

        private void unregisterHotkeys()
        {
            foreach (ProgramGroup group in Program.PGM.programGroups)
            {
                group.unregisterHotkeys();
            }
        }
        private void rebuildGroups()
        {
            foreach (ProgramGroup group in Program.PGM.programGroups)
            {
                group.updateActiveSessions();
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
                    AudioSessionList.SelectedIndex), false);
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
                foreach (AudioSession session in Program.PGM.programGroups.ElementAt(programGroupList.SelectedIndex).loadedAudioSessions)
                {
                    programsInGroupList.Items.Add(session.Process.ProcessName);
                }
            
                //*****display hotkey Box
                ProgramGroup targetGroup = Program.PGM.getProgramGroup(programGroupList.SelectedIndex);

                //display volume
                this.curVolume.Text = targetGroup.getVolume().ToString() + "%";


                //display currently binded hotkeys
                checkBoxCTRL.Checked = false;
                checkBoxALT.Checked = false;
                checkBoxWIN.Checked = false;
                if (targetGroup.hasHotkey)
                {
                    foreach (string modifiers in targetGroup.mods)
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
                    this.volUpHotkey.Text = targetGroup.getVolumeUpHotkey();
                    this.volDownHotkey.Text = targetGroup.getVolumeDownHotkey();
                }
                else
                {
                    this.volUpHotkey.Text = "None Binded";
                    this.volDownHotkey.Text = "None Binded";
                }
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
            ReloadActiveSessionList();
        }

        private void ReloadActiveSessionList()
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

            Program.PGM.createProgramGroup(addGroupTextBox.Text);
            addGroupTextBox.Clear();
            programGroupList.Items.Clear();
            foreach (ProgramGroup group in Program.PGM.programGroups)
            {
                programGroupList.Items.Add(group.getName());
            }
        }

        private void removeGroupButton_Click(object sender, EventArgs e)
        {
            if (programGroupList.SelectedItem != null)
            {
                int toRemove = programGroupList.SelectedIndex;
                Program.PGM.removeProgramGroup(toRemove);
                programGroupList.Items.Clear();
                foreach (ProgramGroup group in Program.PGM.programGroups)
                {
                    programGroupList.Items.Add(group.getName());
                } 
            }
        }

        private void renameGroupButton_Click(object sender, EventArgs e)
        {
            if (programGroupList.SelectedItem != null)
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
        }
        
        private void saveButton_Click(object sender, EventArgs e)
        {
            SecureJsonSerializer<ProgramGroupManagerData> pgmDataFile = new SecureJsonSerializer<ProgramGroupManagerData>("pgmSave.json");
            pgmDataFile.Save(Program.PGM.generateProgramGroupManagerData());
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void infoBox_Enter(object sender, EventArgs e)
        {

        }

        private void saveKbButton_Click(object sender, EventArgs e)
        {
            if (programGroupList.SelectedItem != null)
            {
                ProgramGroup targetGroup = Program.PGM.getProgramGroup(programGroupList.SelectedIndex);

                targetGroup.unregisterHotkeys();

                //hotkey data
                List<string> keyModifiers = new List<string>();

                //store the keystroke modifiers
                if (checkBoxCTRL.Checked)
                {
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
                targetGroup.setVolumeHotkeyModifiers(keyModifiers);
                targetGroup.registerHotkey(this);
            }
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

        private void DefaultVolBtn_Click(object sender, EventArgs e)
        {
            foreach (ProgramGroup group in Program.PGM.programGroups)
            {
                group.setVolume(100);
            }
            updateDisplayedVolume();
        }

        private void setVolumeUp_Click(object sender, EventArgs e)
        {
            ProgramGroup targetGroup = Program.PGM.getProgramGroup(programGroupList.SelectedIndex);
            Form prompt = new KeybindingPrompt(targetGroup, "Up");
            prompt.ShowDialog();
            this.volUpHotkey.Text = targetGroup.getVolumeUpHotkey();
            saveKbButton_Click(null, null);
        }

        private void setVolumeDown_Click(object sender, EventArgs e)
        {
            ProgramGroup targetGroup = Program.PGM.getProgramGroup(programGroupList.SelectedIndex);
            Form prompt = new KeybindingPrompt(targetGroup, "Down");
            prompt.ShowDialog();
            this.volDownHotkey.Text = targetGroup.getVolumeDownHotkey();
            saveKbButton_Click(null, null);
        }
    }
}
