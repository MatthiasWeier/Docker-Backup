namespace DockerBackup.Utils;

internal static class Logger {
  public static void PrintAndLog(string message) {
    var line = $"{DateTime.Now}:\t{message}";
    Console.WriteLine(line);
    File.AppendAllText(Constants.LOG_FILE, line + Environment.NewLine);
  }
}