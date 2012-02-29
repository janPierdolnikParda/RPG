using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public interface ISomethingMoving
    {
        Statistics Statistics
        {
            get;
            set;
        }

        Character.FriendType FriendlyType
        {
            get;
            set;
        }

        float ZasiegWzroku
        {
            get;
            set;
        }

        float ZasiegOgolny
        {
            get;
            set;
        }

        Enemy.StateTypes State
        {
            get;
            set;
        }

        int DropExp
        {
            get;
            set;
        }

        String ProfName
        {
            get;
            set;
        }
    }
}
