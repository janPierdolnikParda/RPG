using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    public abstract class SelectableObject : GameObject
    {
        public bool IsContainer;

        public abstract String DisplayName
        {
            get;
            set;
        }
        public abstract String Description
        {
            get;
            set;
        }
        public abstract Vector3 DisplayNameOffset
        {
            get;
            set;
        }

        public bool ShowName;
        public TalkReaction TalkRoot;
    }
}
