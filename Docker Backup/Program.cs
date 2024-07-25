// todo: apply basic code best practices
var wantToFullBackup = Settings.Default.WantToFullBackup;
if(wantToFullBackup)
    await BackupService.RunBackupAsync();
