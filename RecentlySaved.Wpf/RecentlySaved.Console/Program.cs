using RecentlySaved.Console;

Options options = CommandLine.Parser.Default.ParseArguments<Options>(args).Value;
LinkService linkService = new LinkService(options.TargetPath);
FileWatcher watcher = new FileWatcher(linkService, options.Paths);
System.Console.WriteLine("Press enter to exit.");
System.Console.ReadLine();
watcher.Dispose();
