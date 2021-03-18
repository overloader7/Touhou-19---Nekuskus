﻿using System;
using System.Linq;
using System.Collections.Generic;
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
        
        
        public static (int, int) Score = (218, 7);
        public static (int, int) Lives = (218, 10);
        public static (int, int) Bombs = (218, 12);
        public static Postacie Postać;
        public static bool IsBarVisible = false;
        public static int DefLives;
        public static int DefBombs;
        public static int CurLives;
        public static int CurBombs;
        public static char Hitbox;

        public enum ObjectType
        {
            Player,
            Enemy,
            PlayerBullet,
            EnemyBullet
        }
        public class GameObject
        {
            public (int, int) Position;
            public ObjectType Type;
            public int MoveCounter;

            public GameObject((int, int) _Position, ObjectType _Type, int _MoveCounter)
            {
                Position = _Position;
                Type = _Type;
                MoveCounter = _MoveCounter;
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
            Console.SetCursorPosition(0, 0);
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
                if(coords.Item2 == 75) return;
                Console.SetCursorPosition(coords.Item1, coords.Item2);
            }
            Console.ForegroundColor = curcolor;
            Console.SetCursorPosition(0, 0);
        }
        static void RenderBackgroundColors()
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            int i = 0;
            while(i < 7)
            {
                WriteHorizontal((201, i), new string(' ', 39));
                i++;
            }
            WriteHorizontal((201, 7), new string(' ', 9));
            WriteHorizontal((228, 7), new string(' ', 12));
            WriteHorizontal((201, 8), new string(' ', 39));
            WriteHorizontal((201, 9), new string(' ', 39));
            WriteHorizontal((201, 10), new string(' ', 9));
            WriteHorizontal((228, 10), new string(' ', 12));
            WriteHorizontal((201, 8), new string(' ', 39));
            WriteHorizontal((201, 11), new string(' ', 39));
            WriteHorizontal((201, 12), new string(' ', 9));
            WriteHorizontal((228, 12), new string(' ', 12));
            i = 13;
            while(i < 74)
            {
                WriteHorizontal((201, i), new string(' ', 39));
                i++;
            }
            WriteHorizontal((201, 74), new string(' ', 38));
            Console.BackgroundColor = ConsoleColor.Black;   
        }
        static void InitUI()
        {
            Console.SetCursorPosition(0, 0);
            Console.SetWindowPosition(0, 0);
            Console.Write(new string(' ', 18000));
            Console.SetWindowSize(240, 75);
            Console.SetWindowPosition(0, 0);
            Console.SetBufferSize(240, 75);
            Console.SetWindowPosition(0, 0);
            WriteVertical((200, 0), new string('|', 75));
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            WriteHorizontal((210, 7), "Score: 00000000000");
            WriteHorizontal((210, 10), "Lives:            ", ConsoleColor.DarkRed);
            WriteHorizontal((210, 12), "Bombs:            ", ConsoleColor.DarkBlue);
            Console.BackgroundColor = ConsoleColor.Black;
            RenderBackgroundColors();
            Console.SetCursorPosition(0, 0);
        }
        static void ClearGameSpace()
        {
            int i = 0;
            while (i < 75)
            {
                WriteHorizontal((0, i), new string(' ', 200));
                i++;
            }
            Console.SetCursorPosition(0, 0);
        }
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            InitUI();
            WriteHorizontal((90, 10), "Choose your character!");
            WriteHorizontal((97, 13), "Reimu", ConsoleColor.Red);
            
            wyborpostaci:
            Console.SetCursorPosition(97, 15);
            string postać = Console.ReadLine();
            if(postać == "R" || postać == "Reimu")
            {
                Postać = Postacie.Reimu;
                MainLoop();
            }
            else
            {
                WriteHorizontal((0, 15), new string(' ', 199));
                goto wyborpostaci;
            }
            Console.ReadKey();
        }
        static void CheckKeys(int counter)
        {
            if(counter % Characters[0].MoveCounter != 0) return;
            if(Keyboard.IsKeyDown(Key.Right))
            {
                if(!(Characters[0].Position.Item1 == 199))
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
                if(!(Characters[0].Position.Item2 == 74))
                {
                    Characters[0].Position.Item2 += 1;
                }
            }
        }
        static void MainLoop()
        {
            int counter = 1;
            Console.WriteLine("Started MainLoop()!");
            ClearGameSpace();
            DefLives = CurLives = 5;
            Characters.Add(new GameObject((100, 55), ObjectType.Player, 1));
            switch(Postać)
            {
                case Postacie.Reimu:
                    DefBombs = CurBombs = 3;
                    Hitbox = 'R';
                    Characters[0].MoveCounter = 2;
                    break;
                case Postacie.Marisa:
                    DefBombs = CurBombs = 2;
                    Hitbox = 'M';
                    Characters[0].MoveCounter = 1;
                    throw new NotImplementedException();
                    break;
            }
            while(true)
            {
                if(counter == 9)
                {
                    counter = 1;
                }
                CheckKeys(counter);
                foreach(GameObject ch in Characters)
                {
                    WriteHorizontal((ch.Position.Item1, ch.Position.Item2), Hitbox.ToString(), ConsoleColor.Red);
                }
                counter++;
                Thread.Sleep(16);
                ClearGameSpace();
            }
        }
    }
}
