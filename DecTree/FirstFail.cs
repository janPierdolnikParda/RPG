using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra.DecTree
{
    public class FirstFail : Node
    {
        public FirstFail(params Node[] children)
            : base()
        {
            Children = children.ToList();
        }

        public override Status Visit(Character ch)
        {
            foreach (Node node in Children)
            {
                Status status = node.Visit(ch);
                if (status != Status.Success)
                    return status;
            }
            return Status.Success;
        }
    }
}
