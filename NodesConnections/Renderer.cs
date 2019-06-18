using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NodesConnections
{
    class Renderer
    {
        private static PictureBox pictureBox;
        private static Bitmap bmp;
        private static Graphics g;

        public static float x;
        public static float y;
        public static void Initialize(PictureBox pb)
        {
            pictureBox = pb;
            bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = bmp;
            g = Graphics.FromImage(bmp);
        }

        public static void Render()
        {
            g.Clear(Color.White);

            float nr = Node.DEFAULT_DRAW_RADIUS;
            float nr2 = nr * 2.0f;
            foreach (Node n in Global.nodes)
            {
                Brush br = Brushes.node_fill;
                Pen pen = Pens.node_out;

                if (n == Global.selected)
                {
                    br = Brushes.node_fill_sel;
                    pen = Pens.node_out_sel;
                }

                if (Global.showRadius)
                {
                    g.FillEllipse(Brushes.radius_fill,
                        x + n.x - n.radius + nr,
                        y + n.y - n.radius + nr,
                        n.radius * 2, n.radius * 2
                        );
                }

                foreach (Route r in n.routes)
                {
                    if (r.TTL != 100) continue;
                    Node m = Global.GetNode(r.NextHop);
                    try
                    {
                        g.DrawLine(Pens.link_out,
                            x + n.x + nr, y + n.y + nr,
                            x + m.x + nr, y + m.y + nr);
                    }
                    catch { }
                }

                g.FillEllipse(br, x + n.x, y + n.y, nr2, nr2);
                g.DrawEllipse(pen, x + n.x, y + n.y, nr2, nr2);
                g.DrawString(n.ID.ToString(), new Font(FontFamily.Families.ElementAt(0), 20.0f), 
                    Brushes.text_fill, n.x + x, n.y + y + 5.0f);

                foreach (Node m in Global.nodes)
                {
                    if (n == m) continue;

                }
            }

            g.Flush();
            pictureBox.Invalidate();
        }

        private class Brushes
        {
            public static Brush node_fill     
                = new SolidBrush(Color.FromArgb(128, Color.Pink));

            public static Brush node_fill_sel 
                = new SolidBrush(Color.FromArgb(128, Color.LightBlue));

            public static Brush radius_fill   
                = new SolidBrush(Color.FromArgb(64, Color.LightGreen));

            public static Brush text_fill
                = new SolidBrush(Color.FromArgb(255, Color.Black));
        }

        private class Pens
        {
            public static Pen node_out     
                = new Pen(Color.Red, 2.0f);

            public static Pen node_out_sel 
                = new Pen(Color.Blue, 2.0f);

            public static Pen radius_out
                = new Pen(Color.Green, 1.0f);

            public static Pen link_out
                = new Pen(Color.Purple, 3.0f);
        }
    }
}
