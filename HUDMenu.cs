using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    class HUDMenu
    {
        bool _IsVisible;
        SubMenu Menu;
        public List<TextLabel> Options2Choose;
        TextLabel Header;

        public SimpleQuad MouseCursor; 

        public HUDMenu()
        {
            Menu = Engine.Singleton.Menu;
            Options2Choose = new List<TextLabel>();
            Header = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.1f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            Header.SetPosition(0.4f, 0.3f);
            MouseCursor = Engine.Singleton.Labeler.NewSimpleQuad("Kursor", 0.0f, 0.0f, Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32), new ColourValue(1, 1, 1), 4);
        }

        public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }
            set
            {
                _IsVisible = value;
                Header.IsVisible = value;
                MouseCursor.IsVisible = value;

                foreach (TextLabel TL in Options2Choose)
                    TL.IsVisible = value;
            }
        }

        public void Update()
        {
            for (int i = 0; i < Options2Choose.Count; i++)
                Engine.Singleton.Labeler.DestroyTextLabel(Options2Choose[i], 2);
            Menu = Engine.Singleton.Menu;
            Options2Choose.Clear();

            foreach (SubMenu SB in Menu.SubMenus)
            {
                if (SB.Selected)
                    Options2Choose.Add(Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.03f, new ColourValue(1.0f, 0, 0.2f), new ColourValue(1, 1.0f, 0.6f), 2));
                else if(SB.Enabled)
                    Options2Choose.Add(Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.03f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2));
                else
                    Options2Choose.Add(Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.03f, new ColourValue(0.22f, 0.22f, 0.22f), new ColourValue(1, 1.0f, 0.6f), 2));
                Options2Choose[Options2Choose.Count - 1].Caption = SB.MenuName;
            }

            Header.Caption = Menu.MenuName;

            for (int i = 0; i < Options2Choose.Count; i++)
            {
                TextLabel TL = Options2Choose[i];
                TL.SetPosition(0.4f, 0.45f + i * 0.05f);
            }

            MouseCursor.SetDimensions(Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs), Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs), Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32));            
        }

        public bool IsOver(TextLabel label)
        {
            return (Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mysz.X.abs) >= label.TextArea.Left && Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mysz.X.abs) <= label.GetTextWidth() + label.TextArea.Left && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mysz.Y.abs) >= label.TextArea.Top && Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mysz.Y.abs) <= (label.TextArea.CharHeight + label.TextArea.Top));
        }
    }
}
