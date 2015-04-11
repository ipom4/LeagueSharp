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
       
        
        public enum MoveModeType
        {
            WalkAround,
            BaseSideTurnAround,
            Flee,
            None
        }        
        
        private GameObject _moveModeTarget;
        private float _moveModeRadius = 0;
        private MoveModeType _moveMode;
        private float _lastMoveModeTick;
        private float _moveModeLapse = 200;
        private float _moveModeLapseLim = 200;
        
        public OrbDancer(Menu attachToMenu): base(attachToMenu)
        {
            _config = attachToMenu;
            _delay = 20;
            Player = ObjectManager.Player;
            _moveMode = MoveModeType.None;
            Game.OnUpdate += GameOnOnGameUpdate;
        }
        
        public Obj_AI_Turret ClosestAlliedTurret()
        {
            return ObjectManager.Get<Obj_AI_Turret>().OrderBy(t => t.Distance(Player, true)).First(t => t.IsAlly);
        }
        
        public Obj_AI_Turret ClosestEnemyTurret()
        {
            return ObjectManager.Get<Obj_AI_Turret>().OrderBy(t => t.Distance(Player, true)).First(t => t.IsEnemy);
        }
        
        public Obj_AI_Turret ClosestAliveEnemyTurret()
        {
            return ObjectManager.Get<Obj_AI_Turret>().OrderBy(t => t.Distance(Player, true)).First(t => (!t.IsDead) && t.IsEnemy);
        }        
        
        private void GameOnOnGameUpdate(EventArgs args)
        {
            try
            {
                if (ActiveMode == Orbwalking.OrbwalkingMode.None)
                {
                    OrbDancer.MoveTo((_orbwalkingPoint.To2D().IsValid()) ? _orbwalkingPoint : Game.CursorPos,
                        _config.Item("HoldPosRadius").GetValue<Slider>().Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            if (_moveMode == MoveModeType.BaseSideTurnAround)
            {
                
            }
        }
        
        public void setMode(Orbwalking.OrbwalkingMode owMode)
        {
            if (owMode == Orbwalking.OrbwalkingMode.None)
            {
                _config.Item("Orbwalk").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Press, false));
                _config.Item("LaneClear").SetValue<KeyBind>(new KeyBind('V', KeyBindType.Press, false));
                _config.Item("Farm").SetValue<KeyBind>(new KeyBind('+', KeyBindType.Press, false));
                _config.Item("LastHit").SetValue<KeyBind>(new KeyBind('X', KeyBindType.Press, false));
            }
            else if (owMode == Orbwalking.OrbwalkingMode.Combo)
            {
                _config.Item("Orbwalk").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Press, true));
                _config.Item("LaneClear").SetValue<KeyBind>(new KeyBind('V', KeyBindType.Press, false));
                _config.Item("Farm").SetValue<KeyBind>(new KeyBind('+', KeyBindType.Press, false));
                _config.Item("LastHit").SetValue<KeyBind>(new KeyBind('X', KeyBindType.Press, false));                
            }
            else if (owMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                _config.Item("Orbwalk").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Press, false));
                _config.Item("LaneClear").SetValue<KeyBind>(new KeyBind('V', KeyBindType.Press, true));
                _config.Item("Farm").SetValue<KeyBind>(new KeyBind('+', KeyBindType.Press, false));
                _config.Item("LastHit").SetValue<KeyBind>(new KeyBind('X', KeyBindType.Press, false));                
            }
            else if (owMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                _config.Item("Orbwalk").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Press, false));
                _config.Item("LaneClear").SetValue<KeyBind>(new KeyBind('V', KeyBindType.Press, false));
                _config.Item("Farm").SetValue<KeyBind>(new KeyBind('+', KeyBindType.Press, true));
                _config.Item("LastHit").SetValue<KeyBind>(new KeyBind('X', KeyBindType.Press, false));                
            }
            else if (owMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                _config.Item("Orbwalk").SetValue<KeyBind>(new KeyBind(32, KeyBindType.Press, false));
                _config.Item("LaneClear").SetValue<KeyBind>(new KeyBind('V', KeyBindType.Press, false));
                _config.Item("Farm").SetValue<KeyBind>(new KeyBind('+', KeyBindType.Press, false));
                _config.Item("LastHit").SetValue<KeyBind>(new KeyBind('X', KeyBindType.Press, true));                
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
        
        public void setMoveMode(MoveModeType movemode, GameObject target, float radius)
        {
            _moveModeTarget = target;
            _moveMode = movemode;
            _moveModeRadius = radius;
        }

        private void WalkAround(GameObject target, float radius)
        {
            if((Game.Time-_lastMoveModeTick) > _moveModeLapse)
            {
                float x = _random.NextFloat(-radius, radius);
                float limz = Math.Sqrt(radius * radius - x * x);
                
                float z = _random.NextFloat(-limz, limz);
    
                SetOrbwalkingPoint(new Vector3(x, 0, z)+target.Position);
                
                _lastMoveModeTick = Game.Time;
                //_moveModeLapse = _random.NextFloat(0, _moveModeLapseLim);
            }
        }
        
        private void BaseSideTurnAround(GameObject target, float radius)
        {
            
        }
    }
}
