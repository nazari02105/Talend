namespace ETLLibrary.Database.Utils
{
    public static class CsvConfigurator
    {
        public const string Path = "csvFiles";

        public static string GetFilePath(string username, string fileName)
        {
            return Path + "/" + username + "/" + fileName;
        }

        public static string GetUserDirectoryPath(string username)
        {
            return Path + "/" + username;
        }
    }
}