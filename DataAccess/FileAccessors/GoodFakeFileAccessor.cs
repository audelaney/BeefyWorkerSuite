using System;
using System.Collections.Generic;

namespace DataAccess
{
    #pragma warning disable 1591
    public class GoodFakeFileAccessor : IFileAccessor 
    {
        public bool DeleteFile(string fileFullPath)
        {
            return true;
        }

        public bool DoesFileExist(string fileFullPath)
        {
            return true;
        }

        public bool DoesFolderExist(string folderFullPath)
        {
            return true;
        }

        public IEnumerable<string> GetFilesInFolder(string folderFullPath)
        {
            return new string[0];
        }

        public ulong GetFileSize(string fileFullPath)
        {
            return 1000;
        }

        public bool MoveFile(string oldPath, string newPath)
        {
            return true;
        }
    }
    #pragma warning restore 1591
}