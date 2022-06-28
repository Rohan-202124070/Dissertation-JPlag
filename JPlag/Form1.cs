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
            Overview overview = new Overview();
            using (StreamReader r = new StreamReader(folderBrowserDialog1.SelectedPath + "\\overview.json"))
            {
                string json = r.ReadToEnd();
                Overview overviews = JsonConvert.DeserializeObject<Overview>(json);
                // List<string> modified_submission_ids = new List<string>();
                /* long colorcode = 100000;
                 foreach (string submission_id in overviews.submission_ids)
                 {
                     string sub_id = submission_id + "#" + colorcode;
                     colorcode = colorcode + 000001;
                     modified_submission_ids.Add(sub_id);
                 }
                 overviews.submission_ids = modified_submission_ids;
 */

                //1
                /* List<List<GroupTopComaprision>> manual_topComparison_list = new List<List<GroupTopComaprision>>();
                 foreach (string submission_id in overviews.submission_ids)
                 {
                     List<GroupTopComaprision> manual_topComparison = new List<GroupTopComaprision>();
                     foreach (Metric metric in overviews.metrics)
                     {
                         foreach (TopComparison topComparison in metric.topComparisons)
                         {
                             GroupTopComaprision groupTopComaprision = new GroupTopComaprision();
                             if ((topComparison.first_submission.Equals(submission_id) || topComparison.second_submission.Equals(submission_id)) && topComparison.match_percentage > 0.0)
                             {
                                 groupTopComaprision.first_submission = topComparison.first_submission;
                                 groupTopComaprision.second_submission = topComparison.second_submission;
                                 groupTopComaprision.match_percentage = topComparison.match_percentage;

                                 groupTopComaprision.group_id = 1;

                                 manual_topComparison.Add(groupTopComaprision);
                             }
                         }
                     }
                     manual_topComparison_list.Add(manual_topComparison);
                 }*/
                // end 1



                //2
                /*Dictionary<string, List<TopComparison>> result = new Dictionary<string, List<TopComparison>>();
                List<List<TopComparison>> topComparison_list = new List<List<TopComparison>>();
                var dic1 = new Dictionary<string, List<TopComparison>>();
                var dic2 = new Dictionary<string, List<TopComparison>>();

                foreach (Metric metric in overviews.metrics)
                {

                    var rs1 = metric.topComparisons.GroupBy(c => c.first_submission)
                    .Select(g => new GroupTopComaprision(g.Key, string.Join(",", g.Select(rm => rm.second_submission))));

                    var list = metric.topComparisons.GroupBy(k => k.first_submission, v => v.second_submission)
                            .Select(g => new GroupTopComaprision(g.Key, string.Join(", ", g)));

                    var results = from item in metric.topComparisons
                                  group new { item.first_submission, item.second_submission } by new { item.first_submission, item.second_submission } into g
                                  select g;
*/

                //https://stackoverflow.com/questions/2669610/how-to-merge-two-dictionaries-in-c-sharp-with-duplicates
                /* dic1 = metric.topComparisons.GroupBy(x => x.first_submission).ToDictionary(g => g.Key, g => g.ToList());
                 dic2 = metric.topComparisons.GroupBy(x => x.second_submission).ToDictionary(g => g.Key, g => g.ToList());
                 result = dic1.
                     Union(dic2)
                     .GroupBy(o => o.Key).ToDictionary(o => o.Key, o => o.SelectMany(kvp => kvp.Value).Where(kvp => kvp.match_percentage > 0.0).ToList());
             }




             foreach (var dictionary in result)
             {
                 topComparison_list.Add(dictionary.Value);
             }



             List<List<TopComparison>> groups = new List<List<TopComparison>>();

             foreach (List<TopComparison> topComparisons in topComparison_list)
             {
                 foreach (TopComparison comparison in topComparisons)
                 {

                 }
             }*/
                // end 2


                //3
                /*bool isbreak = false;
                List<string> names = new List<string>();
                List<List<string>> names_list = new List<List<string>>();
                foreach (Metric metric in overviews.metrics)
                {
                    *//*List<group> group_list = new List<group>();*//*
                    foreach (TopComparison topComparison in metric.topComparisons)
                    {
                        List<string> local_group = new List<string>();

                        foreach (List<string> group in names_list)
                        {
                            if (group != null)
                            {
                                isbreak = false;
                                if (group.Contains(topComparison.first_submission) || group.Contains(topComparison.second_submission))
                                {
                                    local_group = group;
                                    group.Add(topComparison.first_submission);
                                    group.Add(topComparison.second_submission);
                                }
                                else
                                {
                                    local_group = new List<string>();
                                    local_group.Add(topComparison.first_submission);
                                    local_group.Add(topComparison.second_submission);
                                }
                            }
                            else
                            {
                                group.Add(topComparison.first_submission);
                                group.Add(topComparison.second_submission);
                            }
                        }

                        names_list.Add(local_group);

                    }
                }*/


                //4 https://stackoverflow.com/questions/72721196/c-sharp-group-the-objects-of-list-by-property
                /*var topComparisonList = new List<List<TopComparison>>();

                foreach (Metric metric in overviews.metrics)
                {
                    for (int i = 0; i < metric.topComparisons.Count - 1; i++)
                    {
                        TopComparison comparison = metric.topComparisons[i];
                        bool continueOuterFor = false;
                        foreach (var resultList in topComparisonList)
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

                        for (int j = i + 1; j < metric.topComparisons.Count; j++)
                        {
                            TopComparison innerComparison = metric.topComparisons[j];
                            if (comparison.first_submission == innerComparison.first_submission
                                || comparison.first_submission == innerComparison.second_submission
                                || comparison.second_submission == innerComparison.first_submission
                                || comparison.second_submission == innerComparison.second_submission)
                            {
                                foundMatch = true;
                                var topComparisonGroup = new List<TopComparison>() { comparison, innerComparison };

                                if (!topComparisonList.Contains(topComparisonGroup))
                                {
                                    topComparisonList.Add(topComparisonGroup);
                                }
                                break;
                            }
                        }

                        if (!foundMatch)
                        {
                            topComparisonList.Add(new List<TopComparison>() { comparison });
                        }
                    }
                }*/

                var topComparisonList = new List<List<TopComparison>>();
                var dic1 = new Dictionary<string, List<TopComparison>>();
                var dic2 = new Dictionary<string, List<TopComparison>>();
                var dic3 = new Dictionary<string, List<TopComparison>>();
                foreach (Metric metric in overviews.metrics)
                {

                    dic1 = metric.topComparisons.GroupBy(x => x.first_submission).ToDictionary(g => g.Key, g => g.ToList());
                    dic2 = metric.topComparisons.GroupBy(x => x.second_submission).ToDictionary(g => g.Key, g => g.ToList());
                    dic3 = dic1.
                    Union(dic2)
                    .GroupBy(o => o.Key).ToDictionary(o => o.Key, o => o.SelectMany(kvp => kvp.Value).Where(kvp => kvp.match_percentage > 0.0).ToList());

                }

                foreach (var dictionary in dic3)
                {
                    topComparisonList.Add(dictionary.Value);
                }


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
                                
                                if (!topComparisonGroup.Contains(innerComparison))
                                {
                                    hSet.Add(innerComparison.first_submission);
                                    hSet.Add(innerComparison.second_submission);
                                    topComparisonGroup.Add(innerComparison);
                                }
                                if (!topComparisonGroup.Contains(comparison))
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

                        if (!foundMatch)
                        {
                            _topComparisonList.Add(new List<TopComparison>() { comparison });
                        }
                    }
                }


                foreach (KeyValuePair<string, List<TopComparison>> outter_dic in dic3)
                {
                    List<TopComparison> topComparisons_list = new List<TopComparison>();
                    foreach (KeyValuePair<string, List<TopComparison>> inner_dic in dic1)
                    {
                        foreach (TopComparison topComparison in inner_dic.Value)
                        {
                            if (outter_dic.Key.Equals(topComparison.first_submission)
                                || outter_dic.Key.Equals(topComparison.second_submission))
                            {
                                topComparisons_list = outter_dic.Value.Union(inner_dic.Value).ToList();
                            }
                        }
                    }
                    topComparisonList.Add(topComparisons_list);
                }


                //below logic is working for properly grouped inputs and it is writen by rohan
                int count = 1;
                foreach (List<TopComparison> topComparison in _topComparisonList)
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
                                    names.Add(first_name);
                                    is_first_name_present = false;

                                }

                                if (!name.Equals(comparison.second_submission))
                                {
                                    second_name = comparison.second_submission;
                                    names.Add(second_name);
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

    }
}
