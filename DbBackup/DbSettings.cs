namespace DbBackup
{
    internal class DbSettings
    {
        public static readonly string SectionName = "DbBackup";

        public static readonly string ConnectionStringName = "DbConnStr";

        public string DbName { get; set; }

        public string ExportDir { get; set; }
    }
}