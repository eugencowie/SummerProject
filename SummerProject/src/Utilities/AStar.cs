// ReSharper disable All

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace SummerProject.AStar
{
    public class Unit
    {
        public bool IsPlayer;
    }

    public enum TileType { Floor, Wall, Door, OpenDoor }

    public class TileInfo
    {
        public TileType TileType = TileType.Floor;
        public Unit UnitOnTile;
    }

    public class AStar
    {
        public struct ListItem
        {
            public int X;
            public int Y;
            public bool Opened;
            public bool Closed;
            public int F;
            public int G;
            public int H;
            public int ParentX;
            public int ParentY;

            public ListItem(int x, int y)
            {
                X = x;
                Y = y;
                Opened = false;
                Closed = false;
                F = 0;
                G = 0;
                H = 0;
                ParentX = 0;
                ParentY = 0;
            }
        }

        TileInfo[,] _colMap;
        Dictionary<Point, ListItem> _list = new Dictionary<Point, ListItem>();
        List<ListItem> _openList = new List<ListItem>();

        int _hWeight;
        int _maxLoopsMultiplier;

        int _goalX;
        int _goalY;
        int _startX;
        int _startY;

        int _width, _height;

        List<Vector2> _path;
        bool _pathAvailable = false;

        #region Getters/setters

        public List<Vector2> Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public bool PathAvailable
        {
            get { return _pathAvailable; }
            set { _pathAvailable = value; }
        }

        #endregion

        public AStar(TileInfo[,] colMap, int hWeight, int maxLoopsMultiplier)
        {
            _hWeight = hWeight;
            _colMap = colMap;
            _path = new List<Vector2>();
            _width = _colMap.GetLength(0);
            _height = _colMap.GetLength(1);
            _maxLoopsMultiplier = maxLoopsMultiplier;
        }

        public bool Start(int Sx, int Sy, int Gx, int Gy)
        {
            _pathAvailable = false;

            _goalX = Gx;
            _goalY = Gy;

            _startX = Sx;
            _startY = Sy;

            _path.Clear();

            _openList.Clear();
           
            _list.Clear();

            bool noPath = false;

            if ((Gx - 1 < 0 || _colMap[Gx - 1, Gy].TileType == TileType.Wall) &&
                (Gx + 1 > _width || _colMap[Gx + 1, Gy].TileType == TileType.Wall) &&
                (Gy - 1 < 0 || _colMap[Gx, Gy - 1].TileType == TileType.Wall) &&
                (Gy + 1 > _width || _colMap[Gx, Gy + 1].TileType == TileType.Wall))
            {
                noPath = true;
            }

            if (!(Sx == Gx && Sy == Gy) && _colMap[Gx, Gy].TileType != TileType.Wall && !noPath)
            {
                _list.Add(new Point(Sx, Sy), new ListItem(Sx, Sy));
                _list.Add(new Point(Gx, Gy), new ListItem(Gx, Gy));
                _openList.Add(new ListItem(Sx, Sy));

                SetOpenOnLowestF(Sx, Sy, Gx, Gy);

                if (_pathAvailable)
                {
                    MakePath(Gx, Gy, Sx, Sy);
                }
                else if (_colMap[Gx, Gy].UnitOnTile != null &&
                         _colMap[Gx, Gy].UnitOnTile.IsPlayer &&
                         Math.Abs(_goalX - _startX) < 2 &&
                         Math.Abs(_goalY - _startY) <  2)
                {
                    // if the block is on the goal tile then return true so we can attack him
                    _path.Add(new Vector2(Gx, Gy));
                    return true;
                }
            }

            //Console.WriteLine(_path.ToString);

            return _pathAvailable;
        }

        private void SetOpenOnLowestF(int Sx, int Sy, int Gx, int Gy)
        {
            int test = 0;
            int loops = 0;
            int removeIndex = 0;

            do
            {
                Point startLoc = new Point(Sx, Sy);
                ListItem listItem = _list[startLoc];
                loops++;

                listItem.Opened = false;
                _openList.RemoveAt(removeIndex);
                listItem.Closed = true;
                _list[startLoc] = listItem;

                int YFin = Sy + 1;
                int XFin = Sx + 1;
                for (int y = Sy - 1; y <= YFin; ++y)
                {
                    if (y < 0 || y > _height - 1)
                        continue;

                    for (int x = Sx - 1; x <= XFin; ++x)
                    {
                        Point location = new Point(x, y);

                        if (!_list.ContainsKey(location))
                            _list.Add(new Point(x, y), new ListItem(x, y));

                        if ((x == Sx && y == Sy) || (x < 0 || x > _width - 1) ||
                            (!IsWalkable(x, y)) || (_list[location].Closed)) // || npcthere(X, Y))
                            continue;

                        TileInfo colNode = _colMap[x, y];

                        ListItem tempListItem = _list[location];

                        if (tempListItem.Opened)
                        {
                            int tempG = 0;
                            if (x == Sx || y == Sy)
                            {
                                if (colNode.UnitOnTile == null)
                                    tempG = 10 + _list[location].G;
                                else
                                    tempListItem.G = 45 + tempListItem.G;

                            }
                            else
                            {
                                if (colNode.UnitOnTile == null)
                                    tempG = 14 + tempListItem.G;
                                else
                                    tempListItem.G = 62 + tempListItem.G;

                            }

                            if (tempG < _list[location].G)
                            {
                                tempListItem.G = tempG;
                                tempListItem.ParentX = Sx;
                                tempListItem.ParentY = Sy;
                                tempListItem.F = tempListItem.G + tempListItem.H;
                            }

                            _list[location] = tempListItem;
                            continue;
                        }

                        if (x == Sx || y == Sy)
                        {
                            if (colNode.UnitOnTile == null)
                                tempListItem.G = 10 + _list[new Point(Sx, Sy)].G;
                            else
                                tempListItem.G = 45 + _list[new Point(Sx, Sy)].G;

                            tempListItem.Opened = true;
                            tempListItem.X = x;
                            tempListItem.Y = y;
                            tempListItem.ParentX = Sx;
                            tempListItem.ParentY = Sy;
                            tempListItem.H = CalculateH(_goalX, x) + CalculateH(_goalY, y);
                            tempListItem.F = tempListItem.G + tempListItem.H;
                            _openList.Add(tempListItem);
                            _list[location] = tempListItem;
                        }
                        else
                        {
                            if ((x + 1 < _width && !IsWalkable(x + 1, y)) || 
                                (x - 1 >= 0 &&!IsWalkable(x - 1, y)) || 
                                (y - 1 >= 0 && !IsWalkable(x, y - 1)) || 
                                (y + 1 < _height && !IsWalkable(x, y + 1)))
                            {
                                tempListItem.Opened = false;
                                _list[location] = tempListItem;
                                continue;
                            }

                            if (colNode.UnitOnTile == null)
                                tempListItem.G = 14 + _list[new Point(Sx, Sy)].G;
                            else
                                tempListItem.G = 62 + _list[new Point(Sx, Sy)].G;

                            tempListItem.Opened = true;
                            tempListItem.X = x;
                            tempListItem.Y = y;
                            tempListItem.ParentX = Sx;
                            tempListItem.ParentY = Sy;
                            tempListItem.H = CalculateH(_goalX, x) + CalculateH(_goalY, y);
                            tempListItem.F = tempListItem.G + tempListItem.H;
                            _openList.Add(tempListItem);
                            _list[location] = tempListItem;
                        }
                    }
                }

                if (!(_list[new Point(Gx, Gy)].Opened))
                {
                    int lowestF = 10000;
                    test = 0;
                    for (int index = 0; index < _openList.Count; ++index)
                    {
                        test++;
                        if (_openList[index].F < lowestF)
                        {
                            lowestF = _openList[index].F;
                            Sx = _openList[index].X;
                            Sy = _openList[index].Y;
                            removeIndex = index;
                        }
                    }
                }

                DisplayCurrent(new Point(Sx, Sy));                
            }
            while (test != 0 && !_list[new Point(Gx, Gy)].Opened && loops < 300 * _maxLoopsMultiplier);

            DisplayCurrent(new Point(Sx, Sy));

            if (test == 0 || loops > 249 * _maxLoopsMultiplier)
                _pathAvailable = false;
            else
                _pathAvailable = true;
        }

        private bool IsWalkable(int x, int y)
        {
            ListItem tempItem = new ListItem();
            Point loc = new Point(x, y);

            if (_colMap[x, y].TileType == TileType.Wall)
            {
                tempItem.Closed = true;
                if (!_list.ContainsKey(loc))
                    _list.Add(loc, tempItem);
                _list[new Point(x, y)] = tempItem;
                return false;
            }
            else if (_colMap[x, y].UnitOnTile != null && _colMap[x, y].UnitOnTile.IsPlayer && x != _goalX && y != _goalY)
            {
                tempItem.Closed = true;
                if (!_list.ContainsKey(loc))
                    _list.Add(loc, tempItem);
                _list[new Point(x, y)] = tempItem;
                return false;
            }

            return true;
        }

        private int CalculateH(int G, int S)
        {
            if (S > G)
                return (S - G) * _hWeight;
            else if (G > S)
                return (G - S) * _hWeight;
            else
                return 0;
        }


        private void MakePath(int Gx, int Gy, int Sx, int Sy)
        {
            Vector2 temp = new Vector2(Gx, Gy);

            while (!(temp.X == Sx && temp.Y == Sy))
            {
                _path.Insert(0, temp);

                int x = (int)temp.X;
                int y = (int)temp.Y;
                Point loc = new Point(x, y);
                temp = new Vector2(_list[loc].ParentX, _list[loc].ParentY);
            }
        }

        private Vector2 GetParentOf(int x, int y)
        {
            Point loc = new Point(x, y);
            return new Vector2(_list[loc].ParentX, _list[loc].ParentY);
        }

        public bool GetOpened(int x, int y)
        {
            Point loc = new Point(x, y);
            return _list[loc].Opened;
        }

        public bool GetClosed(int x, int y)
        {
            Point loc = new Point(x, y);
            return _list[loc].Closed;
        }

        public int GetF(int x, int y)
        {
            Point loc = new Point(x, y);
            return _list[loc].F;
        }

        public int GetG(int x, int y)
        {
            Point loc = new Point(x, y);
            return _list[loc].G;
        }

        public int GetH(int x, int y)
        {
            Point loc = new Point(x, y);
            return _list[loc].H;
        }

        public void DisplayCurrent(Point p)
        {
/*
#if WINDOWS
            Console.Clear();

            Console.WriteLine("Open or Closed");

            for (int Y = -1; Y < _colMap.GetLength(1); ++Y)
            {
                for (int X = -1; X < _colMap.GetLength(0); ++X)
                {
                    if (X == -1 || Y == -1)
                    {
                        if (Y == -1 && X == -1)
                            Console.Write("\t");
                        if (Y == -1 && X != -1)
                        {
                            if (X < 10)
                                Console.Write("  " + X);
                            else
                                Console.Write(" " + X);
                        }
                        if (Y != -1 && X == -1)
                            Console.Write(Y + "\t");
                    }
                    else
                    {
                        Point loc = new Point(X, Y);
                        if (_startX == X && _startY == Y)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(" S ");
                        }
                        else if (p.X == X && p.Y == Y)
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write(" X ");
                        }
                        else if (_goalX == X && _goalY == Y)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(" G ");
                        }
                        else if (_list.ContainsKey(loc) && GetOpened(X, Y))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" O ");
                        }
                        else if (_list.ContainsKey(loc) && GetClosed(X, Y))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" C ");
                        }
                        else if (_colMap[X, Y].TileType == TileInfo.TileType.Wall)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write(" # ");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write(" X ");
                        }

                    }


                }
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                
            }
            Console.ReadKey();
            //Console.WriteLine("Values");

            //for (int Y = -1; Y < _colMap.GetLength(1); ++Y)
            //{
            //    for (int X = -1; X < _colMap.GetLength(0); ++X)
            //    {
            //        if (X == -1 || Y == -1)
            //        {
            //            if (Y == -1 && X == -1)
            //                Console.Write("\t");
            //            if (Y == -1 && X != -1)
            //            {
            //                if (X < 10)
            //                    Console.Write("   " + X);
            //                else
            //                    Console.Write("  " + X);
            //            }
            //            if (Y != -1 && X == -1)
            //                Console.Write(Y + "\t");
            //        }
            //        else
            //        {
            //            Point loc = new Point(X, Y);
            //            if (_list.ContainsKey(loc))
            //            {
            //                Console.ForegroundColor = ConsoleColor.Yellow;
            //                if (_list[loc].F < 10)
            //                    Console.Write(" 00" + _list[loc].H + " ");
            //                else if (_list[loc].F < 100)
            //                    Console.Write(" 0" + _list[loc].H + " ");
            //                else
            //                    Console.Write(" " + _list[loc].H + " ");
            //            }
            //            else
            //                Console.Write(" XXX");
            //        }


            //    }
            //    Console.WriteLine();
            //    Console.WriteLine();
            //    Console.ForegroundColor = ConsoleColor.White;
            //}


            //Console.WriteLine();
            //Console.WriteLine("Values of G");

            //for (int Y = 0; Y < _colMap.GetLength(1); ++Y)
            //{
            //    for (int X = 0; X < _colMap.GetLength(0); ++X)
            //    {
            //        Point loc = new Point(X, Y);
            //        if (_list.ContainsKey(loc))
            //        {
            //            if (_list[loc].G > 9)
            //                Console.Write("" + _list[loc].G + " ");
            //            else
            //                Console.Write(" 0" + _list[loc].G + " ");
            //        }
            //    }
            //    Console.WriteLine();
            //    Console.WriteLine();
            //}

            //Console.WriteLine();
            //Console.WriteLine("H Values");

            //     for (int Y = 0; Y < _colMap.GetLength(1); ++Y)
            //{
            //    for (int X = 0; X < _colMap.GetLength(0); ++X)
            //    {
            //        Point loc = new Point(X, Y);
            //        if (_list.ContainsKey(loc))
            //        {
            //            if (_list[loc].H > 9)
            //            Console.Write(" " + _list[loc].H + " ");
            //                else
            //            Console.Write(" 0" + _list[loc].H + " ");
            //        }
            //    }
            //    Console.WriteLine();
            //    Console.WriteLine();
            //}


            //Console.ReadKey();



        } // End of Function
#endif
*/
        }
    }
}
