using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NodesConnections
{
    public class Node
    {
        public Node(float x, float y, float radius)
        {
            ID = LastID++;
            this.x = x;
            this.y = y;
            this.radius = radius;
            this.term = new Term(this);

            this.timer = new Timer();
            this.timer.Tick += tick;
            this.timer.Interval = 500;
            this.timer.Start();
        }

        public int ID;

        public float x;
        public float y;
        public float radius;
        public PointF xy { get => new PointF((float)x, (float)y); }
        public const float DEFAULT_DRAW_RADIUS = 15.0f;

        private static int LastID = 0;

        public List<Package> rx = new List<Package>();
        public List<Package> tx = new List<Package>();
        public Dictionary<int, int> xx = new Dictionary<int, int>();

        public List<Route> routes = new List<Route>();

        private List<Node> near = new List<Node>();

        public Term term;

        public Timer timer;

        public override string ToString()
        {
            return "Node #" + this.ID.ToString() + "(" + this.routes.Count.ToString() + " routes)";
        }

        private void tick(object sender, EventArgs e)
        {
            List<Route> newRoutes = new List<Route>();
            foreach (Route r in this.routes)
            {
                r.HP--;
                bool viaAlive = false;
                foreach (Node n in this.near)
                {
                    if (n.ID == r.NextHop) viaAlive = true;
                }
                if (viaAlive && r.HP > 0) newRoutes.Add(r);
            }
            this.routes = newRoutes;

            Dictionary<int, int> xy = new Dictionary<int, int>();
            foreach (KeyValuePair<int, int> kvp in xx)
            {
                int k = kvp.Key;
                int v = kvp.Value;
                v--;
                if (v > 0) xy.Add(k, v);
            }
            xx = xy;

            this.processRx();

            Random rnd = new Random();
            if (rnd.Next(100) > -10)
            {
                Package pkg = new Package(
                    ID, ID, null, null, ID, Package.PackageType.Echo
                );
                this.tx.Add(pkg);
            }

            if (rnd.Next(100) > -50)
            {
                Package pkg2 = new Package(
                    ID, ID, null, null, routes, Package.PackageType.Route
                );
                this.tx.Add(pkg2);
            }

            this.sendTx();
        }

        public void processRx()
        {
            Debug.WriteLine("{0}\t\thave {1}\t\troutes!", ID, routes.Count);
            Package[] rxx = new Package[rx.Count];
            rx.CopyTo(rxx);
            foreach (Package pack in rxx.ToList())
            {
                if (pack.TTL < 0) continue;

                if (xx.ContainsKey(pack.ID)) continue;
                this.xx.Add(pack.ID, 25);
                if (pack.TTL <= 0) continue;

                if (pack.via == ID)
                {
                    this.tx.Add(pack);
                }

                if (pack.to != null && pack.to != ID) {
                    if (pack.via != null && pack.via != ID)
                    {
                        //pack.from = ID;
                        //this.tx.Add(pack);
                    }

                    continue;
                };

                switch (pack.type)
                {
                    case Package.PackageType.Echo:
                        {
                            int NodeID = (int)pack.data;
                            Route r = new Route(NodeID, NodeID, 100);
                            bool has = false;
                            Route[] temp = new Route[this.routes.Count];
                            this.routes.CopyTo(temp);
                            List<Route> newRoutes = temp.ToList();
                            foreach (Route t in this.routes)
                            {
                                if (r.Target == t.Target)
                                {
                                    has = true;
                                    if (r.TTL > t.TTL)
                                    {
                                        newRoutes.Remove(t);
                                        newRoutes.Add(r);
                                    }
                                    else if (r.TTL == t.TTL)
                                    {
                                        t.HP = Route.MAX_HP;
                                    }
                                }
                            }
                            if (!has) this.routes.Add(r);
                            else this.routes = newRoutes;

                            if (!has && false)
                            {
                                Package pkg = new Package(
                                    ID, ID, null, null, this.routes, Package.PackageType.Route
                                    );
                                tx.Add(pkg);
                            }
                        }
                        break;

                    case Package.PackageType.Route:
                        {
                            List<Route> routesPack = (List<Route>)pack.data;
                            Route[] temp = new Route[this.routes.Count];
                            this.routes.CopyTo(temp);
                            List<Route> newRoutes = temp.ToList();
                            
                            foreach (Route r in this.routes)
                            {
                                if (r.NextHop == pack.from && r.Target != pack.from)
                                {
                                    bool stillHave = false;
                                    foreach (Route t in routesPack)
                                    {
                                        if (t.Target == r.Target)
                                        {
                                            stillHave = true;
                                            break;
                                        }
                                    }
                                    if (!stillHave) newRoutes.Remove(r);
                                }
                            }
                            this.routes = newRoutes;

                            foreach (Route rr in routesPack)
                            {
                                if (rr.Target == ID) continue;
                                if (rr.NextHop == ID) continue;
                                Route r = new Route(rr.Target, pack.from, rr.TTL);
                                r.TTL -= 10;
                                bool has = false;
                                Route[] tempRoutes = new Route[this.routes.Count];
                                this.routes.CopyTo(tempRoutes);
                                foreach (Route t in tempRoutes)
                                {
                                    if (r.Target == t.Target)
                                    {
                                        has = true;
                                        if (r.TTL > t.TTL-10)
                                        {
                                            newRoutes.Remove(t);
                                            newRoutes.Add(r);
                                        }
                                        else
                                        {
                                            t.HP = Route.MAX_HP;
                                        }
                                    }
                                }

                                if (!has)
                                {
                                    newRoutes.Add(r);
                                }

                                this.routes = newRoutes;

                                //Package pkg = new Package(
                                //    ID, ID, null, null, this.routes, Package.PackageType.Route 
                                //    );
                                //tx.Add(pkg);
                            }
                        }
                        break;

                    case Package.PackageType.Ping:
                        {
                            MessageBox.Show(ID.ToString() + ": " + pack.source.ToString() + " pings: " + pack.data.ToString() + "( TTL: " + pack.TTL + ")");
                            Package pkg = new Package(
                                this.ID, this.ID, null, pack.source, pack.data, Package.PackageType.Pong
                                );
                            this.tx.Add(pkg);
                        }
                        break;

                    case Package.PackageType.Pong:
                        {
                            MessageBox.Show(ID.ToString() + ": " + pack.source.ToString() + " pongs: " + pack.data.ToString() + "( TTL: " + pack.TTL + ")");
                        }
                        break;

                    default: break;
                }
            }
            this.rx.Clear();
        }

        public void sendTx()
        {
            foreach (Package pack in this.tx)
            {
                pack.from = ID;
                int? via = null;
                foreach (Route r in this.routes)
                {
                    if (r.Target == pack.to)
                    {
                        via = r.NextHop;
                        break;
                    }
                }
                pack.via = via;
                pack.NextStep();

                foreach (Node n in this.near)
                {
                    try
                    {
                        if (n.xx.ContainsKey(pack.ID)) continue;
                        if (n.rx.Contains(pack)) continue;
                        n.rx.Add(pack);
                    }
                    catch (Exception Ex)
                    {
                        Debug.WriteLine("{0} already has {1} (from {2})", n.ID, pack.ID, this.ID);
                    }
                }
            }
            this.tx.Clear();
        }

        public void RecheckNear()
        {
            foreach (Node n in this.near)
            {
                n.CheckNear();
            }

            this.CheckNear();
        }

        public void CheckNear()
        {
            this.near.Clear();
            foreach (Node n in Global.nodes)
            {
                if (n.ID == this.ID) continue;

                float ds = Utils.dist(n.xy, this.xy);
                if (ds < this.radius && !this.near.Contains(n))
                    this.near.Add(n);
            }
        }
    }
}
