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
        string output = "";
        Dictionary<string, HashSet<TopComparison>> dic_top_comparision = new Dictionary<string, HashSet<TopComparison>>();
        Dictionary<string, HashSet<string>> dic_groups_names = new Dictionary<string, HashSet<string>>();

        public class group
        {
            public List<string> names { get; set; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) // Test result.
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
            using (StreamReader r = new StreamReader(folderBrowserDialog1.SelectedPath + "\\overview.json"))
            {
                string json = r.ReadToEnd();
                Overview overviews = JsonConvert.DeserializeObject<Overview>(json);

                var _topComparisonList = new HashSet<HashSet<TopComparison>>();

                foreach (Metric metric in overviews.metrics)
                {
                    for (int i = 0; i < metric.topComparisons.Count; i++)
                    {
                        TopComparison comparison = metric.topComparisons[i];
                        bool continueOuterFor = false;
                        foreach (var resultList in _topComparisonList)
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
                        var topComparisonGroup = new HashSet<TopComparison>();
                        HashSet<string> hSet = new HashSet<string>();
                        for (int j = i + 1; j < metric.topComparisons.Count; j++)
                        {
                            TopComparison innerComparison = metric.topComparisons[j];
                            if (comparison.first_submission == innerComparison.first_submission
                                || comparison.first_submission == innerComparison.second_submission
                                || comparison.second_submission == innerComparison.first_submission
                                || comparison.second_submission == innerComparison.second_submission
                                || hSet.Contains(innerComparison.first_submission)
                                || hSet.Contains(innerComparison.second_submission))
                            {
                                foundMatch = true;

                                if (!topComparisonGroup.Contains(innerComparison) && innerComparison.match_percentage > 70.0)
                                {
                                    hSet.Add(innerComparison.first_submission);
                                    hSet.Add(innerComparison.second_submission);
                                    topComparisonGroup.Add(innerComparison);
                                }
                                if (!topComparisonGroup.Contains(comparison) && comparison.match_percentage > 70.0)
                                {
                                    hSet.Add(comparison.first_submission);
                                    hSet.Add(comparison.second_submission);
                                    topComparisonGroup.Add(comparison);
                                }
                            }
                        }

                        if (!_topComparisonList.Contains(topComparisonGroup) && topComparisonGroup.Count > 0)
                        {
                            _topComparisonList.Add(topComparisonGroup);
                        }

                        if (!foundMatch && comparison.match_percentage > 70.0)
                        {
                            _topComparisonList.Add(new HashSet<TopComparison>() { comparison });
                        }
                    }
                }

                var new_topComparisonList = new HashSet<HashSet<TopComparison>>();
                var distinct_topComparisonList = _topComparisonList.Distinct().ToList();

                for (int i = 0; i < distinct_topComparisonList.Count; i++)
                {
                    HashSet<TopComparison> _loop_topComparision = new HashSet<TopComparison>();
                    HashSet<TopComparison> _strict_topComparision = new HashSet<TopComparison>();
                    HashSet<string> outter_name_hashset = new HashSet<string>();
                    HashSet<string> inner_name_hashset = new HashSet<string>();
                    bool foundAnyOverlap = false;

                    foreach (TopComparison outter_topComparison in distinct_topComparisonList[i])
                    {
                        outter_name_hashset.Add(outter_topComparison.first_submission);
                        outter_name_hashset.Add(outter_topComparison.second_submission);
                    }

                    for (int j = i + 1; j < distinct_topComparisonList.Count; j++)
                    {
                        inner_name_hashset = new HashSet<string>();
                        foreach (TopComparison inner_topComparison in distinct_topComparisonList[j])
                        {
                            inner_name_hashset.Add(inner_topComparison.first_submission);
                            inner_name_hashset.Add(inner_topComparison.second_submission);
                        }

                        if (outter_name_hashset.Overlaps(inner_name_hashset))
                        {
                            foundAnyOverlap = true;
                            bool is_main_overlaps = false;
                            _loop_topComparision = distinct_topComparisonList[i].Union(distinct_topComparisonList[j]).ToHashSet();
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
                            for (int k = 0; k < new_topComparisonList.ToList().Count; k++)
                            {
                                var in_hash = new HashSet<string>();

                                foreach (TopComparison top in new_topComparisonList.ToList()[k])
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
                                        new_topComparisonList.Remove(new_topComparisonList.ToList()[k]);
                                        new_topComparisonList.Add(_loop_topComparision);
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
                                new_topComparisonList.Add(_loop_topComparision);
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
                        int index_count = 0;
                        bool is_overlaps = false;
                        foreach (var outter in new_topComparisonList)
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
                            new_topComparisonList.Add(distinct_topComparisonList[i]);
                        }

                    }
                    if (new_topComparisonList.Count < 1)
                    {
                        new_topComparisonList.Add(distinct_topComparisonList[i]);
                    }
                }

                //below logic is working for properly grouped inputs and it is writen by rohan
                int count = 1;
                foreach (HashSet<TopComparison> topComparison in new_topComparisonList)
                {
                    dic_top_comparision.Add("Group " + count, topComparison);
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

                    dic_groups_names.Add("Group " + count, (names));
                    count++;
                }
            }
            GroupsTopComaprision groupsTopComaprision = new GroupsTopComaprision();
            groupsTopComaprision.groups_names = dic_groups_names;
            groupsTopComaprision.groups_top_comparision = dic_top_comparision;
            var m = new Groups();
            m.show_groups(groupsTopComaprision);
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

            output = "";
            //StreamWriter ToCMDShell = null;
            ProcessStartInfo startInfo = new ProcessStartInfo("CMD.exe");
            Process p = new Process();
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            p.EnableRaisingEvents = true;
            p = Process.Start(startInfo);
            p.EnableRaisingEvents = true;
            p.StandardInput.WriteLine(@"Echo on");
            p.StandardInput.WriteLine(@"chdir " + textBox2.Text);
            p.StandardInput.WriteLine(@"mvn clean package assembly:single" + "& exit");
            /*string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            
            Console.Write(output);
            p.Close();
            Console.Read();*/

            // ToCMDShell = p.StandardInput;
            // ToCMDShell.AutoFlush = true;
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            /*p.OutputDataReceived += (s, m) => Console.WriteLine(m.Data);
            p.ErrorDataReceived += (s, m) => Console.WriteLine($"ERR: {m.Data}");*/
            p.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(Process_outputDataRecieved);
            p.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(Process_ErrortDataRecieved);
            p.Exited += new System.EventHandler(Process_exited);
            // var run = ToCMDShell.WriteLineAsync("ping 8.8.8.8"); //Execute a long running command in cmd terminal.
            // do some stuff
            //run.Wait(); // wait for long command to complete
            //ToCMDShell.WriteLine("exit"); //Done
            //int code = p.ExitCode;
        }

        void Process_exited(Object sender, EventArgs eventArgs)
        {
            if (output.Contains("BUILD SUCCESS"))
            {

                MessageBox.Show("JPlag jars have been built successfully!!!\n", "Build Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("1. Please check the JPlag project path is proper. \n" +
                    "2. Please check Maven is configured in your local machine. \n" +
                    "3. Please take update from the JPlag repo. \n", "Build Failure", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        void Process_outputDataRecieved(Object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            output += dataReceivedEventArgs.Data;
            Console.WriteLine(dataReceivedEventArgs.Data);
        }

        void Process_ErrortDataRecieved(Object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            output += dataReceivedEventArgs.Data;
            Console.WriteLine(dataReceivedEventArgs.Data);
        }
    }
}
