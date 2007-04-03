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
            if (args.Length > 0)
            {
                string[] args1 = new string[args.Length - 1];
                Array.Copy(args, 1, args1, 0, args.Length - 1);
                Platform.StartApp(new ClassNameExtensionFilter(args[0]), args1);
            }
            else
            {
                Platform.StartApp();
            }
        }
    }
}
