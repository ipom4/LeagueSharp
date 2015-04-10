using System;
using System.Linq;
using SharpDX;
using LeagueSharp;
using LeagueSharp.Common;

namespace Prince_Urgot
{
    internal class OrbDancer : Orbwalking.Orbwalker
    {
        //private const float LaneClearWaitTimeMod = 2f;
        private static Menu _config;
        private static float lastMoveOrder;
        private static int _delay;
        public static int LastMoveCommandT;
        public static Vector3 LastMoveCommandPosition = Vector3.Zero;
        private static Obj_AI_Hero Player;
        //private Obj_AI_Base _forcedTarget;
        //private Orbwalking.OrbwalkingMode _mode = Orbwalking.OrbwalkingMode.None;
        private Vector3 _orbwalkingPoint;
        //private Obj_AI_Minion _prevMinion;
        private static float _minDistance = 400;
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);


        public OrbDancer(Menu attachToMenu): base(attachToMenu)
        {
            _config = attachToMenu;
            _delay = 20;
            Player = ObjectManager.Player;
            Game.OnUpdate += GameOnOnGameUpdate;
        }
        
        private void GameOnOnGameUpdate(EventArgs args)
        {
            try
            {
                if (ActiveMode == Orbwalking.OrbwalkingMode.None)
                {
                    this.MoveTo((_orbwalkingPoint.To2D().IsValid()) ? _orbwalkingPoint : Game.CursorPos,
                        _config.Item("HoldPosRadius").GetValue<Slider>().Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            /*
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
            }*/
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
        }
        
        /// <summary>
        ///     Forces the orbwalker to move to that point while orbwalking (Game.CursorPos by default).
        /// </summary>
        public void SetOrbwalkingPoint(Vector3 point) 
        {
            base.SetOrbwalkingPoint(point);
            _orbwalkingPoint = point;
        }
        
        private static void MoveTo(Vector3 position,
            float holdAreaRadius = 0,
            bool overrideTimer = false,
            bool useFixedDistance = true,
            bool randomizeMinDistance = true)
        {
            if (Utils.TickCount - LastMoveCommandT < _delay && !overrideTimer)
            {
                return;
            }

            LastMoveCommandT = Utils.TickCount;

            if (Player.ServerPosition.Distance(position, true) < holdAreaRadius * holdAreaRadius)
            {
                if (Player.Path.Count() > 1)
                {
                    Player.IssueOrder((GameObjectOrder)10, Player.ServerPosition);
                    Player.IssueOrder(GameObjectOrder.HoldPosition, Player.ServerPosition);
                    LastMoveCommandPosition = Player.ServerPosition;
                }
                return;
            }

            var point = position;
            if (useFixedDistance)
            {
                point = Player.ServerPosition +
                        (randomizeMinDistance ? (_random.NextFloat(0.6f, 1) + 0.2f) * _minDistance : _minDistance) *
                        (position.To2D() - Player.ServerPosition.To2D()).Normalized().To3D();
            }
            else
            {
                if (randomizeMinDistance)
                {
                    point = Player.ServerPosition +
                            (_random.NextFloat(0.6f, 1) + 0.2f) * _minDistance *
                            (position.To2D() - Player.ServerPosition.To2D()).Normalized().To3D();
                }
                else if (Player.ServerPosition.Distance(position) > _minDistance)
                {
                    point = Player.ServerPosition +
                            _minDistance * (position.To2D() - Player.ServerPosition.To2D()).Normalized().To3D();
                }
            }

            Player.IssueOrder(GameObjectOrder.MoveTo, point);
            LastMoveCommandPosition = point;
        }
    }
}
