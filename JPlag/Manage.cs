using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JPlag
{
    public partial class Manage : Form
    {
        string build_output_log = "";
        Process project_build_process = new Process();

        public Manage()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) // Test result.
            {
                try
                {
                    textBox2.Text = folderBrowserDialog1.SelectedPath;
                }
                catch (IOException)
                {
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //https://stackoverflow.com/questions/4871013/clr-has-been-unable-to-transition-from-com-context-0x3b2d70-to-com-context
            //https://social.msdn.microsoft.com/Forums/vstudio/en-US/f07f7744-0ea5-40b3-a787-ea1c10ec55f3/cmdexe-from-cnet-application?forum=netfxbcl
            //https://stackoverflow.com/questions/65522516/determine-if-a-command-has-been-finished-executing-in-cmd-in-c-sharp

            build_output_log = "";
            ProcessStartInfo startInfo = new ProcessStartInfo("CMD.exe");
            project_build_process = new Process();
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            project_build_process.EnableRaisingEvents = true;
            project_build_process = Process.Start(startInfo);
            project_build_process.EnableRaisingEvents = true;
            project_build_process.StandardInput.WriteLine(@"Echo on");
            project_build_process.StandardInput.WriteLine(@"chdir " + textBox2.Text);
            project_build_process.StandardInput.WriteLine(@"mvn clean package assembly:single" + "& exit");
            project_build_process.BeginOutputReadLine();
            project_build_process.BeginErrorReadLine();
            project_build_process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(ProcessRecievedForProjectBuild);
            project_build_process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(ProcessRecievedForProjectBuildError);
            project_build_process.Exited += new System.EventHandler(ProcessBuildExited);
        }

        void ProcessBuildExited(Object sender, EventArgs eventArgs)
        {
            if (build_output_log.Contains("BUILD SUCCESS"))
            {

                MessageBox.Show("JPlag jars have been built successfully!!!\n", "Build Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("1. Please check the JPlag project path is proper. \n" +
                    "2. Please check Maven is configured in your local machine. \n" +
                    "3. Please take update/pull from the JPlag repo. \n", "Build Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        void ProcessRecievedForProjectBuild(Object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            build_output_log += dataReceivedEventArgs.Data;
            Console.WriteLine(dataReceivedEventArgs.Data);
        }

        void ProcessRecievedForProjectBuildError(Object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            build_output_log += dataReceivedEventArgs.Data;
            Console.WriteLine(dataReceivedEventArgs.Data);
        }
    }
}
