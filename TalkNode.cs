using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class TalkNode
    {
      public List<TalkText> Text;
      public List<TalkReply> Replies;
      public event Action Actions;
 
      public TalkNode()
      {
        Text = new List<TalkText>();
        Replies = new List<TalkReply>();
      }
 
      public void CallActions()
      {
        if (Actions!=null)
        {
          Actions();
        }
      }
    }
}