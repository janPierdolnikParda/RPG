using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class ItemSword : DescribedProfile
    {
        public bool InUse;
        public int IloscRzutow;
        public int JakoscRzutow;
        public Vector3 HandleOffset;

        public new ItemSword Clone()
        {
            return (ItemSword)MemberwiseClone();
        }
    }
}
