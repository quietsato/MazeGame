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
        public Location Start { get; private set; }

        /// <summary>
        /// 迷路のゴール地点
        /// </summary>
        public Location Goal { get; private set; }

        public Location[] Route
        {
            get { return _route; }
            set
            {
                if (value != null)
                {
                    foreach (var r in value)
                    {
                        if (!r.Equals(Start) && !r.Equals(Goal))
                        {
                            Map[r.X, r.Y] = MazeConstants.Route;
                        }
                    }
                }

                _route = value;
            }
        }

        private Location[] _route;

        /// <summary>
        /// 新しい迷路インスタンスを生成します
        /// </summary>
        /// <param name="maze">迷路を表す2次元配列</param>
        /// <param name="start">迷路のスタート地点</param>
        /// <param name="goal">迷路のゴール地点</param>
        public Maze(char[,] maze, Location start, Location goal)
        {
            Map = maze;
            Start = start.Copy();
            Goal = goal.Copy();
            Width = maze.GetLength(0);
            Height = maze.GetLength(1);
            Route = null;
        }
    }
}