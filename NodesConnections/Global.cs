using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NodesConnections
{
    class Global
    {
        public static List<Node> nodes = new List<Node>();
        public static Node selected = null;
        public static Node pingStart = null;
        public static bool showRadius = false;
        public const string FILE_PATH = "nodes_info.txt";

        public static Node GetNode(int ID)
        {
            foreach (Node n in Global.nodes)
            {
                if (ID == n.ID) return n;
            }

            return null;
        }
    }
}
