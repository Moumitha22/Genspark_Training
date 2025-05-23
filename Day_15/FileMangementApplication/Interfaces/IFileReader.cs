using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMangementApplication.Interfaces
{
    public interface IFileReader
    {
        string Read(string filePath);
    }

}
