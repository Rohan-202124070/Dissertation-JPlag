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
    public partial class Groups : Form
    {
        GroupsTopComaprision groupsTopComaprision = new GroupsTopComaprision();
        public Groups()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        internal void show_groups(GroupsTopComaprision groups)
        {
            groupsTopComaprision = groups;
            var group_form = new Groups();
            group_form.Show();
            group_form.label2.Hide();
            group_form.label4.Hide();
            Button[] buttons = new Button[groups.groups_names.Count];
            int i = 0;
            
            int x = 2;
            foreach (KeyValuePair<string, HashSet<string>> group in groups.groups_names)
            {
                buttons[i] = new Button();
                buttons[i].Text = group.Key;
                buttons[i].TextAlign = ContentAlignment.MiddleCenter;
                buttons[i].Font = new Font("Arial", 15, FontStyle.Regular);
                buttons[i].BackColor = Color.DarkSeaGreen;
                buttons[i].Size = new Size(195, 40);
                buttons[i].Location = new Point(1, x);
                /*buttons[i].Left = 10;
                buttons[i].Top = basey;*/
                /*buttons[i].Left = 50;
                buttons[i].Top = (i + 1) * 80;*/
                buttons[i].Click += (sender, e) => { open_group_clicked(sender, e, group.Value, groups.groups_top_comparision[group.Key]); };
                //group_form.Controls.Add(buttons[i]);
                group_form.panel1.Controls.Add(buttons[i]);
                i++;
                x += 50;
            }

            void open_group_clicked(object sender, EventArgs e, HashSet<string> in_groups, HashSet<TopComparison> in_top_comparision)
            {
                group_form.label2.Show();
                group_form.label4.Show();
                group_form.panel2.Controls.Clear();
                DataGridView name_grid_view = new DataGridView();
                name_grid_view.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
                name_grid_view.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                name_grid_view.BackgroundColor = Color.WhiteSmoke;
                name_grid_view.ColumnHeadersDefaultCellStyle.Font =
                    new Font(name_grid_view.Font, FontStyle.Bold);

                name_grid_view.Location = new Point(10, 50);
                name_grid_view.Size = new Size(400, 250);
                group_form.panel2.Controls.Add(name_grid_view);
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
                comparision_grid_view.BackgroundColor = Color.WhiteSmoke;
                comparision_grid_view.ColumnHeadersDefaultCellStyle.Font =
                    new Font(comparision_grid_view.Font, FontStyle.Bold);

                comparision_grid_view.Location = new Point(10, 345);
                comparision_grid_view.Size = new Size(700, 250);
                group_form.panel2.Controls.Add(comparision_grid_view);
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

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
