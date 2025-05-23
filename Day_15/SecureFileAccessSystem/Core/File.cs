using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureFileAccessSystem.Interfaces;

namespace SecureFileAccessSystem.Core
{
    public class File : IFile
    {
        private string _fileName;

        public File(string fileName)
        {
            _fileName = fileName;
        }

        public void Read()
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), _fileName);

                if (!System.IO.File.Exists(path))
                {
                    Console.WriteLine($"[Error] File '{_fileName}' not found at: {path}");
                    return;
                }

                string content = System.IO.File.ReadAllText(path);
                Console.WriteLine("[Access Granted] File Content:");
                Console.WriteLine(content);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Failed to read the file: {ex.Message}");
            }
        }
    }
}
