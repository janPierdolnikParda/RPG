using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra.DecTree
{
    public class Assert : Node
    {
        Func<Character, bool> Predicate;

        public Assert(Func<Character, bool> predicate)
            : base()
        {
            Predicate = predicate;
        }

        public override Status Visit(Character ch)
        {
            if (Predicate(ch))
                return Status.Success;
            else
                return Status.Fail;
        }
    }
}
