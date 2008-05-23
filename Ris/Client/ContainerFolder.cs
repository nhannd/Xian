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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// A folder that acts strictly as a parent for other folders, and does not itself contain any items.
	/// </summary>
    public class ContainerFolder : Folder
    {
        public ContainerFolder(string path, bool startExpanded)
			:base(path, startExpanded)
        {
        }

        #region Folder overrides

		protected override bool IsItemCountKnown
		{
			get { return true; }
		}

        public override string Text
        {
            get { return this.FolderPath.LastSegment.LocalizedText; }
        }

        public override void Refresh()
        {
        }

        public override void RefreshCount()
        {
        }

        public override ITable ItemsTable
        {
            get { return null; }
        }

        public override int TotalItemCount
        {
            get { return 0; }
        }

        public override DragDropKind CanAcceptDrop(object[] items, DragDropKind kind)
        {
			// can't drop items into a container folder, since it contains only other folders
	        return DragDropKind.None;
        }

        public override DragDropKind AcceptDrop(object[] items, DragDropKind kind)
        {
			// can't drop items into a container folder, since it contains only other folders
			return DragDropKind.None;
        }

		protected override IconSet OpenIconSet
		{
			get { return new IconSet(IconScheme.Colour, "ContainerFolderOpenSmall.png", "ContainerFolderOpenMedium.png", "ContainerFolderOpenMedium.png"); }
		}

		protected override IconSet ClosedIconSet
		{
			get { return new IconSet(IconScheme.Colour, "ContainerFolderClosedSmall.png", "ContainerFolderClosedMedium.png", "ContainerFolderClosedMedium.png"); }
		}


        #endregion
    }
}
