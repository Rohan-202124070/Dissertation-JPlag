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
        Button[] buttons { get; set; }
        Groups group_form { get; set; }
        GroupsTopComaprision groupsTopComaprision { get; set; }
        string group_number { get; set; }
        HashSet<string> in_groups { get; set; }
        HashSet<TopComparison> in_top_comparision { get; set; }

        public Groups()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        internal void show_groups(GroupsTopComaprision groups, string type_view)
        {
            group_form = new Groups();
            group_form.Show();
            group_form.groupsTopComaprision = groups;
            group_form.buttons = new Button[groups.groups_names.Count];
            int i = 0;

            int x = 2;
            foreach (KeyValuePair<string, HashSet<string>> group in groups.groups_names)
            {
                group_form.buttons[i] = new Button();
                group_form.buttons[i].Text = group.Key;
                group_form.buttons[i].Name = group.Key;
                group_form.buttons[i].TextAlign = ContentAlignment.MiddleCenter;
                group_form.buttons[i].Font = new Font("Arial", 15, FontStyle.Regular);
                group_form.buttons[i].BackColor = Color.DarkSeaGreen;
                group_form.buttons[i].Size = new Size(195, 40);
                group_form.buttons[i].Location = new Point(1, x);
                group_form.buttons[i].Click += (sender, e) => { open_group_clicked(sender, e, group.Key, group.Value, groups.groups_top_comparision[group.Key], type_view); };
                group_form.panel1.Controls.Add(group_form.buttons[i]);
                i++;
                x += 50;
            }

            //https://stackoverflow.com/questions/1097853/how-can-i-trigger-the-default-button-on-a-form-without-clicking-it-winforms
            group_form.buttons[0].PerformClick();
        }

        void open_group_clicked(object sender, EventArgs e, string group_number, HashSet<string> in_groups, HashSet<TopComparison> in_top_comparision, string type_view)
        {
            this.group_form.panel2.Controls.Clear();
            this.group_form.group_number = group_number;
            this.group_form.in_groups = in_groups;
            this.group_form.in_top_comparision = in_top_comparision;
            foreach (Button button in this.group_form.buttons)
            {
                if (button.Name.Equals(group_number))
                {
                    button.BackColor = Color.Honeydew;
                }
                else
                {
                    button.BackColor = Color.DarkSeaGreen;
                }
            }

            if (type_view.Equals("chart_view"))
            {
                this.group_form.button2.BackColor = Color.Honeydew;
                this.group_form.button3.BackColor = Color.DarkSeaGreen;
                //Graph view
                //https://csharp.hotexamples.com/examples/Microsoft.Msagl.Drawing/Graph/-/php-graph-class-examples.html
                //https://github.com/albahari/automatic-graph-layout
                //https://stackoverflow.com/questions/61568496/visualize-data-structures-such-as-trees-and-graphs-in-c-sharp
                var graph = new Microsoft.Msagl.Drawing.Graph(group_number);
                graph.Attr.Color = Microsoft.Msagl.Drawing.Color.DarkSeaGreen;
                graph.Attr.BackgroundColor = Microsoft.Msagl.Drawing.Color.AliceBlue;
                foreach (TopComparison topComparison in in_top_comparision)
                {
                    Microsoft.Msagl.Drawing.Edge arco = graph.AddEdge(topComparison.first_submission, topComparison.second_submission);
                    arco.LabelText = topComparison.match_percentage.ToString();
                    arco.Label.FontSize = 6.0;
                    arco.Attr.Color = Microsoft.Msagl.Drawing.Color.DarkSeaGreen;
                    arco.SourceNode.Attr.Color = Microsoft.Msagl.Drawing.Color.DarkSeaGreen;
                    arco.TargetNode.Attr.Color = Microsoft.Msagl.Drawing.Color.DarkSeaGreen;
                }
                gViewer1.Graph = graph;
                this.group_form.panel2.Controls.Add(gViewer1);
            }
            else
            {
                Label names_table = new Label();
                names_table.Text = "List of Names";
                names_table.AutoSize = true;
                names_table.Location = new Point(130, 20);
                names_table.ForeColor = this.group_form.label3.ForeColor;
                names_table.Font = this.group_form.label1.Font;
                this.group_form.panel2.Controls.Add(names_table);

                Label comparision_table = new Label();
                comparision_table.Text = "List of Comparision";
                comparision_table.AutoSize = true;
                comparision_table.Location = new Point(200, 315);
                comparision_table.ForeColor = this.group_form.label3.ForeColor;
                comparision_table.Font = this.group_form.label1.Font;
                this.group_form.panel2.Controls.Add(comparision_table);

                this.group_form.button2.BackColor = Color.DarkSeaGreen;
                this.group_form.button3.BackColor = Color.Honeydew;
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

                DataGridView comparision_grid_view = new DataGridView();
                comparision_grid_view.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
                comparision_grid_view.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                comparision_grid_view.BackgroundColor = Color.WhiteSmoke;
                comparision_grid_view.ColumnHeadersDefaultCellStyle.Font =
                    new Font(comparision_grid_view.Font, FontStyle.Bold);

                comparision_grid_view.Location = new Point(10, 345);
                comparision_grid_view.Size = new Size(600, 250);
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        internal void button2_Click(object sender, EventArgs e)
        {
            this.group_form = (Groups)button2.FindForm();
            this.open_group_clicked(sender, e, this.group_number, this.in_groups, this.in_top_comparision, "chart_view");
        }

        internal void button3_Click(object sender, EventArgs e)
        {
            this.group_form = (Groups)button3.FindForm();
            this.open_group_clicked(sender, e, this.group_number, this.in_groups, this.in_top_comparision, "table_view");
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
