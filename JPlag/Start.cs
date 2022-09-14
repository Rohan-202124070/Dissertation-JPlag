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
    public partial class JPlag : Form
    {
        public JPlag()
        {
            InitializeComponent();
            button1.BackColor = Color.AliceBlue;
            Home home = new Home();
            home.TopLevel = false;
            home.AutoScroll = true;
            this.panel1.Controls.Add(home);
            home.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.BackColor = Color.DodgerBlue;
            button3.BackColor = Color.DodgerBlue;
            button4.BackColor = Color.DodgerBlue;
            button2.BackColor = Color.AliceBlue;
            this.panel1.Controls.Clear();
            Administartive myForm = new Administartive();
            myForm.TopLevel = false;
            myForm.AutoScroll = true;
            this.panel1.Controls.Add(myForm);
            myForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button2.BackColor = Color.DodgerBlue;
            button3.BackColor = Color.DodgerBlue;
            button4.BackColor = Color.DodgerBlue;
            button1.BackColor = Color.AliceBlue;
            this.panel1.Controls.Clear();
            Home home = new Home();
            home.TopLevel = false;
            home.AutoScroll = true;
            this.panel1.Controls.Add(home);
            home.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button2.BackColor = Color.DodgerBlue;
            button1.BackColor = Color.DodgerBlue;
            button4.BackColor = Color.DodgerBlue;
            button3.BackColor = Color.AliceBlue;
            this.panel1.Controls.Clear();
            Manage manage = new Manage();
            manage.TopLevel = false;
            manage.AutoScroll = true;
            this.panel1.Controls.Add(manage);
            manage.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button2.BackColor = Color.DodgerBlue;
            button1.BackColor = Color.DodgerBlue;
            button4.BackColor = Color.AliceBlue;
            button3.BackColor = Color.DodgerBlue;
            this.panel1.Controls.Clear();
            Help help = new Help();
            help.TopLevel = false;
            help.AutoScroll = true;
            this.panel1.Controls.Add(help);
            help.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
