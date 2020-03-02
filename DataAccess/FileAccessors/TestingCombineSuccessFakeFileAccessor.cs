using System;
using System.Collections.Generic;

namespace DataAccess
{
    /// <summary>
    /// For testing purposes
    /// </summary>
    public class TestingCombineSuccessFakeFileAccessor : IFileAccessor
    {
        /// <summary></summary>
        public bool DeleteFile(string fileFullPath)
        {
            throw new NotImplementedException();
        }

        /// <summary></summary>
        public bool DoesFileExist(string fileFullPath)
        {
            throw new NotImplementedException();
        }

        /// <summary></summary>
        public bool DoesFolderExist(string folderFullPath) => true;

        /// <summary></summary>
        public IEnumerable<string> GetFilesInFolder(string folderFullPath)
        {
            return new List<string>
            {
                "verifiedOutput.mkv"
            };
        }

        /// <summary></summary>
        public ulong GetFileSize(string fileFullPath)
        {
            throw new NotImplementedException();
        }

        /// <summary></summary>
        public bool MoveFile(string oldPath, string newPath)
        {
            throw new NotImplementedException();
        }
    }
}