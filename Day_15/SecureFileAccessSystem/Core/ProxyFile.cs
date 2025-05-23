using SecureFileAccessSystem.Interfaces;
using SecureFileAccessSystem.Models;

namespace SecureFileAccessSystem.Core
{
    public class ProxyFile : IFile
    {
        private File _realFile;
        private string _filePath;
        private User _user;

        public ProxyFile(string filePath, User user)
        {
            _filePath = filePath;
            _realFile = new File(filePath);
            _user = user;
        }

        public void Read()
        {
            switch (_user.Role)
            {
                case Role.Admin:
                    _realFile.Read();
                    break;

                case Role.User:
                    ShowFileMetadata(_filePath);
                    break;

                case Role.Guest:
                default:
                    Console.WriteLine("[Access Denied] You do not have permission to read this file.");
                    break;
            }
        }

        public void ShowFileMetadata(string filePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);

                if (!fileInfo.Exists)
                {
                    Console.WriteLine($"[Error] File '{fileInfo.Name}' does not exist.");
                    return;
                }

                Console.WriteLine("[Limited Access] File Metadata:");
                Console.WriteLine($"File Name: {fileInfo.Name}");
                Console.WriteLine($"Size: {fileInfo.Length / 1024.0:F2} KB");
                Console.WriteLine($"Created On: {fileInfo.CreationTime}");
                Console.WriteLine($"Last Modified: {fileInfo.LastWriteTime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Unable to read file metadata: {ex.Message}");
            }
        }

    }

}
