using System;
using System.Collections.Generic;

namespace DataAccess
{
    /// <summary>
    /// Basic good returns
    /// </summary>
    public class GoodFakeFileAccessor : IFileAccessor 
    {
        /// <summary></summary>
        public bool DeleteFile(string fileFullPath) => true;

        /// <summary></summary>
        public bool DoesFileExist(string fileFullPath) => true;

        /// <summary></summary>
        public bool DoesFolderExist(string folderFullPath) => true;

        /// <summary></summary>
        public IEnumerable<string> GetFilesInFolder(string folderFullPath) => new string[0];

        /// <summary></summary>
        public ulong GetFileSize(string fileFullPath) => 50 * 1024;

        /// <summary></summary>
        public bool MoveFile(string oldPath, string newPath) => true;
    }
}