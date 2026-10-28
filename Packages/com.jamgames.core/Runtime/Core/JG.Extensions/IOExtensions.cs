using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Core.JG.Extensions
{
    public class IOExtensions
    {
        public static void DeleteDirectory(string targetDir)
        {
            if (Directory.Exists(targetDir))
            {
                string[] files = GetFiles(targetDir, true);
                string[] dirs = GetDirectories(targetDir, true);

                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                Directory.Delete(targetDir, false);
            }
        }

        public static void DeleteFile(string file)
        {
            if (File.Exists(file))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs,
            bool overwrite = true)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, overwrite);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subDir.Name);
                    DirectoryCopy(subDir.FullName, tempPath, true);
                }
            }
        }

        public static string[] GetAllFiles(string directory, string exclude, string include)
        {
            List<string> files = new List<string>();
            GetAllFiles(files, directory, exclude, include);
            return files.ToArray();
        }

        public static string[] GetAllFiles(string directory)
        {
            return GetAllFiles(directory, null, null);
        }

        public static string GetLatestFile(string directory)
        {
            return GetLatestFile(directory, null, null);
        }

        public static string GetLatestFile(string directory, string exclude, string include)
        {
            List<string> allFiles = new List<string>();
            allFiles.AddRange(GetAllFiles(directory, exclude, include));
            if (allFiles.Count == 0)
                return null;

            DateTime lastDate = new DateTime(1900, 1, 1);
            string latestPath = allFiles[allFiles.Count - 1];

            for (int i = 0; i < allFiles.Count; i++)
            {
                FileInfo info = new FileInfo(allFiles[i]);
                info.Refresh();

                DateTime creationTime = info.CreationTime;
                if (creationTime > lastDate)
                {
                    lastDate = creationTime;
                    latestPath = allFiles[i];
                }
            }

            return latestPath;
        }

        public static void GetAllFiles(List<string> allFiles, string directory, string exclude, string include)
        {
            Regex excludeRegex = exclude != null ? new Regex(exclude) : null;
            Regex includeRegex = include != null ? new Regex(include) : null;
            GetAllFiles(allFiles, directory, directory, excludeRegex, includeRegex);
        }

        public static string[] GetDirectories(string directory, bool ignoreErrors)
        {
            try
            {
                return Directory.GetDirectories(directory);
            }
            catch (Exception exc)
            {
                if (ignoreErrors)
                {

                }
                throw;
            }
        }

        public static string[] GetFiles(string directory, bool ignoreErrors)
        {
            try
            {
                return Directory.GetFiles(directory);
            }
            catch (Exception exc)
            {
                if (ignoreErrors)
                {
                    return new string[0];
                }
                throw;
            }
        }

        public static void GetAllFiles(List<string> allFiles, string topDirectory, string directory, Regex exclude,
            Regex include, bool ignoreErrors = true)
        {
            if (Directory.Exists(directory))
            {
                string[] localFiles = GetFiles(directory, ignoreErrors);

                for (int i = 0; i < localFiles.Length; i++)
                {
                    string name = localFiles[i];
                    if (exclude != null)
                    {
                        string relativePath = name.Substring(topDirectory.Length);
                        if (exclude.IsMatch(relativePath))
                            continue;
                    }
                    if (include != null)
                    {
                        if (!include.IsMatch(name))
                            continue;
                    }
                    allFiles.Add(name);
                }

                string[] directories = GetDirectories(directory, ignoreErrors);
                for (int i = 0; i < directories.Length; i++)
                    GetAllFiles(allFiles, topDirectory, directories[i], exclude, include);
            }
        }

        public static string FindDirectory(string root, string name)
        {
            string result = null;
            string[] dirs = GetDirectories(root, true);
            foreach (string dir in dirs)
            {
                if (dir.EndsWith(name))
                {
                    result = dir;
                    break;
                }
                result = FindDirectory(dir, name);
                if (result != null)
                {
                    break;
                }
            }
            if (result != null)
                result = result.Replace('\\', '/');
            return result;
        }

        public static string SplitPathByDirectory(string path, string name)
        {
            path = path.Replace('\\', '/');
            string[] full = path.Split('/');
            var half = new StringBuilder();
            bool collect = false;
            for (int i = 0; i < full.Length; i++)
            {
                if (!collect && full[i].Equals(name))
                {
                    collect = true;
                }

                if (collect)
                {
                    half.Append(full[i]);
                    if (i < full.Length - 1)
                        half.Append('/');
                }
            }
            return half.ToString();
        }

        public static bool ioBlocked = false;

        public static bool TryOperation(Action action)
        {
            if (ioBlocked)
                return false;

            try
            {
                action();
                return true;
            }
            catch (IOException exc)
            {
                ioBlocked = true;

                // Exc.Report(ExcMap.FREE_SPACE_ERROR);
                // Exc.Report(ExcMap.IO_DETAILED_EXCEPTION, exc: exc);
            }
            catch (UnauthorizedAccessException exc)
            {
                ioBlocked = true;

                // Exc.Report(ExcMap.FREE_SPACE_ERROR);
                // Exc.Report(ExcMap.IO_DETAILED_EXCEPTION, exc: exc);
            }
            catch (ObjectDisposedException exc)
            {
                ioBlocked = true;
                // Exc.Report(ExcMap.IO_DETAILED_EXCEPTION, exc: exc);
            }
            catch (Exception exc)
            {
                // Exc.Report(ExcMap.IO_DETAILED_EXCEPTION, exc: exc);
            }
            return false;
        }

        public static void TryWriteToStream(Stream stream, byte[] bytes, byte[] separatorBytes)
        {
            if (ioBlocked)
                return;

            TryOperation(() =>
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Write(separatorBytes, 0, separatorBytes.Length);
                stream.Flush();
            });
        }

        public static void TrySaveFile(string key, byte[] bytes)
        {
            if (ioBlocked)
                return;

            TryOperation(() =>
            {
                if (File.Exists(key))
                    return;

                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(Path.GetDirectoryName(key));
                File.WriteAllBytes(key, bytes);
            });
        }

        public static bool TrySaveText(string filePath, string text)
        {
            if (ioBlocked)
                return false;
            return TryOperation(() =>
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    File.WriteAllText(filePath, text);
                });
        }

        public static string Get(string fileName)
        {
            if (File.Exists(fileName))
                return File.ReadAllText(fileName);

            return null;
        }
    }
}
