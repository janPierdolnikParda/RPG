using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
	public class Activators
	{
		public static void Null()
		{}

		public static void soundOddawajPiec()
		{
			Engine.Singleton.SoundManager.PlayDialog("oddawaj_moj_piec.mp3");
		}







	}
}
