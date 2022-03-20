using System;

namespace MenuSystem
{
    public static class ConsoleEx
    {
        public static void ClearAt(int lineIndex)
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, lineIndex);
            Console.Write(new string(' ', Console.WindowWidth)); 
            Console.SetCursorPosition(0, currentLineCursor);
        }
        
        public static void WriteTo(int lineIndex, string toWrite)
        {
            var currentLineCursor = Console.CursorTop;

            Console.SetCursorPosition(0, lineIndex);
            Console.WriteLine(toWrite); 
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}