using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public enum ActivityType
    {
        WAIT,
        WALK
    }

    public class Activity
    {
        public Vector3 v3;
        public String s;
        public long i;
        public long i2;
        public bool b;

        public ActivityType Type;
    }
}
