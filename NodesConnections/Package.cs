using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodesConnections
{
    public class Package
    {
        public int ID;
        private static int lastID = 0;

        public int source;

        public int from;
        public int? via;
        public int? to;

        //public int[,] mtx;
        public object data;

        public int TTL = 100;

        public PackageType type;

        public Package(int source, int from, int? via, int? to, object data, PackageType type)
        {
            this.from = from;
            this.source = source;
            this.via = via;
            this.to = to;
            this.data = data;
            this.type = type;

            ID = lastID++;
        }

        public void NextStep()
        {
            TTL -= 10;
        }

        public enum PackageType
        {
            Echo,
            Route,
            Ping,
            Pong,
            Command,
            Message
        }
    }
}
