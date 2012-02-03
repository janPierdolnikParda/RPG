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

        public Dialog()
        {
            Reactions = new Dictionary<String, TalkReaction>();
            Edges = new Dictionary<String, TalkEdge>();
            Nodes = new Dictionary<String, TalkNode>();
            Replies = new Dictionary<String, TalkReply>();

            if (File.Exists("Media\\Others\\dialog1.xml"))
            {
                XmlDocument File1 = new XmlDocument();
                File1.Load("Media\\Others\\dialog1.xml");
                XmlElement root = File1.DocumentElement;
                XmlNodeList Items = root.SelectNodes("//Dialogs//Dialog");

                foreach (XmlNode item in Items)
                {
                    XmlNodeList TalkReactions = item["Reactions"].ChildNodes;

                    foreach (XmlNode tr in TalkReactions)
                    {
                        TalkReaction justReaction = new TalkReaction();
                        Reactions.Add(tr["TalkReactionID"].InnerText, justReaction);
                    }

                    XmlNodeList TalkReplies = item["Replies"].ChildNodes;

                    foreach (XmlNode rep in TalkReplies)
                    {
                        TalkReply justReply = new TalkReply();
                        justReply.IsEnding = bool.Parse(rep["IsEnding"].InnerText);
                        justReply.Text.Add(new TalkText((rep["Text"].InnerText), 3.0f));

                        if (!justReply.IsEnding)
                            justReply.Reaction = Reactions[rep["TalkReaction"].InnerText];

                        Replies.Add(rep["TalkReplyID"].InnerText, justReply);
                    }

                    XmlNodeList TalkNodes = item["Nodes"].ChildNodes;

                    foreach (XmlNode tn in TalkNodes)
                    {
                        TalkNode justNode = new TalkNode();
                        justNode.Text.Add(new TalkText((tn["Text"].InnerText), 3.0f));

                        XmlNodeList RepliesInNode = tn["NodeReplies"].ChildNodes;

                        foreach (XmlNode rin in RepliesInNode)
                        {
                            justNode.Replies.Add(Replies[rin["ReplyID"].InnerText]);
                        }

                        XmlNodeList ActionsInNode = tn["Actions"].ChildNodes;

                        foreach (XmlNode ain in ActionsInNode)
                        {
                            List<ActionType> actionList = new List<ActionType>();
                            actionList.Add((ActionType)int.Parse(ain["ActionType"].InnerText));
                            justNode.AddActions(actionList);
                        }

                        //justNode.Edge = Edges[tn["TalkEdgeID"].InnerText];
                        justNode.Quest = Quests.Quest1.Name;

                        Nodes.Add(tn["TalkNodeID"].InnerText, justNode);
                    }

                    XmlNodeList TalkEdges = item["Edges"].ChildNodes;

                    foreach (XmlNode te in TalkEdges)
                    {
                        TalkEdge justEdge = new TalkEdge(Nodes[te["ToWhere"].InnerText]);
                        Edges.Add(te["TalkEdgeID"].InnerText, justEdge);
                        Reactions[te["FromWhere"].InnerText].Edges.Add(Edges[te["TalkEdgeID"].InnerText]);
                    }
                }
            }

            /*TalkReaction firstOne = new TalkReaction();
            Reactions.Add("startowa", firstOne);

            TalkReply firstReply = new TalkReply();
            firstReply.Text.Add(new TalkText("IM NOT ENDING", 3.0f));
            firstReply.IsEnding = false;
            firstReply.Reaction = Reactions["startowa"];
            Replies.Add("1", firstReply);

            TalkReply secondReply = new TalkReply();
            secondReply.Text.Add(new TalkText("IM ENDING!", 3.0f));
            secondReply.IsEnding = true;
            Replies.Add("2", secondReply);

            TalkNode firstNode = new TalkNode();
            List<ActionType> listaAkcji = new List<ActionType>();
            //listaAkcji.Add(ActionType.MakeFirstFalse);
            listaAkcji.Add(ActionType.GiveQuest);
            firstNode.AddActions(listaAkcji);
            firstNode.Text.Add(new TalkText("bla", 3.0f));
            firstNode.Replies.Add(Replies["1"]);
            firstNode.Replies.Add(Replies["2"]);
            firstNode.Quest = "Pierwszy";
            Nodes.Add("pierwszynode", firstNode);

            TalkEdge edz = new TalkEdge(Nodes["pierwszynode"]);
            Reactions["startowa"].Edges.Add(edz);

            firstNode.Edge = edz;*/
        }
    }
}
