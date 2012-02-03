using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class TalkEdge : TalkConditional
    {
        public TalkNode TalkNode;

        public bool FirstTalk = true;
        public bool GotQuest = true;
        public bool IsQuestDone = true;
        public bool IsQuestFinished = true;
        public bool Other = true;

        public TalkEdge(TalkNode target)
        {
            TalkNode = target;
            base.Conditions += (() => FirstTalk && GotQuest && IsQuestDone && IsQuestFinished);
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
