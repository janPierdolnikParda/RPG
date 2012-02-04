using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;
using System.Xml;
using System.IO;

namespace Gra
{
    class Conversations
    {
        public static TalkReaction ConversationRoot;
        static TalkNode FirstGreeting;
        static TalkReply WhereIAm;
        static TalkReply WhatAboutQuest;
        static TalkReaction RespWhatIsIt;
        static TalkNode ItIsATest;
        static TalkReply IDontWantToBother;
        static TalkReply Ending;
        static TalkNode HelloAgain;
        static TalkNode QuestIsDone;
        static TalkNode QuestIsNotDone;
        static TalkReaction Lol;

        public static Dictionary<String, Dialog> D;

        public static bool FirstTalk = true;

        static Conversations()
        {
            D = new Dictionary<String, Dialog>();

            ConversationRoot = new TalkReaction();

            FirstGreeting = new TalkNode();

            TalkEdge firstTalkEdge = new TalkEdge(FirstGreeting);
            firstTalkEdge.Conditions += (() => {

                foreach (Quest Q in NPCManager.npc.Quests)
                {
                    foreach (Quest P in Engine.Singleton.HumanController.Character.ActiveQuests.Quests)
                        if (Q.Name == P.Name)
                            return false;
                   
                }

                return true;

            }
                
                );            
            ConversationRoot.Edges.Add(firstTalkEdge);
            FirstGreeting.Text.Add(new TalkText("Ej, stary, jest sprawa. Przynies mi butelke, to dam Ci miecz.", 3.0f));
            FirstGreeting.Actions += (() => { FirstTalk = false; Engine.Singleton.HumanController.Character.ActiveQuests.Add(Quests.Quest1); });

            // funkcja anonimowa przyjmuje jeden argument typu elementów listy i zwraca wartość typu bool
            //bool mniejszeOd50 = JakasLista.TrueForAll(element => element < 50);

            HelloAgain = new TalkNode();
            HelloAgain.Text.Add(new TalkText("Witaj ponownie, co moge dla ciebie zrobic?", 1.0f));
            ConversationRoot.Edges.Add(new TalkEdge(HelloAgain));

            WhatAboutQuest = new TalkReply();
            WhatAboutQuest.Text.Add(new TalkText("Jak wyglada sprawa z butelka?", 3.0f));
            FirstGreeting.Replies.Add(WhatAboutQuest);
            HelloAgain.Replies.Add(WhatAboutQuest);

            Lol = new TalkReaction();

            QuestIsDone = new TalkNode();
            TalkEdge questTalkEdge = new TalkEdge(QuestIsDone);
            questTalkEdge.Conditions += (() => Engine.Singleton.HumanController.Character.ActiveQuests.Quests[Engine.Singleton.HumanController.Character.ActiveQuests.Quests.IndexOf(Quests.Quest1)].isDone);
            Lol.Edges.Add(questTalkEdge);
            QuestIsDone.Text.Add(new TalkText("Wykonales zadanie! Oto twoj miecz.", 3.0f));
            QuestIsDone.Actions += (() => { Engine.Singleton.HumanController.Character.ActiveQuests.MakeFinished(Quests.Quest1); });

            QuestIsNotDone = new TalkNode();
            QuestIsNotDone.Text.Add(new TalkText("Wciaz nie dostalem butelki.", 3.0f));
            Lol.Edges.Add(new TalkEdge(QuestIsNotDone));

            WhatAboutQuest.Reaction = Lol;

            WhereIAm = new TalkReply();
            WhereIAm.Text.Add(new TalkText("Co to za miejsce?", 3.0f));
            FirstGreeting.Replies.Add(WhereIAm);
            HelloAgain.Replies.Add(WhereIAm);

            IDontWantToBother = new TalkReply();
            IDontWantToBother.Text.Add(new TalkText("Nie chcę Ci przeszkadzać.", 2.5f));
            IDontWantToBother.IsEnding = true;
            FirstGreeting.Replies.Add(IDontWantToBother);
            HelloAgain.Replies.Add(IDontWantToBother);

            ItIsATest = new TalkNode();
            ItIsATest.Text.Add(new TalkText("Tez chcialbym wiedziec.", 2.0f));
            ItIsATest.Text.Add(new TalkText("Be my guest.", 2.0f));

            RespWhatIsIt = new TalkReaction();
            RespWhatIsIt.Edges.Add(new TalkEdge(ItIsATest));

            WhereIAm.Reaction = RespWhatIsIt;

            Ending = new TalkReply();
            Ending.Text.Add(new TalkText("Dzięki Stary.", 1.5f));
            Ending.IsEnding = true;

            ItIsATest.Replies.Add(Ending);


            if (File.Exists("Media\\Others\\Dialogi.xml"))
            {
                XmlDocument File1 = new XmlDocument();
                File1.Load("Media\\Others\\Dialogi.xml");
                XmlElement root = File1.DocumentElement;
                XmlNodeList Items = root.SelectNodes("//Dialogs//Dialog");

                foreach (XmlNode item in Items)
                {
                    Dialog justDialog = new Dialog();

                    XmlNodeList TalkReactions = item["Reactions"].ChildNodes;

                    foreach (XmlNode tr in TalkReactions)
                    {
                        TalkReaction justReaction = new TalkReaction();
                        justDialog.Reactions.Add(tr["TalkReactionID"].InnerText, justReaction);
                    }

                    XmlNodeList TalkReplies = item["Replies"].ChildNodes;

                    foreach (XmlNode rep in TalkReplies)
                    {
                        TalkReply justReply = new TalkReply();
                        justReply.IsEnding = bool.Parse(rep["IsEnding"].InnerText);
                        justReply.Text.Add(new TalkText((rep["Text"].InnerText), 3.0f));

                        if (!justReply.IsEnding)
                            justReply.Reaction = justDialog.Reactions[rep["TalkReaction"].InnerText];

                        justDialog.Replies.Add(rep["TalkReplyID"].InnerText, justReply);
                    }

                    XmlNodeList TalkNodes = item["Nodes"].ChildNodes;

                    foreach (XmlNode tn in TalkNodes)
                    {
                        TalkNode justNode = new TalkNode();
                        justNode.Text.Add(new TalkText((tn["Text"].InnerText), 3.0f));

                        XmlNodeList RepliesInNode = tn["NodeReplies"].ChildNodes;

                        foreach (XmlNode rin in RepliesInNode)
                        {
                            justNode.Replies.Add(justDialog.Replies[rin["ReplyID"].InnerText]);
                        }

                        XmlNodeList ActionsInNode = tn["Actions"].ChildNodes;
                        List<ActionType> actionList = new List<ActionType>();

                        foreach (XmlNode ain in ActionsInNode)
                        {
                            actionList.Add((ActionType)int.Parse(ain["ActionType"].InnerText));
                        }

                        actionList.Add(ActionType.MakeFirstFalse);
                        justNode.AddActions(actionList);
                        justDialog.EdgesToNodes.Add(tn["TalkEdgeID"].InnerText, tn["TalkNodeID"].InnerText);
                        justNode.Quest = Quests.Quest1.Name;

                        justDialog.Nodes.Add(tn["TalkNodeID"].InnerText, justNode);
                    }

                    XmlNodeList TalkEdges = item["Edges"].ChildNodes;

                    foreach (XmlNode te in TalkEdges)
                    {
                        TalkEdge justEdge = new TalkEdge(justDialog.Nodes[te["ToWhere"].InnerText]);

                        XmlNodeList ConditionsInEdge = te["Conditions"].ChildNodes;
                        List<Condition> listaWarunkow = new List<Condition>();

                        foreach (XmlNode cin in ConditionsInEdge)
                        {
                            listaWarunkow.Add((Condition)int.Parse(cin["ConditionType"].InnerText));
                        }

                        justEdge.AddConditions(listaWarunkow);

                        while (justDialog.EdgesToNodes.ContainsKey(te["TalkEdgeID"].InnerText))
                        {
                            justDialog.Nodes[justDialog.EdgesToNodes[te["TalkEdgeID"].InnerText]].Edge = justEdge;
                            justDialog.EdgesToNodes.Remove(te["TalkEdgeID"].InnerText);
                        }

                        justDialog.Edges.Add(te["TalkEdgeID"].InnerText, justEdge);
                        justDialog.Reactions[te["FromWhere"].InnerText].Edges.Add(justDialog.Edges[te["TalkEdgeID"].InnerText]);
                    }

                    justDialog.ID = item["DialogID"].InnerText;
                    D.Add(item["DialogID"].InnerText, justDialog);
                }
            }

        }
    }
}
