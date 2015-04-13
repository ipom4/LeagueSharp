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

        static readonly ItemId[] SRShopList = { ItemId.Hextech_Revolver, ItemId.Hextech_Gunblade, ItemId.Sword_of_the_Occult , ItemId.Spirit_Visage, ItemId.Rod_of_Ages, ItemId.Will_of_the_Ancients, ItemId.Last_Whisper };//, ItemId.Liandrys_Torment, ItemId.Lich_Bane, ItemId.Locket_of_the_Iron_Solari, ItemId.Rod_of_Ages, ItemId.Void_Staff, ItemId.Hextech_Gunblade, ItemId.Sorcerers_Shoes };
        
        
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
            new AutoLevel(TreesAutoLevel.GetSequence().Select(l=>l-1));
            AutoLevel.Enable();
            
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
        
        private static void GameOnOnGameUpdate(EventArgs args)
        {
            //var minionChef = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.Distance(ShitNexus, true)).First(m => m.Team == Player.Team);
            Obj_AI_Minion minionChef = ObjectManager.Get<Obj_AI_Minion>().OrderBy(m => m.Distance(ShitNexus, true)).First(m => m.Name.Contains("L1") && m.Name.Contains("Minion") && (!m.IsDead) && (m.IsAlly) && (m.Distance(ObjectManager.Get<Obj_AI_Turret>().OrderBy(t => t.Distance(m, true)).First(t => t.Team != m.Team), true)>800));
            
            Obj_AI_Turret turretChef = ObjectManager.Get<Obj_AI_Turret>().OrderBy(t => t.Distance(Player, true)).First(t => t.IsEnemy);
            
            
            var target = TargetSelector.GetTarget(-1, TargetSelector.DamageType.Physical);
            //Game.PrintChat(Player.Distance(turretChef, true).ToString());
            if(Player.Distance(turretChef, true)>1000000)//Si played not under turret
            {
                if (target != null)
                {
                
                    Orbwalker.setMode(Orbwalking.OrbwalkingMode.Combo);
                    //Orbwalker.SetOrbwalkingPoint(minionChef.Position);//(target.Position - minionChef.Position)*0.5f+minionChef.Position);
                    Orbwalker.setMoveMode(OrbDancer.MoveModeType.WalkAround, minionChef, 500);
                    //Game.PrintChat(minionChef.Name);
                }
                else
                {
                    Orbwalker.setMode(Orbwalking.OrbwalkingMode.LaneClear);
                    //Orbwalker.SetOrbwalkingPoint(minionChef.Position);
                    Orbwalker.setMoveMode(OrbDancer.MoveModeType.WalkAround, minionChef, 500);
                    //Game.PrintChat(minionChef.Name);
                }
            }
            else
            {
                if(minionChef.Distance(turretChef, true)>1000000)
                {
                    Orbwalker.setMode(Orbwalking.OrbwalkingMode.LastHit);
                    //Orbwalker.SetOrbwalkingPoint(minionChef.Position);
                    Orbwalker.setMoveMode(OrbDancer.MoveModeType.WalkAround, minionChef, 10);
                }
                else
                {
                    Orbwalker.setMode(Orbwalking.OrbwalkingMode.LastHit);
                    //Orbwalker.SetOrbwalkingPoint(minionChef.Position);
                    Orbwalker.setMoveMode(OrbDancer.MoveModeType.WalkAround, minionChef, 500);
                }
            }
            
            if (SpellClass.W.IsReady())// && distance <= 100 || (distance >= 900 && distance <= 1200) && t.HasBuff("urgotcorrosivedebuff", true))
            {
                SpellClass.W.Cast(Player, true);
            }
            if (SpellClass.D.IsReady())
            {
                //SpellClass.D.Cast(minionChef, true);
            }
            
                            
            foreach (var item in SRShopList)
                    {
                        if (!Items.HasItem((int)item))
                        {
                            Player.BuyItem(item);
                        }
                    }
        }
    }
}
