namespace Good.Editor.Command
{
    public interface ICommand
    {
        void Do();
        void Undo();
    }
}
