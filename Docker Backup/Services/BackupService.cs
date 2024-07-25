using System;
using System.Diagnostics;
using System.IO;
using Docker_Backup.Utils;
using DockerBackup.Utils;

namespace DockerBackup.Services;

public class BackupService {
  private readonly DockerService _dockerService = new();

  public void RunBackup() {
    Logger.PrintAndLog(Constants.TX_BACKUP_START);
    this.CheckBackupDestination();
    var containers = this._dockerService.GetContainerList();
    foreach (var container in containers) {
      var parts = container.Split(' ');
      var containerId = parts[0];
      var imageName = parts[1];
      this.BackupContainer(containerId, imageName);
    }

    this.CreateSingleBackup();
  }

  private void BackupContainer(string containerId, string imageName) {
    var containerBackupPath = Path.Combine(Constants.TEMP_DIR, "container_" + containerId);
    Directory.CreateDirectory(containerBackupPath);

    this._dockerService.ExportContainer(containerId, Path.Combine(containerBackupPath, "container_backup.tar"));
    this._dockerService.SaveImage(imageName, Path.Combine(containerBackupPath, "image_backup.tar"));
  }

  private string GetTime() => DateTime.Now.ToString("yyyyMMddHHmmss");

  private void CreateSingleBackup() {
    var timestamp = this.GetTime();
    var backupFilename = $"docker_backup_{timestamp}.tar.gz";
    var backupPath = Path.Combine(Constants.BACKUP_DESTINATION_DIR, backupFilename);
    Logger.PrintAndLog(Constants.TX_CREATING_SINGLE_TAR);
    var tarCmd = $"tar -czf {backupPath} -C {Constants.TEMP_DIR} .";
    this.ExecuteCommand(tarCmd, $"Failed to create backup {backupPath}");

    Logger.PrintAndLog($"{Constants.TX_FINISHED_BACKUP} {backupPath}");
    Directory.Delete(Constants.TEMP_DIR, true);
  }

  private void CheckBackupDestination() {
    if (!Directory.Exists(Constants.BACKUP_DESTINATION_DIR))
      throw new InvalidOperationException(
        $"Backup destination {Constants.BACKUP_DESTINATION_DIR} does not exist or is not writable."
      );

    Logger.PrintAndLog($"[Info] Backup destination {Constants.BACKUP_DESTINATION_DIR} is ready.");
  }

  private void ExecuteCommand(string command, string errorMessage) {
    var process = new Process {
      StartInfo = new() {
        FileName = "cmd.exe",
        Arguments = $"/c \"{command}\"",
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
      }
    };
    process.Start();
    process.StandardOutput.ReadToEnd();
    var error = process.StandardError.ReadToEnd();
    process.WaitForExit();
    if (process.ExitCode != 0) throw new InvalidOperationException($"{errorMessage}: {error}");
  }
}
