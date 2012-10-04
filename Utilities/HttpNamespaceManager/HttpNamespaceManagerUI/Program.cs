#region License (non-CC)

// Copyright (c) 2007, Paul Wheeler
//
// This work is licensed under a Creative Commons Attribution 3.0 Unported License.
// For the complete license, see http://creativecommons.org/licenses/by/3.0/
// Or, you may send a letter to: 
//    Creative Commons
//    171 Second Street, Suite 300
//    San Francisco, California, 94105, USA.

#endregion

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace HttpNamespaceManager.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
            {
                Application.Run(new MainForm());
            }
            else if (args.Length == 2)
            {
                Regex actionRegex = new Regex(@"(-|--|/)(?'action'ad?d?|ed?i?t?|re?m?o?v?e?)", RegexOptions.IgnoreCase);
                Match m = actionRegex.Match(args[0]);
                if (m.Success && m.Groups["action"] != null && m.Groups["action"].Success && !String.IsNullOrEmpty(m.Groups["action"].Value))
                {
                    string action = m.Groups["action"].Value.ToLower();
                    if (action.StartsWith("a"))
                    {
                        Application.Run(new MainForm(NamespaceManagerAction.Add, args[1]));
                    }
                    else if (action.StartsWith("e"))
                    {
                        Application.Run(new MainForm(NamespaceManagerAction.Edit, args[1]));
                    }
                    else
                    {
                        Application.Run(new MainForm(NamespaceManagerAction.Remove, args[1]));
                    }
                }
                else
                {
                    Application.Run(new UsageForm());
                }
            }
            else
            {
                Application.Run(new UsageForm());
            }
        }
    }
}