using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;

namespace Touhou_19___Nekuskus
{
    static class Program
    {
        public enum Postacie
        {
            Reimu,
            Marisa
        }
        
        public static (int, int) Stage = (102, 3);
        public static (int, int) Score = (102, 7);
        public static (int, int) Lives = (102, 10);
        public static (int, int) Bombs = (102, 12);
        public static Postacie Postać;
        public static bool IsBarVisible = false;
        public static int DefLives;
        public static int DefBombs;
        public static int CurLives;
        public static int CurBombs;
        public static char Hitbox;
        public static char BossHitbox;
        public static ConsoleColor CharacterColor;
        public static decimal counter = 1.0m;

        public enum ObjectType
        {
            Player,
            Enemy,
            Boss,
            PlayerBullet,
            EnemyBullet
        }
        public class GameObject
        {
            public (int, int) Position;
            public ObjectType Type;
            public decimal MoveCounter;
            public int PerishTime;
            public int PerishCount = 1;
            public string Direction;
            public bool exists = true;
            private int _hp;
            public int Hp //functions as damage for bullets since they don't have hp
            {
                get
                {
                    if(Type == ObjectType.Player)
                    {
                        return CurLives;
                    }
                    else
                    { 
                        return _hp;
                    }
                }
                set => _hp = value;
            }
            public GameObject((int, int) _Position, ObjectType _Type, decimal _MoveCounter, int _Health)
            {
                Position = _Position;
                Type = _Type;
                MoveCounter = _MoveCounter;
                Hp = _Health;
            }
            public GameObject((int, int) _Position, ObjectType _Type, decimal _MoveCounter, string _Direction, int Damage, int _PerishTimer = 0)
            {
                Position = _Position;
                Type = _Type;
                MoveCounter = _MoveCounter;
                PerishTime = _PerishTimer;
                Direction = _Direction;
                _hp = Damage;
            }
        }
        public static List<GameObject> Bullets = new List<GameObject>();
        public static List<GameObject> Characters = new List<GameObject>();

        static void WriteHorizontal(ValueTuple<int, int> coords, string text, ConsoleColor color = ConsoleColor.White) //1 is col, 2 is row
        {
            ConsoleColor curcolor = Console.ForegroundColor;
            (int, int) cursorpos = (Console.CursorLeft, Console.CursorTop);
            if(text.Length + coords.Item1 > Console.WindowWidth) throw new ArgumentOutOfRangeException();
            Console.ForegroundColor = color;
            Console.SetCursorPosition(coords.Item1, coords.Item2);
            Console.Write(text);
            Console.SetCursorPosition(cursorpos.Item1, cursorpos.Item2);
            Console.ForegroundColor = curcolor;
        }
        static void WriteVertical(ValueTuple<int, int> coords, string text, ConsoleColor color = ConsoleColor.White) //1 is col, 2 is row
        {
            ConsoleColor curcolor = Console.ForegroundColor;
            (int, int) cursorpos = (Console.CursorLeft, Console.CursorTop);
            if(text.Length + coords.Item2 > Console.WindowHeight) throw new ArgumentOutOfRangeException();
            Console.SetCursorPosition(coords.Item1, coords.Item2);
            Console.ForegroundColor = color;
            foreach(char c in text)
            {
                Console.Write(c);
                coords.Item2 = coords.Item2 + 1;
                if(coords.Item2 == Console.BufferHeight) return;
                Console.SetCursorPosition(coords.Item1, coords.Item2);
            }
            Console.ForegroundColor = curcolor;
            Console.SetCursorPosition(cursorpos.Item1, cursorpos.Item2);
        }
        static void RenderBackgroundColors()
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            int i = 0;
            while(i < 7)
            {
                WriteHorizontal((91, i), new string(' ', 29));
                i++;
            }
            WriteHorizontal((91, 7), new string(' ', 4));
            WriteHorizontal((113, 7), new string(' ', 7));
            WriteHorizontal((91, 8), new string(' ', 29));
            WriteHorizontal((91, 9), new string(' ', 29));
            WriteHorizontal((91, 10), new string(' ', 4));
            WriteHorizontal((113, 10), new string(' ', 7));
            WriteHorizontal((91, 8), new string(' ', 29));
            WriteHorizontal((91, 11), new string(' ', 29));
            WriteHorizontal((91, 12), new string(' ', 4));
            WriteHorizontal((113, 12), new string(' ', 7));
            i = 13;
            while(i < 49)
            {
                WriteHorizontal((91, i), new string(' ', 29));
                i++;
            }
            WriteHorizontal((91, 49), new string(' ', 28));
            Console.BackgroundColor = ConsoleColor.Black;
        }
        static void InitUI()
        {
            Console.SetCursorPosition(0, 0);
            Console.SetWindowPosition(0, 0);
            Console.Write(new string(' ', 3000));
            Console.SetWindowSize(120, 50);
            Console.SetWindowPosition(0, 0);
            Console.SetBufferSize(120, 50);
            Console.SetWindowPosition(0, 0);
            WriteVertical((90, 0), new string('|', 50));
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            WriteHorizontal((95, 7), "Score: 00000000000");
            WriteHorizontal((95, 10), "Lives:            ", ConsoleColor.DarkRed);
            WriteHorizontal((95, 12), "Bombs:            ", ConsoleColor.DarkBlue);
            Console.BackgroundColor = ConsoleColor.Black;
            RenderBackgroundColors();
            Console.SetCursorPosition(0, 0);
        }
        static void ClearGameSpace()
        {
            int i = 0;
            while (i < Console.BufferHeight)
            {
                WriteHorizontal((0, i), new string(' ', 90));
                i++;
            }
            Console.SetCursorPosition(0, 0);
        }
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            InitUI();
            WriteHorizontal((38, 10), "Choose your character!");
            WriteHorizontal((43, 13), "Reimu", ConsoleColor.Red);
            WriteHorizontal((50, 13), "Marisa", ConsoleColor.Yellow);
            
            wyborpostaci:
            Console.SetCursorPosition(46, 15);
            string postać = Console.ReadLine();
            if(postać == "R" || postać == "Reimu")
            {
                Postać = Postacie.Reimu;
                MainLoop();
            }
            else if(postać == "M" || postać == "Marisa")
            {
                Postać = Postacie.Marisa;
                MainLoop();
            }
            else
            {
                WriteHorizontal((0, 15), new string(' ', 90));
                goto wyborpostaci;
            }
            Console.ReadKey();
        }
        static void CheckKeys(decimal counter)
        {
            if(counter % Characters[0].MoveCounter != 0) return;
            if(Keyboard.IsKeyDown(Key.Right))
            {
                if(!(Characters[0].Position.Item1 == 89))
                {
                    Characters[0].Position.Item1 += 1;
                }
            }
            if(Keyboard.IsKeyDown(Key.Left))
            {
                if(!(Characters[0].Position.Item1 == 0))
                {
                    Characters[0].Position.Item1 -= 1;
                }
            }
            if(Keyboard.IsKeyDown(Key.Up))
            {
                if(!(Characters[0].Position.Item2 == (IsBarVisible ? 1 : 0)))
                {
                    Characters[0].Position.Item2 -= 1;
                }
            }
            if(Keyboard.IsKeyDown(Key.Down))
            {
                if(!(Characters[0].Position.Item2 == 49))
                {
                    Characters[0].Position.Item2 += 1;
                }
            }
            if(Keyboard.IsKeyDown(Key.Z))
            {
                if(Characters[0].Position.Item2 > 1)
                {
                    switch(Postać)
                    {
                        case Postacie.Reimu:
                            if(Characters[0].Position.Item1 > 0)
                            { 
                                GameObject ball1 = new GameObject((Characters[0].Position.Item1 - 1, Characters[0].Position.Item2 - 1), ObjectType.PlayerBullet, 1m, "left", 1);
                                Bullets.Add(ball1);
                            }
                            GameObject ball2 = new GameObject((Characters[0].Position.Item1, Characters[0].Position.Item2 - 1), ObjectType.PlayerBullet, 1m, "forward", 1);
                            Bullets.Add(ball2);
                            if(Characters[0].Position.Item1 < 89)
                            {
                                GameObject ball3 = new GameObject((Characters[0].Position.Item1 + 1, Characters[0].Position.Item2 - 1), ObjectType.PlayerBullet, 1m, "right", 1);
                                Bullets.Add(ball3);
                            }
                            break;
                        case Postacie.Marisa:
                            GameObject laser = new GameObject((Characters[0].Position.Item1, Characters[0].Position.Item2 - 1), ObjectType.PlayerBullet, 0.5m, "forward", 2);
                            Bullets.Add(laser);
                            break;
                    }
                }
            }
            if(Keyboard.IsKeyDown(Key.X))
            {

            }
        }
        static void ProcessEnemyMoves(decimal counter, ref List<GameObject> bullets_to_add)
        {
            foreach(GameObject ch in Characters)
            {
                if(ch.Type == ObjectType.Player) continue;
                if(counter % ch.MoveCounter == 0)
                {
                    GameObject enemyBullet = new GameObject((ch.Position.Item1, ch.Position.Item2 + 1), ObjectType.EnemyBullet, 1m, "forward", 1);
                    Bullets.Add(enemyBullet);
                }
            }
        }
        static void MainLoop()
        {
            try
            {
                Console.CursorVisible = false;
            }
            catch(Exception ex)
            {
                
            }
            Console.WriteLine("Started MainLoop()!");
            ClearGameSpace();
            DefLives = CurLives = 5;
            Characters.Add(new GameObject((50, 35), ObjectType.Player, 1, DefLives));
            switch(Postać)
            {
                case Postacie.Reimu:
                    DefBombs = CurBombs = 3;
                    Hitbox = 'R';
                    Characters[0].MoveCounter = 1m;
                    CharacterColor = ConsoleColor.Red;
                    break;
                case Postacie.Marisa:
                    DefBombs = CurBombs = 2;
                    Hitbox = 'M';
                    Characters[0].MoveCounter = 0.5m;
                    CharacterColor = ConsoleColor.Yellow;
                    break;
            }
            Thread t = new Thread(new ParameterizedThreadStart(GameProgress));
            t.Start(1);
            while(true)
            {
                if(counter == 8)
                {
                    counter = 1;
                }
                CheckKeys(counter);
                foreach(GameObject ch in Characters)
                {
                    WriteHorizontal((ch.Position.Item1, ch.Position.Item2), (ch.Type == ObjectType.Player) ? Hitbox.ToString() : (ch.Type == ObjectType.Boss) ? BossHitbox.ToString() : "E" , CharacterColor);
                }
                List<GameObject> bullets_to_add = new List<GameObject>();
                List<GameObject> bullets_to_delete = new List<GameObject>();
                ProcessEnemyMoves(counter, ref bullets_to_add);
                foreach(GameObject b in Bullets)
                {
                    if(!b.exists)
                    {
                        bullets_to_delete.Add(b);
                        continue;
                    }
                    WriteHorizontal((b.Position.Item1, b.Position.Item2), (b.Type == ObjectType.PlayerBullet) ? (Postać == Postacie.Reimu) ? "." : "|" : "#");
                    b.PerishCount += 1;      
                    if(b.Position.Item2 - 1 > (IsBarVisible ? 1 : 0))
                    {
                        switch (b.Type)
                        {
                            case ObjectType.EnemyBullet:
                                if(b.Position.Item2 != 49)
                                {
                                    if(b.exists)
                                    {
                                        switch (b.Direction)
                                        {
                                            case "left":
                                                bullets_to_add.Add(new GameObject((b.Position.Item1, b.Position.Item2 + 1), ObjectType.EnemyBullet, 1, "forward", 1));
                                                break;
                                            case "forward":
                                                if(b.exists) bullets_to_add.Add(new GameObject((b.Position.Item1, b.Position.Item2 + 1), ObjectType.EnemyBullet, 1, "forward", 1));
                                                break;
                                            case "right":
                                                break;
                                        }
                                    }
                                }
                                break;
                            case ObjectType.PlayerBullet:
                                if(Postać == Postacie.Reimu)
                                {
                                    switch(b.Direction)
                                    {
                                            case "left":
                                                if(b.Position.Item1 != 0)
                                                bullets_to_add.Add(new GameObject((b.Position.Item1 - 1, b.Position.Item2 - 1), ObjectType.PlayerBullet, 1m, "left", 1));
                                                break;
                                            case "forward":
                                                bullets_to_add.Add(new GameObject((b.Position.Item1, b.Position.Item2 - 1), ObjectType.PlayerBullet, 1m, "forward", 1));
                                                break;
                                            case "right":
                                                if(b.Position.Item1 != 89)
                                                bullets_to_add.Add(new GameObject((b.Position.Item1 + 1, b.Position.Item2 - 1), ObjectType.PlayerBullet, 1m, "right", 1));
                                                break;
                                    }
                                }
                                else if(Postać == Postacie.Marisa)
                                {
                                    bullets_to_add.Add(new GameObject((b.Position.Item1, b.Position.Item2 - 1), ObjectType.PlayerBullet, 0.5m, "forward" ,2));
                                }
                                break;
                        }
                    }
                    if(b.PerishCount >= b.PerishTime)
                    {
                        bullets_to_delete.Add(b);
                    }
                }
#if DEBUG
                WriteHorizontal((Bombs.Item1 - 7, Bombs.Item2 + 3), $"Bullets: {Bullets.Count}");
                WriteHorizontal((Bombs.Item1 - 7, Bombs.Item2 + 4), $"bullets_to_add: {bullets_to_add.Count}");
                WriteHorizontal((Bombs.Item1 - 7, Bombs.Item2 + 5), $"bullets_to delete: {bullets_to_delete.Count}");
#endif

                foreach(GameObject b in Bullets)
                {
                    if(b.Position == Characters[0].Position)
                    {
                        CurLives -= 1;
                        foreach(GameObject b1 in Bullets)
                        {
                            if(b.Type == ObjectType.EnemyBullet)
                            {
                                b.exists = false;
                                bullets_to_delete.Add(b);
                            }
                        }
                    }
                }
                foreach(GameObject b in bullets_to_add)
                {
                    Bullets.Add(b);
                }
                foreach(GameObject b in bullets_to_delete)
                {
                    Bullets.Remove(b);
                }
                if(CurLives == 0)
                {
                    ClearGameSpace();
                }
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                WriteHorizontal((Lives.Item1, Lives.Item2), new string('*', CurLives - 1) + new string(' ', 11 - CurLives), ConsoleColor.DarkRed);
                WriteHorizontal((Bombs.Item1, Bombs.Item2), new string('*', CurBombs), ConsoleColor.DarkBlue);
                Console.BackgroundColor = ConsoleColor.Black;
                counter += 0.5m;
                Thread.Sleep(33);
                ClearGameSpace();
            }
        }
        static void GameProgress(object stage) //1 - 7, 7 equals Extra
        {
            if(!(stage is int)) throw new ArgumentException();
            Console.BackgroundColor = ConsoleColor.Cyan;
            WriteHorizontal((Stage.Item1, Stage.Item2), "Stage 1", ConsoleColor.DarkGreen);
            Console.BackgroundColor = ConsoleColor.Black;

            //Creating enemies and bullets starts here uwu
            GameObject enemy = new GameObject((45, 5), ObjectType.Enemy, 4m, 5);
            Characters.Add(enemy);

        }
    }
}
