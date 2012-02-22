using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public enum Condition
    {
        FirstTalk,
        GotQuest,
        IsQuestDone,
        IsQuestFinished
    };

    public class TalkEdge : TalkConditional
    {
        public TalkNode TalkNode;

        public bool FirstTalk = true;
        public bool GotQuest = true;
        public bool IsQuestDone = true;
        public bool IsQuestFinished = true;
        public bool Other = true;

        public string Quest;
        public string ID;

        public List<Condition> Condits;

        public TalkEdge(TalkNode target)
        {
            Condits = new List<Condition>();
            TalkNode = target;
            base.Conditions += (() => Other);
        }

        public TalkEdge()
        {
        }

        public void AddConditions(List<Condition> conditions)
        {
            Condits = conditions;
        }
    }

    public class TalkReaction
    {
        public List<TalkEdge> Edges;
        public Character WhoSays;

        public TalkReaction()
        {
            Edges = new List<TalkEdge>();
        }

        public TalkNode PickNode()
        {
            foreach (TalkEdge edge in Edges)
            {
                edge.Reset();
                edge.Conditions += (() => edge.Other);
                bool Flag = false;                  // FLAG!!! TROLOLOLOLOLOLOL!! !!!!!!!!oneone11111!! :_)

                foreach (Condition c in edge.Condits)
                {
                        switch (c)
                        {
                            case Condition.FirstTalk:
                                edge.Conditions += (() => edge.FirstTalk);
                                break;

                            case Condition.GotQuest:
                                edge.Conditions += (() => Engine.Singleton.HumanController.Character.ActiveQuests.Quests.Contains(Quests.Q[edge.Quest]));
                                break;

                            case Condition.IsQuestDone:
                                if (Engine.Singleton.HumanController.Character.ActiveQuests.Quests.Contains(Quests.Q[edge.Quest]))
                                    edge.Conditions += (() => Engine.Singleton.HumanController.Character.ActiveQuests.Quests[Engine.Singleton.HumanController.Character.ActiveQuests.Quests.IndexOf(Quests.Q[edge.Quest])].isDone);
                                else
                                    edge.Conditions += (() => Flag);
                                break;

                            case Condition.IsQuestFinished:
                                if (Engine.Singleton.HumanController.Character.ActiveQuests.Quests.Contains(Quests.Q[edge.Quest]))
                                    edge.Conditions += (() => Engine.Singleton.HumanController.Character.ActiveQuests.Quests[Engine.Singleton.HumanController.Character.ActiveQuests.Quests.IndexOf(Quests.Q[edge.Quest])].IsFinished);
                                else
                                    edge.Conditions += (() => Flag);
                                break;
                        }
                }

                if (edge.IsConditionFulfilled())
                {
                    edge.TalkNode.WhoSays = WhoSays;
                    edge.TalkNode.CallActions();
                    return edge.TalkNode;
                }
            }
            throw new Exception("Reaction without edges!");
        }

        public TalkNode PickNode(Character whosays)
        {
            foreach (TalkEdge edge in Edges)
            {
                edge.Conditions += (() => edge.Other);

                foreach (Condition c in edge.Condits)
                {
                    switch (c)
                    {
                        case Condition.FirstTalk:
                            edge.Conditions += (() => edge.FirstTalk);
                            break;

                        case Condition.GotQuest:
                            edge.Conditions += (() => Engine.Singleton.HumanController.Character.ActiveQuests.Quests.Contains(Quests.Q[edge.Quest]));
                            break;

                        case Condition.IsQuestDone:
                            edge.Conditions += (() => Engine.Singleton.HumanController.Character.ActiveQuests.Quests[Engine.Singleton.HumanController.Character.ActiveQuests.Quests.IndexOf(Quests.Q[edge.Quest])].isDone);
                            break;

                        case Condition.IsQuestFinished:
                            if (Engine.Singleton.HumanController.Character.ActiveQuests.Quests.Contains(Quests.Q[edge.Quest]))
                                edge.Conditions += (() => Engine.Singleton.HumanController.Character.ActiveQuests.Quests[Engine.Singleton.HumanController.Character.ActiveQuests.Quests.IndexOf(Quests.Q[edge.Quest])].IsFinished);
                            break;
                    }
                }

                if (edge.IsConditionFulfilled())
                {
                    edge.TalkNode.WhoSays = whosays;
                    edge.TalkNode.CallActions();
                    return edge.TalkNode;
                }
            }
            throw new Exception("Reaction without edges!");
        }
    }
}
