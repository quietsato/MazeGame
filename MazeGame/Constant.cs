namespace MazeGame
{
    public static class MazeConstants
    {
        public const char Wall = '+';
        public const char Path = ' ';
        public const char Start = 'S';
        public const char Goal = 'G';
        public const char Player = '@';
        public const char Route = '･';
    }

    public class Location
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Location Copy()
        {
            return new Location(X, Y);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Location))
                return false;

            var o = (Location) obj;
            return o.X == X && o.Y == Y;
        }

        public override int GetHashCode()
        {
            return X + Y;
        }
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