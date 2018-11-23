using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeGame
{
    public class RouteGenerator
    {
        private Maze Maze { get; set; }
        
        private int[] VisitedCell { get; set; }
        
        private Location[] Route { get; set; }

        public RouteGenerator(Maze maze)
        {
            Maze = maze;
            VisitedCell = new int[maze.Width * maze.Height];
            VisitedCell = Enumerable.Repeat(-1, VisitedCell.Length).ToArray();
        }

        public Location[] FindRoute()
        {
            var nextCell = new Queue<Location>();
            nextCell.Enqueue(new Location(Maze.Start[0], Maze.Start[1]));

            while (nextCell.Count > 0)
            {
                var target = nextCell.Dequeue();
            
                foreach (Direction d in Enum.GetValues(typeof(Direction)))
                {
                    var next = target.Copy();
                    switch (d)
                    {
                        case Direction.Up:
                            next.Y--;
                            break;
                        case Direction.Down:
                            next.Y++;
                            break;
                        case Direction.Left:
                            next.X--;
                            break;
                        case Direction.Right:
                            next.X++;
                            break;
                    }
                    if(next.X < 0 || next.Y < 0 || next.X >= Maze.Width || next.Y >= Maze.Height)    
                        continue;
                    if (Maze.Map[next.X, next.Y] != MazeConstants.Path)
                    {
                        if (Maze.Map[next.X, next.Y] == MazeConstants.Goal)
                        {
                            SetVisited(target, next);
                            nextCell.Clear();
                            SetRoute();
                            break;
                        }
                    }
                    else if(VisitedCell[ToIndex(next)] < 0)
                    {
                        SetVisited(target, next);
                        nextCell.Enqueue(next.Copy());
                        Console.WriteLine("{0},{1}",next.X, next.Y);
                    }
                }
            }
            
            return Route;
        }

        private void SetVisited(Location target, Location next)
        {
            VisitedCell[ToIndex(next)] = ToIndex(target);
        }

        private void SetRoute()
        {
            var routeList = new List<Location>();
            var now = ToIndex(new Location(Maze.Goal[0], Maze.Goal[1]));
            routeList.Add(ToCell(now));
            while (VisitedCell[now] > 0)
            {
                var prev = VisitedCell[now];
                routeList.Add(ToCell(prev));
                now = prev;
            }

            routeList.Reverse();
            
            Route = routeList.ToArray();
        }

        private int ToIndex(Location cell)
        {
            return cell.X + (cell.Y * Maze.Width);
        }

        private Location ToCell(int index)
        {
            var x = index % Maze.Width;
            var y = index / Maze.Width;
            return new Location(x, y);
        }
    }
}