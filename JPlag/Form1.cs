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
    public partial class Form1 : Form
    {
        string build_output_log = "";
        string report_output_log = "";
        string plagairism_detection_log = "";
        Dictionary<string, HashSet<TopComparison>> distinct_topcomparision = new Dictionary<string, HashSet<TopComparison>>();
        Dictionary<string, HashSet<string>> distinct_groups_names = new Dictionary<string, HashSet<string>>();
        Process report_view_process = new Process();
        Process project_build_process = new Process();
        public class group
        {
            public List<string> names { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            textBox4.Text = "50.0";
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
            distinct_topcomparision = new Dictionary<string, HashSet<TopComparison>>();
            distinct_groups_names = new Dictionary<string, HashSet<string>>();
            double considered_percentage = 0.0;
            if (textBox4.Text != null || !textBox4.Text.Equals(""))
            {
                if (!double.TryParse(textBox4.Text, out considered_percentage))
                {
                    MessageBox.Show("Please enter valid percenatge\n", "Considered Plagiarism Percentage", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (considered_percentage >= 0.0 && considered_percentage <= 100.0)
                    {
                        using (StreamReader r = new StreamReader(folderBrowserDialog1.SelectedPath + "\\overview.json"))
                        {
                            string json = r.ReadToEnd();
                            Overview overviews = JsonConvert.DeserializeObject<Overview>(json);

                            var first_top_comparison_hashset = new HashSet<HashSet<TopComparison>>();

                            foreach (Metric metric in overviews.metrics)
                            {
                                for (int i = 0; i < metric.topComparisons.Count; i++)
                                {
                                    TopComparison comparison = metric.topComparisons[i];
                                    bool continueOuterFor = false;
                                    foreach (var resultList in first_top_comparison_hashset)
                                    {
                                        foreach (var result in resultList)
                                        {
                                            if (comparison.Equals(result))
                                            {
                                                continueOuterFor = true;
                                                continue;
                                            }
                                        }
                                        if (continueOuterFor) { continue; }
                                    }
                                    if (continueOuterFor) { continue; }

                                    bool foundMatch = false;
                                    var topcomparison_group_hsahset = new HashSet<TopComparison>();
                                    HashSet<string> hashset_names = new HashSet<string>();
                                    for (int j = i + 1; j < metric.topComparisons.Count; j++)
                                    {
                                        TopComparison innerComparison = metric.topComparisons[j];
                                        if (comparison.first_submission == innerComparison.first_submission
                                            || comparison.first_submission == innerComparison.second_submission
                                            || comparison.second_submission == innerComparison.first_submission
                                            || comparison.second_submission == innerComparison.second_submission
                                            || hashset_names.Contains(innerComparison.first_submission)
                                            || hashset_names.Contains(innerComparison.second_submission))
                                        {
                                            foundMatch = true;

                                            if (!topcomparison_group_hsahset.Contains(innerComparison) && innerComparison.match_percentage > considered_percentage)
                                            {
                                                hashset_names.Add(innerComparison.first_submission);
                                                hashset_names.Add(innerComparison.second_submission);
                                                topcomparison_group_hsahset.Add(innerComparison);
                                            }
                                            if (!topcomparison_group_hsahset.Contains(comparison) && comparison.match_percentage > considered_percentage)
                                            {
                                                hashset_names.Add(comparison.first_submission);
                                                hashset_names.Add(comparison.second_submission);
                                                topcomparison_group_hsahset.Add(comparison);
                                            }
                                        }
                                    }

                                    if (!first_top_comparison_hashset.Contains(topcomparison_group_hsahset) && topcomparison_group_hsahset.Count > 0)
                                    {
                                        first_top_comparison_hashset.Add(topcomparison_group_hsahset);
                                    }

                                    if (!foundMatch && comparison.match_percentage > considered_percentage)
                                    {
                                        first_top_comparison_hashset.Add(new HashSet<TopComparison>() { comparison });
                                    }
                                }
                            }

                            //below logic is working for properly grouped inputs and it is writen by rohan
                            var unique_topcomparison_hashset = new HashSet<HashSet<TopComparison>>();
                            var second_topcomparison_hashset = first_top_comparison_hashset.Distinct().ToList();

                            for (int i = 0; i < second_topcomparison_hashset.Count; i++)
                            {
                                HashSet<TopComparison> _loop_topComparision = new HashSet<TopComparison>();
                                HashSet<TopComparison> _strict_topComparision = new HashSet<TopComparison>();
                                HashSet<string> outter_name_hashset = new HashSet<string>();
                                HashSet<string> inner_name_hashset = new HashSet<string>();
                                bool foundAnyOverlap = false;

                                foreach (TopComparison outter_topComparison in second_topcomparison_hashset[i])
                                {
                                    outter_name_hashset.Add(outter_topComparison.first_submission);
                                    outter_name_hashset.Add(outter_topComparison.second_submission);
                                }

                                for (int j = i + 1; j < second_topcomparison_hashset.Count; j++)
                                {
                                    inner_name_hashset = new HashSet<string>();
                                    foreach (TopComparison inner_topComparison in second_topcomparison_hashset[j])
                                    {
                                        inner_name_hashset.Add(inner_topComparison.first_submission);
                                        inner_name_hashset.Add(inner_topComparison.second_submission);
                                    }

                                    if (outter_name_hashset.Overlaps(inner_name_hashset))
                                    {
                                        foundAnyOverlap = true;
                                        bool is_main_overlaps = false;
                                        _loop_topComparision = second_topcomparison_hashset[i].Union(second_topcomparison_hashset[j]).ToHashSet();
                                        var out_hash = new HashSet<string>();
                                        foreach (TopComparison top in _loop_topComparision)
                                        {
                                            out_hash.Add(top.first_submission);
                                            out_hash.Add(top.second_submission);
                                            if (!_strict_topComparision.Contains(top))
                                            {
                                                _strict_topComparision.Add(top);
                                            }
                                        }
                                        for (int k = 0; k < unique_topcomparison_hashset.ToList().Count; k++)
                                        {
                                            var in_hash = new HashSet<string>();

                                            foreach (TopComparison top in unique_topcomparison_hashset.ToList()[k])
                                            {
                                                in_hash.Add(top.first_submission);
                                                in_hash.Add(top.second_submission);
                                            }

                                            if (out_hash.Overlaps(in_hash))
                                            {
                                                is_main_overlaps = true;
                                                //they have same values so swap the existing list value
                                                if (out_hash.Count > in_hash.Count)
                                                {
                                                    //new_topComparisonList.ToList().ForEach(a => a.Value = _loop_topComparision);
                                                    unique_topcomparison_hashset.Remove(unique_topcomparison_hashset.ToList()[k]);
                                                    unique_topcomparison_hashset.Add(_loop_topComparision);
                                                }

                                            }
                                            /*else
                                            {
                                                // if it does'nt find the match and its a last index then add it to the list
                                                if (k == new_topComparisonList.ToList().Count - 1)
                                                {
                                                    new_topComparisonList.Add(_loop_topComparision);
                                                }
                                            }*/
                                        }
                                        if (!is_main_overlaps)
                                        {
                                            unique_topcomparison_hashset.Add(_loop_topComparision);
                                        }
                                    }
                                }

                                if (foundAnyOverlap)
                                {
                                    // do nothing............ other wise produces duplicates
                                    // adding duplicates

                                    /*foreach (TopComparison top in _loop_topComparision)
                                    {
                                        if (!_strict_topComparision.Contains(top))
                                        {
                                            _strict_topComparision.Add(top);
                                        }
                                    }
                                    new_topComparisonList.Add(_strict_topComparision);*/
                                }
                                else
                                {
                                    bool is_overlaps = false;
                                    foreach (var outter in unique_topcomparison_hashset)
                                    {
                                        HashSet<string> _name_hashset = new HashSet<string>();
                                        foreach (TopComparison comparison in outter)
                                        {
                                            _name_hashset.Add(comparison.first_submission);
                                            _name_hashset.Add(comparison.second_submission);
                                        }
                                        /*if (!outter_name_hashset.Overlaps(_name_hashset) && index_count == new_topComparisonList.ToList().Count - 1)
                                        {
                                            is_overlaps = true;
                                            index_count++;
                                            new_topComparisonList.Add(distinct_topComparisonList[i]);
                                            break;
                                        }
                                        else
                                        {
                                            index_count++;
                                            //break;
                                        }*/

                                        if (outter_name_hashset.Overlaps(_name_hashset))
                                        {
                                            is_overlaps = true;
                                        }
                                    }
                                    if (!is_overlaps)
                                    {
                                        unique_topcomparison_hashset.Add(second_topcomparison_hashset[i]);
                                    }

                                }
                                if (unique_topcomparison_hashset.Count < 1)
                                {
                                    unique_topcomparison_hashset.Add(second_topcomparison_hashset[i]);
                                }
                            }

                            //below logic is working for properly grouped inputs and it is writen by rohan
                            int count = 1;
                            foreach (HashSet<TopComparison> topComparison in unique_topcomparison_hashset)
                            {
                                distinct_topcomparision.Add("Group " + count, topComparison);
                                HashSet<string> names = new HashSet<string>();
                                foreach (TopComparison comparison in topComparison)
                                {
                                    string first_name = null;
                                    string second_name = null;
                                    bool is_first_name_present = true;
                                    bool is_second_name_present = true;
                                    if (names.Count > 0)
                                    {
                                        foreach (string name in names)
                                        {
                                            if (!name.Equals(comparison.first_submission))
                                            {
                                                first_name = comparison.first_submission;
                                                is_first_name_present = false;

                                            }

                                            if (!name.Equals(comparison.second_submission))
                                            {
                                                second_name = comparison.second_submission;
                                                is_second_name_present = false;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        first_name = comparison.first_submission;
                                        is_first_name_present = false;
                                        second_name = comparison.second_submission;
                                        is_second_name_present = false;
                                    }

                                    if (!is_second_name_present)
                                    {
                                        names.Add(second_name);
                                    }
                                    if (!is_first_name_present)
                                    {
                                        names.Add(first_name);
                                    }
                                }

                                distinct_groups_names.Add("Group " + count, (names));
                                count++;
                            }
                        }

                        GroupsTopComaprision groupsTopComaprision = new GroupsTopComaprision();
                        groupsTopComaprision.groups_names = distinct_groups_names;
                        groupsTopComaprision.groups_top_comparision = distinct_topcomparision;
                        var m = new Groups();
                        m.show_groups(groupsTopComaprision);

                    }
                    else
                    {
                        MessageBox.Show("Please enter valid percenatge\n", "Considered Plagiarism Percentage", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
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

        private void button5_Click(object sender, EventArgs e)
        {
            report_output_log = "";
            ProcessStartInfo startInfo = new ProcessStartInfo("CMD.exe");
            report_view_process = new Process();
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            report_view_process.EnableRaisingEvents = true;
            report_view_process = Process.Start(startInfo);
            report_view_process.EnableRaisingEvents = true;
            report_view_process.StandardInput.WriteLine(@"Echo on");
            report_view_process.StandardInput.WriteLine(@"chdir " + textBox2.Text + "\\report-viewer");
            report_view_process.StandardInput.WriteLine("/c " + @"npm i" + "& npm run serve" + "& EXIT");
            report_view_process.BeginOutputReadLine();
            report_view_process.BeginErrorReadLine();
            report_view_process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(ProcessRecievedForReportView);
            report_view_process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(ProcessRecievedForReportViewError);
            report_view_process.Exited += new System.EventHandler(ProcessReportViewExited);
        }

        void ProcessReportViewExited(Object sender, EventArgs eventArgs)
        {
            if (report_output_log.Contains("No issues found.") || report_output_log.Contains("App running at:"))
            {
                var addresses = new List<string>();
                var log_links = report_output_log.Split("\t\n ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Where(s => s.StartsWith("http://") || s.StartsWith("https://"));
                foreach (string link in log_links)
                {
                    addresses.Add(link);
                    if (link.Contains("localhost"))
                    {
                        System.Diagnostics.Process.Start(link);
                    }
                }

                var stringBuilder = new StringBuilder();
                var links = new List<LinkLabel.Link>();

                foreach (var address in addresses)
                {
                    if (stringBuilder.Length > 0) stringBuilder.AppendLine();

                    // We cannot add the new LinkLabel.Link to the LinkLabel yet because
                    // there is no text in the label yet, so the label will complain about
                    // the link location being out of range. So we'll temporarily store
                    // the links in a collection and add them later.
                    links.Add(new LinkLabel.Link(stringBuilder.Length, address.Length, address));
                    stringBuilder.Append(address);
                }

                var linkLabel = new LinkLabel();
                // We must set the text before we add the links.
                linkLabel.Text = stringBuilder.ToString();
                foreach (var link in links)
                {
                    linkLabel.Links.Add(link);
                }
                linkLabel.AutoSize = true;
                linkLabel.LinkClicked += (s, e) =>
                {
                    System.Diagnostics.Process.Start((string)e.Link.LinkData);
                };

                MessageBox.Show("Report view server started successfully!!!\n" + Environment.NewLine + "Below are the report view links :  \n" + Environment.NewLine + linkLabel.Text, "Report Viewer Running", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("1. Please check the JPlag project path is proper. \n" +
                    "2. Please check node js has installed and configured in your local machine. \n" +
                    "3. Please take update/pull from the JPlag repo. \n", "Build Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        void ProcessRecievedForReportView(Object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            report_output_log += dataReceivedEventArgs.Data;
            Console.WriteLine(dataReceivedEventArgs.Data);
            if (!report_view_process.HasExited && report_output_log.Contains("No issues found."))
            {
                report_view_process.Kill();
            }
        }

        void ProcessRecievedForReportViewError(Object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            report_output_log += dataReceivedEventArgs.Data;
            Console.WriteLine(dataReceivedEventArgs.Data);
            if (!report_view_process.HasExited && report_output_log.Contains("issues found."))
            {
                report_view_process.Kill();
            }
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
            project_build_process.StandardInput.WriteLine(@"chdir " + textBox2.Text + "\\jplag.cli\\target");
            project_build_process.StandardInput.WriteLine(@"java -jar jplag-4.0.0-SNAPSHOT-jar-with-dependencies.jar -l " + comboBox1.Text + " -r " + textBox1.Text + " -c " + comboBox2.Text + " " + textBox3.Text + "& exit");
            project_build_process.BeginOutputReadLine();
            project_build_process.BeginErrorReadLine();
            project_build_process.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(ProcessRecievedForRunPlagairismDetection);
            project_build_process.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(ProcessRecievedForRunPlagairismDetectionError);
            project_build_process.Exited += new System.EventHandler(RunPlagairismDetectionExited);
        }

        void RunPlagairismDetectionExited(Object sender, EventArgs eventArgs)
        {
            if (plagairism_detection_log.Contains("submissions parsed successfully!") && (plagairism_detection_log.Contains("0 parser errors!") || plagairism_detection_log.Contains("0 parser error!")))
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
    }
}
