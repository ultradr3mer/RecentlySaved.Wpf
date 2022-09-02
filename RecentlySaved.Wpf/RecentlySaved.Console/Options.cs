using CommandLine;

namespace RecentlySaved.Console
{
  class Options
  {

    [Option('p', "paths", Separator = ',', Required = true, HelpText = "Paths to be watched.")]
    public IEnumerable<string> Paths { get; set; } = new List<string>();

    [Option('t', "target", Required = true, HelpText = "Path for the replicated links.")]
    public string TargetPath { get; set; } = string.Empty;

  }
}
