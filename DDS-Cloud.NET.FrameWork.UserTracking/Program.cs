using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace UserFolderSizeChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            // 获取所有用户的文件夹路径
            var userFolders = GetUserFolders();

            // 检查每个用户文件夹的大小
            foreach (var folder in userFolders)
            {
                try
                {
                    var size = GetDirectorySize(folder);
                    Console.WriteLine($"用户文件夹: {folder}, 大小: {size} 字节");

                    // 如果文件夹大小超过1GB
                    if (size > 1 * 1024 * 1024 * 1024) // 1GB in bytes
                    {
                        Console.WriteLine($"警告: 用户文件夹 {folder} 超过1GB.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"无法访问 {folder}: {ex.Message}");
                }
            }

            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }

        static long GetDirectorySize(string directory)
        {
            var size = 0L;
            foreach (var folder in Directory.GetDirectories(directory))
            {
                size += GetDirectorySize(folder);
            }
            foreach (var file in Directory.GetFiles(directory))
            {
                size += new FileInfo(file).Length;
            }
            return size;
        }

        static string[] GetUserFolders()
        {
            // 获取所有用户的SID
            var userSids = GetUserSids();

            // 获取每个用户的文件夹路径
            var userFolders = userSids.Select(sid => GetUserFolder(sid)).ToArray();
            return userFolders;
        }

        static string[] GetUserSids()
        {
            const string keyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList";
            var registryKey = Registry.LocalMachine.OpenSubKey(keyPath);
            var userSids = registryKey.GetSubKeyNames();
            return userSids;
        }

        static string GetUserFolder(string sid)
        {
            const string keyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList";
            using (var registryKey = Registry.LocalMachine.OpenSubKey(keyPath))
            {
                var subKey = registryKey.OpenSubKey(sid);
                if (subKey != null)
                {
                    var profileImagePath = subKey.GetValue("ProfileImagePath").ToString();
                    return profileImagePath;
                }
            }
            return null;
        }
    }
}