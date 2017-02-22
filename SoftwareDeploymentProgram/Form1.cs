using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSCP;

namespace SoftwareDeploymentProgram
{
    public partial class Form1 : Form
    {
        FileSystemWatcher uploadFolderWatcher = new FileSystemWatcher(@"C:\Users\canpy\OneDrive\Documents\GitHub\groupVolumeControl(ATLANTIS)\Installer\Release");

        public string host = "";
        public string usr = "";
        public string pw = "";
        public string sshKey = "";

        public Form1()
        {
            InitializeComponent();

            //load credentials
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("svrcred.txt"))
                {
                    // Read the stream to a string, and write the string to the console.
                    host = sr.ReadLine();
                    usr = sr.ReadLine();
                    pw = sr.ReadLine();
                    sshKey = sr.ReadLine();


                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
//                FolderBrowserDialog ofd = new FolderBrowserDialog();
//                ofd.ShowDialog();
                string folderToUpload = @"C:\Users\canpy\OneDrive\Documents\GitHub\groupVolumeControl(ATLANTIS)\Installer\Release\*";

                // Setup session options
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = host,
                    UserName = usr,
                    Password = pw,
                    SshHostKeyFingerprint = sshKey
                };

                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);
                    RemoteDirectoryInfo targetServerDir = session.ListDirectory("/opt/mean/public/VolumeControlUtility/test/");
                    Console.WriteLine(targetServerDir.ToString());
                    // Upload files
                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult transferResult;
                    //transferResult = session.PutFiles(@"d:\toupload\*", "/home/user/", false, transferOptions);
                    session.RemoveFiles("/opt/mean/public/VolumeControlUtility/test/*");
                    transferResult = session.PutFiles(folderToUpload, "/opt/mean/public/VolumeControlUtility/test/",
                        false, transferOptions);

                    // Throw on any error
                    transferResult.Check();

                    // Print results
                    foreach (TransferEventArgs transfer in transferResult.Transfers)
                    {
                        Console.WriteLine("Upload of {0} to deployment server has succeeded", transfer.FileName);
                    }
                }

            }
            catch (Exception err)
            {
                Console.WriteLine("Error: {0}", err);
            }
        }
    }
}
