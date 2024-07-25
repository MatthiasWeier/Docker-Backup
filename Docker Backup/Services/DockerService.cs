using System;
using System.Diagnostics;

namespace DockerBackup.Services;

public class DockerService {
  public string[] GetContainerList() {
    var process = new Process {
      StartInfo = new() {
        FileName = "docker",
        Arguments = "ps -a --format \"{{.ID}} {{.Image}}\"",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
      }
    };
    process.Start();
    var output = process.StandardOutput.ReadToEnd();
    process.WaitForExit();
    return output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
  }

  public void ExportContainer(string containerId, string exportPath) {
    var exportCmd = $"docker export -o {exportPath} {containerId}";
    this.ExecuteCommand(exportCmd, $"Failed to export container {containerId}");
  }

  public void SaveImage(string imageName, string savePath) {
    var saveCmd = $"docker save -o {savePath} {imageName}";
    this.ExecuteCommand(saveCmd, $"Failed to save image {imageName}");
  }

  private void ExecuteCommand(string command, string errorMessage) {
    var process = new Process {
      StartInfo = new() {
        FileName = "cmd.exe",
        Arguments = $"/c \"{command}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
      }
    };
    process.Start();
    _ = process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    process.WaitForExit();
    if (process.ExitCode != 0) throw new InvalidOperationException($"{errorMessage}: {error}");
  }
}
