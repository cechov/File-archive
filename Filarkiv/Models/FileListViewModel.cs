using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Filarkiv.Models
{
    public class FileListViewModel
    {
        public IEnumerable<File> Files { get; set; }
        public string Category { get; set; }
    }

    public class File
    {
        public string fileName { get; set; }
        public string filePath { get; set; }
    }
}