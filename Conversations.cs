using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

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

        }
    }
}
