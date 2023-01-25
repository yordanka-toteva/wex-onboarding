using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace wex_onboarding.Operations
{
    public static class ZipFileExtract
    {
        public static string ExtractAndCopyFileFromZIP(string srcFile, string destFolder)
        {
            string pathToExtractedFile = String.Empty;
            using (ZipArchive zip = ZipFile.Open(srcFile, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    string fileName = entry.Name;
                    pathToExtractedFile = Path.Combine(destFolder, fileName);
                    var directoryName = Path.GetDirectoryName(pathToExtractedFile);

                    if (directoryName != null) Directory.CreateDirectory(directoryName);

                    if (!fileName.Equals("[Content_Types].xml"))
                    {
                        entry.ExtractToFile(pathToExtractedFile);
                        break;
                    }
                }
            }

            return pathToExtractedFile;
        }

        
        public static void DeleteFileAndDirectory(string pathToExtractedFile)
        {
            string directoryName = Path.GetDirectoryName(pathToExtractedFile);

            if (Directory.Exists(directoryName))
            {
                Directory.Delete(directoryName, true);
            }
        }
    }
}
