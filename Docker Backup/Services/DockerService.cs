using GoogleApi.Entities.Translate.Translate.Request.Enums;

namespace Services;

internal class DockerService {
  // todo: initialize this with StartProcessService - need to create overload 
  public static async Task<string[]> GetContainerListAsync() {
    // await StartProcessService.ExecuteCommand("docker",$"ps - a--format \\\"{{.ID}} {{.Image}}\\\"");
    
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

  public static async Task SaveImageAsync(string imageName, FileInfo saveContainer) {
    var saveCmd = $"save -o {saveContainer.FullName} {imageName}";
    await StartProcessService.ExecuteCommand("docker", saveCmd);
  }
}
