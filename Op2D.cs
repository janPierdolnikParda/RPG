using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    class Op2D
    {
        public static Vector3 XZ = new Vector3(1, 0, 1);

        public static float Cross(Vector3 a, Vector3 b)
        {
            Vector3 axz = (a * XZ).NormalisedCopy;
            Vector3 bxz = (b * XZ).NormalisedCopy;
            return new Vector2(axz.x, axz.z).CrossProduct(new Vector2(bxz.x, bxz.z));
        }

        public static float TriArea(Vector3 a, Vector3 b)
        {
            return 0.5f * (a * XZ).CrossProduct(b * XZ).Length;
        }

        public static float Dist(Vector3 a, Vector3 b)
        {
            return ((a * XZ) - (b * XZ)).Length;
        }
    }
}
