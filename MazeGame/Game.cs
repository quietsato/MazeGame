using System;

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
            Func<int, int, bool> isPath = (x, y) => _Maze.Map[x, y] == MazeConstants.Path;
            switch (d)
            {
                case Direction.Up:
                    if (isPath(_Player[0], _Player[1] - 1))
                        _Player[1]--;
                    break;
                case Direction.Down:
                    if (isPath(_Player[0], _Player[1] + 1))
                        _Player[1]++;
                    break;
                case Direction.Left:
                    if (isPath(_Player[0] + 1, _Player[1]))
                        _Player[0]++;
                    break;
                case Direction.Right:
                    if (isPath(_Player[0] - 1, _Player[1]))
                        _Player[0]--;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("d", d, null);
            }
            UpdateDisplay();
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

            InitializeDisplay();
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
                Console.Write(Environment.NewLine);
            }
        }

        private void UpdateDisplay()
        {
            // プレイヤーの動いた周辺だけを書き換える
        }

        public void WaitInput()
        {
            // プレイヤーの入力を待つ
        }
    }

}