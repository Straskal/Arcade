namespace Good.Core
{
    public abstract class GameState
    {
        public virtual bool UpdateBelow => false;
        public virtual bool DrawBelow => false;

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void FrameStart() { }
        public virtual void FrameEnd() { }
        public virtual void Update() { }
        public virtual void Draw() { }
    }
}
