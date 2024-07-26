namespace Services;

internal static class BackupService {
  private const string _TIME_FORMAT = "yyyyMMdd";
  private const string _IMAGE_FILE_NAME = "image-backup.tar";

  public static async Task RunBackupAsync(DirectoryInfo backupDirectory) {
    ArgumentNullException.ThrowIfNull(backupDirectory);
    Logger.PrintAndLog(Constants.TX_BACKUP_START);
    var containers = await DockerService.GetContainerListAsync();
    foreach (var container in containers) {
      var firstSpaceIndex = container.IndexOf(Constants.SPACE_DELIMITER, StringComparison.Ordinal);

      if (firstSpaceIndex != -1) {
        var containerId = container[..firstSpaceIndex];
        var imageName = container[(firstSpaceIndex + 1)..];

        Logger.PrintAndLog($"[INFO] Backing up {imageName}");
        await BackupContainerAsync(containerId, imageName, backupDirectory);
      } else
        Logger.PrintAndLog($"[ERROR] Invalid container format: {container}");

    }
  }

  private static async Task BackupContainerAsync(string containerId, string imageName, DirectoryInfo backupDirectory) {
    var timestamp = GetCurrentTimestamp();

    var imageBackup = backupDirectory.File($"{timestamp}-{containerId}-{BackupService._IMAGE_FILE_NAME}");

    await DockerService.SaveImageAsync(imageName, imageBackup);
  }

  private static string GetCurrentTimestamp() => DateTime.Now.ToString(BackupService._TIME_FORMAT);
}
