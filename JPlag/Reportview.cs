using Newtonsoft.Json;
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
        Process project_build_process = new Process();
        Dictionary<string, HashSet<TopComparison>> distinct_topcomparision = new Dictionary<string, HashSet<TopComparison>>();
        Dictionary<string, HashSet<string>> distinct_groups_names = new Dictionary<string, HashSet<string>>();

        public Manage()
        {
            InitializeComponent();
            textBox4.Text = "50.0";
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
            
        }

        private void Manage_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            JPlagReport plagiarismReport = new JPlagReport();
            plagiarismReport.View_Plagiarism_Report(textBox2.Text);
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
                } else if (folderBrowserDialog1.SelectedPath != null && folderBrowserDialog1.SelectedPath.Equals(""))
                {
                    MessageBox.Show("Please select the correct output folder.\n", "Output files not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        m.show_groups(groupsTopComaprision, "chart_view");

                    }
                    else
                    {
                        MessageBox.Show("Please enter valid percenatge\n", "Considered Plagiarism Percentage", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
