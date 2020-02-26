using System;
using DataAccess.Exceptions;
using System.Collections.Generic;
using System.IO;

namespace DataAccess
{
    /// <summary></summary>
    public class RealFileAccessor : IFileAccessor
    {
        /// <summary></summary>
        public bool DeleteFile(string fileFullPath)
        {
            if (!File.Exists(fileFullPath))
            { throw new FileNotFoundException(fileFullPath); }
            try
            {
                File.Delete(fileFullPath);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        /// <summary></summary>
        public bool DoesFileExist(string fileFullPath)
        {
            return File.Exists(fileFullPath);
        }

        /// <summary></summary>
        public bool DoesFolderExist(string folderFullPath)
        {
            return Directory.Exists(folderFullPath);
        }

        /// <summary></summary>
        public IEnumerable<string> GetFilesInFolder(string folderFullPath)
        {
            if (!Directory.Exists(folderFullPath))
            { throw new DirectoryNotFoundException(folderFullPath); }
            try
            {
                return Directory.GetFiles(folderFullPath);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        /// <summary></summary>
        public ulong GetFileSize(string fileFullPath)
        {
            if (!File.Exists(fileFullPath))
            { throw new FileNotFoundException(fileFullPath); }
            var file = new FileInfo(fileFullPath);
            return (ulong)file.Length;
        }

        /// <summary></summary>
        public bool MoveFile(string oldPath, string newPath)
        {
            if (!File.Exists(oldPath))
            { throw new FileNotFoundException(oldPath); }
            if (File.Exists(newPath))
            { throw new FileAlreadyExistsException(newPath); }
            try
            {
                File.Move(oldPath, newPath);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}