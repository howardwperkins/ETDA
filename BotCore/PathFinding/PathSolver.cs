using BotCore.Types;
using System;
using System.Collections.Generic;

namespace BotCore.PathFinding
{
    public class PathSolver
    {
        public class PathNode
        {
            public DateTime LastAccessed { get; set; }
            public int Steps { get; set; }
            public bool HasReactor { get; set; }
            public bool IsDoor { get; set; }
            public bool IsBlock { get; set; }
        }

        public static List<PathFinderNode> FindPath(ref PathNode[,] matrix, Position start, Position end)
        {
            if (start == null || end == null)
                return null;
        
            int width = matrix.GetLength(0);
            int height = matrix.GetLength(1);
        
            bool InBounds(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;
        
            try
            {
                if (!InBounds(start.X, start.Y) || !InBounds(end.X, end.Y))
                    return null;
        
                matrix[start.X, start.Y].IsBlock = false;
                matrix[end.X, end.Y].IsBlock = false;
        
                var closedNodes = new bool[width, height];
                var stack = new List<PathFinderNode> { new PathFinderNode { X = start.X, Y = start.Y, Heuristic = 0 } };
                int heuristic = 0;
                PathFinderNode finalNode = null;
        
                // Directions: left, right, up, down
                int[] dx = { -1, 1, 0, 0 };
                int[] dy = { 0, 0, -1, 1 };
        
                while (finalNode == null && stack.Count > 0)
                {
                    var newStack = new List<PathFinderNode>();
                    foreach (var node in stack)
                    {
                        if (node.Heuristic > heuristic)
                        {
                            newStack.Add(node);
                            continue;
                        }
        
                        for (int dir = 0; dir < 4; dir++)
                        {
                            int nx = node.X + dx[dir];
                            int ny = node.Y + dy[dir];
        
                            if (!InBounds(nx, ny) || closedNodes[nx, ny] || matrix[nx, ny].IsBlock)
                                continue;
        
                            var lastNode = new PathFinderNode
                            {
                                LastNode = node.LastNode,
                                X = node.X,
                                Y = node.Y,
                                Heuristic = node.Heuristic
                            };
        
                            var newNode = new PathFinderNode
                            {
                                X = nx,
                                Y = ny,
                                NextNode = null,
                                Heuristic = lastNode.Heuristic + 1 // or adjust heuristic as needed
                            };
                            lastNode.NextNode = newNode;
                            newNode.LastNode = lastNode;
        
                            if (nx == end.X && ny == end.Y)
                            {
                                finalNode = newNode;
                                break;
                            }
                            closedNodes[nx, ny] = true;
                            newStack.Add(newNode);
                        }
                        if (finalNode != null)
                            break;
                    }
                    heuristic++;
                    stack = newStack;
                }
        
                if (finalNode != null)
                {
                    var path = new List<PathFinderNode>();
                    while (finalNode != null)
                    {
                        path.Add(finalNode);
                        finalNode = finalNode.LastNode;
                    }
                    path.Reverse();
                    return path;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                return null;
            }
        }
        public static List<PathFinderNode> _FindPath(ref PathNode[,] matrix, Position start, Position end)
        {

            if (start == null || end == null)
                return null;

            var width = matrix.GetLength(0);
            var height = matrix.GetLength(1);

            bool InBounds(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;
            
            try
            {
                if (!InBounds(start.X, start.Y))
                {
                    Console.WriteLine("Start position [" + start.X + "," + start.Y + "] is out of bounds of [" + width + "," + height + "]");
                    return null;
                }
                
                if (!InBounds(end.X, end.Y))
                {
                    Console.WriteLine("End position [" + start.X + "," + start.Y + "] is out of bounds of [" + width + "," + height + "]");
                    return null;
                }
                
                matrix[start.X, start.Y].IsBlock = false;
                matrix[end.X, end.Y].IsBlock = false;

                var closedNodes = new bool[matrix.GetUpperBound(0) + 1, matrix.GetUpperBound(1) + 1];
                var stack = new List<PathFinderNode>(new[] { new PathFinderNode { X = start.X, Y = start.Y, Heuristic = 0 } });
                var heuristic = 0;

                PathFinderNode finalNode = null;

                // Loop until we find the end node or exhaust all possibilities.
                while ((finalNode == null) && stack.Count > 0)
                {
                    var newStack = new List<PathFinderNode>();
                    for (var i = 0; i < stack.Count; i++)
                    {
                        if (stack[i].Heuristic > heuristic)
                        {
                            newStack.Add(stack[i]);
                            continue;
                        }

                        int x = stack[i].X;
                        int xToRight = x + 1;
                        int xToLeft = x - 1;
                        int y = stack[i].Y;
                        int yToUp = y - 1;
                        int yToDown = y + 1;
                        
                        if (xToLeft <= matrix.GetUpperBound(0))
                            if (xToLeft >= 0)
                                if (!closedNodes[xToLeft, y])
                                    if (!matrix[xToLeft, y].IsBlock)
                                    {
                                        var lastNode = new PathFinderNode
                                        {
                                            LastNode = stack[i].LastNode,
                                            X = x,
                                            Y = y,
                                            Heuristic = stack[i].Heuristic
                                        };
                                        var newNode = new PathFinderNode
                                        {
                                            X = xToLeft,
                                            Y = y,
                                            NextNode = null,
                                            Heuristic = lastNode.Heuristic + (byte)(matrix[x, yToDown].IsBlock ? 1 : 0)
                                        };
                                        lastNode.NextNode = newNode;
                                        newNode.LastNode = lastNode;
                                        if (xToLeft == end.X && y == end.Y)
                                        {
                                            finalNode = newNode;
                                            break;
                                        }
                                        closedNodes[xToLeft, y] = true;
                                        newStack.Add(newNode);
                                    }
                        if (xToRight <= matrix.GetUpperBound(0))
                            if (xToRight >= 0)
                                if (!closedNodes[xToRight, y])
                                    if (!matrix[xToRight, y].IsBlock)
                                    {
                                        var lastNode = new PathFinderNode
                                        {
                                            LastNode = stack[i].LastNode,
                                            X = x,
                                            Y = y,
                                            Heuristic = stack[i].Heuristic
                                        };
                                        
                                        var newNode = new PathFinderNode
                                        {
                                            X = xToRight,
                                            Y = y,
                                            NextNode = null,
                                            Heuristic = lastNode.Heuristic + (byte)(matrix[x, yToDown].IsBlock ? 1 : 0)
                                        };
                                        lastNode.NextNode = newNode;
                                        newNode.LastNode = lastNode;
                                        if (xToRight == end.X && y == end.Y)
                                        {
                                            finalNode = newNode;
                                            break;
                                        }
                                        closedNodes[xToRight, y] = true;
                                        newStack.Add(newNode);
                                    }
                        if (yToUp <= matrix.GetUpperBound(1))
                            if (yToUp >= 0)
                                if (!closedNodes[x, yToUp])
                                    if (!matrix[x, yToUp].IsBlock)
                                    {
                                        var lastNode = new PathFinderNode
                                        {
                                            LastNode = stack[i].LastNode,
                                            X = x,
                                            Y = y,
                                            Heuristic = stack[i].Heuristic
                                        };
                                        var newNode = new PathFinderNode
                                        {
                                            X = x,
                                            Y = yToUp,
                                            NextNode = null,
                                            Heuristic = lastNode.Heuristic + (byte)(matrix[x, yToDown].IsBlock ? 1 : 0)
                                        };
                                        lastNode.NextNode = newNode;
                                        newNode.LastNode = lastNode;
                                        if (x == end.X && yToUp == end.Y)
                                        {
                                            finalNode = newNode;
                                            break;
                                        }
                                        closedNodes[x, yToUp] = true;
                                        newStack.Add(newNode);
                                    }
                        if (yToDown <= matrix.GetUpperBound(1))
                            if (yToDown >= 0)
                                if (!closedNodes[x, yToDown])
                                    if (!matrix[x, yToDown].IsBlock)
                                    {
                                        var lastNode = new PathFinderNode
                                        {
                                            LastNode = stack[i].LastNode,
                                            X = x,
                                            Y = y,
                                            Heuristic = stack[i].Heuristic
                                        };
                                        var newNode = new PathFinderNode
                                        {
                                            X = x,
                                            Y = yToDown,
                                            NextNode = null,
                                            Heuristic = lastNode.Heuristic + (byte)(matrix[x, yToDown].IsBlock ? 1 : 0)
                                        };
                                        lastNode.NextNode = newNode;
                                        newNode.LastNode = lastNode;
                                        if (x == end.X && yToDown == end.Y)
                                        {
                                            finalNode = newNode;
                                            break;
                                        }
                                        closedNodes[x, yToDown] = true;
                                        newStack.Add(newNode);
                                    }
                    }
                    heuristic++;
                    stack = newStack;
                }
                if (finalNode != null)
                {
                    stack = new List<PathFinderNode>();
                    while (finalNode != null)
                    {
                        stack.Add(finalNode);
                        finalNode = finalNode.LastNode;
                    }
                    stack.Reverse();
                    return stack;
                }
                return null;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                return null;
            }
        }

        public class PathFinderNode
        {
            public PathFinderNode LastNode;
            public PathFinderNode NextNode;
            public int X { get; set; }
            public int Y { get; set; }
            public int Heuristic { get; set; }
        }
    }
}
