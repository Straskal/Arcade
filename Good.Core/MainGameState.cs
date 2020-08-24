using Microsoft.Xna.Framework;

namespace Good.Core
{
    public abstract class MainGameState
    {
        public virtual bool IsTranscendent => false;
        public virtual bool IsTransparent => false;
        public virtual Matrix? Transformation => null;

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void FrameStart() { }
        public virtual void FrameEnd() { }
        public virtual void Update() { }
        public virtual void Draw() { }
    }
}
