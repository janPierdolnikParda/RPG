using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra.DecTree.Enemies
{
    public class e_Assert : e_Node
    {
        Func<Enemy, bool> e_Predicate;

        public e_Assert(Func<Enemy, bool> e_predicate)
            : base()
        {
            e_Predicate = e_predicate;
        }

        public override Status e_Visit(Enemy en)
        {
            if (e_Predicate(en))
                return Status.Success;
            else
                return Status.Fail;
        }
    }
}
