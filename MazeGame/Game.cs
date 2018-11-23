using System;
using System.Threading;

namespace MazeGame
{
    public class Game
    {
        private Maze _maze;

        private DateTime StartTime { get; set; }

        private DateTime GoalTime { get; set; }

        private Location Player { get; set; }

        private Maze Maze
        {
            set
            {
                Player = value.Start.Copy();
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
                    if (isPath(Player.X, Player.Y - 1))
                    {
                        Player.Y--;
                        InitializeDisplay();
                    }

                    break;

                case Direction.Down:
                    if (isPath(Player.X, Player.Y + 1))
                    {
                        Player.Y++;
                        InitializeDisplay();
                    }

                    break;

                case Direction.Left:
                    if (isPath(Player.X - 1, Player.Y))
                    {
                        Player.X--;
                        InitializeDisplay();
                    }

                    break;

                case Direction.Right:
                    if (isPath(Player.X + 1, Player.Y))
                    {
                        Player.X++;
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

                StartTime = DateTime.Now;

                WaitInput();

                // ゲーム終了時のコンティニュー処理
                isContinue = CheckContinue();
            } while (isContinue);

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
            for (var y = 0; y < Maze.Height; y++)
            {
                for (var x = 0; x < Maze.Width; x++)
                {
                    if (x == Player.X && y == Player.Y)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(MazeConstants.Player);
                    }
                    else
                    {
                        switch (Maze.Map[x, y])
                        {
                            case MazeConstants.Route:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;

                            case MazeConstants.Start:
                            case MazeConstants.Goal:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;

                            default:
                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                break;
                        }
                        Console.Write(Maze.Map[x, y]);
                    }
                }

                if (y < Maze.Height - 1) Console.Write(Environment.NewLine);
            }
        }

        private void WaitInput()
        {
            Func<bool> isGoal = () => (Player.Equals(Maze.Goal));

            Console.CancelKeyPress += (sender, e) =>
            {
                FinishGame();
                Environment.Exit(0);
            };
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
                            Player = Maze.Start.Copy();

                        InitializeDisplay();
                        break;
                    case ConsoleKey.X:
                        if(Maze.Route == null)
                            Maze.Route = new RouteGenerator(Maze).FindRoute();
                        InitializeDisplay();
                        break;
                }
            }

            Goal();
        }

        private void Goal()
        {
            GoalTime = DateTime.Now;
            var clearTime = GoalTime - StartTime;

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
            Console.WriteLine("クリア時間は{0:0}分{1}秒でした", clearTime.TotalMinutes, clearTime.Seconds);
        }

        private static void FinishGame()
        {
            Console.Clear();
            Console.CursorVisible = true;
        }
    }
}