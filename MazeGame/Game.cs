
using System;

namespace MazeGame
{
    public class Game
    {
        public int[] Player { get; private set; }
        public Maze Map { get; private set; }

        public void Move(Direction d)
        {
            Func<int, int, bool> isPath = (x, y) => Map.Map[x, y] == MazeConstants.Path;
            switch (d)
            {
                case Direction.Up:
                    if (isPath(Player[0], Player[1] - 1))
                        Player[1]--;
                    break;
                case Direction.Down:
                    if (isPath(Player[0], Player[1] + 1))
                        Player[1]++;
                    break;
                case Direction.Left:
                    if (isPath(Player[0] + 1, Player[1]))
                        Player[0]++;
                    break;
                case Direction.Right:
                    if (isPath(Player[0] - 1, Player[1]))
                        Player[0]--;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("d", d, null);
            }
            UpdateDisplay();
        }

        public void GenerateMaze(MazeGenerator generator)
        {
            
        }

        public void Start()
        {
            
        }

        public void End()
        {
            
        }

        private void InitializeDisplay()
        {
            // 全画面を書き換える = 画面を初期化する
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