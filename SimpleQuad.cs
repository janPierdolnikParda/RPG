using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    class SimpleQuad
    {
        public OverlayContainer Panel;

        public SimpleQuad(String name, String materialName,
            float left, float top, float width,
            float height, ColourValue colour)
        {
            Panel = OverlayManager.Singleton.CreateOverlayElement("Panel", name)
              as OverlayContainer;
            Panel.MetricsMode = GuiMetricsMode.GMM_RELATIVE;
            Panel.SetDimensions(width, height);
            Panel.SetPosition(left, top);
            Panel.MaterialName = materialName;
            Panel.Colour = colour;
        }

        public void SetDimensions(float left, float top, float width, float height)
        {
            Panel.SetDimensions(width, height);
            Panel.SetPosition(left, top);
        }

        public Boolean IsVisible
        {
            get { return Panel.IsVisible; }
            set
            {
                if (value)
                    Panel.Show();
                else
                    Panel.Hide();
            }
        }

    }
}
