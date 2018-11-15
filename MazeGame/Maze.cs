namespace MazeGame
{
    public class Maze
    {
        /// <summary>
        /// 保持している迷路の横幅
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// 保持している迷路の縦幅
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// 保持している迷路配列
        /// </summary>
        public char[,] Map { get; private set; }

        /// <summary>
        /// 迷路のスタート地点
        /// </summary>
        public int[] Start { get; private set; }

        /// <summary>
        /// 迷路のゴール地点
        /// </summary>
        public int[] Goal { get; private set; }

        /// <summary>
        /// 新しい迷路インスタンスを生成します
        /// </summary>
        /// <param name="maze">迷路を表す2次元配列</param>
        /// <param name="start">迷路のスタート地点</param>
        /// <param name="goal">迷路のゴール地点</param>
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