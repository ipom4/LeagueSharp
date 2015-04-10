
﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Urgot
{
    internal class DecisionExecutor
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        public DecisionExecutor(Menu comboMenu)
        {
            ComboMenu = comboMenu;
            Menu();
            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            PacketCast = ComboMenu.SubMenu("misc").Item("packetcast").GetValue<bool>();
            SpellClass.R.Range = 400 + (150 * SpellClass.R.Level);

            if (Program.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                Combo();
        }
        #region Menu
        internal static void Menu()
        {
            ComboMenu.AddSubMenu(new Menu("Combo", "combo"));

            ComboMenu.SubMenu("combo").AddItem(new MenuItem("useQ", "Use Q")).SetValue(true);
            ComboMenu.SubMenu("combo").AddItem(new MenuItem("useW", "Use W")).SetValue(true);
            ComboMenu.SubMenu("combo").AddItem(new MenuItem("useE", "Use E")).SetValue(true);
            ComboMenu.SubMenu("combo").AddItem(new MenuItem("useR", "Use R (Auto R under Tower)")).SetValue(true);
            ComboMenu.SubMenu("combo").AddItem(new MenuItem("preE", "HitChance E").SetValue(new StringList((new[] { "Low", "Medium", "High", "Very High" }))));

            ComboMenu.AddSubMenu(new Menu("Misc", "misc"));
            ComboMenu.SubMenu("misc").AddItem(new MenuItem("autoInt", "Interrupt with " + "R").SetValue(false));
            ComboMenu.SubMenu("misc").AddItem(new MenuItem("packetcast", "Use PacketCast")).SetValue(true);
        }
        #endregion

        #region Interrupter
        private static void Interrupter_OnPossibleToInterrupt (Obj_AI_Base unit, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (SpellClass.R.IsReady() && unit.IsEnemy && SpellClass.R.IsInRange(unit) && ComboMenu.Item("autoInt").GetValue<bool>())
            {
                SpellClass.R.CastOnUnit(unit, PacketCast);
            }
        }
        #endregion

        #region Combo
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(1200, TargetSelector.DamageType.Physical);
            var castQ = (ComboMenu.Item("useQ").GetValue<bool>());
            var castE = (ComboMenu.Item("useW").GetValue<bool>());
            var castW = (ComboMenu.Item("useE").GetValue<bool>());
            var castR = (ComboMenu.Item("useR").GetValue<bool>());

            if (target == null)
            {
                return;
            }
            if (castE)
            {
                SpellE(target);
            }
            if (castQ)
            {
                SpellSecondQ();
                SpellQ(target);
            }
            if (castW)
            {
                SpellW(target);
            }
            if (castR)
            {
                AutoR(target);
            }
            
        }
        #endregion
    }
}
