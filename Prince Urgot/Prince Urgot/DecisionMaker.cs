using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Urgot
{
	internal class DecisionMaker
	{

        
	    private DecisionExecutor DE;
	    
	    private static Obj_AI_Hero Player
	    {
	        get { return ObjectManager.Player; }
	    }
	
	    public DecisionMaker(DecisionExecutor dE)
	    {
	    	DE = dE;
	    	Game.OnUpdate += Game_OnGameUpdate;
	    }
	    
	    public DecisionMaker()
	    {
	    	this(new DecisionExecutor());
	    }
	    
	    
	    private static void Game_OnGameUpdate(EventArgs args)
	    {
	    	DE.setDecision(DE.DecisionOrder.Kill);
	    }
	}
}

