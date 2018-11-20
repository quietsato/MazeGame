namespace MazeGame
{
    public static class MazeConstants
    {
        public const char Wall = '+';
        public const char Path = ' ';
        public const char Start = 'S';
        public const char Goal = 'G';
        public const char Player = '@';
    }

    public enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    public enum Orientation
    {
        Vertical = 0,
        Horizontal = 1
    }
}