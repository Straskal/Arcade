namespace Good.Core
{
    public abstract class Behavior
    {
        public virtual void Load(Sprite sprite) { }
        public virtual void Loaded(Sprite sprite) { }
        public virtual void Update(Sprite sprite) { }
    }
}
