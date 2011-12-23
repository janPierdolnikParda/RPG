using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra.DecTree
{
    public class Job : Node
    {
        public Func<Character, bool> Func;

        public Job(Func<Character, bool> func)
            : base()
        {
            Func = func;
        }

        public override Status Visit(Character ch)
        {
            if (Func(ch))
                return Status.Success;
            else
                return Status.HalfSuccess;
        }
    }
}
