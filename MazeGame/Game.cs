using System;

namespace MazeGame
{
    public class Game
    {
        private Maze _maze;

        private int[] _player;

        private Maze Maze
        {
            set
            {
                _player = value.Start;
                _maze = value;
            }
            get { return _maze; }
        }

        private readonly MazeGenerator _generator;

        /// <summary>
        /// 新しいゲームのインスタンスを生成します
        /// </summary>
        /// <param name="generator">迷路の生成方法</param>
        public Game(MazeGenerator generator)
        {
            _generator = generator;
        }

        private void Move(Direction d)
        {
            Func<int, int, bool> isPath = (x, y) =>
            {
                if (x < 0 || x >= Maze.Width) return false;
                if (y < 0 || y >= _maze.Height) return false;
                return Maze.Map[x, y] != MazeConstants.Wall;
            };

            switch (d)
            {
                case Direction.Up:
                    if (isPath(_player[0], _player[1] - 1))
                    {
                        _player[1]--;
                        InitializeDisplay();
                    }

                    break;
                case Direction.Down:
                    if (isPath(_player[0], _player[1] + 1))
                    {
                        _player[1]++;
                        InitializeDisplay();
                    }

                    break;
                case Direction.Left:
                    if (isPath(_player[0] - 1, _player[1]))
                    {
                        _player[0]--;
                        InitializeDisplay();
                    }

                    break;
                case Direction.Right:
                    if (isPath(_player[0] + 1, _player[1]))
                    {
                        _player[0]++;
                        InitializeDisplay();
                    }

                    break;
            }
        }

        /// <summary>
        /// ゲームを開始します
        /// </summary>
        public void Start()
        {
            Func<string, int> inputSize = s =>
            {
                try
                {
                    int i = int.Parse(s);
                    return i;
                }
                catch (Exception)
                {
                    Console.WriteLine("正しい値を入力してください");
                    return int.MinValue;
                }
            };

            Console.WriteLine("迷路の種類を選択してください");
            Console.WriteLine("------------------------");
            Console.WriteLine("1: ウィンドウサイズに合わせる");
            Console.WriteLine("2: サイズを指定する");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadKey().Key;
                if (input == ConsoleKey.D1)
                {
                    Maze = _generator.GetResponsiveMaze();
                    break;
                }

                if (input == ConsoleKey.D2)
                {
                    int width, height;

                    Console.WriteLine("\n迷路のサイズを設定します");
                    Console.WriteLine("--------------------");
                    Console.WriteLine("迷路の横幅を指定してください");
                    do
                    {
                        width = inputSize(Console.ReadLine());
                    } while (width == int.MinValue);

                    Console.WriteLine("迷路の縦幅を設定してください");
                    do
                    {
                        height = inputSize(Console.ReadLine());
                    } while (height == int.MinValue);

                    Maze = _generator.GetFixedMaze(width, height);
                    break;
                }
            }

            Console.CursorVisible = false;

            InitializeDisplay();

            WaitInput();
        }

        private void InitializeDisplay()
        {
            // 全画面を書き換える = 画面を初期化する
            Console.Clear();
            for (var y = 0; y < Maze.Height; y++)
            {
                for (var x = 0; x < Maze.Width; x++)
                {
                    if (!(x == _player[0] && y == _player[1])) Console.Write(Maze.Map[x, y]);
                    else Console.Write(MazeConstants.Player);
                }

                if (y < Maze.Height - 1) Console.Write(Environment.NewLine);
            }
        }

        private void WaitInput()
        {
            // プレイヤーの入力を待つ
            while (true)
            {
                if (!Console.KeyAvailable) continue;
                var keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.W:
                        Move(Direction.Up);
                        break;
                    case ConsoleKey.A:
                        Move(Direction.Left);
                        break;
                    case ConsoleKey.S:
                        Move(Direction.Down);
                        break;
                    case ConsoleKey.D:
                        Move(Direction.Right);
                        break;
                }
            }
        }
    }
}