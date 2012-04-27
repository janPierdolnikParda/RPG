using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    class HUDInventory
    {
        class InventorySlot
        {
            public const float Size = 0.11f;
            public static float Width;
            SimpleQuad BgQuad;
            SimpleQuad Picture;
            SimpleQuad BlueQuad;

            public InventorySlot(float left, float top)
            {
                BgQuad = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left, top, Width, Size, new ColourValue(1, 1, 1), 1);
                Picture = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", left, top, Width, Size, new ColourValue(1, 1, 1), 2);
                BlueQuad = Engine.Singleton.Labeler.NewSimpleQuad("HighlightBlueMaterial", left, top, Width, Size, new ColourValue(1, 1, 1), 3);

                Picture.IsVisible = false;
                BlueQuad.IsVisible = false;
            }

            public bool IsVisible
            {
                set
                {
                    BgQuad.IsVisible = value;
                    Picture.IsVisible = value;
                    BlueQuad.IsVisible = value;
                }
            }

            public void SetItem(DescribedProfile item)
            {
                if (item != null)
                {
                    Picture.Panel.MaterialName = item.InventoryPictureMaterial;
                    BlueQuad.IsVisible = item.IsEquipment;
                }
                else
                {
                    Picture.Panel.MaterialName = "QuadMaterial";
                    BlueQuad.IsVisible = false;
                }
            }
        }

        const int SlotsCount = 7;
        const float SlotsSpacing = 0.02f;

        SimpleQuad SelectQuad;
        InventorySlot[] Slots;

        int _SelectIndex;
        int _ViewIndex;

        SimpleQuad DescriptionBg;
        SimpleQuad SelectedPicture;
        TextLabel DescriptionLabel;

        SimpleQuad GoldBg;
        TextLabel GoldLabel;
        SimpleQuad InventoryBg;
        SimpleQuad CharacterPicture;

        public HUDInventory()
        {
            InventorySlot.Width = InventorySlot.Size / Engine.Singleton.Camera.AspectRatio;
            Slots = new InventorySlot[SlotsCount];
            for (int i = 0; i < SlotsCount; i++)
                Slots[i] = new InventorySlot(SlotsSpacing, SlotsSpacing + i * (InventorySlot.Size + SlotsSpacing));

            SelectQuad = Engine.Singleton.Labeler.NewSimpleQuad("HighlightMaterial", SlotsSpacing, SlotsSpacing, InventorySlot.Width, InventorySlot.Size, new ColourValue(1, 1, 1), 4);

            DescriptionBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.2f, 0.5f, 0.6f, 0.45f, ColourValue.White, 1);
            SelectedPicture = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial",
                0.21f,
                0.58f,
                0.3f / Engine.Singleton.Camera.AspectRatio,
                0.3f, ColourValue.White, 2);
            DescriptionLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.03f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            DescriptionLabel.SetPosition(0.45f, 0.51f);

            GoldBg = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.7f, 0.1f, 0.2f, 0.1f, new ColourValue(1, 1, 1), 1);
            GoldLabel = Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.05f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2);
            GoldLabel.SetPosition(0.71f, 0.13f);

            InventoryBg = Engine.Singleton.Labeler.NewSimpleQuad("InventoryBgMaterial", 0.01f, 0.01f, 0.98f, 0.98f, new ColourValue(1, 1, 1), 0);
            CharacterPicture = Engine.Singleton.Labeler.NewSimpleQuad("Default", 0.4f, 0.1f, 0.2f / Engine.Singleton.Camera.AspectRatio, 0.2f, new ColourValue(1, 1, 1), 1);

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
            for (int i = ViewIndex; i < ViewIndex + SlotsCount; i++)
                if (i < Character.Inventory.Count)
                    Slots[i - ViewIndex].SetItem(Character.Inventory[i]);
                else
                    Slots[i - ViewIndex].SetItem(null);
            CharacterPicture.Panel.MaterialName = Character.Profile.PictureMaterial;
            GoldLabel.Caption = "Zloto: " + Character.Profile.Gold;
        }

        public void UpdateItem(int i)
        {
            if (i >= ViewIndex && i < ViewIndex + SlotsCount)
                Slots[i - ViewIndex].SetItem(Character.Inventory[i]);
        }

        public void UpdateDescription()
        {
            if (SelectIndex != -1)
            {
                DescriptionLabel.Caption =
                    Character.Inventory[SelectIndex].DisplayName
                    + "\n\n"
                    + Character.Inventory[SelectIndex].Description;

                if (Character.Inventory[SelectIndex] is ItemSword)
                    DescriptionLabel.Caption += "\nObrazenia: "
                        + (Character.Inventory[SelectIndex] as ItemSword).IloscRzutow.ToString() + "k"
                        + (Character.Inventory[SelectIndex] as ItemSword).JakoscRzutow.ToString();

                SelectedPicture.Panel.MaterialName = Character.Inventory[SelectIndex].InventoryPictureMaterial;
            }
            else
            {
                DescriptionLabel.Caption = "";
                SelectedPicture.Panel.MaterialName = "QuadMaterial";
            }
        }

        public int ViewIndex
        {
            get
            {
                return _ViewIndex;
            }
            set
            {
                if (value != _ViewIndex)
                {
                    _ViewIndex = System.Math.Max(0, value);
                    UpdateView();
                }
            }
        }

        public int SelectIndex
        {
            get
            {
                return (_SelectIndex >= Character.Inventory.Count ? -1 : _SelectIndex);
            }
            set
            {
                if (value < 0)
                    value = -1;
                else if (value >= Character.Inventory.Count)
                    value = Character.Inventory.Count - 1;
                _SelectIndex = value;
                if (_SelectIndex >= ViewIndex + SlotsCount)
                    ViewIndex = _SelectIndex - SlotsCount + 1;
                else if (_SelectIndex < ViewIndex)
                    ViewIndex = _SelectIndex;

                SelectQuad.Panel.Top = SlotsSpacing + (_SelectIndex - ViewIndex) * (InventorySlot.Size + SlotsSpacing);
                UpdateDescription();
            }
        }

        public bool IsVisible
        {
            set
            {
                foreach (var slot in Slots) slot.IsVisible = value;
                SelectQuad.IsVisible = value;
                DescriptionBg.IsVisible = value;
                DescriptionLabel.IsVisible = value;
                SelectedPicture.IsVisible = value;
                GoldBg.IsVisible = value;
                GoldLabel.IsVisible = value;
                InventoryBg.IsVisible = value;
                CharacterPicture.IsVisible = value;

                if (value)
                {
                    UpdateView();
                    SelectIndex = SelectIndex;
                }
            }
        }
    }
}
