namespace MazeGame
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game(new DigMazeGenerator());
            game.Start();
        }
    }
}
