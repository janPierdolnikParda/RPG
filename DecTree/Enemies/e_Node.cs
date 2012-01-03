using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra.DecTree.Enemies
{
    public abstract class e_Node
    {
        public enum Status
        {
            Fail,
            HalfSuccess,
            Success
        }

        public List<e_Node> e_Children = new List<e_Node>();

        public abstract Status e_Visit(Enemy en);
    }
}
