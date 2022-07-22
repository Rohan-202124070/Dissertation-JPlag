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
    public partial class Comparision : Form
    {
        public Comparision()
        {
            InitializeComponent();
        }

        internal void View_Comparision(SubmissionMatch submissionMatch)
        {
            Comparision comparision = new Comparision();
            comparision.richTextBox1.Lines = submissionMatch.files_of_first_submission[0].lines.ToArray();
            comparision.richTextBox2.Lines = submissionMatch.files_of_second_submission[0].lines.ToArray();
            comparision.Show();
            
            foreach(Match match in submissionMatch.matches)
            {

                for (int i = match.start_in_first - 1; i < match.end_in_first - 1; i++)
                {
                    string text = comparision.richTextBox1.Lines[i];
                    comparision.richTextBox1.Select(comparision.richTextBox1.GetFirstCharIndexFromLine(i), text.Length);
                    comparision.richTextBox1.SelectionColor = Color.Red;
                }

                for (int i = match.start_in_second - 1; i < match.end_in_second - 1; i++)
                {
                    string text = comparision.richTextBox1.Lines[i];
                    comparision.richTextBox2.Select(comparision.richTextBox1.GetFirstCharIndexFromLine(i), text.Length);
                    comparision.richTextBox2.SelectionColor = Color.Red;
                }
            }
        }

            private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Comparision_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
