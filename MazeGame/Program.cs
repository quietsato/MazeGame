using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // 迷路生成オプションを受け取る

            // 迷路が固定サイズか可変サイズかによって使用する迷路生成メソッドを分ける

            // ゲーム開始準備

            // For Debugging
            var generator = new DigMazeGenerator();
            generator.GetFixedMaze(10, 9);
            generator.GetResponsiveMaze();
        }
    }
}
