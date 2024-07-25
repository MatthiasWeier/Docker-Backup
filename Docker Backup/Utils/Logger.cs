using System;
using System.IO;
using DockerBackup.Utils;

namespace Docker_Backup.Utils;

public static class Logger {
  public static void PrintAndLog(string message) {
    var line = $"{DateTime.Now}:\t{message}";
    Console.WriteLine(line);
    File.AppendAllText(Constants.LOG_FILE, line + Environment.NewLine);
  }
}
