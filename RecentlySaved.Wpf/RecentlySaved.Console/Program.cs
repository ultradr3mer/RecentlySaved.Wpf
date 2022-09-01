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

      [Option('p', "paths", Required = true, HelpText = "Patths to be watched.")]
      public IEnumerable<string> Paths { get; set; }

    }


    static void Main(string[] args)
    {
      CommandLine.Parser.Default.ParseArguments<Options>(args).
        WithParsed(RunOptions).
        WithNotParsed(HandleParseError);

      Console.Pause();

      watcher.Dispose();
    }

    private static void HandleParseError(IEnumerable<Error> obj)
    {
      throw new System.NotImplementedException();
    }

    private static void RunOptions(Options obj)
    {
      Program.watcher = new FileWatcher(obj.Paths);
    }
  }
}
