using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class TalkReply : TalkConditional
    {
        public List<TalkText> Text;
        public bool IsEnding;
        public TalkReaction Reaction;
        public String Sound;

        public TalkReply()
        {
            Text = new List<TalkText>();
        }
    }
}
