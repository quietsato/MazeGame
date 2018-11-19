using System;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace MazeGame
{
    public class Game
    {
        private Maze maze;

        private int[] _Player;

        private Maze _Maze
        {
            set
            {
                _Player = value.Start;
                maze = value;
            }
            get { return maze; }
        }

        private MazeGenerator generator;

        /// <summary>
        /// 新しいゲームのインスタンスを生成します
        /// </summary>
        /// <param name="generator">迷路の生成方法</param>
        public Game(MazeGenerator generator)
        {
            this.generator = generator;
        }

        public void Move(Direction d)
        {
            Func<int, int, bool> isPath = (x, y) =>
            {
                if (x < 0 || x >= _Maze.Width) return false;
                if (y < 0 || y >= maze.Height) return false;
                return _Maze.Map[x, y] != MazeConstants.Wall;
            };

            switch (d)
            {
                case Direction.Up:
                    if (isPath(_Player[0], _Player[1] - 1))
                    {
                        _Player[1]--;
                        InitializeDisplay();
                    }

                    break;
                case Direction.Down:
                    if (isPath(_Player[0], _Player[1] + 1))
                    {
                        _Player[1]++;
                        InitializeDisplay();
                    }

                    break;
                case Direction.Left:
                    if (isPath(_Player[0] - 1, _Player[1]))
                    {
                        _Player[0]--;
                        InitializeDisplay();
                    }

                    break;
                case Direction.Right:
                    if (isPath(_Player[0] + 1, _Player[1]))
                    {
                        _Player[0]++;
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
                ConsoleKey input = Console.ReadKey().Key;
                if (input == ConsoleKey.D1)
                {
                    _Maze = generator.GetResponsiveMaze();
                    break;
                }
                else if (input == ConsoleKey.D2)
                {
                    int width, height = 0;

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

                    _Maze = generator.GetFixedMaze(width, height);
                    break;
                }
            }

            Console.CursorVisible = false;

            InitializeDisplay();

            WaitInput();

            while (true)
            {
            }
        }

        public void End()
        {
        }

        private void InitializeDisplay()
        {
            // 全画面を書き換える = 画面を初期化する
            Console.Clear();
            for (int y = 0; y < _Maze.Height; y++)
            {
                for (int x = 0; x < _Maze.Width; x++)
                {
                    if (!(x == _Player[0] && y == _Player[1])) Console.Write(_Maze.Map[x, y]);
                    else Console.Write(MazeConstants.Player);
                }

                if (y < _Maze.Height - 1) Console.Write(Environment.NewLine);
            }
        }

        public void WaitInput()
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