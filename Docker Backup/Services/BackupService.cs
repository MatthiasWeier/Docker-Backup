using Services;

namespace DockerBackup.Services;

internal static class BackupService {
  
  public static async Task RunBackupAsync(DirectoryInfo backupDirectory) {
    ArgumentNullException.ThrowIfNull(backupDirectory);
    Logger.PrintAndLog(Constants.TX_BACKUP_START);
    var containers = await DockerService.GetContainerListAsync();
    foreach (var container in containers) {
      var parts = container.Split(' ');
      var containerId = parts[0];
      var imageName = parts[1];
      await BackupContainerAsync(containerId, imageName, backupDirectory);
    }

    await CreateSingleBackupAsync(backupDirectory);
  }

  private static async Task BackupContainerAsync(string containerId, string imageName, DirectoryInfo backupDirectory) {
    var containerBackup = new FileInfo(
      Path.Combine(backupDirectory.FullName, "container_" + containerId + "container_backup.tar")
    );

    

    ArgumentNullException.ThrowIfNull(containerBackup);

    await DockerService.ExportContainerAsync(containerId, containerBackup);

    var imageBackup = new FileInfo(
      Path.Combine(backupDirectory.FullName, "container_" + containerId + "image_backup.tar")
    );

    ArgumentNullException.ThrowIfNull(imageBackup);

    await DockerService.SaveImageAsync(imageName, imageBackup);
  }

  private static string GetTime() => DateTime.Now.ToString("yyyyMMddHHmm");

  private static async Task CreateSingleBackupAsync(DirectoryInfo backupDirectory) {
    var timestamp = GetTime();
    var backupFile = new FileInfo(Path.Combine(backupDirectory.FullName, $"docker_backup_{timestamp}.tar.gz"));
    var tempDir = backupDirectory.CreateSubdirectory("temp_backup");

    Logger.PrintAndLog(Constants.TX_CREATING_SINGLE_TAR);
    var tarCmd = $"-czf {backupFile.FullName} -C {tempDir.FullName} .";
    await StartProcessService.ExecuteCommand("tar",tarCmd);

    Logger.PrintAndLog($"{Constants.TX_FINISHED_BACKUP} {backupFile.FullName}");
    Directory.Delete(tempDir.FullName, true);
  }
}
