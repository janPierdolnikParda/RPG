using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra.DecTree
{
    public abstract class Node
    {
        public enum Status
        {
            Fail,
            HalfSuccess,
            Success
        }

        public List<Node> Children = new List<Node>();

        public abstract Status Visit(Character ch);
    }
}
