using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gra
{
    public class SoundManager
    {
        FMOD.RESULT Result;                 // FMOD zwraca przy wykonaniu każdej funkcji informacje typu RESULT
        FMOD.System System = null;          // Główny silnik FMOD'a
        float _volume = 1.0f;                      // Główna głośność dźwięków i muzyki zmieniana przez Volume, wartości: <0, 1.0f>
		public float BGMVolume = 0.3f;		// mnożniki głośności poszczególnych kanałów
		public float DialogVolume = 1.0f;

        //listy dźwięków
        public List<string> BGMPlaylist; int BGMId = 0;


        // czanele
        FMOD.Channel ChannelBGM = null;
		FMOD.Channel ChannelDialog = null;


        // aktualnie wybrane do odtwarzania dźwięki
        FMOD.Sound SoundBGM = null;
		FMOD.Sound SoundDialog = null;


        // czy dany dźwięk jest odtwarzany
        bool IsBGMPlaying = false;


        public SoundManager()
        {

            Result = FMOD.Factory.System_Create(ref System);
            Result = System.init(4, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);  // 4 oznacza maksymalną ilość dźwięków naraz
                                // 1-bgm, 2-odgłosy chodzenia, 3-dźwięki otoczenia, 4-głosy postaci. Może coś jeszcze?

            BGMPlaylist = new List<string>();


			ChannelBGM = new FMOD.Channel();
			ChannelDialog = new FMOD.Channel();
			
        }

        public void PlayBGM(string path = null)
        {
            string play;
			

            if (path == null)
            {
                path = BGMPlaylist[BGMId];   
            }
            
            play = "Media/Music/" + path;
            
            Result = System.createStream(play, FMOD.MODE.DEFAULT, ref SoundBGM);

            Result = System.playSound(0, SoundBGM, false, ref ChannelBGM);

            IsBGMPlaying = true;
			Volume = _volume;
        }

        public void TogglePauseBGM()
        {
            
            if (IsBGMPlaying)
                IsBGMPlaying = false;
            else
                IsBGMPlaying = true;

            Result = ChannelBGM.setPaused(!IsBGMPlaying);
			Volume = _volume;
        }

        public void StopBGM()
        {
            IsBGMPlaying = false;
            Result = ChannelBGM.stop();
        }

        public void NextBGM()
        {
            BGMId++;

            if (BGMId == BGMPlaylist.Count)
                BGMId = 0;

            PlayBGM(BGMPlaylist[BGMId]);
			Volume = _volume;

        }

        public void PreviousBGM()
        {
            BGMId--;

            if (BGMId < 0)
                BGMId = BGMPlaylist.Count - 1;

            PlayBGM(BGMPlaylist[BGMId]);
			Volume = _volume;

        }


        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {                                                   /// TUTAJ TRZA DOPISYWAĆ WSZYSTKIE KOLEJNE CZANELE
                _volume = value;

                ChannelBGM.setVolume(value * BGMVolume);
				ChannelDialog.setVolume(value * DialogVolume);
                //...
            }
        }

		public void PlayDialog(string path)
		{
			if (path != null)
			{
				string play;

				play = "Media/Sounds/Dialogs/" + path;

				Result = System.createStream(play, FMOD.MODE.DEFAULT, ref SoundDialog);

				Result = System.playSound(FMOD.CHANNELINDEX.REUSE, SoundDialog, false, ref ChannelDialog);
				Volume = _volume;
			}
		}

		public void StopDialog()
		{
            Result = ChannelDialog.stop();
		}

		public void PlaySound(string path)
		{
			if (path != null)
			{
				string play;

				play = "Media/Sounds/" + path;

				Result = System.createStream(play, FMOD.MODE.DEFAULT, ref SoundDialog);

				Result = System.playSound(FMOD.CHANNELINDEX.REUSE, SoundDialog, false, ref ChannelDialog);
				Volume = _volume;
			}
		}


    }
}
