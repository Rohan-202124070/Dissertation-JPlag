using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace JPlag
{

    public partial class JPlagReport : Form
    {
        DataGridView comparision_grid_view = new DataGridView();
        Dictionary<string, string> distribution = new Dictionary<string, string>();
        string result_path = "";
        public JPlagReport()
        {
            InitializeComponent();
        }

        private void PlagiarismReport_Load(object sender, EventArgs e)
        {

        }

        public void View_Plagiarism_Report(string path)
        {
            JPlagReport plagiarismReport = new JPlagReport();
            plagiarismReport.Show();
            result_path = path;
            using (StreamReader r = new StreamReader(path + "\\overview.json"))
            {
                string json = r.ReadToEnd();
                Overview overviews = JsonConvert.DeserializeObject<Overview>(json);

                plagiarismReport.label9.Text = overviews.submission_folder_path[0];
                plagiarismReport.label10.Text = overviews.language;
                plagiarismReport.label11.Text = overviews.match_sensitivity.ToString();
                plagiarismReport.label12.Text = overviews.submission_ids.Count.ToString();
                plagiarismReport.label13.Text = overviews.date_of_execution;
                plagiarismReport.label14.Text = overviews.execution_time.ToString();

                // comparision grid view 
                comparision_grid_view = new DataGridView();
                comparision_grid_view.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
                comparision_grid_view.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                comparision_grid_view.BackgroundColor = Color.WhiteSmoke;
                comparision_grid_view.ColumnHeadersDefaultCellStyle.Font =
                    new Font(comparision_grid_view.Font, FontStyle.Bold);
                comparision_grid_view.Size = new Size(500, 600);
                plagiarismReport.panel1.Controls.Add(comparision_grid_view);
                comparision_grid_view.ScrollBars = ScrollBars.Both;
                comparision_grid_view.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                int comapre_count = 1;

                comparision_grid_view.AutoGenerateColumns = true;
                comparision_grid_view.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                comparision_grid_view.ColumnCount = 4;
                comparision_grid_view.Columns[0].Name = "No";
                comparision_grid_view.Columns[1].Name = "First Student Number/Name";
                comparision_grid_view.Columns[2].Name = "Second Student Number/Name";
                comparision_grid_view.Columns[3].Name = "Match percentage";
                comparision_grid_view.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                comparision_grid_view.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);

                int zero_to_twenty_count = 0;
                int twenty_to_forty_count = 0;
                int forty_to_fifty_count = 0;
                int fifty_to_sixty_count = 0;
                int sixty_to_seventy_count = 0;
                int seventy_to_eighty_count = 0;
                int eighty_to_nighty_count = 0;
                int nighty_to_hundred_count = 0;

                foreach (Metric metric in overviews.metrics)
                {
                    foreach (TopComparison topComparison in metric.topComparisons)
                    {
                        if (topComparison.match_percentage >= 0.0 && topComparison.match_percentage <= 20.0)
                        {
                            zero_to_twenty_count++;
                        }

                        if (topComparison.match_percentage >= 20.0 && topComparison.match_percentage <= 40.0)
                        {
                            twenty_to_forty_count++;
                        }

                        if (topComparison.match_percentage >= 40.0 && topComparison.match_percentage <= 50.0)
                        {
                            forty_to_fifty_count++;

                        }

                        if (topComparison.match_percentage >= 50.0 && topComparison.match_percentage <= 60.0)
                        {
                            fifty_to_sixty_count++;
                        }

                        if (topComparison.match_percentage >= 60.0 && topComparison.match_percentage <= 70.0)
                        {
                            sixty_to_seventy_count++;
                        }

                        if (topComparison.match_percentage >= 70.0 && topComparison.match_percentage <= 80.0)
                        {
                            seventy_to_eighty_count++;
                        }

                        if (topComparison.match_percentage >= 80.0 && topComparison.match_percentage <= 90.0)
                        {
                            eighty_to_nighty_count++;
                        }

                        if (topComparison.match_percentage >= 90.0 && topComparison.match_percentage <= 100.0)
                        {
                            nighty_to_hundred_count++;
                        }

                        string[] row = { comapre_count.ToString(), topComparison.first_submission, topComparison.second_submission, topComparison.match_percentage.ToString() };
                        comparision_grid_view.Rows.Add(row);
                        comparision_grid_view.DefaultCellStyle.Font = new Font("Tahoma", 9);
                        comapre_count++;
                    }
                }
                plagiarismReport.chart1.Series["Distribution Percentage"].Points.AddXY("0-20%", zero_to_twenty_count.ToString());
                plagiarismReport.chart1.Series["Distribution Percentage"].Points.AddXY("20-40%", twenty_to_forty_count.ToString());
                plagiarismReport.chart1.Series["Distribution Percentage"].Points.AddXY("40-50%", forty_to_fifty_count.ToString());
                plagiarismReport.chart1.Series["Distribution Percentage"].Points.AddXY("50-60%", fifty_to_sixty_count.ToString());
                plagiarismReport.chart1.Series["Distribution Percentage"].Points.AddXY("60-70%", sixty_to_seventy_count.ToString());
                plagiarismReport.chart1.Series["Distribution Percentage"].Points.AddXY("70-80%", seventy_to_eighty_count.ToString());
                plagiarismReport.chart1.Series["Distribution Percentage"].Points.AddXY("80-90%", eighty_to_nighty_count.ToString());
                plagiarismReport.chart1.Series["Distribution Percentage"].Points.AddXY("90-100%", nighty_to_hundred_count.ToString());
                plagiarismReport.chart1.GetToolTipText += this.chart1_GetToolTipText;
            }
        }

        //https://stackoverflow.com/questions/11260843/getting-data-from-selected-datagridview-row-and-which-event
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            Comparision comparision = new Comparision();
            DataGridViewRow row = this.comparision_grid_view.Rows[e.RowIndex];
            string first_name = row.Cells["First Student Number/Name"].Value.ToString();
            string second_name = row.Cells["Second Student Number/Name"].Value.ToString();
            bool fileExist = File.Exists(result_path + "\\" + first_name + "-" + second_name + ".json");
            if (fileExist)
            {
                using (StreamReader r = new StreamReader(result_path + "\\" + first_name + "-" + second_name + ".json"))
                {
                    string json = r.ReadToEnd();
                    SubmissionMatch submissionMatch = JsonConvert.DeserializeObject<SubmissionMatch>(json);
                    comparision.View_Comparision(submissionMatch);
                }
            }
            else
            {
                MessageBox.Show("Comparision file has not generated, contact JPlag admin.\n", "Plagiarism Comparision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        //https://stackoverflow.com/questions/10648828/see-values-of-chart-points-when-the-mouse-is-on-points
        private void chart1_GetToolTipText(object sender, ToolTipEventArgs e)
        {
            // Check selected chart element and set tooltip text for it
            switch (e.HitTestResult.ChartElementType)
            {
                case ChartElementType.DataPoint:
                    var dataPoint = e.HitTestResult.Series.Points[e.HitTestResult.PointIndex];
                    e.Text = string.Format("Matches: {0}", dataPoint.YValues[0]);
                    break;
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
