using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    public class TextLabel3D : TextLabel
    {
        public Vector3 Position3D;
        float HalfWidth;

        public TextLabel3D(
          String name, String fontName, float fontSize,
          ColourValue colourtop, ColourValue colourbottom)
            : base(name, fontName, fontSize, colourtop, colourbottom)
        {
        }

        public override void Update()
        {
            Vector3 projected = Engine.Singleton.Camera.ProjectionMatrix
              * (Engine.Singleton.Camera.ViewMatrix * Position3D);
            float x2d = ((projected.x * 0.5f) + 0.5f);
            float y2d = 1.0f - ((projected.y * 0.5f) + 0.5f);
            if (x2d < 0 || x2d > 1 || y2d < 0 || y2d > 1)
                TextArea.Hide();
            else
                if (IsVisible) TextArea.Show();

            SetPosition(x2d - HalfWidth, y2d);

            if (IsCaptionChanged)
            {
                HalfWidth = GetTextWidth() * 0.5f;

                base.Update();
                IsCaptionChanged = false;
            }
        }
    }
}
