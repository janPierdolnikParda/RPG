using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Gra
{
    public class Dialog
    {
        public String ID;
        public Dictionary<String, TalkReaction> Reactions;
        public Dictionary<String, TalkEdge> Edges;
        public Dictionary<String, TalkNode> Nodes;
        public Dictionary<String, TalkReply> Replies;
        public Dictionary<String, String> EdgesToNodes;

        public Dialog()
        {
            Reactions = new Dictionary<String, TalkReaction>();
            Edges = new Dictionary<String, TalkEdge>();
            Nodes = new Dictionary<String, TalkNode>();
            Replies = new Dictionary<String, TalkReply>();
            EdgesToNodes = new Dictionary<String, String>();
        }
    }
}
