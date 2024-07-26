namespace DockerBackup.Utils;

internal static class Logger {
  private static readonly DirectoryInfo _LOG_DIR = new(Settings.Default.BackupDestinationDirectory);
  private static readonly FileInfo _LOG_FILE = Logger._LOG_DIR.File("temp.log");

  public static void PrintAndLog(string message) {
    var line = $"{DateTime.UtcNow}:\t{message}";
    Console.WriteLine(line);

    Logger._LOG_FILE.AppendLine(line);
  }
}