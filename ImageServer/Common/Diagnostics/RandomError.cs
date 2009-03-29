using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common.Diagnostics
{
    public delegate void ErrorDelegate();

    public static class RandomError
    {
        public static void Generate(bool condition, string description, ErrorDelegate del)
        {
            if (condition == true)
            {
                Random ran = new Random();
                if (ran.Next() % 10 == 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("\n\n\t**********************************************************************************************************\n");
                    sb.AppendFormat("\t                 SIMULATING ERROR: {0}\n", description);
                    sb.AppendFormat("\t**********************************************************************************************************\n");
                    Platform.Log(LogLevel.Error, sb.ToString());
                    if (del!=null)
                        del();
                    else
                    {
                        throw new Exception("Simulated Random Exception");
                    }
                }
            }
        }

        public static void Generate(bool condition, string description)
        {
            Generate(condition, description, null);
        }
    }
}
