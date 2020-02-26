using System;
using System.Collections.Generic;
using System.IO;

namespace DataAccess
{
    /// <summary>
    /// Interface specifying basic, non-logical file based behavior/functionality
    /// </summary>
    public interface IFileAccessor
    {
        /// <summary>
        /// If the file path leads to an existing file.
        /// </summary>
        bool DoesFileExist(string fileFullPath);
        /// <summary>
        /// Moves a file
        /// </summary>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown if:
        ///     - The old path to the file is invalid or the file is not found
        /// </exception>
        /// <exception cref="DataAccess.Exceptions.FileAlreadyExistsException">
        /// Thrown if:
        ///     - The path that the file is going to be move to is already occupied
        /// </exception>
        bool MoveFile(string oldPath, string newPath);
        /// <summary>
        /// Deletes a file
        /// </summary>
        bool DeleteFile(string fileFullPath);
        /// <summary>
        /// If the folder path leads to an existing folder.
        /// </summary>
        bool DoesFolderExist(string folderFullPath);
        /// <summary>
        /// Returns the full paths of all files in a folder
        /// </summary>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// Thrown if:
        ///     - The path to the folder is invalid or the folder is not found
        /// </exception>
        IEnumerable<string> GetFilesInFolder(string folderFullPath);
        /// <summary>
        /// Returns the size in bytes of the specified file
        /// </summary>
        /// <exception cref="System.IO.FileNotFoundException">
        /// Thrown if:
        ///     - The path to the file is invalid or the file is not found
        /// </exception>
        ulong GetFileSize(string fileFullPath);
    }
}