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
    await process.WaitForExitAsync();
    return output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
  }

  public static async Task ExportContainerAsync(string containerId, FileInfo exportPath) {
    var exportCmd = $"export -o {exportPath} {containerId}";
    await StartProcessService.ExecuteCommand("docker", exportCmd);
  }

  public static async Task SaveImageAsync(string imageName, FileInfo saveContainer) {
    var saveCmd = $"docker save -o {saveContainer.FullName} {imageName}";
    await StartProcessService.ExecuteCommand("docker", saveCmd);
  }
}
