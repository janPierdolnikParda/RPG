using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    public abstract class GameObject
    {
        public String Name;

        public abstract void Update();

        public abstract Vector3 Position
        {
            get;
            set;
        }

        public abstract Quaternion Orientation
        {
            get;
            set;
        }

        public virtual void Destroy()
        {
            Exists = false;
        }
        public bool Exists = true;

    }
}
