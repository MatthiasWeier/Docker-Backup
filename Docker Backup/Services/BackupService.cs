namespace Services;

internal static class BackupService {
  private const string _TIME_FORMAT = "yyyyMMdd";
  private const string _IMAGE_FILE_NAME = "image-backup.tar";

  public static async Task RunBackupAsync(DirectoryInfo backupDirectory) {
    ArgumentNullException.ThrowIfNull(backupDirectory);
    Logger.PrintAndLog(Constants.TX_BACKUP_START);
    var containers = await DockerService.GetContainerListAsync();
    foreach (var container in containers) {
      var parts = container.Split(' ');
      var containerId = parts[0];
      var imageName = parts[1];
      Logger.PrintAndLog($"[INFO] Backing up {imageName}");
      await BackupContainerAsync(containerId, imageName, backupDirectory);
    }
  }

  private static async Task BackupContainerAsync(string containerId, string imageName, DirectoryInfo backupDirectory) {
    var timestamp = GetCurrentTimestamp();

    var imageBackup = backupDirectory.File($"{timestamp}-{containerId}-{BackupService._IMAGE_FILE_NAME}");

    await DockerService.SaveImageAsync(imageName, imageBackup);
  }

  private static string GetCurrentTimestamp() => DateTime.Now.ToString(BackupService._TIME_FORMAT);
}
