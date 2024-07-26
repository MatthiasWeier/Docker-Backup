namespace Services;

internal class DockerService {
  public static async Task<string[]> GetContainerListAsync() {
    var output = await StartProcessService.ExecuteCommand("docker","ps -a --format \"{{.ID}} {{.Image}}\"");
    return output.Lines(StringSplitOptions.RemoveEmptyEntries);
  }

  public static async Task SaveImageAsync(string imageName, FileInfo saveContainer) {
    var saveCmd = $"save -o {saveContainer.FullName} {imageName}";
    await StartProcessService.ExecuteCommand("docker", saveCmd);
  }
}
