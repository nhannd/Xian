using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Server.Executable
{
    class Program
    {
        static void Main(string[] args)
        {
            Platform.StartApp(new ClassNameExtensionFilter(args[0]), args);
        }
    }
}
