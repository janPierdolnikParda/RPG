using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    class TypedInput
    {
        int KeysCount;
        public bool[] IsKeyTyped;
        float[] KeyDownTime;

        public TypedInput()
        {
            KeysCount = (int)(Enum.GetValues(typeof(MOIS.KeyCode)) as MOIS.KeyCode[]).Max();
            IsKeyTyped = new bool[KeysCount];
            KeyDownTime = new float[KeysCount];
        }

        public void Update()
        {
            for (int keyCode = 0; keyCode < KeysCount; keyCode++)
            {
                IsKeyTyped[keyCode] = false;                

                if (Engine.Singleton.Keyboard.IsKeyDown((MOIS.KeyCode)keyCode))
                {
                    if (KeyDownTime[keyCode] <= 0)
                    {
                        KeyDownTime[keyCode] = 0.7f;
                        IsKeyTyped[keyCode] = true;
                    }
                    KeyDownTime[keyCode] -= Engine.FixedTimeStep;
                }
                else
                    KeyDownTime[keyCode] = 0;
            }
        }
    }
}
