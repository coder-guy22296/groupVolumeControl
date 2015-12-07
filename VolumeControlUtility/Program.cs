using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VolumeControlUtility;
using GlobalHotkeys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using GroupVolumeControl;
using System.Threading;

namespace VolumeControlUtility
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        

        public static ProgramGroupManager PGM = new ProgramGroupManager();
        public static AudioSessionManager ASM = new AudioSessionManager();
        public static SecureJsonSerializer<ProgramGroupManagerData> pgmDataFile = new SecureJsonSerializer<ProgramGroupManagerData>("pgmSave.json");
        [STAThread]
        static void Main()
        {
           
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            bool saveFileExists = File.Exists("pgmSave.json");

            if (saveFileExists)
            {
                PGM = (new ProgramGroupManagerFactory().getProgramGroupManager(pgmDataFile.Load()));
            }

            Application.Run(new Form1());
            /*
            string proceed = "y";
            do
            {
                ConsoleManager.Show();
                Console.Clear();
                Console.WriteLine("0) Create a program group");
                Console.WriteLine("1) Delete a program group");
                Console.WriteLine("2) Add a program to a group");
                Console.WriteLine("3) Show Programs in a group");
                Console.WriteLine("4) Set Volume of a group");
                Console.WriteLine("5) Set the volume of a particular Application");
                Console.WriteLine("6) Quit");
                Console.Write("Enter your selection: ");
                int ans;
                if (!(int.TryParse(Console.ReadLine(), out ans)))
                {
                    ans = 99999;
                }

                switch (ans)
                {
                    case 0:
                        Console.Clear();
                        Console.Write("Enter a name for the Program Group: ");
                        string groupName = Console.ReadLine();
                        PGM.addProgramGroup(groupName);
                        break;
                    case 1:
                        PGM.displayProgramGroups();
                        if (PGM.getNumOfProgramGroups() == 0)
                        {
                            Console.Write("press Enter to continue...");
                            Console.ReadLine();
                            break;
                        }

                        int toRemove;
                        do
                        {
                            Console.WriteLine("Select a Program Group from above to remove: ");
                        } while (!(int.TryParse(Console.ReadLine(), out toRemove)));
                        PGM.removeProgramGroup(toRemove);
                        break;
                    case 2:
                        PGM.displayProgramGroups();
                        if (PGM.getNumOfProgramGroups() == 0)
                        {
                            Console.Write("press Enter to continue...");
                            Console.ReadLine();
                            break;
                        }
                        int groupNum;
                        do
                        {
                            Console.WriteLine("Select a Program Group to add a program to: ");
                        } while (!(int.TryParse(Console.ReadLine(), out groupNum)));
                        ProgramGroup toBeUpdated = PGM.getProgramGroup(groupNum);
                        ASM.updateActiveAudioSessions();
                        ASM.displayActiveAudioSessions();
                        int ASIndex;
                        do
                        {
                            Console.Write("enter application number from above list to add to '"
                                + toBeUpdated.getName() + "' Program Group: ");
                        } while (!(int.TryParse(Console.ReadLine(), out ASIndex)));
                        toBeUpdated.addAudioSession(ASM.getAudioSession(ASIndex));
                        Console.WriteLine(ASM.getAudioSession(ASIndex).Process.ProcessName + " has been added to " + toBeUpdated.getName());
                        Console.Write("press Enter to continue...");
                        Console.ReadLine();
                        break;
                    case 3:
                        PGM.displayProgramGroups();
                        if (PGM.getNumOfProgramGroups() == 0)
                        {
                            Console.Write("press Enter to continue...");
                            Console.ReadLine();
                            break;
                        }
                        do
                        {
                            Console.WriteLine("Select a Program Group to show the programs it contains: ");
                        } while (!(int.TryParse(Console.ReadLine(), out groupNum)));
                        PGM.getProgramGroup(groupNum).displayAudioSessions();
                        Console.Write("press Enter to continue...");
                        Console.ReadLine();
                        break;
                    case 4:
                        PGM.displayProgramGroups();
                        if (PGM.getNumOfProgramGroups() == 0)
                        {
                            Console.Write("press Enter to continue...");
                            Console.ReadLine();
                            break;
                        }
                        do
                        {
                            Console.WriteLine("Select a Program Group to adjust the volume: ");
                        } while (!(int.TryParse(Console.ReadLine(), out groupNum)));
                        PGM.getProgramGroup(groupNum).displayAudioSessions();
                        ProgramGroup toAdjustVol = PGM.getProgramGroup(groupNum);
                        Console.WriteLine("current volume: " + toAdjustVol.getVolume());
                        int vol;
                        do
                        {
                            Console.Write("enter the new volume percentage(0-100): ");
                        } while (!(int.TryParse(Console.ReadLine(), out vol)));
                        toAdjustVol.setVolume(vol);
                        Console.WriteLine("Volume has been Set!");
                        Console.Write("press Enter to continue...");
                        Console.ReadLine();
                        break;
                    case 5:
                        Console.Clear();
                        singleAppVolumeControl();
                        break;
                    case 6:
                        proceed = "n";
                        pgmDataFile.Save(PGM.generateProgramGroupManagerData());
                        break;
                    default:
                        Console.WriteLine("Please enter a valid number from the options above");
                        Console.Write("press Enter to continue...");
                        Console.ReadLine();
                        break;
                }
            } while (proceed != "n");
            */
        }

        public static void singleAppVolumeControl()
        {
            AudioSessionManager ASM = new AudioSessionManager();
            int ans = 99;
            int tPid = 99;
            string proceed = "n";

            ASM.updateActiveAudioSessions();
            ASM.displayActiveAudioSessions();
            do
            {
                Console.Write("enter application number from above list: ");
            } while (!(int.TryParse(Console.ReadLine(), out ans)));
            tPid = ASM.getActiveAudioSessions().ElementAt(ans).ProcessId;
            float vol;
            do
            {
                Console.Write("enter the volume as a percentage(0-100): ");
            } while (!(float.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out vol)));
            VolumeMixer.SetApplicationVolume(tPid, vol);
            Console.WriteLine("Volume has been Set!");
            Console.Write("Press Enter to continue......");
            proceed = Console.ReadLine();
        }
    }
}
