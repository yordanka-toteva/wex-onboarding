using System.IO;

namespace wex_onboarding.Operations
{
    public static class FileCommon
    {
        public static bool CheckFileExists(string pathToCreatedFile)
        {
            FileInfo fileInfo = new FileInfo(pathToCreatedFile);

            if (fileInfo.Exists)
            {
                if (fileInfo.Length > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static void DeleteFileAndDirectory(string directoryName)
        {
            if (Directory.Exists(directoryName))
            {
                Directory.Delete(directoryName, true);
            }
        }
    }
}
