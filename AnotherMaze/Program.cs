
using System;
using System.Threading;
using System.IO;
using System.Windows;


namespace ActuallyNotSoEasyLabyrinth
{
    public enum CellType
    {
        Wall,
        Empty,
        Door,
        Key,
        EndCell,
    }
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right

    }
    public enum EventType
    {
        FoundAKey,
        NoKeys,
        SpentKey,
        Win
    }
    public struct Player
    {
        public int playerPosX;
        public int playerPosY;
        public int playerKeys;
        public Player(int x, int y,int _playerKeys)
        {
            playerPosX = x;
            playerPosY = y;
            playerKeys = _playerKeys;
        }
    }
    public struct Cell
    {
        public int cellPosX;
        public int cellPosY;
        public CellType cellType;
        public bool isVisible;
        public Cell(int x, int y, CellType type, bool IsOpen=false) { cellPosX = x; cellPosY = y; cellType = type; isVisible = IsOpen; }

    }
    class Program
    {
        public static bool isPlaying = true;

        public static Player player;
        public static Cell[,] cells;

        static void Main(string[] args)
        {
            cells = ReadMazeFromFile();


            DrawBottomLine();


            GameLoop();
           

        }

        private static void DrawBottomLine()
        {
            Console.SetCursorPosition(0, cells.GetLength(0) + 3);
            for(int i = 0; i < cells.GetLength(1); i++)
            {
                Console.Write("-");
            }
        } //Просто отрисовка линии снизу лабиринта чтобы отличить лабиринт от "Интерфейса"

        public static List<string> GetMazeList()
        {
            
            StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "maze.txt"));
            List<string> mazeList = new List<string>();

            foreach (string line in sr.ReadToEnd().Split())
            {
                if (line != "") {
                    mazeList.Add(line);
                }
            }
            return mazeList;

        } //Перенос текстового документа с лабиринтом в список строк(сделал через список потому что так удобнее узнать высоту лабиринта)
        public static Cell[,] ReadMazeFromFile() //Перенос клеток лабиринта из текстового типа в массив структур "Cell"
        {

            List<string> mazeList = GetMazeList(); 
            Cell[,] maze = new Cell[mazeList.Count, mazeList[0].Length];

            for (int i = 0; i < mazeList.Count; i++)
            {
                for (int j = 0; j < mazeList[0].Length; j++)
                {
                    switch (mazeList[i][j])
                    {
                        case '0':
                            maze[i, j] = new Cell(i, j, CellType.Empty);
                            break;
                        case '1':
                            maze[i, j] = new Cell(i, j, CellType.Wall);
                            break;
                        case '2':
                            player = new Player(j, i,1);
                            maze[i, j] = new Cell(i, j, CellType.Empty);
                            break;
                        case '3':
                            maze[i, j] = new Cell(i, j, CellType.Door);
                            break;
                        case '4':
                            maze[i, j] = new Cell(i, j, CellType.Key);
                            break;
                        case '5':
                            maze[i, j] = new Cell(i, j, CellType.EndCell);
                            break;
                        default:

                            if (mazeList[i][j] != '\n' & mazeList[i][j] != '\r')
                                Console.WriteLine("Вы где то допустили ошибку записи лабиринта, исправьте её и перезапустите программу");
                            Console.WriteLine(mazeList[i][j]);

                            break;

                    }

                }
            }
            return maze;

        }
        private static void GameLoop()
        {

            isPlaying = true;
            SetNearCellsVisible();
            while (isPlaying)
            {
              DrawUI();
                DrawMaze();
                DrawPlayer();
                HandleMovement();
                CheckCurrentCell();
                Thread.Sleep(20);
                
            }
            Console.Clear();
            Console.Write("You won");
            Console.ReadKey();
        } //Общий игровой цикл(можно считать за 1 кадр)
        
        private static void DrawUI()
        {
            Console.SetCursorPosition(0, cells.GetLength(0) + 7);
            Console.Write($"У вас {player.playerKeys} ключ(ей)");
        } //Отрисовка типо интерфейса

        private static void HandleMovement()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            ConsoleKey key = keyInfo.Key;
          
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if ((player.playerPosY - 1) >= 0)
                    {
                        if(cells[player.playerPosY - 1, player.playerPosX ].cellType != CellType.Wall) {
                            if (cells[player.playerPosY - 1, player.playerPosX].cellType == CellType.Door)
                            {
                                CheckForKeys(player.playerPosY - 1, player.playerPosX, Direction.Left);
                            }
                            else { 
                            player.playerPosY -= 1;
                         
                            }
                        }

                    }
                    break;
                case (ConsoleKey.LeftArrow):

                    if (cells[player.playerPosY, player.playerPosX-1].cellType != CellType.Wall) {
                        if (cells[player.playerPosY, player.playerPosX - 1].cellType == CellType.Door)
                        {
                            CheckForKeys(player.playerPosY, player.playerPosX - 1, Direction.Left);
                        }
                        else
                        {
                            player.playerPosX -= 1;

                        }
                       
                
                    }

                    break;
                case (ConsoleKey.DownArrow):


                    if (cells[player.playerPosY+1, player.playerPosX].cellType != CellType.Wall)
                    {
                        if (cells[player.playerPosY + 1, player.playerPosX].cellType == CellType.Door)
                        {
                            CheckForKeys(player.playerPosY + 1, player.playerPosX,Direction.Down);
                           
                        }
                        else
                        {
                            player.playerPosY += 1;

                        }
                        
                       
                    }

                    break;
                case (ConsoleKey.RightArrow):

                    if (cells[player.playerPosY, player.playerPosX+1 ].cellType != CellType.Wall)
                    {
                        if (cells[player.playerPosY, player.playerPosX + 1].cellType == CellType.Door)
                        {
                            CheckForKeys(player.playerPosY, player.playerPosX + 1,Direction.Right);
                        }
                        else
                        {
                            player.playerPosX += 1;

                        }
                        
                        
                    }
                    break;

            } //Выбор 1 из 4ёх стрелочек
            SetNearCellsVisible();
        } //Обработка управления игрока

        private static void CheckForKeys(int y,int x,Direction dir) //Проверка игрока на наличие ключей при попытке подойти к двери
        {
            if (player.playerKeys > 0)
            {
               
                player.playerKeys -= 1;
                EventHandler( EventType.SpentKey);
                switch (dir)
                {
                    case Direction.Down:
                        player.playerPosY += 1;
                        break;
                    case Direction.Up:      
                        player.playerPosY -= 1;
                        break;
                    case Direction.Right:
                        player.playerPosX += 1;
                        break;
                    case Direction.Left:
                        player.playerPosX -= 1;
                        break;
                }
            }
            else
            {
                EventHandler(EventType.NoKeys);

            }
        }

        private static void EventHandler(EventType eventType)
        {
            Console.SetCursorPosition(0, cells.GetLength(0) + 5);
            switch (eventType)
            {
                case EventType.FoundAKey:
                    Console.WriteLine("Вы нашли ключ");
                    break;
                case EventType.NoKeys:
                    Console.WriteLine("У вас нет ключей ");
                    break;
                case EventType.SpentKey:
                    Console.WriteLine("Вы потратили ключ");
                    break;
                case EventType.Win:
                    Console.WriteLine("Вы прошли лабиринт");
                    isPlaying = false;
                    break;
            }
           
          
        } //Обработчик событий(поднять ключ, открыть дверь и т.д)

        private static void DrawMaze() //Отрисовка лабиринта
        {
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                for (int j = 0; j < cells.GetLength(1); j++)
                {
                    Console.SetCursorPosition(j, i);
                    if (cells[i, j].isVisible) { 
                    switch (cells[i, j].cellType)
                    {
                        case CellType.Wall:
                            Console.Write("█");
                            break;
                        case CellType.Empty:
                            Console.Write(" ");
                            break;
                            case CellType.Door:
                                Console.Write("╠");
                                break;
                            case CellType.Key:
                                Console.Write("╾");
                                break;
                            case CellType.EndCell:
                                Console.Write("‼");
                                break;
                                
                        }
                    }
                    else
                    {
                        Console.Write("▒");
                    }
                }
            }

        }

        private static void DrawPlayer()
        {
            Console.SetCursorPosition(player.playerPosX, player.playerPosY);
            Console.Write("☺");
            Console.SetCursorPosition(cells.GetLength(1) + 1, cells.GetLength(0) + 1);
        } //Отрисовка игрока

        private static void SetNearCellsVisible()
        {
            cells[player.playerPosY, player.playerPosX].isVisible = true;
            cells[player.playerPosY, player.playerPosX + 1].isVisible = true;
            cells[player.playerPosY, player.playerPosX-1].isVisible = true;
            cells[player.playerPosY + 1, player.playerPosX].isVisible = true;
            cells[player.playerPosY-1, player.playerPosX].isVisible = true;
            cells[player.playerPosY + 1, player.playerPosX+1].isVisible = true;
            cells[player.playerPosY+1, player.playerPosX - 1].isVisible = true;
            cells[player.playerPosY - 1,player.playerPosX+1].isVisible = true;
            cells[player.playerPosY-1, player.playerPosX - 1].isVisible = true;
        } //Отпускание рядом-стоящих клеток из тумана войны

        private static void CheckCurrentCell() //Проверка клетки на которой стоит игрок на наличие каких нибудь её особенностей
        {
           
            switch (cells[player.playerPosY, player.playerPosX].cellType)
            {

                case CellType.Key:
                    player.playerKeys += 1;
                    EventHandler(EventType.FoundAKey);
                    break;
                case CellType.EndCell:
                    EventHandler(EventType.Win);
                    break;
                    
            }
            cells[player.playerPosY, player.playerPosX].cellType = CellType.Empty;
        }

    }
}


