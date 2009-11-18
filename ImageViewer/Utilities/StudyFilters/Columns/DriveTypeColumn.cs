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

using System.IO;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Columns
{
	[ExtensionOf(typeof(SpecialColumnExtensionPoint))]
	public class DriveTypeColumn : SpecialColumn<string>, ILexicalSortableColumn
	{
		public const string KEY = "DriveType";

		public DriveTypeColumn() : base(SR.DriveType, KEY) { }

		public override string GetTypedValue(StudyItem item)
		{
			DriveInfo drive = DriveColumn.GetDriveInfo(item);
			if (drive != null)
			{
				switch (drive.DriveType)
				{
					case DriveType.Removable:
						return SR.RemovableMedia;
					case DriveType.Fixed:
						return SR.FixedMedia;
					case DriveType.Network:
						return SR.NetworkMedia;
					case DriveType.CDRom:
						return SR.CDRomMedia;
					case DriveType.Ram:
						return SR.RAM;
				}
			}
			return SR.Unknown;
		}

		public override bool Parse(string input, out string output)
		{
			output = input;
			return true;
		}

		public override int Compare(StudyItem x, StudyItem y)
		{
			return this.CompareLexically(x, y);
		}

		public int CompareLexically(StudyItem x, StudyItem y)
		{
			return this.GetTypedValue(x).CompareTo(this.GetTypedValue(y));
		}
	}
}