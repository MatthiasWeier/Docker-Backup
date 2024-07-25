using Services;

namespace DockerBackup.Services;

internal class DockerService {
  // todo: initialize this with StartProcessService
  public static async Task<string[]> GetContainerListAsync() {
    var process = new Process {
      StartInfo = new ProcessStartInfo {
        FileName = "docker",
        Arguments = "ps -a --format \"{{.ID}} {{.Image}}\"",
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
      }
    };
    process.Start();
    var output = await process.StandardOutput.ReadToEndAsync();
    process.WaitForExit();
    return output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
  }

  public static async Task ExportContainerAsync(string containerId, string exportPath) {
    var exportCmd = $"docker export -o {exportPath} {containerId}";
    await ExecuteCommandAsync(exportCmd);
  }

  public static async Task SaveImageAsync(string imageName, string savePath) {
    var saveCmd = $"docker save -o {savePath} {imageName}";
    await ExecuteCommandAsync(saveCmd);
  }


  private static async Task ExecuteCommandAsync(string command) {
    StartProcessService.ExecuteCommand($"cmd.exe {command}");
  }
}
