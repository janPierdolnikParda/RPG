using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class ActivityManager
    {
        public List<Activity> Activities;
        public int Index;
        public bool InProgress;
        public bool Repeat;

        public ActivityManager()
        {
            Index = 0;
            Activities = new List<Activity>();
            InProgress = false;
            Repeat = false;
        }

        public void Reset()
        {
            Index = 0;
        }
    }
}
