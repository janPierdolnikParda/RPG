using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class TextLabel
    {
        public TextAreaOverlayElement TextArea;
        protected bool IsCaptionChanged;
        bool _IsVisible;

        public TextLabel(
            String name, String fontName, float fontSize, ColourValue colourtop, ColourValue colourbottom)
        {
            TextArea = OverlayManager.Singleton.CreateOverlayElement("TextArea", name)
              as TextAreaOverlayElement;
            TextArea.MetricsMode = GuiMetricsMode.GMM_RELATIVE;
            TextArea.SetDimensions(1.0f, 1.0f);
            TextArea.CharHeight = fontSize;
            TextArea.FontName = fontName;
            TextArea.ColourTop = colourtop;
            TextArea.ColourBottom = colourbottom;

            TextArea.SpaceWidth = TextArea.CharHeight * 0.5f;

           

        }

        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                _IsVisible = value;
                if (value)
                    TextArea.Show();
                else
                    TextArea.Hide();
            }
        }

        public void SetPosition(float x, float y)
        {
            TextArea.SetPosition(x, y);
        }

        public String Caption
        {
            get { return TextArea.Caption; }
            set
            {
                TextArea.Caption = value;
                IsCaptionChanged = true;
            }
        }

       public virtual void Update()
        {
            if (IsCaptionChanged)
            {
                TextArea.Caption = TextArea.Caption;
                IsCaptionChanged = false;
            }
        }

       public void SetColor(ColourValue colourtop, ColourValue colourbottom)
       {
           TextArea.ColourTop = colourtop;
           TextArea.ColourBottom = colourbottom;
       }

       public float GetTextWidth()
       {
           FontPtr font = FontManager.Singleton.GetByName(TextArea.FontName);
           float textWidth = 0;

           for (int i = 0; i < TextArea.Caption.Length; i++)
           {
               if (TextArea.Caption[i] == 32)
                   textWidth += 0.5f;
               else
                   textWidth += font.GetGlyphAspectRatio(TextArea.Caption[i]);
           }

           return (float)textWidth * (float)TextArea.CharHeight / (float)Engine.Singleton.Camera.AspectRatio;

           

       }
        
    }
}
