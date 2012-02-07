using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class TalkText
    {
        public String Text;
        public float Duration;
		public String Sound;

        public TalkText(String text, float duration ,string sound)    // @@@@@@@@@@@@@ Do zrobienia tekst mówiony.
        {
            Text = text;
            Duration = duration;
			Sound = sound;
        }
    }
}
