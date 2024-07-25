using Services;

namespace DockerBackup.Services;

internal static class BackupService {
  public static async Task RunBackupAsync() {
    Logger.PrintAndLog(Constants.TX_BACKUP_START);
    CheckBackupDestination();
    var containers = await DockerService.GetContainerListAsync();
    foreach (var container in containers) {
      var parts = container.Split(' ');
      var containerId = parts[0];
      var imageName = parts[1];
      await BackupContainerAsync(containerId, imageName);
    }

    await CreateSingleBackupAsync();
  }

  private static async Task BackupContainerAsync(string containerId, string imageName) {
    var containerBackup = new FileInfo(
      Path.Combine(Constants.TEMP_DIR, "container_" + containerId + "container_backup.tar")
    );

    if (containerBackup.Directory != null)
      Directory.CreateDirectory(containerBackup.Directory.ToString());

    await DockerService.ExportContainerAsync(containerId, containerBackup.FullName);
    await DockerService.SaveImageAsync(imageName, containerBackup.FullName);
  }

  private static string GetTime() => DateTime.Now.ToString("yyyyMMddHHmmss");

  private static async Task CreateSingleBackupAsync() {
    var timestamp = GetTime();
    var backupFilename = $"docker_backup_{timestamp}.tar.gz";
    var backupFile = new FileInfo(Path.Combine(Constants.BACKUP_DESTINATION_DIR, backupFilename));
    Logger.PrintAndLog(Constants.TX_CREATING_SINGLE_TAR);
    var tarCmd = $"tar -czf {backupFile.FullName} -C {Constants.TEMP_DIR} .";
    await StartProcessService.ExecuteCommand(tarCmd);

    Logger.PrintAndLog($"{Constants.TX_FINISHED_BACKUP} {backupFile.FullName}");
    Directory.Delete(Constants.TEMP_DIR, true);
  }

  private static void CheckBackupDestination() {
    if (!Directory.Exists(Constants.BACKUP_DESTINATION_DIR))
      throw new InvalidOperationException(
        $"Backup destination {Constants.BACKUP_DESTINATION_DIR} does not exist or is not writable."
      );

    Logger.PrintAndLog($"[Info] Backup destination {Constants.BACKUP_DESTINATION_DIR} is ready.");
  }
}
