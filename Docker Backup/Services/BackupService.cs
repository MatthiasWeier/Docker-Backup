using System.ComponentModel.DataAnnotations;

namespace Services;

internal static class BackupService {
  
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
    var containerBackup = new FileInfo(
      Path.Combine($"{backupDirectory.FullName}\\{timestamp}-{containerId}container-backup.tar")
    );
    ArgumentNullException.ThrowIfNull(containerBackup);

    var imageBackup = new FileInfo(
      Path.Combine($"{backupDirectory.FullName}\\{timestamp}-{containerId}-image-backup.tar")
    );

    ArgumentNullException.ThrowIfNull(imageBackup);

    await DockerService.SaveImageAsync(imageName, imageBackup);
  }

  private static string GetCurrentTimestamp() => DateTime.Now.ToString("yyyyMMdd");
}
