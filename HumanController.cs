using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;

namespace Gra
{
    class HumanController
    {
        public enum HumanControllerState
        {
            FREE,
            TALK,
            INVENTORY,
            CONTAINER,
            SHOP,
            ATTACK,
            MENU,
            STATS,
            CREATOR_STATS
        }

        public enum HumanTalkState
        {
            CHOOSING_REPLY,
            REPLYING,
            PAUSE,
            LISTENING
        }

		// config
		public bool InvertMouse = false;
		public float MouseSpeed = 1.0f;

        public Character Character;
        public SelectableObject FocusObject;
        TextLabel3D TargetLabel;

        SimpleQuad TalkBox;
        List<TextLabel> TalkLabels;

        HumanControllerState State;
        HumanTalkState TalkState;

        List<TalkReply> ValidReplies;
        int SelectedReply = 0;

        float TextRemainingTime = 0.0f;
        int TextIndex;
        TalkReply CurrentReply;
        TalkNode CurrentNode;

        HUDInventory HUDInventory;
        HUDContainer HUDContainer;
        public HUDShop HUDShop;
        public HUDMenu HUDMenu;
        public HUD HUD;
        public HUDStats HUDStats;
        public HUDNewCharacterStats HUDNewCharacterStats;
        public MOIS.MouseState_NativePtr Mysz;

		public bool InitShop;
        public Statistics StatisticsB4;
        public int ExpB4;

        public bool AddedToKillList;


        public HumanController()
        {
            TargetLabel = Engine.Singleton.Labeler.NewTextLabel3D("Primitive", 0.04f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 0);

            TalkBox = Engine.Singleton.Labeler.NewSimpleQuad("QuadMaterial", 0.05f, 0.85f, 0.9f, 0.1f, new ColourValue(1, 1, 1), 1);
            TalkBox.IsVisible = false;

            TalkLabels = new List<TextLabel>();
            for (int i = 0; i < 8; i++)
            {
                TalkLabels.Add(Engine.Singleton.Labeler.NewTextLabel("Primitive", 0.04f, new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f), 2));
                TalkLabels[i].SetPosition(0.07f, 0.63f + i * 0.037f);
            }
            ValidReplies = new List<TalkReply>();

            HUDInventory = new HUDInventory();
            HUDContainer = new HUDContainer();
            HUDShop = new HUDShop();
            HUD = new HUD();
            HUDMenu = new HUDMenu();

            Mysz = new MOIS.MouseState_NativePtr();
        }


        public void HideTalkOverlay()
        {
            foreach (TextLabel lab in TalkLabels) lab.IsVisible = false;
            TalkBox.IsVisible = false;
        }


        public void ShowReplies()
        {
            TalkBox.IsVisible = true;
            foreach (TextLabel lab in TalkLabels) lab.IsVisible = true;
            TalkBox.SetDimensions(0.05f, 0.6f, 0.9f, 0.35f);
            TalkLabels.Last().SetPosition(0.07f, TalkLabels.Last().TextArea.Top);
        }

        public void ShowTalkBox()
        {
            TalkBox.IsVisible = true;
            foreach (TextLabel lab in TalkLabels) lab.IsVisible = false;
            TalkLabels.Last().IsVisible = true;
            TalkBox.SetDimensions(0.05f, 0.85f, 0.9f, 0.1f);
        }

        public void BeginTextDisplay(TalkText text)
        {
            TalkLabels.Last().Caption = text.Text;
            TalkLabels.Last().SetPosition(0.5f - TalkLabels.Last().GetTextWidth() * 0.5f, 0.889f);
			Engine.Singleton.SoundManager.PlayDialog(text.Sound);
            TextRemainingTime = text.Duration;
        }

        public void SwitchState(HumanControllerState newState)
        {
            if (State == HumanControllerState.FREE)
            {
                if (newState == HumanControllerState.CREATOR_STATS)
                {
                    HUDNewCharacterStats.IsVisible = true;
                    HUDNewCharacterStats.KwadratyLosu[0].Zaznaczony = true;
                    HUDNewCharacterStats.KtoryZaznaczony = 0;
                }
				if (newState == HumanControllerState.STATS)
				{
					HUDStats.IsVisible = true;
					Character.MoveLeftOrder = false;
					Character.MoveRightOrder = false;
					Character.MoveOrder = false;
					Character.MoveOrderBack = false;
                    StatisticsB4 = Character.Statistics.statistics_Clone();
                    ExpB4 = Character.Profile.Exp;
				}
                if (newState == HumanControllerState.MENU)
                    HUDMenu.IsVisible = true;
                if (newState == HumanControllerState.TALK)
                    SwitchTalkState(HumanTalkState.LISTENING);
                if (newState == HumanControllerState.INVENTORY)
                    HUDInventory.IsVisible = true;
                if (newState == HumanControllerState.CONTAINER)
                {

                    HUDContainer.IsVisible = true;
                    if (HUDContainer.Container.Contains.Count > 0)
                    {
                        HUDContainer.ActiveEq = 1;
                        HUDContainer.SelectIndex2 = 0;
                        HUDContainer.SelectIndex1 = -1;
                    }
                    else
                    {
                        HUDContainer.ActiveEq = 0;
                        HUDContainer.SelectIndex1 = 0;
                        HUDContainer.SelectIndex2 = -1;
                    }



                    HUDContainer.UpdateViewAll();


                }

                if (newState == HumanControllerState.SHOP)
                {
                    HUDShop.IsVisible = true;
                }
            }
            else if (State == HumanControllerState.TALK)
            {
                if (newState == HumanControllerState.FREE)
                {
                    HideTalkOverlay();
                    Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_SPACE);

                    if ((FocusObject as Character).Activities.Paused)
                    {
                        (FocusObject as Character).Activities.Paused = false;
                    }

                }
            }
            else if (State == HumanControllerState.CONTAINER)
            {
                if (newState == HumanControllerState.FREE)
                {


                    HUDContainer.IsVisible = false;

                }
            }

            else if (State == HumanControllerState.INVENTORY)
            {
                if (newState == HumanControllerState.FREE)
                    HUDInventory.IsVisible = false;
            }
            else if (State == HumanControllerState.SHOP)
            {
                if (newState == HumanControllerState.FREE)
                    HUDShop.IsVisible = false;
            }

            else if (State == HumanControllerState.MENU)
                HUDMenu.IsVisible = false;

            else if (State == HumanControllerState.STATS)
            {
                HUDStats.IsVisible = false;
                Character.Profile.Exp = ExpB4;
                Character.Statistics = StatisticsB4.statistics_Clone();
            }

            else if (State == HumanControllerState.CREATOR_STATS)
            {
                HUDNewCharacterStats.IsVisible = false;
            }


            State = newState;
        }

        public void SwitchTalkState(HumanTalkState newState)            // zmiana stanu bohatera w trakcie rozmowy
        {
            if (newState == HumanTalkState.CHOOSING_REPLY)
            {
                ShowReplies();
            }
            else if (newState == HumanTalkState.PAUSE)
            {
                HideTalkOverlay();
                TextRemainingTime = 0.5f;
            }
            else if (newState == HumanTalkState.LISTENING || newState == HumanTalkState.REPLYING)
            {
                TextIndex = 0;
                if (newState == HumanTalkState.LISTENING)
                {
                    TalkLabels.Last().SetColor(new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f));
                    BeginTextDisplay(CurrentNode.Text[0]);
                }
                else
                {
                    TalkLabels.Last().SetColor(new ColourValue(0.4f, 0.4f, 0.4f), new ColourValue(1.0f, 1.0f, 1.0f));
                    BeginTextDisplay(CurrentReply.Text[0]);
                }

                ShowTalkBox();
            }
            TalkState = newState;
        }

        public void UpdateRepliesColours()                  // podświetlenie aktualnie wybranej odpowiedzi w dialogu
        {
            for (int i = 0; i < ValidReplies.Count; i++)
            {
                if (i == SelectedReply)
                    TalkLabels[i].SetColor(new ColourValue(1, 1, 1), new ColourValue(1, 1, 1));
                else
                    TalkLabels[i].SetColor(new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f));
            }
        }

        public void SetRepliesText(TalkNode node)
        {
            SelectedReply = 0;
            ValidReplies.Clear();
            foreach (TextLabel label in TalkLabels)
                label.Caption = "";

            foreach (TalkReply reply in CurrentNode.Replies)
            {
                if (reply.IsConditionFulfilled())
                {
                    if (ValidReplies.Count > TalkLabels.Count)
                        throw new Exception("Zbyt dużo możliwych odpowiedzi");
                    TalkLabels[ValidReplies.Count].Caption = reply.Text[0].Text;
                    ValidReplies.Add(reply);
                }
            }
        }

        public void HandleConversation()                        // @@ funkcja odpowiedzialna za obsługę rozmowy
        {   
            TextRemainingTime -= Engine.FixedTimeStep;

            if (TalkState == HumanTalkState.LISTENING)
            {
                if (TextRemainingTime <= 0)
                {
                    TextIndex++;
                    if (TextIndex == CurrentNode.Text.Count)
                    {
                        if (CurrentNode.Replies.Count == 0)
                        {
                            SwitchState(HumanControllerState.FREE); // zakończenie rozmowy przez npc'a
                        }
                        else
                        {
                            SetRepliesText(CurrentNode);
                            SwitchTalkState(HumanTalkState.CHOOSING_REPLY);
                        }
                    }
                    else
                    {
                        BeginTextDisplay(CurrentNode.Text[TextIndex]);
                    }
                }
				else if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_SPACE))
				{
					TextRemainingTime = 0;
					Engine.Singleton.SoundManager.StopDialog();
				}
            }
            else if (TalkState == HumanTalkState.REPLYING)
            {
                if (TextRemainingTime <= 0)
                {
                    TextIndex++;
                    if (TextIndex == CurrentReply.Text.Count)
                    {
                        if (CurrentReply.IsEnding)
                        {
                            SwitchState(HumanControllerState.FREE);
                        }
                        else
                        {
                            TalkNode Tymczas = new TalkNode();
                            Tymczas.WhoSays = CurrentNode.WhoSays;
                            CurrentNode = CurrentReply.Reaction.PickNode(Tymczas.WhoSays);
                            SwitchTalkState(HumanTalkState.PAUSE);
                        }
                    }
                    else
                    {
                        BeginTextDisplay(CurrentReply.Text[TextIndex]);
                    }
                }
				else if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_SPACE) && !CurrentReply.IsEnding)
				{
					TextRemainingTime = 0;
					Engine.Singleton.SoundManager.StopDialog();
				}
            }
            else if (TalkState == HumanTalkState.PAUSE)
            {
                if (TextRemainingTime <= 0)
                {
                    SwitchTalkState(HumanTalkState.LISTENING);
                }
            }
            else if (TalkState == HumanTalkState.CHOOSING_REPLY)
            {
                if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_S))
                {
                    SelectedReply++;
                    if (SelectedReply == ValidReplies.Count)
                        SelectedReply = 0;
                }
                if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_W))
                {
                    SelectedReply--;
                    if (SelectedReply == -1)
                        SelectedReply = ValidReplies.Count - 1;
                }
                UpdateRepliesColours();

                if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_SPACE))
                {
                    CurrentReply = ValidReplies[SelectedReply];
                    SwitchTalkState(HumanTalkState.REPLYING);
                }
            }
        }

        private void HandleMenu()
        {
            HUDMenu.Update();
            bool Klik = false;
            int IndexKlika = 0;

            for (int i = 0; i < 6; i++)
            {
                TextLabel TL = HUDMenu.Options2Choose[i];
				if (TL.IsVisible)
				{
					if (HUDMenu.IsOver(TL) && Engine.Singleton.Menu.SubMenus[i].Enabled)
						Engine.Singleton.Menu.SubMenus[i].Selected = true;
					else
						Engine.Singleton.Menu.SubMenus[i].Selected = false;

					if (Engine.Singleton.Mysza && Engine.Singleton.Przycisk == MOIS.MouseButtonID.MB_Left && Engine.Singleton.Menu.SubMenus[i].Selected)
					{
						Engine.Singleton.Mysza = false;
						Klik = true;
						IndexKlika = i;
					}
				}
            }

            if (Klik)
            {
                if (Engine.Singleton.Menu.SubMenus[IndexKlika].Ending)
                {
                    SwitchState(HumanControllerState.FREE);                    
                }
                else
                    Engine.Singleton.Menu = Engine.Singleton.Menu.SubMenus[IndexKlika];

                Engine.Singleton.Menu.SubMenus[IndexKlika].CallActions();
                Klik = false;
                IndexKlika = 0;
            }
            
            
        }

        private void HandleInventory()                               // @@ funkcja odpowiedzialna za obsługę ekwipunku
        {
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_S))      // następny przedmiot z listy
                HUDInventory.SelectIndex++;
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_W))        // poprzedni przedmiot z listy
                HUDInventory.SelectIndex--;

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_Q))     // wyrzucenie wybranego przedmiotu
            {
                if (HUDInventory.SelectIndex != -1)
                {
                    if (Character.DropItem(HUDInventory.SelectIndex))
                    {
                        HUDInventory.SelectIndex = HUDInventory.SelectIndex;
                        HUDInventory.UpdateView();
                    }
                }
            }

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_F))              // założenie / zdjęcie wybranego przedmiotu
            {
                if (HUDInventory.SelectIndex != -1)
                {
                    if (Character.Inventory[HUDInventory.SelectIndex] is ItemSword)
                    {
                        if (Character.Sword != Character.Inventory[HUDInventory.SelectIndex])
                            Character.Sword = Character.Inventory[HUDInventory.SelectIndex] as ItemSword;
                        else
                            Character.Sword = null;
                        HUDInventory.UpdateItem(HUDInventory.SelectIndex);
                    }
                }
            }

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_TAB))       // opuszczenie ekranu ekwipunku
                SwitchState(HumanControllerState.FREE);
        }

        public void Update()
        {
            //Mysz.height = (int)Engine.Singleton.RenderWindow.Height;
            //Mysz.width = (int)Engine.Singleton.RenderWindow.Width;
            if (Character != null)
            {
                HUD.IsVisible = false;



                if (State == HumanControllerState.FREE)
                {
                    HandleMovement();
                    HUD.IsVisible = true;
                }
                else if (State == HumanControllerState.TALK)
                    HandleConversation();
                else if (State == HumanControllerState.INVENTORY)
                    HandleInventory();
                else if (State == HumanControllerState.CONTAINER)
                    HandleContainer();
                else if (State == HumanControllerState.SHOP)
                    HandleShop();
                else if (State == HumanControllerState.ATTACK)
                {
                    //FightInterface show!
                    HUD.IsVisible = true;
                    HUD.DrawEnemyHP = true;
                    HUD.DrawLog = true;
                    HandleAttack();

                }

                else if (State == HumanControllerState.STATS)
                    HandleStats();

                else if (State == HumanControllerState.MENU)
                    HandleMenu();

                else if (State == HumanControllerState.CREATOR_STATS)
                    HandleCreatorStats();

				if (InitShop)
				{
					State = HumanControllerState.SHOP;
                    HUDShop.SelectedOne = -1;
                    HUDShop.AktywnaStrona = 0;
					HandleShop();
					InitShop = false;
					HideTalkOverlay();
					HUDShop.IsVisible = true;
					HUDShop.UpdateViewAll();
				}
            }
        }

        private void HandleCreatorStats()
        {
            HUDNewCharacterStats.Update();
			if (Engine.Singleton.Mysza && Engine.Singleton.Przycisk == MOIS.MouseButtonID.MB_Left)
            {
				Engine.Singleton.Mysza = false;
                for (int i = 0; i < 6; i++)
                {
                    if (HUDNewCharacterStats.StatyLosu[i].IsOverAddPoint() && HUDNewCharacterStats.StatyLosu[i].AddPoint_Available)
                    {
                        switch (i)
                        {
                            case 0:
                                Character.Statistics.WalkaWrecz +=
                                    int.Parse(HUDNewCharacterStats.KwadratyLosu[HUDNewCharacterStats.KtoryZaznaczony].Wartosc.Caption);
                                break;

                            case 1:
                                Character.Statistics.Krzepa +=
                                    int.Parse(HUDNewCharacterStats.KwadratyLosu[HUDNewCharacterStats.KtoryZaznaczony].Wartosc.Caption);
                                break;

                            case 2:
                                Character.Statistics.Zrecznosc +=
                                    int.Parse(HUDNewCharacterStats.KwadratyLosu[HUDNewCharacterStats.KtoryZaznaczony].Wartosc.Caption);
                                break;

                            case 3:
                                Character.Statistics.Charyzma +=
                                    int.Parse(HUDNewCharacterStats.KwadratyLosu[HUDNewCharacterStats.KtoryZaznaczony].Wartosc.Caption);
                                break;

                            case 4:
                                Character.Statistics.Opanowanie +=
                                    int.Parse(HUDNewCharacterStats.KwadratyLosu[HUDNewCharacterStats.KtoryZaznaczony].Wartosc.Caption);
                                break;

                            case 5:
                                Character.Statistics.Odpornosc +=
                                    int.Parse(HUDNewCharacterStats.KwadratyLosu[HUDNewCharacterStats.KtoryZaznaczony].Wartosc.Caption);
                                break;

                        }

                        HUDNewCharacterStats.KwadratyLosu[HUDNewCharacterStats.KtoryZaznaczony].Zaznaczony = false;
                        HUDNewCharacterStats.KwadratyLosu[HUDNewCharacterStats.KtoryZaznaczony].Uzyty = false;
                        HUDNewCharacterStats.KtoryZaznaczony++;

                        if (HUDNewCharacterStats.KtoryZaznaczony < 6)
                            HUDNewCharacterStats.KwadratyLosu[HUDNewCharacterStats.KtoryZaznaczony].Zaznaczony = true;
                        else
                        {
                            HUDNewCharacterStats.DownArrowEnabled = true;
                        }

                        HUDNewCharacterStats.StatyLosu[i].SetAddPointUnavailable();
                    }
                }

                if (HUDNewCharacterStats.IsOverDownArrow() && HUDNewCharacterStats.DownArrowEnabled)
                {
                    Character.Statistics.Zywotnosc += Engine.Singleton.Kostka(1, 3);
                    Character.Statistics.aktualnaZywotnosc = Character.Statistics.Zywotnosc;
                    Character.Statistics.UpdateStatistics();
                    SwitchState(HumanControllerState.FREE);
                }

                if (HUDNewCharacterStats.IsOverQuad(HUDNewCharacterStats.ResetBg))
                {
                    Character.Statistics = new Statistics();
                    HUDNewCharacterStats.KtoryZaznaczony = 0;
                    HUDNewCharacterStats.DownArrowEnabled = false;

                    for (int i = 0; i < 6; i++)
                    {
                        HUDNewCharacterStats.KwadratyLosu[i].Uzyty = false;
                        HUDNewCharacterStats.KwadratyLosu[i].Zaznaczony = false;
                        HUDNewCharacterStats.StatyLosu[i].SetAddPointAvailable();
                    }

                    HUDNewCharacterStats.KwadratyLosu[0].Zaznaczony = true;
                }
            }
        }

        private void HandleAttack()
        {
			if (Engine.Singleton.Mouse.MouseState.ButtonDown(MOIS.MouseButtonID.MB_Left))
			{
				bool IsAlive = Character.FocusedEnemy.Statistics.aktualnaZywotnosc > 0;

				if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_W) && IsAlive)
				{
					Character.AttackOrder = true;
				}

                else if (!IsAlive && !AddedToKillList)
                {
                    String KogoZabiles = "";
                    if (CharacterProfileManager.E.ContainsKey(Character.FocusedEnemy.ProfName))
                        KogoZabiles = CharacterProfileManager.E[Character.FocusedEnemy.ProfName].DisplayName;
                    else
                        KogoZabiles = CharacterProfileManager.C[Character.FocusedEnemy.ProfName].DisplayName;
                    HUD.LogAdd(KogoZabiles + " umiera (" + Character.FocusedEnemy.DropExp.ToString() + " PD)", new ColourValue(1, 1, 0));
                    AddedToKillList = true;
                    Character.Profile.Exp += Character.FocusedEnemy.DropExp;

                    foreach (Quest q in Character.ActiveQuests.Quests)
                    {
                        if (q.KilledEnemies.ContainsKey(Character.FocusedEnemy.ProfName))
                            q.KilledEnemies[Character.FocusedEnemy.ProfName] += 1;
                    }
                }
			}

			else
			{
				Character.FocusedEnemy = null;
				HUD.DrawEnemyHP = false;
				SwitchState(HumanControllerState.FREE);
			}
        }

        private void HandleShop()           // @@ funka odpowiedzialna za obsluge SKLEPUFFFFFf
        {                                   //      TROLLLL!!!111111oneoneoneone!!!111TROLLED!
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_TAB))
            {
                SwitchState(HumanControllerState.FREE);
            }

			if (Engine.Singleton.Mysza && Engine.Singleton.Przycisk == MOIS.MouseButtonID.MB_Left)
            {
				Engine.Singleton.Mysza = false;
                bool Flag = false;


                int Obieg = 0;

                if (HUDShop.AktywnaStrona == 0)
                {
                    foreach (HUDShop.Slot S in HUDShop.Slots)
                    {
                        if (HUDShop.IsOver(S.BgQuad) && S.ItemPicture.Panel.MaterialName != "QuadMaterial")
                        {
                            if (S.isSelected && HUDShop.SelectedOne > -1 && !S.BlueQuad.IsVisible)
                            {
                                float CenaSprzedazy = Character.Inventory[HUDShop.SelectedOne].Price * 0.5f;
                                CenaSprzedazy += CenaSprzedazy * (Character.Statistics.Charyzma * 0.25f) / 100.0f;

                                if (HUDShop.Shop.Gold >= (int)CenaSprzedazy)
                                {
                                    HUDShop.Shop.Gold -= (int)CenaSprzedazy;
                                    Character.Profile.Gold += (ulong)CenaSprzedazy;
                                    HUDShop.Shop.Items.Add(Character.Inventory[HUDShop.SelectedOne]);
                                    HUDShop.Shop.WhoSays.Inventory.Add(Character.Inventory[HUDShop.SelectedOne]);
                                    HUDShop.Shop.WhoSays.Profile.Gold -= (ulong)CenaSprzedazy;
                                    Character.Inventory.RemoveAt(HUDShop.SelectedOne);
                                    HUDShop.AktywnaStrona = 0;
                                    HUDShop.SelectedOne = -1;
                                    HUDShop.UpdateViewAll();
                                    HUDShop.UpdateDescription();
                                    Flag = true;
                                }
                            }

                            else
                            {
                                HUDShop.SelectedOne = Obieg + HUDShop.SlotsCount * HUDShop.KtoraStrona;
                                S.isSelected = true;
                                Flag = true;
                            }
                        }
                        else
                            S.isSelected = false;

                        Obieg++;
                    }

                    if (!Flag)
                        HUDShop.AktywnaStrona = 1;
                }

                Obieg = 0;

                if (HUDShop.AktywnaStrona == 1)
                {
                    foreach (HUDShop.Slot S in HUDShop.Slots2)
                    {
                        if (HUDShop.IsOver(S.BgQuad) && S.ItemPicture.Panel.MaterialName != "QuadMaterial")
                        {
                            if (S.isSelected && HUDShop.SelectedOne > -1)
                            {
                                float CenaKupna = HUDShop.Shop.Items[HUDShop.SelectedOne].Price * HUDShop.Shop.Mnoznik;
                                CenaKupna -= CenaKupna * (Character.Statistics.Charyzma * 0.25f) / 100.0f;

                                if (Character.Profile.Gold >= (ulong)CenaKupna)
                                {
                                    Character.Profile.Gold -= (ulong)CenaKupna;
                                    HUDShop.Shop.Gold += (int)CenaKupna;
                                    HUDShop.Shop.WhoSays.Profile.Gold += (ulong)CenaKupna;
                                    Character.Inventory.Add(HUDShop.Shop.Items[HUDShop.SelectedOne]);
                                    HUDShop.Shop.Items.RemoveAt(HUDShop.SelectedOne);
                                    HUDShop.Shop.WhoSays.Inventory.RemoveAt(HUDShop.SelectedOne);
                                    HUDShop.AktywnaStrona = 0;
                                    HUDShop.SelectedOne = -1;
                                    HUDShop.UpdateViewAll();
                                    HUDShop.UpdateDescription();
                                }

                            }

                            else
                            {
                                HUDShop.SelectedOne = Obieg + HUDShop.SlotsCount * HUDShop.KtoraStrona;
                                S.isSelected = true;
                                Flag = true;
                            }
                        }
                        else
                            S.isSelected = false;

                        Obieg++;
                    }

                    if (Flag == false)
                    {
                        HUDShop.AktywnaStrona = 0;
                    }
                }

                if (Flag == false)
                {                    
                    HUDShop.SelectedOneB4 = HUDShop.SelectedOne;
                    HUDShop.SelectedOne = -1;
                }

                HUDShop.UpdateView();
                HUDShop.UpdateDescription();
            }

            if (Engine.Singleton.Mouse.MouseState.Z.rel > 0 && HUDShop.KtoraStrona > 0)     //scroll - gora!
            {
                HUDShop.KtoraStrona--;
                HUDShop.SelectedOne = -1;
                HUDShop.UpdateView();
                HUDShop.UpdateDescription();
            }

            else if (Engine.Singleton.Mouse.MouseState.Z.rel < 0)   //scroll - dol!
            {
                if (HUDShop.AktywnaStrona == 0 && Character.Inventory.Count >= HUDShop.KtoraStrona * HUDShop.SlotsCount)
                {
                    HUDShop.KtoraStrona++;
                    HUDShop.SelectedOne = -1;
                    HUDShop.UpdateView();
                    HUDShop.UpdateDescription();
                }

                if (HUDShop.AktywnaStrona == 1 && HUDShop.Shop.Items.Count >= HUDShop.KtoraStrona * HUDShop.SlotsCount)
                {
                    HUDShop.KtoraStrona++;
                    HUDShop.SelectedOne = -1;
                    HUDShop.UpdateView();
                    HUDShop.UpdateDescription();
                }
            }

            HUDShop.MouseCursor.SetDimensions(Engine.Singleton.GetFloatFromPxWidth(Engine.Singleton.Mouse.MouseState.X.abs), Engine.Singleton.GetFloatFromPxHeight(Engine.Singleton.Mouse.MouseState.Y.abs), Engine.Singleton.GetFloatFromPxWidth(32), Engine.Singleton.GetFloatFromPxHeight(32));
        }

        private void HandleStats()
        {
            HUDStats.Update();
			

            bool kibel = false;

            for (int i = 0; i < 7; i++)
            {
                if (HUDStats.Stats[i].AddAble && HUDStats.Stats[i].IsOverAddPoint())
                {
                    if (!HUDStats.Stats[i].AddPoint_Available)
                    {
                        String Req = "Wymagane ";
                        switch (i)
                        {
                            case 0:
                                Req += (Character.Statistics.Ile_WW + 1) * 100;
                                break;
                            case 1:
                                Req += (Character.Statistics.Ile_KR + 1) * 100;
                                break;
                            case 2:
                                Req += (Character.Statistics.Ile_ZR + 1) * 100;
                                break;
                            case 3:
                                Req += (Character.Statistics.Ile_ZY + 1) * 100;
                                break;
                            case 4:
                                Req += (Character.Statistics.Ile_CH + 1) * 100;
                                break;
                            case 5:
                                Req += (Character.Statistics.Ile_OP + 1) * 100;
                                break;
                            case 6:
                                Req += (Character.Statistics.Ile_ODP + 1) * 100;
                                break;
                        }

                        Req += " PD";
                        kibel = true;
                        HUDStats.Required.IsVisible = true;
                        HUDStats.RequiredBg.IsVisible = true;
                        HUDStats.Required.Caption = Req;
                    }
                }
            }

            if (!kibel)
            {
                HUDStats.Required.IsVisible = false;
                HUDStats.RequiredBg.IsVisible = false;
            }

            if (Engine.Singleton.Mysza && Engine.Singleton.Przycisk == MOIS.MouseButtonID.MB_Left)
            {
                Engine.Singleton.Mysza = false;

                for (int i = 0; i < 7; i++)
                {
                    if (HUDStats.Stats[i].AddAble && HUDStats.Stats[i].IsOverAddPoint())
                    {
                        if (HUDStats.Stats[i].AddPoint_Available)
                        {
                            switch (i)
                            {
                                case 0:
                                    Character.Statistics.Ile_WW++;
                                    Character.Statistics.WalkaWrecz += 5;
                                    Character.Profile.Exp -= (Character.Statistics.Ile_WW) * 100;
                                    break;

                                case 1:
                                    Character.Statistics.Ile_KR++;
                                    Character.Statistics.Krzepa += 5;
                                    Character.Profile.Exp -= (Character.Statistics.Ile_KR) * 100;
                                    Character.Statistics.Sila = Character.Statistics.Krzepa / 10;
                                    break;

                                case 2:
                                    Character.Statistics.Ile_ZR++;
                                    Character.Statistics.Zrecznosc += 5;
                                    Character.Profile.Exp -= (Character.Statistics.Ile_ZR) * 100;
                                    break;

                                case 3:
                                    Character.Statistics.Ile_ZY++;
                                    Character.Statistics.Zywotnosc += 1;
                                    Character.Profile.Exp -= (Character.Statistics.Ile_ZY) * 100;
                                    break;

                                case 4:
                                    Character.Statistics.Ile_CH++;
                                    Character.Statistics.Charyzma += 5;
                                    Character.Profile.Exp -= (Character.Statistics.Ile_CH) * 100;
                                    break;

                                case 5:
                                    Character.Statistics.Ile_OP++;
                                    Character.Statistics.Opanowanie += 5;
                                    Character.Profile.Exp -= (Character.Statistics.Ile_OP) * 100;
                                    break;

                                case 6:
                                    Character.Statistics.Ile_ODP++;
                                    Character.Statistics.Odpornosc += 5;
                                    Character.Profile.Exp -= (Character.Statistics.Ile_ODP) * 100;
                                    Character.Profile.Statistics.Wytrzymalosc = Character.Statistics.Odpornosc / 10;
                                    break;
                            }
                        }
                    }

                    else if (HUDStats.Stats[i].AddAble && HUDStats.Stats[i].IsOverRemovePoint() && HUDStats.Stats[i].RemovePoint_Available)
                    {
                        switch (i)
                        {
                            case 0:
                                Character.Statistics.Ile_WW--;
                                Character.Statistics.WalkaWrecz -= 5;
                                Character.Profile.Exp += (Character.Statistics.Ile_WW + 1) * 100;
                                break;

                            case 1:
                                Character.Statistics.Ile_KR--;
                                Character.Statistics.Krzepa -= 5;
                                Character.Profile.Exp += (Character.Statistics.Ile_KR + 1) * 100;
                                Character.Statistics.Sila = Character.Statistics.Krzepa / 10;
                                break;

                            case 2:
                                Character.Statistics.Ile_ZR--;
                                Character.Statistics.Zrecznosc -= 5;
                                Character.Profile.Exp += (Character.Statistics.Ile_ZR + 1) * 100;
                                break;

                            case 3:
                                Character.Statistics.Ile_ZY--;
                                Character.Statistics.Zywotnosc -= 1;
                                Character.Profile.Exp += (Character.Statistics.Ile_ZY + 1) * 100;
                                break;

                            case 4:
                                Character.Statistics.Ile_CH--;
                                Character.Statistics.Charyzma -= 5;
                                Character.Profile.Exp += (Character.Statistics.Ile_CH + 1) * 100;
                                break;

                            case 5:
                                Character.Statistics.Ile_OP--;
                                Character.Statistics.Opanowanie -= 5;
                                Character.Profile.Exp += (Character.Statistics.Ile_OP + 1) * 100;
                                break;

                            case 6:
                                Character.Statistics.Ile_ODP--;
                                Character.Statistics.Odpornosc -= 5;
                                Character.Profile.Exp += (Character.Statistics.Ile_ODP + 1) * 100;
                                Character.Profile.Statistics.Wytrzymalosc = Character.Statistics.Odpornosc / 10;
                                break;
                        }
                    }

                    else if (HUDStats.IsOverZmiany())
                    {
                        StatisticsB4 = Character.Statistics.statistics_Clone();
                        ExpB4 = Character.Profile.Exp;
                        SwitchState(HumanControllerState.FREE);
                    }
                }
            }

            if (HUDStats.IsOverZmiany())
            {
                HUDStats.Zmiany.SetColor(new ColourValue(1.0f, 0, 0.2f), new ColourValue(1, 1.0f, 0.6f));
            }

            else
            {
                HUDStats.Zmiany.SetColor(new ColourValue(0.7f, 0.4f, 0), new ColourValue(1, 1.0f, 0.6f));
            }

			if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_C))
			{
				SwitchState(HumanControllerState.FREE);
				HUDStats.IsVisible = false;
			}
        }

        private void HandleContainer()      // @@ funkcja odpowiedzialna za obsługę kontenerów
        {
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_S))      // następny przedmiot z listy
            {
                if (HUDContainer.ActiveEq == 1)
                    HUDContainer.SelectIndex2++;
                else if (HUDContainer.ActiveEq == 0)
                    HUDContainer.SelectIndex1++;

                HUDContainer.UpdateView();
            }
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_W))        // poprzedni przedmiot z listy
            {
                if (HUDContainer.ActiveEq == 1)
                    HUDContainer.SelectIndex2--;
                else if (HUDContainer.ActiveEq == 0)
                    HUDContainer.SelectIndex1--;

                HUDContainer.UpdateView();
            }
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_A))  // zmiana aktualnego ekwipunku
            {
                if (HUDContainer.ActiveEq == 1)
                {
                    HUDContainer.ActiveEq = 0;
                    HUDContainer.SelectIndex2 = -1;
                    HUDContainer.SelectIndex1 = 0;
                }
            }
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_D)) // zmiana aktualnego ekwipunku
            {
                if (HUDContainer.ActiveEq == 0)
                {
                    HUDContainer.ActiveEq = 1;
                    HUDContainer.SelectIndex2 = 0;
                    HUDContainer.SelectIndex1 = -1;
                }

                HUDContainer.UpdateView();
            }
            
            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_F))              // przerzucenie przedmiotu między ekwipunkami
            {
                if (HUDContainer.ActiveEq == 1 && HUDContainer.SelectIndex2 != -1)
                {
                    Character.Inventory.Add(HUDContainer.Container.Contains[HUDContainer.SelectIndex2]);
                    HUDContainer.Container.Contains.RemoveAt(HUDContainer.SelectIndex2);
                    HUDContainer.SelectIndex2 = 0;
                }
                else if (HUDContainer.ActiveEq == 0 && HUDContainer.Container.CanAdd && HUDContainer.Container.Contains.Count < HUDContainer.Container.MaxItems && HUDContainer.SelectIndex1 != -1)
                {
                    HUDContainer.Container.Contains.Add(Character.Inventory[HUDContainer.SelectIndex1]);
                    Character.Inventory.RemoveAt(HUDContainer.SelectIndex1);
                    HUDContainer.SelectIndex1 = 0;
                }
                


                HUDContainer.UpdateViewAll();
            }


            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_TAB))       // opuszczenie ekranu kontenera
            {
                HUDContainer.Container = new Container();
                SwitchState(HumanControllerState.FREE);
            }
        }

        private void HandleMovement()                             // @@ funkcja odpowiedzialna za całokształt poruszania się
        {
			Character.TurnDelta = -Engine.Singleton.Mouse.MouseState.X.rel * 0.1f * MouseSpeed;			// obracanie postaci

			Degree akt = Engine.Singleton.GameCamera.Angle;
			Degree zmiana;

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_U))
            {
                long currentTime = Engine.Singleton.Root.Timer.Milliseconds / 1000;
                Console.WriteLine(currentTime);
                Activity Walk = new Activity();
                Walk.v3 = new Vector3(6.42f, -1.0f, 9);
                Walk.Type = ActivityType.WALK;

                Activity Walk2 = new Activity();
                Walk2.v3 = new Vector3(-9.3f, -1.07f, 8.94f);
                Walk2.Type = ActivityType.WALK;

                Activity Wait = new Activity();
                Wait.i = 5;
                Wait.Type = ActivityType.WAIT;

                if (FocusObject is Character)
                {
                    (FocusObject as Character).Activities.Repeat = true;
                    (FocusObject as Character).Activities.Activities.Add(Walk);
                    (FocusObject as Character).Activities.Activities.Add(Walk2);
                    (FocusObject as Character).Activities.Activities.Add(Wait);
                }
            }

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_F5))  // quick save
            {
                if (System.IO.Directory.Exists("Saves\\QuickSave"))
                    System.IO.Directory.Delete("Saves\\QuickSave", true);
                Engine.Singleton.CopyAll(new System.IO.DirectoryInfo("Saves\\AutoSave"), new System.IO.DirectoryInfo("Saves\\QuickSave"));
                Engine.Singleton.AutoSave("QuickSave");
            }

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_F9)) // quick load
                Engine.Singleton.Load("QuickSave");

			if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_J))
			{
				if (FocusObject is Character)
				{
					(FocusObject as Character).obejdz = true;

				}

			}
	
			if (!InvertMouse)																	// ruszanie kamerą (góra i dół)
				zmiana = new Degree(Engine.Singleton.Mouse.MouseState.Y.rel * 0.1f * MouseSpeed);
			else
				zmiana = new Degree(-Engine.Singleton.Mouse.MouseState.Y.rel * 0.1f * MouseSpeed);
			
			if (zmiana + akt < new Degree(70) && zmiana + akt > new Degree(-45))
				Engine.Singleton.GameCamera.Angle += zmiana;
			
			if (Engine.Singleton.Mouse.MouseState.Z.rel != 0)
	        {

				if (Engine.Singleton.GameCamera.Distance -(float)Engine.Singleton.Mouse.MouseState.Z.rel * 0.005f < 10.0f && Engine.Singleton.GameCamera.Distance - (float)Engine.Singleton.Mouse.MouseState.Z.rel * 0.005f > 2.0f)
					Engine.Singleton.GameCamera.Distance -= (float)Engine.Singleton.Mouse.MouseState.Z.rel *0.005f;

	        }

			if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_F))     // podnoszenie, otwieranie, itp. 
            {
                if (FocusObject != null)
                {
                    if (FocusObject is Described)
                    {
						if ((FocusObject as Described).IsPickable)
							Character.TryPick(FocusObject as Described);
						else if ((FocusObject as Described).IsContainer)             // czy jest kontenerem
						{
							HUDContainer.Container = (FocusObject as Described).Container;
							SwitchState(HumanControllerState.CONTAINER);
						}
						else														// AKTYWATOR!!!!!! WOLOLO!
						{
							(FocusObject as Described).PerformAkt = true;
						}
                    }
                    if (FocusObject is Character)
                    {
                        if ((FocusObject as Character).IsContainer)             // czy człowiek jest kontenerem
                        {
                            HUDContainer.Container.Contains = (FocusObject as Character).Inventory;
                            HUDContainer.Container.Gold = (int)((FocusObject as Character).Profile.Gold);
                            SwitchState(HumanControllerState.CONTAINER);
                        }
                    }

                    if (FocusObject is Enemy)
                    {
                        if ((FocusObject as Enemy).IsContainer)             // czy człowiek jest kontenerem
                        {
                            HUDContainer.Container.Contains = (FocusObject as Enemy).DropPrize.ItemsList;
                            Character.Profile.Gold += (ulong)(FocusObject as Enemy).DropPrize.AmountGold;
                            (FocusObject as Enemy).DropPrize.AmountGold = 0;
                            SwitchState(HumanControllerState.CONTAINER);
                        }
                    }

                }
            }

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_C))
                SwitchState(HumanControllerState.STATS);

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_K))
                Character.Statistics.aktualnaZywotnosc = Character.Statistics.Zywotnosc;
                //SwitchState(HumanControllerState.CREATOR_STATS);

            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_Z))         // tzw. skok
                Character.Position = new Vector3(Character.Position.x, Character.Position.y + 1, Character.Position.z);

            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_X))          // wypisanie w konsoli aktualnej pozycji postaci
            {
                Console.Write("Pozycja: ");
                Console.Write(Character.Position.x);
                Console.Write(", ");
                Console.Write(Character.Position.y);
                Console.Write(", ");
                Console.WriteLine(Character.Position.z);
            }

            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_B))           // wypisanie w konsoli aktywnych kłestów
            {
                Console.WriteLine("Aktywne Questy: ");
                foreach (Quest Q in Engine.Singleton.HumanController.Character.ActiveQuests.Quests)
                {
                    if (!Q.IsFinished)
                    {
                        Console.Write("- ");
                        Console.Write(Q.Name + "\n");
                    }

                }
            }

            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_SPACE))       // @@ rozpoczęcie rozmowy
            {
                if (FocusObject != null)
                {
                    if (Character.TalkPerm && FocusObject.TalkRoot != null)
                    {
                        if (FocusObject is Character)
                        {
                            FocusObject.TalkRoot.WhoSays = (FocusObject as Character);
                            (FocusObject as Character).TurnTo(Character.Position);

                            if ((FocusObject as Character).Activities.InProgress)
                            {
                                (FocusObject as Character).Activities.Paused = true;
                                (FocusObject as Character).Activities.InProgress = false;
                                (FocusObject as Character).Waiting = false;
                                (FocusObject as Character).FollowPathOrder = false;
                            }
                        }

                        CurrentNode = FocusObject.TalkRoot.PickNode();
                        SwitchState(HumanControllerState.TALK);
                    }
                }
            }

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_TAB)                    // @@ Otwarcie ekwipunku
                && Character.InventoryPerm)
                SwitchState(HumanControllerState.INVENTORY);

			if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_A))          // obrót postaci
			{
				Character.MoveRightOrder = false;
				Character.MoveLeftOrder = true;
				Character.MoveOrder = false;
				Character.RunOrder = false;
				Character.MoveOrderBack = false;
			}
			else
				Character.MoveLeftOrder = false;

			if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_D))
			{
				Character.MoveRightOrder = true;
				Character.MoveLeftOrder = false;
				Character.MoveOrder = false;
				Character.RunOrder = false;
				Character.MoveOrderBack = false;
			}
			else
				Character.MoveRightOrder = false;

			if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_L))
				HUD.ToggleLog();

            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_W))            // chodzenie do przodu +bieganie
            {
                Character.MoveOrder = true;
                Character.MoveOrderBack = false;
				Character.MoveRightOrder = false;
				Character.MoveLeftOrder = false;
                if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_LSHIFT))
                {
                    if (!Character.RunOrder)
                        Character.RunOrder = true;
                    else
                        Character.RunOrder = false;
                }
            }
            else
            {
                Character.MoveOrder = false;
                Character.RunOrder = false;
            }
				
            if (Engine.Singleton.Keyboard.IsKeyDown(MOIS.KeyCode.KC_S))			    // "chodzenie" do tyłu. W aktualnej wersji 
            {                                                                       // przesuwanie o wektor przeciwny w osiach 
                Character.MoveOrder = false;                                        // x i z do wektora skierowanego do przodu
                Character.MoveOrderBack = true;                                     // postaci (zwyczajny brak odpowiedniej animacji)
				Character.MoveRightOrder = false;
				Character.MoveLeftOrder = false;
            }
            else
                Character.MoveOrderBack = false;

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_COMMA))     // poprzednia piosenka
                Engine.Singleton.SoundManager.PreviousBGM();
            else if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_PERIOD)) // następna piosenka
                Engine.Singleton.SoundManager.NextBGM();

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_SLASH))
                Engine.Singleton.SoundManager.TogglePauseBGM();

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_H))             // wyświetlanie i chowanie HUD'a
            {
                HUD.ToggleVisibility();
            }

            if (Engine.Singleton.IsKeyTyped(MOIS.KeyCode.KC_E))                 // @@@ Wyciąganie i chowanie miecza.
            {
                if (Character.Sword != null && !Character.Sword.InUse)
                {
                    Character.MoveOrder = false;
                    Character.RunOrder = false;
                    Character.MoveOrderBack = false;
                    Character.GetSwordOrder = true;
					HUD.DrawLog = true;
                }
                else if (Character.Sword != null && Character.Sword.InUse)
                {
                    Character.MoveOrder = false;
                    Character.RunOrder = false;
                    Character.MoveOrderBack = false;
                    Character.HideSwordOrder = true;
					HUD.DrawLog = false;
                }
            }

			if (Engine.Singleton.Mouse.MouseState.ButtonDown(MOIS.MouseButtonID.MB_Left))
			{
				if (Character.Contact != null)
				{
					if (Character.Contact is ISomethingMoving)
					{
						if ((Character.Contact as ISomethingMoving).FriendlyType != Gra.Character.FriendType.FRIENDLY && (Character.Contact as ISomethingMoving).State != Enemy.StateTypes.DEAD)
						{
							if (Character.Sword != null && Character.Sword.InUse)
							{
                                if ((Character.Contact as ISomethingMoving).FriendlyType == Gra.Character.FriendType.NEUTRAL)
                                    (Character.Contact as ISomethingMoving).FriendlyType = Gra.Character.FriendType.ENEMY;
                                
                                Character.MoveLeftOrder = false;
                                Character.MoveRightOrder = false;
                                Character.MoveOrder = false;
                                Character.MoveOrderBack = false;
                                AddedToKillList = false;
								Character.FocusedEnemy = (ISomethingMoving)Character.Contact;
								SwitchState(HumanControllerState.ATTACK);
							}
						}
					}
				}
			}

            if (Character.Contact != null && Character.Contact.Exists)
            {

                FocusObject = Character.Contact as SelectableObject;
                string lol = null;

                if (FocusObject is ISomethingMoving)
                {
                }

                if (FocusObject.IsContainer == true)
                    lol = "\n(Otworz)";

                TargetLabel.Caption = FocusObject.DisplayName + lol;
                TargetLabel.Position3D = FocusObject.Position + FocusObject.DisplayNameOffset;
                TargetLabel.IsVisible = true;
            }
            else
            {
                FocusObject = null;
                TargetLabel.IsVisible = false;
            }
        }

        public void ToggleHud()
        {
            HUD.ToggleVisibility();
        }
    }
}
