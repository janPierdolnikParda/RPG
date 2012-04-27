using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    class HUDContainer
    {
        class InventorySlot
        {
            public const float Size = 0.11f;
            public static float Width;
            SimpleQuad BgQuad;
            SimpleQuad Picture;
            

            public InventorySlot(float left, float top)
            {
                BgQuad = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left, top, Width, Size, new ColourValue(1, 1, 1), 1);
                Picture = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left, top, Width, Size, new ColourValue(1, 1, 1), 2);
                

                Picture.IsVisible = false;
                
            }

            public bool IsVisible
            {
                set
                {
                    BgQuad.IsVisible = value;
                    Picture.IsVisible = value;
                    
                }
            }

            public void SetItem(DescribedProfile item)
            {
                if (item != null)
                {
                    Picture.Panel.MaterialName = item.InventoryPictureMaterial;                    
                }
                else
                {
                    Picture.Panel.MaterialName = "QuadMaterial";                    
                }
            }
        }

        const int SlotsCount = 7;
        const float SlotsSpacing = 0.02f;

        SimpleQuad SelectQuad1;
        SimpleQuad SelectQuad2;

        InventorySlot[] Slots1;
        InventorySlot[] Slots2;

        int _SelectIndex1;
        int _SelectIndex2;
        int _ViewIndex1;
        int _ViewIndex2;

        public int ActiveEq;

        SimpleQuad DescriptionBg;
        SimpleQuad SelectedPicture;
        TextLabel DescriptionLabel;

        SimpleQuad GoldBg;
        TextLabel GoldLabel;
        SimpleQuad InventoryBg;

        SimpleQuad FreeSlotsBg;
        TextLabel FreeSlotsLabel;

        public Container Container = new Container();

        public HUDContainer()
        {


            ActiveEq = 1;

            InventorySlot.Width = InventorySlot.Size / Engine.Singleton.Camera.AspectRatio;
            Slots1 = new InventorySlot[SlotsCount];
            Slots2 = new InventorySlot[SlotsCount];
            for (int i = 0; i < SlotsCount; i++)
            {
                Slots1[i] = new InventorySlot(SlotsSpacing, SlotsSpacing + i * (InventorySlot.Size + SlotsSpacing));
                Slots2[i] = new InventorySlot(1.0f - (InventorySlot.Size), SlotsSpacing + i * (InventorySlot.Size + SlotsSpacing));
            }


            SelectQuad1 = Engine.Singleton.Labeler.NewSimpleQuad("HighlightMaterial", SlotsSpacing, SlotsSpacing, InventorySlot.Width, InventorySlot.Size, new ColourValue(1, 1, 1), 4);
            SelectQuad2 = Engine.Singleton.Labeler.NewSimpleQuad("HighlightMaterial", 1.0f - InventorySlot.Size, SlotsSpacing, InventorySlot.Width, InventorySlot.Size, new ColourValue(1, 1, 1), 4);


            DescriptionBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.2f, 0.5f, 0.6f, 0.45f, ColourValue.White, 1);
            SelectedPicture = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial",
                0.21f,
                0.58f,
                0.3f / Engine.Singleton.Camera.AspectRatio,
                0.3f, ColourValue.White, 2);
            DescriptionLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.03f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            DescriptionLabel.SetPosition(0.45f, 0.51f);

            GoldBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.5f, 0.1f, 0.2f, 0.1f, new ColourValue(1, 1, 1), 1);
            GoldLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            GoldLabel.SetPosition(0.51f, 0.13f);

            FreeSlotsBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.45f, 0.3f, 0.3f, 0.1f, new ColourValue(1, 1, 1), 1);
            FreeSlotsLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            FreeSlotsLabel.SetPosition(0.46f, 0.33f);

            InventoryBg = Engine.Singleton.Labeler.NewSimpleQuad("InventoryBgMaterial", 0.01f, 0.01f, 0.98f, 0.98f, new ColourValue(1, 1, 1), 0);
            

            IsVisible = false;

        }

        public Character Character
        {
            get
            {
                return Engine.Singleton.HumanController.Character;
            }
        }

        public void UpdateView()
        {
            if (ActiveEq == 1)
            {
                for (int i = ViewIndex2; i < ViewIndex2 + SlotsCount; i++)
                    if (i < Container.Contains.Count)
                        Slots2[i - ViewIndex2].SetItem(Container.Contains[i]);
                    else
                        Slots2[i - ViewIndex2].SetItem(null);
            }
            else if (ActiveEq == 0)
            {
                for (int i = ViewIndex1; i < ViewIndex1 + SlotsCount; i++)
                    if (i < Character.Inventory.Count)
                        Slots1[i - ViewIndex1].SetItem(Character.Inventory[i]);
                    else
                        Slots1[i - ViewIndex1].SetItem(null);
            }
            
            GoldLabel.Caption = "Zloto: " + Character.Profile.Gold;
            FreeSlotsLabel.Caption = "Zajete: " + Container.Contains.Count.ToString() + "/" + Container.MaxItems.ToString(); 
        }

        public void UpdateViewAll()
        {
            for (int i = ViewIndex2; i < ViewIndex2 + SlotsCount; i++)
                    if (i < Container.Contains.Count)
                        Slots2[i - ViewIndex2].SetItem(Container.Contains[i]);
                    else
                        Slots2[i - ViewIndex2].SetItem(null);

         
                for (int i = ViewIndex1; i < ViewIndex1 + SlotsCount; i++)
                    if (i < Character.Inventory.Count)
                        Slots1[i - ViewIndex1].SetItem(Character.Inventory[i]);
                    else
                        Slots1[i - ViewIndex1].SetItem(null);

            

            GoldLabel.Caption = "Zloto: " + Character.Profile.Gold;
            FreeSlotsLabel.Caption = "Zajete: " + Container.Contains.Count.ToString() + "/" + Container.MaxItems.ToString(); 
        }

        public void UpdateItem(int i)
        {
            if (ActiveEq == 0)
            {
                if (i >= ViewIndex1 && i < ViewIndex1 + SlotsCount)
                    Slots1[i - ViewIndex1].SetItem(Character.Inventory[i]);
            }
            else if (ActiveEq == 1)
            {
                if (i >= ViewIndex2 && i < ViewIndex2 + SlotsCount)
                    Slots2[i - ViewIndex2].SetItem(Container.Contains[i]);
            }
        }

        public void UpdateDescription()
        {
            if (ActiveEq == 0)
            {
                if (SelectIndex1 != -1)
                {
                    DescriptionLabel.Caption =
                        Character.Inventory[SelectIndex1].DisplayName
                        + "\n\n"
                        + Character.Inventory[SelectIndex1].Description;

                    if (Character.Inventory[SelectIndex1] is ItemSword)
                        DescriptionLabel.Caption += "\nObrazenia: "
                        + (Character.Inventory[SelectIndex1] as ItemSword).IloscRzutow.ToString() + "k"
                        + (Character.Inventory[SelectIndex1] as ItemSword).JakoscRzutow.ToString();

                    SelectedPicture.Panel.MaterialName = Character.Inventory[SelectIndex1].InventoryPictureMaterial;
                }
                else
                {
                    DescriptionLabel.Caption = "";
                    SelectedPicture.Panel.MaterialName = "QuadMaterial";
                }
            }
            else if (ActiveEq == 1)
            {
                if (SelectIndex2 != -1)
                {
                    DescriptionLabel.Caption =
                        Container.Contains[SelectIndex2].DisplayName
                        + "\n\n"
                        + Container.Contains[SelectIndex2].Description;

                    if (Container.Contains[SelectIndex2] is ItemSword)
                        DescriptionLabel.Caption += "\nObrazenia: "
                        + (Character.Inventory[SelectIndex2] as ItemSword).IloscRzutow.ToString() + "k"
                        + (Character.Inventory[SelectIndex2] as ItemSword).JakoscRzutow.ToString();

                    SelectedPicture.Panel.MaterialName = Container.Contains[SelectIndex2].InventoryPictureMaterial;
                }
                else
                {
                    DescriptionLabel.Caption = "";
                    SelectedPicture.Panel.MaterialName = "QuadMaterial";
                }
            }
            
        }

        public int ViewIndex1
        {
            get
            {
                return _ViewIndex1;
            }
            set
            {
                if (value != _ViewIndex1)
                {
                    _ViewIndex1 = System.Math.Max(0, value);
                    UpdateView();
                }
            }
        }

        public int ViewIndex2
        {
            get
            {
                return _ViewIndex2;
            }
            set
            {
                if (value != _ViewIndex2)
                {
                    _ViewIndex2 = System.Math.Max(0, value);
                    UpdateView();
                }
            }
        }

        public int SelectIndex1
        {
            get
            {
                return (_SelectIndex1 >= Character.Inventory.Count ? -1 : _SelectIndex1);
            }
            set
            {
                if (value < 0)
                    value = -1;
                else if (value >= Character.Inventory.Count)
                    value = Character.Inventory.Count - 1;
                _SelectIndex1 = value;
                if (_SelectIndex1 >= ViewIndex1 + SlotsCount)
                    ViewIndex1 = _SelectIndex1 - SlotsCount + 1;
                else if (_SelectIndex1 < ViewIndex1)
                    ViewIndex1 = _SelectIndex1;

                SelectQuad1.Panel.Top = SlotsSpacing + (_SelectIndex1 - ViewIndex1) * (InventorySlot.Size + SlotsSpacing);
                UpdateDescription();
            }
        }

        public int SelectIndex2
        {
            get
            {
                return (_SelectIndex2 >= Container.Contains.Count ? -1 : _SelectIndex2);
            }
            set
            {
                if (value < 0)
                    value = -1;
                else if (value >= Container.Contains.Count)
                    value = Container.Contains.Count - 1;
                _SelectIndex2 = value;
                if (_SelectIndex2 >= ViewIndex2 + SlotsCount)
                    ViewIndex2 = _SelectIndex2 - SlotsCount + 1;
                else if (_SelectIndex2 < ViewIndex2)
                    ViewIndex2 = _SelectIndex2;

                SelectQuad2.Panel.Top = SlotsSpacing + (_SelectIndex2 - ViewIndex2) * (InventorySlot.Size + SlotsSpacing);
                UpdateDescription();
            }
        }

        public bool IsVisible
        {
            set
            {
                foreach (var slot in Slots1) slot.IsVisible = value;
                foreach (var slot in Slots2) slot.IsVisible = value;
                SelectQuad1.IsVisible = value;
                SelectQuad2.IsVisible = value;
                DescriptionBg.IsVisible = value;
                DescriptionLabel.IsVisible = value;
                SelectedPicture.IsVisible = value;
                GoldBg.IsVisible = value;
                GoldLabel.IsVisible = value;
                InventoryBg.IsVisible = value;

                FreeSlotsBg.IsVisible = value;
                FreeSlotsLabel.IsVisible = value;



                if (value)
                {
                    UpdateView();
                    if (ActiveEq == 0)
                        SelectIndex1 = SelectIndex1;
                    else if (ActiveEq == 1)
                        SelectIndex2 = SelectIndex2;
                }
            }
        }
    }
}
