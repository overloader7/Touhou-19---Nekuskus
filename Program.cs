using System;
using System.Collections.Generic;

namespace Touhou_19___Nekuskus
{
    class Program
    {
        public static (int, int) Score = (218, 7);
        public static (int, int) Lives = (218, 10);
        public static (int, int) Bombs = (218, 12);
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
            Console.WriteLine("Test");
            WriteVertical((25, 0), new string('|', 75));
            Console.ReadKey(true);
            ClearGameSpace();
            Console.ReadKey(true);
        }
        static void MainLoop()
        {

        }
    }
}
