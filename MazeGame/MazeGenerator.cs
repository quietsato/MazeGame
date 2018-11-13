using System;
using System.Collections.Generic;

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
    private static string[,] maze;
    private static int width = 5;
    private static int height = 5;
    private static List<int[,]> startCells;
    private static Orientation MazeOrientation;
    private static Random _random = new Random();
    const string Wall = "#";
    const string Path = " ";

    override Maze GetFixedMaze(int width, int height)
    {
        // 入力値をwidth, heightに代入
        width = x;
        height = y;

        // 規格外かどうかを調べる
        if (width <= 6) width = 5;
        if (height <= 6) height = 5;
        if (width % 2 == 0) width = x - 1;
        if (height % 2 == 0) height = y - 1;

        maze = new string[width, height];
        startCells = new List<Location>();

        MazeOrientation = _random.Next(2) == 0 ? Orientation.Vertical : Orientation.Horizontal;

        string[,] mMaze = CreateMaze();
        Location mStart = GetMazeStart();
        Location mGoal = GetMazeGoal();

        mMaze[mStart.X, mStart.Y] = "S";
        mMaze[mGoal.X, mGoal.Y] = "G";

        return new Maze(mMaze, mStart, mGoal);
    }
    public override Maze GetResponsiveMaze()
    {
        int x = Console.WindowWidth;
        int y = Console.WindowHeight;
        return GetFixedMaze(x, y);
    }

    private static string[,] CreateMaze()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 外周部を通路にする
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    maze[x, y] = Path;
                }
                else
                {
                    maze[x, y] = Wall;
                }
            }
        }

        Dig(1, 1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 外周部を壁に戻す(スタート地点とゴール地点は開ける)
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
                {
                    maze[x, y] = Wall;
                }
            }
        }
        return maze;
    }

    private static void Dig(int x, int y)
    {
        var r = new Random();
        while (true)
        {
            // 掘ることのできる方向のリストを作成
            var directions = new List<Direction>();
            if (maze[x, y - 1] == Wall && maze[x, y - 2] == Wall)
                directions.Add(Direction.Up);
            if (maze[x, y + 1] == Wall && maze[x, y + 2] == Wall)
                directions.Add(Direction.Down);
            if (maze[x - 1, y] == Wall && maze[x - 2, y] == Wall)
                directions.Add(Direction.Left);
            if (maze[x + 1, y] == Wall && maze[x + 2, y] == Wall)
                directions.Add(Direction.Right);

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
        if (cell != null)
        {
            Dig(cell.X, cell.Y);
        }
    }

    private static void SetPath(int x, int y)
    {
        maze[x, y] = Path;
        if (x % 2 == 1 && y % 2 == 1)
        {
            startCells.Add(new Location(x, y));
        }
    }

    private static Location GetStartCell()
    {
        if (startCells.Count == 0) return null;

        // ランダムに取得
        var r = new Random();
        var index = r.Next(startCells.Count);
        var cell = startCells[index];
        startCells.RemoveAt(index);

        return cell;
    }

    private static Location GetMazeStart()
    {
        Location mStart = new Location(0, 0);
        int randomResult = 0;

        switch (MazeOrientation)
        {
            case Orientation.Vertical:
                do
                {
                    randomResult = _random.Next(maze.GetLength(0));
                    mStart.X = randomResult;
                } while (maze[mStart.X, mStart.Y + 1] == Wall);
                break;
            case Orientation.Horizontal:
                do
                {
                    randomResult = _random.Next(maze.GetLength(1));
                    mStart.Y = randomResult;
                } while (maze[mStart.X + 1, mStart.Y] == Wall);
                break;
        }

        return mStart;
    }

    private static Location GetMazeGoal()
    {
        Location mGoal = new Location(maze.GetLength(0) - 1, maze.GetLength(1) - 1);
        int randomResult = 0;

        switch (MazeOrientation)
        {
            case Orientation.Vertical:
                do
                {
                    randomResult = _random.Next(maze.GetLength(0));
                    mGoal.X = randomResult;
                } while (maze[mGoal.X, mGoal.Y - 1] == Wall);
                break;
            case Orientation.Horizontal:
                do
                {
                    randomResult = _random.Next(maze.GetLength(1));
                    mGoal.Y = randomResult;
                } while (maze[mGoal.X - 1, mGoal.Y] == Wall);
                break;
        }
        return mGoal;
    }

    private enum Orientation
    {
        Vertical = 0,
        Horizontal = 1
    }
}
