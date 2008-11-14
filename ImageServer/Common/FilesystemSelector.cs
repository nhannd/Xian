#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Provide sorting function on <see cref="ServerFilesystemInfo"/>
    /// </summary>
    public static class FilesystemSorter
    {
        public static int SortByFreeSpace(ServerFilesystemInfo fs1, ServerFilesystemInfo fs2)
        {
            Platform.CheckForNullReference(fs1, "fs1");
            Platform.CheckForNullReference(fs2, "fs2");
            Platform.CheckForNullReference(fs1.Filesystem, "fs1.Filesystem");
            Platform.CheckForNullReference(fs2.Filesystem, "fs2.Filesystem");

            if (fs1 == fs2)
                return 0;

            if (fs1.Filesystem.FilesystemTierEnum.Enum.Equals(fs2.Filesystem.FilesystemTierEnum.Enum))
            {
                // descending order on available size.. smaller margin means less available space
                return fs2.HighwaterMarkMargin.CompareTo(fs1.HighwaterMarkMargin);
            }
            else
            {
                // ascending order on tier
                return fs1.Filesystem.FilesystemTierEnum.Enum.CompareTo(fs2.Filesystem.FilesystemTierEnum.Enum);
            }
        }
    }

    /// <summary>
    /// Class used for incoming studies to select which filesystem the study should be 
    /// stored to.
    /// </summary>
    public class FilesystemSelector
    {
        private readonly FilesystemMonitor _monitor;

        public FilesystemSelector(FilesystemMonitor monitor)
        {
            _monitor = monitor;
        }

        public ServerFilesystemInfo SelectFilesystem(DicomMessageBase msg)
        {
            return SelectFilesystem();
        }

        public ServerFilesystemInfo SelectFilesystem()
        {
            IList<ServerFilesystemInfo> list =
                _monitor.GetFilesystems(delegate(ServerFilesystemInfo fs) { return !fs.Writeable; });

            if (list == null || list.Count == 0)
                return null;

            list = CollectionUtils.Sort(list, FilesystemSorter.SortByFreeSpace);
            ServerFilesystemInfo selectedFS = CollectionUtils.FirstElement(list);
            Platform.CheckForNullReference(selectedFS, "selectedFS");
            return selectedFS;
        }
    }
}
