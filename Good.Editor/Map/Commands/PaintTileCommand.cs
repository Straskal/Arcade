using Good.Editor.Command;
using System.Collections.Generic;

namespace Good.Editor.Map.Commands
{
    public class PaintTileCommand : ICommand
    {
        private struct PaintSingleTile 
        {
            public int X;
            public int Y;
            public int Index;
            public int PreviousIndex;
        }

        private readonly List<PaintSingleTile> eachTile = new List<PaintSingleTile>();
        private readonly int[,] data;

        private bool undone = false;

        public PaintTileCommand(int[,] data) 
        {
            this.data = data;
        }

        public void PaintTile(int x, int y, int index) 
        {
            eachTile.Add(new PaintSingleTile
            {
                X = x,
                Y = y,
                Index = index,
                PreviousIndex = data[y, x]
            });

            data[y, x] = index;
        }

        public void Do()
        {
            if (undone) 
            {
                foreach (var t in eachTile) 
                    data[t.Y, t.X] = t.Index;

                undone = false;
            }

        }

        public void Undo()
        {
            foreach (var t in eachTile)
                data[t.Y, t.X] = t.PreviousIndex;

            undone = true;
        }
    }
}
