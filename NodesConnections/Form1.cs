using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NodesConnections
{
    public partial class Form1 : Form
    {
        Timer timer = new Timer();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Renderer.Initialize(pictureBox);

            timer.Tick += Timer_Tick;
            timer.Interval = 1000;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Renderer.Render();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            Global.showRadius = ((CheckBox)sender).Checked;
            Renderer.Render();
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Global.selected = null;

            foreach (Node n in Global.nodes)
            {
                PointF nPos = new PointF(n.x + Renderer.x + Node.DEFAULT_DRAW_RADIUS,
                                         n.y + Renderer.y + Node.DEFAULT_DRAW_RADIUS);
                if (Utils.dist(nPos, e.Location) < Node.DEFAULT_DRAW_RADIUS)
                {
                    Global.selected = n;
                    break;
                };
            }

            if (Global.pingStart != null)
            {
                //PathFinder pf = new PathFinder(Global.nodes);
                //List<Node> path = pf.GetPath(Global.pingStart, Global.selected);
                //MessageBox.Show()
            }

            Renderer.Render();
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point old;
            if (pictureBox.Tag == null)
            {
                pictureBox.Tag = e.Location;
                return;
            }

            old = (Point)pictureBox.Tag;

            pictureBox.Tag = e.Location;

            if (e.Button != MouseButtons.Left) return;


            Point ds = new Point(
                e.X - old.X,
                e.Y - old.Y
                );
            if (Global.selected == null)
            {
                Renderer.x += ds.X;
                Renderer.y += ds.Y;
            }
            else
            {
                Global.selected.x += ds.X;
                Global.selected.y += ds.Y;
                Global.selected.RecheckNear();
            }

            Renderer.Render();
        }

        //add
        private void Button1_Click(object sender, EventArgs e)
        { // add
            Node n = new Node(-Renderer.x + 200, -Renderer.y + 200, 100);
            Global.nodes.Add(n);
            foreach (Node nn in Global.nodes)
            {
                nn.CheckNear();
            }
            Renderer.Render();
        }

        //delete
        private void Button2_Click(object sender, EventArgs e)
        { // delete
            if (Global.selected == null) return;

            Global.selected.timer.Stop();
            Global.nodes.Remove(Global.selected);
            Global.selected = null;
            Renderer.Render();
        }

        //clear
        private void Button3_Click(object sender, EventArgs e)
        { // clear
            Global.nodes.Clear();
            Global.selected = null;
            Renderer.Render();
        }

        //term
        private void Button6_Click(object sender, EventArgs e)
        { // terminal
            if (Global.selected == null)
            {
                MessageBox.Show("Please, select node!");
                return;
            }
            Term t = new Term(Global.selected);
            t.Show();
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            button7.Text = "Ping to " + numericUpDown1.Value.ToString();
        }

        //initiate
        private void Button7_Click(object sender, EventArgs e)
        {
            try
            {
                int ID = (int)numericUpDown1.Value;
                Node initial = null;
                foreach (Node n in Global.nodes)
                {
                    if (n.ID == ID)
                    {
                        initial = n;
                        break;
                    }
                }
                Package pkg = new Package(
                    Global.selected.ID, Global.selected.ID, null, initial.ID, "Hi!", Package.PackageType.Ping
                    );
                Global.selected.tx.Add(pkg);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
        }

        // save
        private void Button4_Click(object sender, EventArgs e)
        {
            List<string> lines = new List<string>();
            lines.Add(Renderer.x.ToString());
            lines.Add(Renderer.y.ToString());

            lines.Add(Global.nodes.Count.ToString());

            for (int i = 0; i < Global.nodes.Count; i++)
            {
                Node n = Global.nodes.ElementAt(i);
                lines.Add(((int)n.x).ToString());
                lines.Add(((int)n.y).ToString());
                lines.Add(((int)n.radius).ToString());
            }

            File.WriteAllLines(Global.FILE_PATH, lines.ToArray());
        }

        // open
        private void Button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Accept", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            Global.nodes.Clear();
            string[] lines = File.ReadAllLines(Global.FILE_PATH);

            Renderer.x = Convert.ToInt32(lines[0]);
            Renderer.y = Convert.ToInt32(lines[1]);

            int count = Convert.ToInt32(lines[2]);

            for (int i = 0; i < count; i++)
            {
                int x  = Convert.ToInt32(lines[i * 3 + 0 + 3]);
                int y  = Convert.ToInt32(lines[i * 3 + 1 + 3]);
                int r  = Convert.ToInt32(lines[i * 3 + 2 + 3]);
                Node node = new Node(
                    x, y, r
                    );
                Global.nodes.Add(node);
            }

            foreach (Node n in Global.nodes)
            {
                n.CheckNear();
            }

            Renderer.Render();
        }
    }
}
