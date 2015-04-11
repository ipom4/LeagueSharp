using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Urgot
{
    internal class Program
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        internal static Menu UrgotConfig;
        internal static Menu TargetSelectorMenu;
        //internal static Orbwalking.Orbwalker Orbwalker;
        internal static OrbDancer Orbwalker;
        
        public static Obj_HQ ShitNexus = ObjectManager.Get<Obj_HQ>().First(n => n.Team != Player.Team);

        public static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Urgot")
            {
                return;
            }
            
            ShitNexus = ObjectManager.Get<Obj_HQ>().First(n => n.Team != Player.Team);
            MainMenu();
            new SpellClass();

            Game.PrintChat("<b><font color =\"#FFFFFF\">Prince Urgot</font></b><font color =\"#FFFFFF\"> by </font><b><font color=\"#FF66FF\">Leia</font></b><font color =\"#FFFFFF\"> loaded!</font>");
        }

        static void MainMenu()
        {
            UrgotConfig = new Menu("Prince " + ObjectManager.Player.ChampionName, "Prince" + ObjectManager.Player.ChampionName, true);

            UrgotConfig.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            //Orbwalker = new Orbwalking.Orbwalker(UrgotConfig.SubMenu("Orbwalking"));
            Orbwalker = new OrbDancer(UrgotConfig.SubMenu("Orbwalking"));

            TargetSelectorMenu = new Menu("Target Selector", "Common_TargetSelector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            UrgotConfig.AddSubMenu(TargetSelectorMenu);

            new ComboClass(UrgotConfig);
            new HarassClass(UrgotConfig);
            new LaneClearClass(UrgotConfig);
            new ItemClass(UrgotConfig);
            new DrawingClass(UrgotConfig);
            UrgotConfig.AddToMainMenu();
            
            Game.OnUpdate += GameOnOnGameUpdate;         
        }
        
        private void GameOnOnGameUpdate(EventArgs args)
        {
            var minionChef = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.Distance(ShitNexus, true)).First(m => m.Team == Player.Team);
            
            var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                Orbwalker.setMode(Orbwalking.OrbwalkingMode.Combo);
                Orbwalker.SetOrbwalkingPoint((target.Position - minionChef.Position)*0.5f+minionChef.Position);
            }
            else
            {
                Orbwalker.setMode(Orbwalking.OrbwalkingMode.LaneClear);
                Orbwalker.SetOrbwalkingPoint(minionChef.Position);
            }

            
            
        }
    }
}
