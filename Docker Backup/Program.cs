var backupDirectory = new DirectoryInfo(Settings.Default.BackupDestinationDirectory);

var wantToFullBackup = Settings.Default.WantToFullBackup;
// todo: apply basic code best practices

if (!backupDirectory.Exists)
  backupDirectory.Create();

if (wantToFullBackup)
    await BackupService.RunBackupAsync(backupDirectory);
