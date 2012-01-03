using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MogreNewt;

namespace Gra
{
    class TextLabeler
    {
        Overlay[] Overlays;
        OverlayContainer[] Panels;
        public List<SimpleQuad> Quads;

        public int LabelID;
        public List<TextLabel> Labels;

        public TextLabeler(int layers)
        {
            Overlays = new Overlay[layers];
            Panels = new OverlayContainer[layers];

            for (ushort i = 0; i < layers; i++)
            {
                Overlays[i] = OverlayManager.Singleton.Create("TextLabeler" + i.ToString());

                Panels[i] = OverlayManager.Singleton.CreateOverlayElement("Panel",
                  "TextLabelerPanel" + i.ToString())
                    as OverlayContainer;
                Panels[i].MetricsMode = GuiMetricsMode.GMM_RELATIVE;
                Panels[i].SetDimensions(1.0f, 1.0f);
                Panels[i].SetPosition(0, 0);

                Overlays[i].Add2D(Panels[i]);
                Overlays[i].Show();
                Overlays[i].ZOrder = i;
            }

            Labels = new List<TextLabel>();
            Quads = new List<SimpleQuad>();
        }

        public string GetUniqueLabelName()
        {
            return (LabelID++).ToString();
        }

        public TextLabel NewTextLabel(String fontName, float fontSize,
                        ColourValue colourtop, ColourValue colourbottom, int layer)
        {
            TextLabel textLabel = new TextLabel(GetUniqueLabelName(),
              fontName, fontSize, colourtop, colourbottom);

            Panels[layer].AddChild(textLabel.TextArea);
            Labels.Add(textLabel);

            return textLabel;
        }

        public void Update()
        {
            foreach (TextLabel label in Labels)
                label.Update();
        }

        public TextLabel3D NewTextLabel3D(
            String fontName, float fontSize, ColourValue colourtop, ColourValue colourbottom, int layer)
        {
            TextLabel3D textLabel = new TextLabel3D(
              GetUniqueLabelName(), fontName, fontSize, colourtop, colourbottom);

            Panels[layer].AddChild(textLabel.TextArea);
            Labels.Add(textLabel);

            return textLabel;
        }

        public SimpleQuad NewSimpleQuad(string materialName, float left, float top,
                            float width, float height, ColourValue colour, int layer)
        {
            SimpleQuad quad = new SimpleQuad(GetUniqueLabelName(), materialName,
              left, top, width, height, colour);

            Overlays[layer].Add2D(quad.Panel);
            Quads.Add(quad);

            return quad;
        }
    }
}
