using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra.DecTree.Enemies
{
    public class e_Job : e_Node
    {
        public Func<Enemy, bool> e_Func;

        public e_Job(Func<Enemy, bool> e_func)
            : base()
        {
            e_Func = e_func;
        }

        public override Status e_Visit(Enemy en)
        {
            if (e_Func(en))
                return Status.Success;
            else
                return Status.HalfSuccess;
        }
    }
}
