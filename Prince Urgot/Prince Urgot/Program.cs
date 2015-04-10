using System;
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
            
            Game.OnUpdate += Game_OnGameUpdate;
         }    

    	    
    	
	    private static void Game_OnGameUpdate(EventArgs args)
	    {
	        Orbwalker.setMode(Orbwalking.OrbwalkingMode.Combo);
	    	var furthest_minion = ObjectManager.Get<Obj_AI_Minion>().Where(minion => minion.IsAlly && !minion.IsDead).OrderBy(turr => turr.Distance(ObjectManager.Player)).First();
	    }        
       
    }
}
