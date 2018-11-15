namespace MazeGame
{
    public class Maze
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public char[,] Map { get; private set; }
        public int[] Start { get; private set; }
        public int[] Goal { get; private set; }

        public Maze(char[,] maze, int[] start, int[] goal)
        {
            Map = maze;
            Start = start;
            Goal = goal;
            Width = maze.GetLength(0);
            Height = maze.GetLength(1);
        }
    }
}