namespace DockerBackup.Utils;

internal static class Logger {
  private static readonly DirectoryInfo _LOG_DIR = new(Settings.Default.BackupDestinationDirectory);
  private static readonly FileInfo _LOG_FILE = new(Path.Combine($"{Logger._LOG_DIR.FullName}\\temp.log"));
  public static void PrintAndLog(string message) {
    var line = $"{DateTime.Now}:\t{message}";
    Console.WriteLine(line);
    if(!Logger._LOG_FILE.Exists)
      Logger._LOG_FILE.Create();

    File.AppendAllText(Logger._LOG_FILE.FullName, line + Environment.NewLine);
  }
}