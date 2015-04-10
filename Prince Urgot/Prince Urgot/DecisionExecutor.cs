using System;
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
    }
}
