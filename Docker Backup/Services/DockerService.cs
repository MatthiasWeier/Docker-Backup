using GoogleApi.Entities.Translate.Translate.Request.Enums;

namespace Services;

internal class DockerService {
  // todo: initialize this with StartProcessService - need to create overload 
  public static async Task<string[]> GetContainerListAsync() {
    var output = await StartProcessService.ExecuteCommand("docker","ps -a --format \"{{.ID}} {{.Image}}\"");
    return output.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
  }

  public static async Task SaveImageAsync(string imageName, FileInfo saveContainer) {
    var saveCmd = $"save -o {saveContainer.FullName} {imageName}";
    await StartProcessService.ExecuteCommand("docker", saveCmd);
  }
}
