namespace DockerBackup.Utils;

internal static class Constants {
  public const string BACKUP_DESTINATION_DIR = @"F:\testbackup";
  public const string LOG_FILE = @"C:\tmp\temp.log";
  public const string DOCKER_PATH = @"/var/lib/docker";
  public const string TEMP_DIR = @"C:\temp_backup_dir";

  public const string TX_BACKUP_START = "[Info] The Backup started! ";
  public const string TX_FINISHED_BACKUP = "[Info] Backup is done! ";
  public const string TX_FAILED_BACKUP = "[Info] Failed to create backup ";
  public const string TX_CREATING_SINGLE_TAR = "[Info] Creating single tar for all containers ";
}
