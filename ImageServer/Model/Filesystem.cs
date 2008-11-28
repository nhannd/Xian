using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClearCanvas.ImageServer.Model
{
    public partial class Filesystem
    {
        public string GetAbsolutePath(string relativePath)
        {
            return Path.Combine(this.FilesystemPath, relativePath);
        }

    }
}
