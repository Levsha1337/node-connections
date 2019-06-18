using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodesConnections
{
    class Utils
    {
        public static float lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public static float map(float x, float x0, float x1, float a, float b)
        {
            float t = (x - x0) / (x1 - x0);
            return lerp(a, b, t);
        }

        public static float dist(float x0, float y0, float x1, float y1)
        {
            float dx = x0 - x1;
            float dy = y0 - y1;
            float ds = dx * dx + dy * dy;
            return (float)Math.Sqrt(ds);
        }

        public static float dist(PointF a, PointF b)
        {
            return (dist(a.X, a.Y, b.X, b.Y));
        }
    }
}
