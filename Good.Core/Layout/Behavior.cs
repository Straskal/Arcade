﻿namespace Good.Core
{
    public abstract class Behavior
    {
        public virtual void Load(Sprite sprite) { }
        public virtual void Created(Sprite sprite) { }
        public virtual void Destroy(Sprite sprite) { }
        public virtual void Update(Sprite sprite) { }
    }
}
