using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JPlag
{
    public partial class ViewGroup : Form
    {
        public ViewGroup()
        {
            InitializeComponent();
        }

        private void ViewGroup_Load(object sender, EventArgs e)
        {

        }

        internal void Show_group_details(List<string> in_groups, List<TopComparison> in_top_comparision)
        {
            var view_group = new ViewGroup();
            view_group.Show();
            view_group.WindowState = FormWindowState.Maximized;
            // names table 
            DataGridView name_grid_view = new DataGridView();
            name_grid_view.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            name_grid_view.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            name_grid_view.BackgroundColor = Color.White;
            name_grid_view.ColumnHeadersDefaultCellStyle.Font =
                new Font(name_grid_view.Font, FontStyle.Bold);

            name_grid_view.Location = new Point(20, 150);
            name_grid_view.Size = new Size(500, 220);
            view_group.Controls.Add(name_grid_view);
            name_grid_view.ScrollBars = ScrollBars.Both;
            name_grid_view.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            int count = 1;

            name_grid_view.AutoGenerateColumns = true;
            name_grid_view.AutoSizeRowsMode =
            DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            name_grid_view.ColumnCount = 2;
            name_grid_view.Columns[0].Name = "No";
            name_grid_view.Columns[1].Name = "Student Number/Name";
            name_grid_view.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            foreach (string name in in_groups)
            {
                string[] row = { count.ToString(), name };
                name_grid_view.Rows.Add(row);
                count++;
            }

            // comparision grid view 
            DataGridView comparision_grid_view = new DataGridView();
            comparision_grid_view.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            comparision_grid_view.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            comparision_grid_view.ColumnHeadersDefaultCellStyle.Font =
                new Font(comparision_grid_view.Font, FontStyle.Bold);

            comparision_grid_view.Location = new Point(550, 150);
            comparision_grid_view.Size = new Size(600, 220);
            view_group.Controls.Add(comparision_grid_view);
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

            foreach (TopComparison topComparison in in_top_comparision)
            {
                string[] row = { comapre_count.ToString(), topComparison.first_submission, topComparison.second_submission, topComparison.match_percentage.ToString() };
                comparision_grid_view.Rows.Add(row);
                comapre_count++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
