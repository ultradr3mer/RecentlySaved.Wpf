using System;

namespace RecentlySaved.Console
{
  internal class Console
  {
    internal static void WriteLine(string v)
    {
      System.Console.WriteLine(v);
    }

    internal static void ReadLine()
    {
      System.Console.WriteLine();
    }

    internal static void WriteLine()
    {
      System.Console.WriteLine();
    }

    internal static void Pause()
    {
      System.Console.WriteLine("Hello, Press any key to progress forward");
      System.Console.ReadKey();
    }
  }
}
