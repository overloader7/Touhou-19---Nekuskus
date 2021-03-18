using System;
using System.Collections.Generic;

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
        static void WriteHorizontal(ValueTuple<int, int> coords, string text) //1 is col, 2 is row
        {
            (int, int) cursorpos = (Console.CursorLeft, Console.CursorTop);
            if(text.Length + coords.Item1 > Console.WindowWidth) throw new ArgumentOutOfRangeException();
            Console.SetCursorPosition(coords.Item1, coords.Item2);
            Console.Write(text);
            Console.SetCursorPosition(cursorpos.Item1, cursorpos.Item2);
        }
        static void WriteVertical(ValueTuple<int, int> coords, string text) //1 is col, 2 is row
        {
            (int, int) cursorpos = (Console.CursorLeft, Console.CursorTop);
            if(text.Length + coords.Item2 > Console.WindowHeight) throw new ArgumentOutOfRangeException();
            Console.SetCursorPosition(coords.Item1, coords.Item2);
            foreach(char c in text)
            {
                Console.Write(c);
                coords.Item2 = coords.Item2 + 1;
                if(coords.Item2 == 75) return;
                Console.SetCursorPosition(coords.Item1, coords.Item2);
            }
            Console.SetCursorPosition(cursorpos.Item1, cursorpos.Item2);
        }
        static void InitUI()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(new string(' ', 18000));
            Console.SetBufferSize(240, 75);
            Console.SetWindowSize(240, 75);
            Console.SetWindowPosition(0, 0);
            WriteVertical((200, 0), new string('|', 75));
            WriteHorizontal((210, 7), "Score: 00000000000");
            WriteHorizontal((210, 10), "Lives: ");
            WriteHorizontal((210, 12), "Bombs: ");
            Console.SetCursorPosition(0, 0);
        }
        static void ClearGameSpace()
        {
            int i = 0;
            while (i < 75)
            {
                WriteHorizontal((0, i), new string(' ', 199));
                i++;
            }
            Console.SetCursorPosition(0, 0);
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            InitUI();
            WriteHorizontal((90, 10), "Choose your character!");
            WriteHorizontal((97, 13), "Reimu");
            
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
        static void MainLoop()
        {
            
            Console.WriteLine("Started MainLoop()!");
            ClearGameSpace();
            DefLives = CurLives = 5;
            switch(Postać)
            {
                case Postacie.Reimu:
                    DefBombs = CurBombs = 3;
                    Hitbox = 'R';
                    break;
                case Postacie.Marisa:
                    DefBombs = CurBombs = 2;
                    Hitbox = 'M';
                    throw new NotImplementedException();
                    break;
            }        
        }
    }
}
