using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMangementApplication.Interfaces
{
    public interface IFileWriter
    {
        void Write(string filePath, string content);
    }
}
