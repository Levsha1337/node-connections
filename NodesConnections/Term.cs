using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NodesConnections
{
    public partial class Term : Form
    {
        private Node node;
        public Term(Node selected)
        {
            InitializeComponent();

            this.node = selected;
        }

        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                string cmd = textBox1.Text;
                textBox1.Text = "";

                Label lbl = new Label
                {
                    Margin = new Padding(5),
                    Height = 20,
                    Text = "> " + cmd
                };
                listView1.Controls.Add(lbl);
            }
        }

        private void Term_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
