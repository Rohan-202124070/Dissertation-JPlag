﻿using Newtonsoft.Json;
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
        Dictionary<string, List<TopComparison>> dic_top_comparision = new Dictionary<string, List<TopComparison>>();
        Dictionary<string, List<string>> dic_groups_names = new Dictionary<string, List<string>>();

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
              
                var _topComparisonList = new List<List<TopComparison>>();

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
                        var topComparisonGroup = new List<TopComparison>();
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
                                
                                if (!topComparisonGroup.Contains(innerComparison) && innerComparison.match_percentage > 0.0)
                                {
                                    hSet.Add(innerComparison.first_submission);
                                    hSet.Add(innerComparison.second_submission);
                                    topComparisonGroup.Add(innerComparison);
                                }
                                if (!topComparisonGroup.Contains(comparison) && comparison.match_percentage > 0.0)
                                {
                                    hSet.Add(comparison.first_submission);
                                    hSet.Add(comparison.second_submission);
                                    topComparisonGroup.Add(comparison);
                                }
                            }
                        }

                        if (!_topComparisonList.Contains(topComparisonGroup) && topComparisonGroup.Count>0)
                        {
                            _topComparisonList.Add(topComparisonGroup);
                        }

                        if (!foundMatch && comparison.match_percentage > 0.0)
                        {
                            _topComparisonList.Add(new List<TopComparison>() { comparison });
                        }
                    }
                }

                var new_topComparisonList = new List<List<TopComparison>>();
                for (int i = 0; i < _topComparisonList.Count; i++)
                {
                    List<TopComparison> _loop_topComparision = new List<TopComparison>();
                    List<TopComparison> _strict_topComparision = new List<TopComparison>();
                    HashSet<string> outter_name_hashset = new HashSet<string>();
                    HashSet<string> inner_name_hashset = new HashSet<string>();
                    bool foundAnyOverlap = false;

                    foreach (TopComparison outter_topComparison in _topComparisonList[i])
                    {
                        outter_name_hashset.Add(outter_topComparison.first_submission);
                        outter_name_hashset.Add(outter_topComparison.second_submission);
                    }

                    for (int j = i + 1; j < _topComparisonList.Count; j++)
                    {
                        inner_name_hashset = new HashSet<string>();
                        foreach (TopComparison inner_topComparison in _topComparisonList[j])
                        {
                            inner_name_hashset.Add(inner_topComparison.first_submission);
                            inner_name_hashset.Add(inner_topComparison.second_submission);
                        }

                        if (outter_name_hashset.Overlaps(inner_name_hashset))
                        {
                            foundAnyOverlap = true;
                            _loop_topComparision = _loop_topComparision.Union(_topComparisonList[i].Union(_topComparisonList[j]).ToList()).ToList();
                        } 
                    }

                    if (foundAnyOverlap)
                    {
                        foreach (TopComparison top in _loop_topComparision)
                        {
                            if (!_strict_topComparision.Contains(top))
                            {
                                _strict_topComparision.Add(top);
                            }
                        }
                        new_topComparisonList.Add(_strict_topComparision);
                    }
                    else
                    {
                        foreach (var outter in new_topComparisonList)
                        {
                            HashSet<string> _name_hashset = new HashSet<string>();
                            foreach (TopComparison comparison in outter)
                            {
                                _name_hashset.Add(comparison.first_submission);
                                _name_hashset.Add(comparison.second_submission);
                            }
                            if (!outter_name_hashset.Overlaps(_name_hashset))
                            {
                                new_topComparisonList.Add(_topComparisonList[i]);
                                break;
                            } else
                            {
                                break;
                            }
                        }
                    }
                    if (new_topComparisonList.Count < 1)
                    {
                        new_topComparisonList.Add(_topComparisonList[i]);
                    }
                }

                //below logic is working for properly grouped inputs and it is writen by rohan
                int count = 1;
                foreach (List<TopComparison> topComparison in new_topComparisonList)
                {
                    dic_top_comparision.Add("Group " + count, topComparison);
                    List<string> names = new List<string>();
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

                    dic_groups_names.Add("Group " + count, (names.Distinct().ToList()));
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


            StreamWriter ToCMDShell = null;
            ProcessStartInfo startInfo = new ProcessStartInfo("CMD.exe");
            Process p = new Process();
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            p = Process.Start(startInfo);
            p.StandardInput.WriteLine(@"Echo on");
            p.StandardInput.WriteLine(@"chdir " + textBox2.Text);
            p.StandardInput.WriteLine(@"mvn clean package assembly:single");
            /*string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            p.WaitForExit();
            Console.Write(output);
            p.Close();
            Console.Read();*/

            ToCMDShell = p.StandardInput;
            ToCMDShell.AutoFlush = true;
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.OutputDataReceived += (s, m) => Console.WriteLine(m.Data);
            p.ErrorDataReceived += (s, m) => Console.WriteLine($"ERR: {m.Data}");
           // var run = ToCMDShell.WriteLineAsync("ping 8.8.8.8"); //Execute a long running command in cmd terminal.
            // do some stuff
            //run.Wait(); // wait for long command to complete
            ToCMDShell.WriteLine("exit"); //Done

        }
    }
}
