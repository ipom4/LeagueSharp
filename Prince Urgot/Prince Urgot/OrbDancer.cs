using System;
using System.Linq;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Urgot
{
    internal class OrbDancer : Orbwalking.Orbwalker
    {
        private const float LaneClearWaitTimeMod = 2f;
        private static Menu _config;
        private readonly Obj_AI_Hero Player;
        private Obj_AI_Base _forcedTarget;
        private Orbwalking.OrbwalkingMode _mode = Orbwalking.OrbwalkingMode.None;
        private Vector3 _orbwalkingPoint;
        private Obj_AI_Minion _prevMinion;
            
            
        public OrbDancer(Menu attachToMenu): base(attachToMenu)
        {
            
        }
        
        private void GameOnOnGameUpdate(EventArgs args)
        {
            
            
            
            try
            {
                if (ActiveMode == Orbwalking.OrbwalkingMode.None)
                {
                    return;
                }

                //Prevent canceling important spells
                if (Player.IsCastingInterruptableSpell(true))
                {
                    return;
                }

                var target = GetTarget();
                Orbwalking.Orbwalk(
                    target, (_orbwalkingPoint.To2D().IsValid()) ? _orbwalkingPoint : Game.CursorPos,
                    _config.Item("ExtraWindup").GetValue<Slider>().Value,
                    _config.Item("HoldPosRadius").GetValue<Slider>().Value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public void setMode(Orbwalking.OrbwalkingMode owMode)
        {
            if (owMode == Orbwalking.OrbwalkingMode.None)
            {
                _config.Item("Orbwalk").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Toggle, false));
                _config.Item("LaneClear").SetValue<KeyBind>(new KeyBind('V', KeyBindType.Toggle, false));
                _config.Item("Farm").SetValue<KeyBind>(new KeyBind('+', KeyBindType.Toggle, false));
                _config.Item("LastHit").SetValue<KeyBind>(new KeyBind('X', KeyBindType.Toggle, false));
            }
            else if (owMode == Orbwalking.OrbwalkingMode.Combo)
            {
                _config.Item("Orbwalk").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Toggle, true));
                _config.Item("LaneClear").SetValue<KeyBind>(new KeyBind('V', KeyBindType.Toggle, false));
                _config.Item("Farm").SetValue<KeyBind>(new KeyBind('+', KeyBindType.Toggle, false));
                _config.Item("LastHit").SetValue<KeyBind>(new KeyBind('X', KeyBindType.Toggle, false));                
            }
            else if (owMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                _config.Item("Orbwalk").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Toggle, false));
                _config.Item("LaneClear").SetValue<KeyBind>(new KeyBind('V', KeyBindType.Toggle, true));
                _config.Item("Farm").SetValue<KeyBind>(new KeyBind('+', KeyBindType.Toggle, false));
                _config.Item("LastHit").SetValue<KeyBind>(new KeyBind('X', KeyBindType.Toggle, false));                
            }
            else if (owMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                _config.Item("Orbwalk").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Toggle, false));
                _config.Item("LaneClear").SetValue<KeyBind>(new KeyBind('V', KeyBindType.Toggle, false));
                _config.Item("Farm").SetValue<KeyBind>(new KeyBind('+', KeyBindType.Toggle, true));
                _config.Item("LastHit").SetValue<KeyBind>(new KeyBind('X', KeyBindType.Toggle, false));                
            }
            else if (owMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                _config.Item("Orbwalk").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Toggle, false));
                _config.Item("LaneClear").SetValue<KeyBind>(new KeyBind('V', KeyBindType.Toggle, false));
                _config.Item("Farm").SetValue<KeyBind>(new KeyBind('+', KeyBindType.Toggle, false));
                _config.Item("LastHit").SetValue<KeyBind>(new KeyBind('X', KeyBindType.Toggle, true));                
            }

/*                    if (_config.Item("Orbwalk").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Combo;
                    }

                    if (_config.Item("LaneClear").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.LaneClear;
                    }

                    if (_config.Item("Farm").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.Mixed;
                    }

                    if (_config.Item("LastHit").GetValue<KeyBind>().Active)
                    {
                        return OrbwalkingMode.LastHit;
                    }

                    return OrbwalkingMode.None;
            Item("Orbwalk").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Toggle, true));
*/

            
        }
    }
}
