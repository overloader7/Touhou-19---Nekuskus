﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Touhou_19___Nekuskus
{
    static class Program
    {
        
        public static (int, int) Stage = (102, 3);
        
        public static (int, int) Score = (102, 7);
        public static int score;

        public static (int, int) Lives = (102, 10);
        public static int DefLives;
        public static int CurLives;

        public static (int, int) Bombs = (102, 12);
        public static int DefBombs;
        public static int CurBombs;


        public static (int, int) Power = (102, 15);
        public static double power = 0;

        public static (int, int) Graze = (102, 17);
        public static int graze = 0;

        //public static (int, int) Points1 = (102, 19);
        //public static (int, int) Points2 = (106, 19);
        public static (int, int) Points = (102, 19);
        public static (int, int) MaxPoints = (106, 19);
		
        public static int points = 0;
        public static int maxpoints;
        


        public static Postacie Postać;
        public static bool IsBarVisible = false;
        
        public static char Hitbox;
        public static char BossHitbox;
        
        public static ConsoleColor CharacterColor;
        
        public static decimal counter = 1.0m;
        public static bool BulletFreeze = false;
        
        public static List<GameObject> Bullets    = new List<GameObject>();
        public static List<GameObject> Characters = new List<GameObject>();
		
        public enum Postacie{
            Reimu,
            Marisa
        }
        public enum ObjectType{
            Player,
            Enemy,
            Boss,
            PlayerBullet,   // Orbs, Arrows
            PlayerBullet2,  // Homing Orbs (soon), Lasers
            EnemyBullet
        }
        public class GameObject
        {
            public (int, int) Position;
            public ObjectType Type;
            public decimal ShotCounter;
            public decimal MoveCounter;
            public int PerishTime;
            public int PerishCount = 1;
            public byte Direction;
            public bool exists = true;
            private int _hp;
            bool IsInvincible = false;
            Func<int> MoveBehavior;
            Func<int> ShotBehavior;
            
            void RemoveInvincibility(object MsTimeout)
            {
                if(!(MsTimeout is int)) throw new ArgumentException();
                Thread.Sleep((int)MsTimeout);
                IsInvincible = false;
            }
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
            public GameObject((int, int) _Position, ObjectType _Type, decimal _ShotCounter, decimal _MoveCounter, int _Health)
            {
                Position = _Position;
                Type = _Type;
                ShotCounter = _ShotCounter;
                MoveCounter = _MoveCounter;
                Hp = _Health;
            }
            public GameObject((int, int) _Position, ObjectType _Type, decimal _MoveCounter, byte _Direction, int Damage, int _PerishTimer = 0)
            {
                Position = _Position;
                Type = _Type;
                MoveCounter = _MoveCounter;
                PerishTime = _PerishTimer;
                Direction = _Direction;
                _hp = Damage;
            }
        }

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
            WriteHorizontal((91, 7),   new string(' ', 4));
            WriteHorizontal((113, 7),  new string(' ', 7));
            WriteHorizontal((91, 8),   new string(' ', 29));
            WriteHorizontal((91, 9),   new string(' ', 29));
            WriteHorizontal((91, 10),  new string(' ', 4));
            WriteHorizontal((113, 10), new string(' ', 7));
            WriteHorizontal((91, 8),   new string(' ', 29));
            WriteHorizontal((91, 11),  new string(' ', 29));
            WriteHorizontal((91, 12),  new string(' ', 4));
            WriteHorizontal((113, 12), new string(' ', 7));
            WriteHorizontal((91, 13),  new string(' ', 29));
            WriteHorizontal((91, 14),  new string(' ', 29));
            WriteHorizontal((91, 15),  new string(' ', 4));
            WriteHorizontal((110, 15), new string(' ', 10));
            WriteHorizontal((91, 16),  new string(' ', 29));
            WriteHorizontal((91, 17),  new string(' ', 4));
            WriteHorizontal((108, 17), new string(' ', 12));
            WriteHorizontal((91, 18),  new string(' ', 29));
            WriteHorizontal((91, 19),  new string(' ', 4));
            WriteHorizontal((108, 19), new string(' ', 12));
            i = 20;
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
            WriteHorizontal((95, 15), "Power:    /4.00", ConsoleColor.DarkRed);
            WriteHorizontal((95, 17), "Graze:        ", ConsoleColor.Green);
            WriteHorizontal((95, 19), "Points:    /   ", ConsoleColor.DarkGray);
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
		static void newBullet(GameObject newObj)
		{
			bool added = false;
            for(int i = 0; i<Bullets.Count; ++i)
                if(!Bullets[i].exists){
					Bullets[i] = newObj;
					added = true;
					break;}
			if(!added)
				Bullets.Add(newObj);
		}
		static void CheckKeys(decimal counter)
        {
            if(counter % Characters[0].ShotCounter != 0) return;

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
                            if(Characters[0].Position.Item1 > 0){ 
                                newBullet(new GameObject((Characters[0].Position.Item1 - 1, Characters[0].Position.Item2 - 1), ObjectType.PlayerBullet, 1m, 7, 1));
                            }
                            newBullet(new GameObject((Characters[0].Position.Item1, Characters[0].Position.Item2 - 1), ObjectType.PlayerBullet, 1m, 8, 1));
                            if(Characters[0].Position.Item1 < 89){
                                newBullet(new GameObject((Characters[0].Position.Item1 + 1, Characters[0].Position.Item2 - 1), ObjectType.PlayerBullet, 1m, 9, 1));
                            }
                            break;
                        case Postacie.Marisa:
                                List<GameObject> lasers = new List<GameObject>();
                                int i = Characters[0].Position.Item2 - 1;
                                if(Characters[0].Position.Item1 != 0){
                                    while(i > (IsBarVisible ? 1 : 0)){
                                        lasers.Add(new GameObject((Characters[0].Position.Item1 - 1, i), ObjectType.PlayerBullet2, 100, 5, 5)); //5 equals none
                                        i--;
                                    }
                                }
                                i = Characters[0].Position.Item2 - 1;
                                if(Characters[0].Position.Item1 != 89){
                                    while(i > (IsBarVisible ? 1 : 0))
                                    {
                                        lasers.Add(new GameObject((Characters[0].Position.Item1 + 1, i), ObjectType.PlayerBullet2, 100, 5, 5)); //5 equals none
                                        i--;
                                    }
                                }
								foreach(GameObject l in lasers)
									if(counter % 30 < 16)
										newBullet(l);
                            newBullet(new GameObject((Characters[0].Position.Item1, Characters[0].Position.Item2 - 1), ObjectType.PlayerBullet, 0.5m, 8, 2));
                            break;
                    }
                }
            }
            if(Keyboard.IsKeyDown(Key.X))
            {

            }
        }
        static void ProcessEnemyMoves(decimal counter)
        {
            foreach(GameObject ch in Characters)
            {
                if(ch.Type != ObjectType.Player && ch.exists)
                {
                    if(counter % ch.ShotCounter == 0 && ch.Position.Item2 + 1 < 49 && !BulletFreeze)
                    {
                        switch(ch.Direction)
                        {
                            case 1:
                                if(ch.Position.Item1 != 0)
                                {
                                    newBullet(new GameObject((ch.Position.Item1 - 1, ch.Position.Item2 - 1), ObjectType.EnemyBullet, 1m, 1, 1));
                                }
                                break;
                            case 4:
                                if(ch.Position.Item1 != 0)
                                {
                                    newBullet(new GameObject((ch.Position.Item1 - 1, ch.Position.Item2), ObjectType.EnemyBullet, 1m, 4, 1));
                                }
                                break;
                            case 2:
                                	newBullet(new GameObject((ch.Position.Item1, ch.Position.Item2 - 1), ObjectType.EnemyBullet, 1m, 2, 1));
                                break;
                            case 3:
                                if(ch.Position.Item1 != 89)
                                { 
                                    newBullet(new GameObject((ch.Position.Item1 + 1, ch.Position.Item2 + 1), ObjectType.EnemyBullet, 1m, 3, 1));
                                }
                                break;
                            case 6:
                                if(ch.Position.Item1 != 89)
                                {
                                    newBullet(new GameObject((ch.Position.Item1 + 1, ch.Position.Item2), ObjectType.EnemyBullet, 1m, 6, 1));
                                }
                                break;
                        }
                    }
                    if(counter % ch.MoveCounter == 0)
                    {
                        // TODO: Implement this
                        // ch.MoveBehavior();
                        if(ch.Position.Item1 < 0 || ch.Position.Item1 > 89 || ch.Position.Item2 < (IsBarVisible ? 1 : 0) || ch.Position.Item2 > 49)
                        {
                            ch.exists = false;
                        }
                    }
                }
            }
        }
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr zeroOnly, string lpWindowName);
        static void MainLoop()
        {
            #region MainLoop Init
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
            Characters.Add(new GameObject((50, 35), ObjectType.Player, 1, 1, DefLives));
            switch(Postać)
            {
                case Postacie.Reimu:
                    DefBombs = CurBombs = 3;
                    Hitbox = 'R';
                    Characters[0].ShotCounter = 1m;
                    CharacterColor = ConsoleColor.Red;
                    break;
                case Postacie.Marisa:
                    DefBombs = CurBombs = 2;
                    Hitbox = 'M';
                    Characters[0].ShotCounter = 0.5m;
                    CharacterColor = ConsoleColor.Yellow;
                    break;
            }
            Thread t = new Thread(new ParameterizedThreadStart(GameProgress));
            t.Start(1);
            #endregion

            // Getting console handle
			IntPtr handle = IntPtr.Zero;
            string originalTitle = Console.Title;
            string uniqueTitle = Guid.NewGuid().ToString();
            Console.Title = uniqueTitle;
            if(Environment.OSVersion.Version.Major != 6 && Environment.OSVersion.Version.Minor != 2)
            	handle = FindWindowByCaption(IntPtr.Zero, uniqueTitle);
            Console.Title = originalTitle;
            
            while(true)
            {
           		if(!(Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 2))
					while(handle!=GetForegroundWindow())Thread.Sleep(33); // Checking focused Window
				
                CheckKeys(counter);
                ProcessEnemyMoves(counter);
                foreach(GameObject ch in Characters)
                {
                    if(ch.Hp <= 0)
                    {
                        ch.exists = false;
                    }
                    if(ch.exists)
                    {
                        WriteHorizontal((ch.Position.Item1, ch.Position.Item2), (ch.Type == ObjectType.Player) ? Hitbox.ToString() : (ch.Type == ObjectType.Boss) ? BossHitbox.ToString() : "E", CharacterColor);
                    }
                }
                foreach(GameObject b in Bullets)
                {
                    if(b.exists)
                    {
                        WriteHorizontal((b.Position.Item1, b.Position.Item2), ((b.Type == ObjectType.PlayerBullet) ? (Postać == Postacie.Reimu) ? '.' : '^' : (b.Type == ObjectType.PlayerBullet2) ? (Postać == Postacie.Reimu) ? 'O': '|' : '#').ToString());
                        b.PerishCount += 1;      
                        if(b.Position.Item2 - 1 > (IsBarVisible ? 1 : 0) && b.exists)
                        {
                            if(b.Type == ObjectType.EnemyBullet)
                            {
                                if(b.Position == Characters[0].Position)
                                {
                                    CurLives -= 1;
									
									BulletFreeze = true;
									Thread unfreeze = new Thread(new ThreadStart(UnfreezeBullets));
									unfreeze.Start();
									
                                    foreach(GameObject b1 in Bullets)
                                        if(b1.Type == ObjectType.EnemyBullet)
                                            b1.exists = false;
									break;
                                }
                                if(b.Position.Item2 != 49 && !BulletFreeze)
                                {
                                    switch (b.Direction)
                                    {
                                        //TODO: Replace this stuff with using the move handler of the bullet
                                        case 1:
                                            if(b.Position.Item1 != 0)
                                            newBullet(new GameObject((b.Position.Item1 - 1, b.Position.Item2 + 1), b.Type, b.MoveCounter, 1, 1));
                                            break;
                                        case 4:
                                            if(b.Position.Item1 != 0)
                                            newBullet(new GameObject((b.Position.Item1 - 1, b.Position.Item2), b.Type, b.MoveCounter, 4, 1));
                                            break;
                                        case 2:
                                            newBullet(new GameObject((b.Position.Item1, b.Position.Item2 + 1), b.Type, b.MoveCounter, 2, 1));
                                            break;
                                        case 3:
                                            if(b.Position.Item1 != 89)
                                            newBullet(new GameObject((b.Position.Item1 + 1, b.Position.Item2 + 1), b.Type, b.MoveCounter, 3, 1));
                                            break;
                                        case 6:
                                            if(b.Position.Item1 != 89)
                                            newBullet(new GameObject((b.Position.Item1 + 1, b.Position.Item2), b.Type, b.MoveCounter, 6, 1));
                                            break;
                                        case 7:
                                            if(b.Position.Item1 != 0)
                                            newBullet(new GameObject((b.Position.Item1 - 1, b.Position.Item2 - 1), b.Type, b.MoveCounter, 7, 1));
                                            break;
                                        case 8:
                                            newBullet(new GameObject((b.Position.Item1, b.Position.Item2 - 1), b.Type, b.MoveCounter, 8, 1));
                                            break;
                                        case 9:
                                            if(b.Position.Item1 != 89)
                                            newBullet(new GameObject((b.Position.Item1 + 1, b.Position.Item2 - 1), b.Type, b.MoveCounter, 9, 1));
                                            break;
                                    }
                                }
                                if(!BulletFreeze && graze < int.MaxValue){
									for(int x = b.Position.Item1-1, xx = 0; xx<3; ++x, ++xx)
										if(x == Characters[0].Position.Item1){
										for(int y = b.Position.Item2-1, yy = 0; yy<3; ++y, ++yy)
											if(y == Characters[0].Position.Item2){
												graze++; //assume that hit check already happened
											break;}
										break;}
                                }
                            }
                            else if(b.Type == ObjectType.PlayerBullet)
                            {
								foreach(GameObject en in Characters.Where((en) => en.Type != ObjectType.Player && en.Position == b.Position))
								{
									en.Hp -= b.Hp;
									goto afterif;
								}
                                if(Postać == Postacie.Reimu)
                                {
                                    newBullet(new GameObject((b.Position.Item1, b.Position.Item2 - 1), ObjectType.PlayerBullet, b.MoveCounter, 8, 2));
                                }
                                else if(Postać == Postacie.Marisa)
                                {
                                    newBullet(new GameObject((b.Position.Item1, b.Position.Item2 - 1), ObjectType.PlayerBullet, b.MoveCounter, 8, 2));
                                }
                            }
                            else if(b.Type == ObjectType.PlayerBullet2)
                            {
								foreach(GameObject en in Characters.Where((en) => en.Type != ObjectType.Player && en.Position == b.Position))
								{
									en.Hp -= b.Hp;
									goto afterif;
								}
								//if()//shouldn't bullets have different speeds?
                                if(Postać == Postacie.Reimu)
                                {
									int clx = 0;//x dir of closest
									int cly = 0;//y dir of closest
									float cld = 99999999;//a lot of fake distance
									foreach(GameObject en in Characters.Where((en) => en.Type != ObjectType.Player))
									{
										int dx = en.Position.Item1-b.Position.Item1;//distance by y
										int dy = en.Position.Item2-b.Position.Item2;//distance by x
										float d = (float)Math.Sqrt(dx*dx+dy*dy);//actual distance
										if(d<cld){
											cld = d;
											clx = dx;
											cly = dy;
										}
									}
									b.Position.Item1+= Math.Sign(clx);
									b.Position.Item2+= Math.Sign(cly);
                                    //TODO: Homing shot here, soon™️
                                }
                                else if(Postać == Postacie.Marisa)
                                {
                                    // No need for implementation here as laser is weird
                                }
                            }
                        }
						afterif:
                        if(b.PerishCount >= b.PerishTime)
							b.exists = false;
                    }
                }
				foreach(GameObject en in Characters.Where((en) => !en.exists && en.Type != ObjectType.Player).ToArray())
                {
                    Characters.Remove(en);
                    score += 150; //Once there are more enemy types, distinctions will be made and they will award different scores.
                }
                if(CurLives == 0)
                {
                    ClearGameSpace();
                }
#if DEBUG
                WriteHorizontal((Bombs.Item1 - 7, Bombs.Item2 + 23), $"Bullets: {Bullets.Count}");
                WriteHorizontal((Bombs.Item1 - 7, Bombs.Item2 + 26), $"handle: {handle}");
                WriteHorizontal((Bombs.Item1 - 7, Bombs.Item2 + 27), $"GFW(): {GFW}");
#endif
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                WriteHorizontal(Score, new string('0', 11 - score.ToString().Length) + score.ToString());
                WriteHorizontal(Lives, new string('*', CurLives - 1) + new string(' ', 11 - CurLives), ConsoleColor.DarkRed);
                WriteHorizontal(Bombs, new string('*', CurBombs), ConsoleColor.DarkBlue);
                WriteHorizontal(Graze, graze.ToString(), ConsoleColor.Green);
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
			GameObject en1 = new GameObject((70, 10), ObjectType.Enemy, 5m, 5m, 5)
			{
				Direction = 1
			};
			GameObject en2 = new GameObject((45, 5), ObjectType.Enemy, 5m, 5m, 5)
			{
				Direction = 2
			};
			GameObject en3 = new GameObject((20, 10), ObjectType.Enemy, 3m, 5m, 5)
			{
				Direction = 3
			};
			GameObject en4 = new GameObject((10, 30), ObjectType.Enemy, 5m, 5m, 5)
			{
				Direction = 6
			};
			GameObject en5 = new GameObject((80, 35), ObjectType.Enemy, 3m, 5m, 5)
			{
				Direction = 4
			};
			GameObject en6 = new GameObject((10, 33), ObjectType.Enemy, 5m, 5m, 5)
			{
				Direction = 6
			};
			GameObject en7 = new GameObject((47, 5), ObjectType.Enemy, 5m, 5m, 5)
			{
				Direction = 2
			};
			Characters.Add(en1);
			Characters.Add(en2);
			Characters.Add(en3);
			Characters.Add(en4);
			Characters.Add(en5);
			Characters.Add(en6);
			Characters.Add(en7);
        }
        static void UnfreezeBullets()
        {
            Thread.Sleep(1500);
            BulletFreeze = false;
        }
    }
}
