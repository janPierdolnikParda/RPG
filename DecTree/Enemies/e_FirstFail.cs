using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra.DecTree.Enemies
{
    public class e_FirstFail : e_Node
    {
        public e_FirstFail(params e_Node[] e_children)
            : base()
        {
            e_Children = e_children.ToList();
        }

        public override Status e_Visit(Enemy en)
        {
            foreach (e_Node e_node in e_Children)
            {
                Status status = e_node.e_Visit(en);
                if (status != Status.Success)
                    return status;
            }
            return Status.Success;
        }
    }
}
