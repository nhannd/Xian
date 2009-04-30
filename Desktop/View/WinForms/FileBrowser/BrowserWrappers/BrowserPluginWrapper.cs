#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms.FileBrowser.ShellDll;

namespace ClearCanvas.Desktop.View.WinForms.FileBrowser.BrowserWrappers
{
    public class BrowserPluginWrapper : Component
    {
        #region Fields

        private ArrayList columnPlugins;
        private ArrayList viewPlugins;
        private ArrayList contextPlugins;

        #endregion

        public BrowserPluginWrapper()
        {
            columnPlugins = new ArrayList();
            viewPlugins = new ArrayList();
            contextPlugins = new ArrayList();

            //LoadPlugins();
        }

        private void LoadPlugins()
        {
            string pluginPath = System.Windows.Forms.Application.StartupPath + @"\plugins";

            if (Directory.Exists(pluginPath))
            {
                string[] files = Directory.GetFiles(pluginPath, "*.dll");

                foreach (string file in files)
                {
                    try
                    {
                        Assembly plugin = Assembly.LoadFile(file);

                        Type[] types = plugin.GetTypes();

                        foreach (Type type in types)
                        {
                            try
                            {
                                IBrowserPlugin browserPlugin =
                                    plugin.CreateInstance(type.ToString()) as IBrowserPlugin;

                                if (browserPlugin != null)
                                {
                                    if (browserPlugin is IColumnPlugin)
                                        columnPlugins.Add(browserPlugin);

                                    if (browserPlugin is IViewPlugin)
                                        viewPlugins.Add(browserPlugin);

                                    //if (browserPlugin is IContextPlugin)
                                        //contextPlugins.Add(browserPlugin);
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        #region Properties

        public ArrayList ColumnPlugins { get { return columnPlugins; } }
        public ArrayList ViewPlugins { get { return viewPlugins; } }
        public ArrayList ContextPlugins { get { return contextPlugins; } }

        #endregion
    }

    #region Plugins

    public interface IBrowserPlugin
    {
        string Name { get; }
        string Info { get; }
    }

    public interface IColumnPlugin : IBrowserPlugin
    {
        string[] ColumnNames { get; }
        
        HorizontalAlignment GetAlignment(string columnName);

        string GetFolderInfo(IDirInfoProvider provider, string columnName, ShellItem item);
        string GetFileInfo(IFileInfoProvider provider, string columnName, ShellItem item);
    }

    public interface IViewPlugin : IBrowserPlugin
    {
        string ViewName { get; }
        Control ViewControl { get; }

        void FolderSelected(IDirInfoProvider provider, ShellItem item);
        void FileSelected(IFileInfoProvider provider, ShellItem item);
        void Reset();
    }

    /*public interface IContextPlugin : IBrowserPlugin
    {
        string MenuText { get; }
        Icon MenuIcon { get; }
        string MenuInfo { get; }
        
        string[] Extensions { get; }

        void MenuSelected(IFileInfoProvider2 streamProvider, IDirectoryInfoProvider2 storageProvider, ShellItem[] items);
    }*/

    #region Provider Interfaces

    public interface IDirInfoProvider
    {
        ShellAPI.STATSTG GetDirInfo();
    }

    public interface IFileInfoProvider
    {
        ShellAPI.STATSTG GetFileInfo();
        Stream GetFileStream();
    }

    public interface IDirectoryInfoProvider2 : IDirInfoProvider
    {
        IStorage GetDirInfo(ShellItem item);
    }

    public interface IFileInfoProvider2 : IFileInfoProvider
    {
        IStream GetFileInfo(ShellItem item);
        Stream GetFileStream(ShellItem item);
    }

    #endregion

    #endregion
}
