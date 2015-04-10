using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Urgot
{
    internal class DecisionExecutor
    {
	    public static enum DecisionOrder
	   {
	            Kill,
	            Mixed,
	            LaneClear,
	            Combo,
	            None
	   }
	    
	        
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
            
        }
    }
}
