using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using System.Reflection;
using System;

namespace Gra
{
    public class IngameConsole
    {

        public bool Visible;
        bool Initialized;

		const int CONSOLE_LINE_LENGTH = 85;
		const int CONSOLE_LINE_COUNT = 15;

		SimpleQuad Bg;
		TextLabel Textbox;

        public float Height;
        public bool Update_overlay;
        public int Start_line;
        public List<string> Lines = new List<string>();
        public string Prompt;


        public Dictionary<string, string> Commands = new Dictionary<string,string>();

		public IngameConsole()
		{

		}

        public void Init()
        {
			Start_line = 0;
            Height = 1;

			Bg = Engine.Singleton.Labeler.NewSimpleQuad("ConsoleMat", 0, 0, 1, 0.5f, ColourValue.ZERO, 3);
			Textbox = Engine.Singleton.Labeler.NewTextLabel("Console", 0.03f, ColourValue.Black, ColourValue.Black, 4);

            Initialized = true; 
        }

        public void Exit()
        {
            if (!Initialized)
                return;
        }

		public void Print(string text)
        {
			string line = "";

			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '\n')
				{
					Lines.Add(line);
					line = "";
				}
				else if (line.Length >= CONSOLE_LINE_LENGTH)
				{
					Lines.Add(line);
					line = " >";
				}
				else if (text[i] != '\n')
					line += text[i];
			}
			
			if (line.Length > 0)
				Lines.Add(line);

			if (Lines.Count > CONSOLE_LINE_COUNT)
			{
				Start_line = Lines.Count - CONSOLE_LINE_COUNT;
			}
			else
				Start_line = 0;

			Update_overlay = true;
        }

		public void Update()
		{
			if (Visible && Height < 1)
			{

				Textbox.IsVisible = true;
				Bg.IsVisible = true;
				
					Height = 1;
			}
			else if (!Visible && Height > 0)
			{
				
				Height = 0;
				Textbox.IsVisible = false;
				Bg.IsVisible = false;				
			}

			if (Visible)
			{
				Textbox.SetPosition(0, (Height - 1) * 0.5f);

				if (Update_overlay)
				{
					string text = null;

					if (Start_line > Lines.Count)
						Start_line = Lines.Count;

					int start = 0, end = 0;

					for (int i = 0; i < Start_line; i++)
					{
						start++;
					}
					end = start;

					for (int i = 0; i < CONSOLE_LINE_COUNT; i++)
					{
						if (end == Lines.Count)
							break;
						end++;
					}

					if (end - start > 0 && Lines.Count > 0) 
						for (int i = start; i < end; i++)
							text += Lines[i] + '\n';

					text += "] " + Prompt;
					Textbox.Caption = text;

					Update_overlay = false;
				}
			}
		}

        public void AddCommand(string command, string func)
        {
			Commands.Add(command, func);
        }

        public void RemoveCommand(string command)
        {
			Commands.Remove(command);
        }

        public void MessageLogged(string message, LogMessageLevel lml, bool maskDebug, string logName)
        {
            Print(logName + ": " + message);
        }

        public void LoadCommands()
        {

        }

    }
}
