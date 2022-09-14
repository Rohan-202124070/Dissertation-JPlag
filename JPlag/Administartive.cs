using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public partial class Administartive : Form
    {
        string build_output_log = "";
        string plagairism_detection_log = "";
        Dictionary<string, HashSet<TopComparison>> distinct_topcomparision = new Dictionary<string, HashSet<TopComparison>>();
        Dictionary<string, HashSet<string>> distinct_groups_names = new Dictionary<string, HashSet<string>>();
        Process report_view_process = new Process();
        Process project_build_process = new Process();
        public class group
        {
            public List<string> names { get; set; }
        }

        public Administartive()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    textBox1.Text = folderBrowserDialog1.SelectedPath;
                }
                catch (IOException)
                {
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void folderBrowserDialog2_HelpRequest(object sender, EventArgs e)
        {

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

        private void button5_Click(object sender, EventArgs e)
        {
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    textBox3.Text = folderBrowserDialog1.SelectedPath;
                }
                catch (IOException)
                {
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            plagairism_detection_log = "";
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
            project_build_process.StandardInput.WriteLine(@"chdir " + textBox5.Text + "\\jplag.cli\\target");
            project_build_process.StandardInput.WriteLine(@"java -jar jplag-4.0.0-SNAPSHOT-jar-with-dependencies.jar -l " + comboBox1.Text + " -r " + textBox1.Text + " -c " + comboBox2.Text + " " + textBox3.Text + "& exit");
            project_build_process.BeginOutputReadLine();
            project_build_process.BeginErrorReadLine();
            project_build_process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(ProcessRecievedForRunPlagairismDetection);
            project_build_process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(ProcessRecievedForRunPlagairismDetectionError);
            project_build_process.Exited += new System.EventHandler(RunPlagairismDetectionExited);
        }

        void RunPlagairismDetectionExited(Object sender, EventArgs eventArgs)
        {
            if ((plagairism_detection_log.Contains("submissions parsed successfully!") && (plagairism_detection_log.Contains("0 parser errors!") || plagairism_detection_log.Contains("0 parser error!"))) || plagairism_detection_log.Contains("Calculating clusters via spectral clustering with cumulative distribution function"))
            {

                MessageBox.Show("JPlag plagiarism detection completed successfully!!!\n", "JPlag Plagiarism Detection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("1. Please check the JPlag project path is proper. \n" +
                    "2. Please check submission folder has valid data. \n" +
                    "3. Please check selected language. \n" +
                    "4. Please bulid the JPlag before running the plagiarism detection. \n", "JPlag Plagiarism Detection Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        void ProcessRecievedForRunPlagairismDetection(Object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            plagairism_detection_log += dataReceivedEventArgs.Data;
            Console.WriteLine(dataReceivedEventArgs.Data);
        }

        void ProcessRecievedForRunPlagairismDetectionError(Object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            plagairism_detection_log += dataReceivedEventArgs.Data;
            Console.WriteLine(dataReceivedEventArgs.Data);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void Administartive_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

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

        private void button9_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) // Test result.
            {
                try
                {
                    textBox5.Text = folderBrowserDialog1.SelectedPath;
                }
                catch (IOException)
                {
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
