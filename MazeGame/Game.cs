using System;
using System.Threading;

namespace MazeGame
{
    public class Game
    {
        private Maze _maze;

        private readonly int[] _player = {0, 0};

        private Maze Maze
        {
            set
            {
                // 深いコピー
                _player[0] = value.Start[0];
                _player[1] = value.Start[1];

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
                if (y < 0 || y >= Maze.Height) return false;
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
            bool isContinue;
            Func<string, int> inputSize = s =>
            {
                try
                {
                    var i = int.Parse(s);
                    return i;
                }
                catch (Exception)
                {
                    Console.WriteLine("正しい値を入力してください");
                    return int.MinValue;
                }
            };

            do
            {
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("迷路の種類を選択してください");
                Console.WriteLine("------------------------");
                Console.WriteLine("1: ウィンドウサイズに合わせる");
                Console.WriteLine("2: サイズを指定する");
                while (true)
                {
                    Console.Write("[1/2]>");
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
                            Console.Write("[横幅]>");
                            width = inputSize(Console.ReadLine());
                            if (width > Console.LargestWindowWidth)
                            {
                                Console.WriteLine("横幅がウィンドウの最大サイズを超えていますがよろしいですか？");
                                Console.Write("[Y/N]>");
                                if (Console.ReadKey().Key != ConsoleKey.Y)
                                {
                                    width = int.MinValue;
                                    Console.Write(Environment.NewLine);
                                }
                            }
                        } while (width == int.MinValue);

                        Console.Write(Environment.NewLine);
                        Console.WriteLine("迷路の縦幅を設定してください");
                        do
                        {
                            Console.Write("[縦幅]>");
                            height = inputSize(Console.ReadLine());
                            if (height > Console.LargestWindowHeight)
                            {
                                Console.WriteLine("縦幅がウィンドウの最大サイズを超えていますがよろしいですか？");
                                Console.Write("[Y/N]>");
                                if (Console.ReadKey().Key != ConsoleKey.Y) 
                                {
                                    height = int.MinValue;
                                    Console.Write(Environment.NewLine);
                                }
                            }
                        } while (height == int.MinValue);

                        Maze = _generator.GetFixedMaze(width, height);
                        break;
                    }
                    Console.Write(Environment.NewLine);
                }

                Console.CursorVisible = false;

                InitializeDisplay();

                WaitInput();

                // ゲーム終了時のコンティニュー処理
                isContinue = CheckContinue();

            }while(isContinue);
            
            FinishGame();
        }

        private static bool CheckContinue()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("新しく迷路を生成してゲームを続けますか？");
            Console.Write("[Y/N]>");
            var input = Console.ReadKey().Key;
            Console.Clear();
            return input == ConsoleKey.Y;
        }

        private void InitializeDisplay()
        {
            // 全画面を書き換える
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            for (var y = 0; y < Maze.Height; y++)
            {
                for (var x = 0; x < Maze.Width; x++)
                {
                    if (!(x == _player[0] && y == _player[1]))
                    {
                        if (Maze.Map[x, y] != MazeConstants.Start && Maze.Map[x,y] != MazeConstants.Goal)
                        {
                            Console.Write(Maze.Map[x, y]);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(Maze.Map[x, y]);
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(MazeConstants.Player);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }

                    
                }

                if (y < Maze.Height - 1) Console.Write(Environment.NewLine);
            }
        }

        private void WaitInput()
        {
            Func<bool> isGoal = () => Maze.Map[_player[0], _player[1]] == MazeConstants.Goal;

            Console.CancelKeyPress += (sender, e) => FinishGame();
            // プレイヤーの入力を待つ
            while (!isGoal())
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
                    case ConsoleKey.R:
                        // スタートからやり直す
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("スタート位置からやり直しますか？ [Y]es/[N]o");
                        Console.Write(">");
                        var input = Console.ReadKey();
                        if (input.Key == ConsoleKey.Y)
                        {
                            _player[0] = Maze.Start[0];
                            _player[1] = Maze.Start[1];
                        }
 
                        InitializeDisplay();
                        break;
                }
            }

            Goal();
        }

        private static void Goal()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            for (var i = 0; i < 4; i++)
            {
                Console.Clear();
                Thread.Sleep(500);
                Console.WriteLine("   _____  ____          _      _ _ ");
                Console.WriteLine("  / ____|/ __ \\   /\\   | |    | | |");
                Console.WriteLine(" | |  __| |  | | /  \\  | |    | | |");
                Console.WriteLine(" | | |_ | |  | |/ /\\ \\ | |    | | |");
                Console.WriteLine(" | |__| | |__| / ____ \\| |____|_|_|");
                Console.WriteLine("  \\_____|\\____/_/    \\_\\______(_|_)");
                Thread.Sleep(500);
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void FinishGame()
        {
            Console.Clear();
            Console.CursorVisible = true;
        }
    }
}