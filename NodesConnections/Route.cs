using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodesConnections
{
    public class Route
    {
        public int ID;
        public int Target;
        public int NextHop;
        public int TTL;

        public int HP = 10;

        private static int LastID = 0;

        public Route(int Target, int NextHop, int TTL)
        {
            this.Target = Target;
            this.NextHop = NextHop;
            this.TTL = TTL;

            this.ID = LastID++;
        }
    }
}
