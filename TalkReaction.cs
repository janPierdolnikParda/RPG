using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class TalkEdge : TalkConditional
    {
        public TalkNode TalkNode;

        public TalkEdge(TalkNode target)
        {
            TalkNode = target;
        }
    }

    public class TalkReaction
    {
        public List<TalkEdge> Edges;

        public TalkReaction()
        {
            Edges = new List<TalkEdge>();
        }

        public TalkNode PickNode()
        {
            foreach (TalkEdge edge in Edges)
            {
                if (edge.IsConditionFulfilled())
                {
                    edge.TalkNode.CallActions();
                    return edge.TalkNode;
                }
            }
            throw new Exception("Reaction without edges!");
        }
    }
}
