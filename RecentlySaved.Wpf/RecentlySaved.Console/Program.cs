using CommandLine;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace RecentlySaved.Console
{
  internal class Program
  {
    private static FileWatcher watcher;

    class Options
    {

      [Option('p', "paths", Separator = ',', Required = true, HelpText = "Patths to be watched.")]
      public IEnumerable<string> Paths { get; set; }

    }

    static void Main(string[] args)
    {
      Options options = CommandLine.Parser.Default.ParseArguments<Options>(args).Value;
      watcher = new FileWatcher(options.Paths);

      Console.Pause();

      watcher.Dispose();
    }
  }
}
