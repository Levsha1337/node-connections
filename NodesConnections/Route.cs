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

        public int HP = MAX_HP;
        public const int MAX_HP = 6;

        private static int LastID = 0;

        public Route(int Target, int NextHop, int TTL)
        {
            this.Target = Target;
            this.NextHop = NextHop;
            this.TTL = TTL;

            this.ID = LastID++;
        }

        public override string ToString()
        {
            return "Route #" + this.ID.ToString() + " (to " + this.Target.ToString() + ")";
        }
    }
}
