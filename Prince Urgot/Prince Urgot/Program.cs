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
        internal static Orbwalker OrbDancer;

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
            
            
            
   /*private static void BuffTest()
        {
            foreach (var turret in ObjectManager.Get<Obj_AI_Turret>().Where(t => t.IsVisible && !t.IsMe && (Player.Position.Distance(turret.Position) < 1500f)))
            {
                foreach (var bufflist in turret.Buffs)
                {
  
                    Drawing.DrawText(Drawing.WorldToScreen(turret.Position).X, Drawing.WorldToScreen(turret.Position).Y, Color.LimeGreen, string.Format("{0}", bufflist.Name));

                }

            }

        }*/           
        }
    }
}
