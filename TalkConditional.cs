using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public abstract class TalkConditional
    {
      public event Func<bool> Conditions;
 
      public Boolean IsConditionFulfilled()
      {
        bool result = true;
        if (Conditions != null)
        {
          foreach (Delegate cond in Conditions.GetInvocationList())
            result &= (bool)cond.DynamicInvoke(null);
        }
        return result;
      }        
    }
}
