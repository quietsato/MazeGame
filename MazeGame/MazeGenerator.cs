using System;
using System.Collections.Generic;

namespace MazeGame
{
    public abstract class MazeGenerator
    {
        /// <summary>
        /// 指定されたサイズの幅の迷路を返します
        /// 引数が適当な数字でなければ、それに最も近い大きさの迷路のうち小さいほうを返します
        /// </summary>
        /// <param name="width">迷路の幅</param>
        /// <param name="height">迷路の高さ</param>
        /// <returns>生成された迷路</returns>
        public abstract Maze GetFixedMaze(int width, int height);

        /// <summary>
        /// ウィンドウサイズに一番近い適当な迷路を返します
        /// </summary>
        /// <returns>生成された迷路</returns>
        public abstract Maze GetResponsiveMaze();
    }

    public class DigMazeGenerator : MazeGenerator
    {
        private const int MinimumSizeOfMaze = 5;

        private char[,] _maze;
        private List<int[]> _startCells;
        private Orientation _mazeOrientation;
        private int _width;
        private int _height;

        private int Width
        {
            set
            {
                _width = value;
                if (_width < MinimumSizeOfMaze) _width = MinimumSizeOfMaze;
                if (_width % 2 == 0) _width--;
            }
            get { return _width; }
        }

        private int Height
        {
            set
            {
                _height = value;
                if (_height < MinimumSizeOfMaze) _height = MinimumSizeOfMaze;
                if (_height % 2 == 0) _height--;
            }
            get { return _height; }
        }

        public override Maze GetFixedMaze(int width, int height)
        {
            // 入力値をwidth, heightに代入
            Width = width;
            Height = height;

            _maze = new char[Width, Height];
            _startCells = new List<int[]>();

            _mazeOrientation = new Random().Next(2) == 0 ? Orientation.Vertical : Orientation.Horizontal;

            var mMaze = CreateMaze();

            var mStart = GetMazeStart();
            var mGoal = GetMazeGoal();

            mMaze[mStart[0], mStart[1]] = MazeConstants.Start;
            mMaze[mGoal[0], mGoal[1]] = MazeConstants.Goal;

            return new Maze(mMaze, mStart, mGoal);
        }

        public override Maze GetResponsiveMaze()
        {
            int x = Console.WindowWidth;
            int y = Console.WindowHeight;
            return GetFixedMaze(x, y);
        }

        private char[,] CreateMaze()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // 外周部を通路にする
                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                    {
                        _maze[x, y] = MazeConstants.Path;
                    }
                    else
                    {
                        _maze[x, y] = MazeConstants.Wall;
                    }
                }
            }

            Dig(1, 1);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // 外周部を壁に戻す(スタート地点とゴール地点は開ける)
                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                    {
                        _maze[x, y] = MazeConstants.Wall;
                    }
                }
            }

            return _maze;
        }

        private void Dig(int x, int y)
        {
            while (true)
            {
                var r = new Random();
                while (true)
                {
                    // 掘ることのできる方向のリストを作成
                    var directions = new List<Direction>();
                    try
                    {
                        if (_maze[x, y - 1] == MazeConstants.Wall && _maze[x, y - 2] == MazeConstants.Wall)
                            directions.Add(Direction.Up);
                        if (_maze[x, y + 1] == MazeConstants.Wall && _maze[x, y + 2] == MazeConstants.Wall)
                            directions.Add(Direction.Down);
                        if (_maze[x - 1, y] == MazeConstants.Wall && _maze[x - 2, y] == MazeConstants.Wall)
                            directions.Add(Direction.Left);
                        if (_maze[x + 1, y] == MazeConstants.Wall && _maze[x + 2, y] == MazeConstants.Wall)
                            directions.Add(Direction.Right);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        // Do Nothing
                    }

                    // 掘ることのできる方向がなければループを抜ける
                    if (directions.Count == 0) break;

                    // 指定座標を通路とする
                    SetPath(x, y);

                    // ランダムに方向を決めて掘る
                    var directionIndex = r.Next(directions.Count);
                    switch (directions[directionIndex])
                    {
                        case Direction.Up:
                            SetPath(x, --y);
                            SetPath(x, --y);
                            break;
                        case Direction.Down:
                            SetPath(x, ++y);
                            SetPath(x, ++y);
                            break;
                        case Direction.Left:
                            SetPath(--x, y);
                            SetPath(--x, y);
                            break;
                        case Direction.Right:
                            SetPath(++x, y);
                            SetPath(++x, y);
                            break;
                    }
                }

                // 掘り進められなくなったとき
                // 開始セルを取得
                var cell = GetStartCell();
                // 候補がなくなったとき穴掘り終了
                if (cell[0] == -1 && cell[1] == -1) break;

                x = cell[0];
                y = cell[1];
            }
        }

        private void SetPath(int x, int y)
        {
            _maze[x, y] = MazeConstants.Path;
            if (x % 2 == 1 && y % 2 == 1)
            {
                _startCells.Add(new[] {x, y});
            }
        }

        private int[] GetStartCell()
        {
            if (_startCells.Count == 0) return new[] {-1, -1};

            // ランダムに取得
            var r = new Random();
            var index = r.Next(_startCells.Count);
            var cell = _startCells[index];
            _startCells.RemoveAt(index);

            return cell;
        }

        private int[] GetMazeStart()
        {
            int[] mStart = {0, 0};
            int randomResult;

            switch (_mazeOrientation)
            {
                case Orientation.Vertical:
                    do
                    {
                        randomResult = new Random((int) DateTime.Now.Ticks).Next(_maze.GetLength(0));
                        mStart[0] = randomResult;
                    } while (_maze[mStart[0], mStart[1] + 1] == MazeConstants.Wall);

                    break;
                case Orientation.Horizontal:
                    do
                    {
                        randomResult = new Random((int) DateTime.Now.Ticks).Next(_maze.GetLength(1));
                        mStart[1] = randomResult;
                    } while (_maze[mStart[0] + 1, mStart[1]] == MazeConstants.Wall);

                    break;
            }

            return mStart;
        }

        private int[] GetMazeGoal()
        {
            int[] mGoal = {_maze.GetLength(0) - 1, _maze.GetLength(1) - 1};
            int randomResult;

            switch (_mazeOrientation)
            {
                case Orientation.Vertical:
                    do
                    {
                        randomResult = new Random((int) DateTime.Now.Ticks).Next(_maze.GetLength(0));
                        mGoal[0] = randomResult;
                    } while (_maze[mGoal[0], mGoal[1] - 1] == MazeConstants.Wall);

                    break;
                case Orientation.Horizontal:
                    do
                    {
                        randomResult = new Random((int) DateTime.Now.Ticks).Next(_maze.GetLength(1));
                        mGoal[1] = randomResult;
                    } while (_maze[mGoal[0] - 1, mGoal[1]] == MazeConstants.Wall);

                    break;
            }

            return mGoal;
        }
    }
}