using System;

namespace Flazzy.Sandbox.Utilities
{
    public static class ConsoleEx
    {
        public static void WriteLineFinished()
        {
            Console.Write(" | ");

            ConsoleColor oldClr = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Finished!");

            Console.ForegroundColor = oldClr;
        }
        public static void WriteLineTitle(string value)
        {
            Console.WriteLine("========== {0} ==========", value);
        }
        public static void WriteLineResult(this bool value)
        {
            Console.Write(" | ");

            ConsoleColor oldClr = Console.ForegroundColor;

            Console.ForegroundColor = (value ? ConsoleColor.Green : ConsoleColor.Red);
            Console.WriteLine(value ? "Success!" : "Failed!");

            Console.ForegroundColor = oldClr;
        }
        public static void WriteLineChanged(string title, object previous, object current)
        {
            Console.Write(title + ": ");

            ConsoleColor oldClr = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(previous);

            Console.ForegroundColor = oldClr;
            Console.Write(" > ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(current);

            Console.ForegroundColor = oldClr;
        }
    }
}