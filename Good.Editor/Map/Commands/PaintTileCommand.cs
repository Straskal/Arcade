using Good.Editor.Command;

namespace Good.Editor.Map.Commands
{
    public class PaintTileCommand : ICommand
    {
        private readonly int x;
        private readonly int y;
        private readonly int index;
        private readonly int previousIndex;
        private int[,] data;

        public PaintTileCommand(int x, int y, int index, int[,] data) 
        {
            this.x = x;
            this.y = y;
            this.index = index;
            this.data = data;
            previousIndex = data[y, x];
        }

        public void Do()
        {
            data[y, x] = index;
        }

        public void Undo()
        {
            data[y, x] = previousIndex;
        }
    }
}
