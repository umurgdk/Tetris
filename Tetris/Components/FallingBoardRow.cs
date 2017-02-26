namespace Tetris.Components
{
    public struct FallingBoardRow
    {
        public readonly int Index;
        public readonly int FallBy;

        public FallingBoardRow(int index, int fallBy)
        {
            Index = index;
            FallBy = fallBy;
        }
    }
}