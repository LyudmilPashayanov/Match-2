using System;

namespace MergeTwo
{
    [Serializable]
    public struct Pos 
    {
        public int x;
        public int y;

        public Pos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"_{x}_{y}";
        }
    }
}