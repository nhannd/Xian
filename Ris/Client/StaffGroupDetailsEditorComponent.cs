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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="StaffGroupDetailsEditorComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class StaffGroupDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// StaffGroupDetailsEditorComponent class.
	/// </summary>
	[AssociateView(typeof(StaffGroupDetailsEditorComponentViewExtensionPoint))]
	public class StaffGroupDetailsEditorComponent : ApplicationComponent
	{
		private StaffGroupDetail _staffGroupDetail;

		public StaffGroupDetail StaffGroupDetail
		{
			get { return _staffGroupDetail; }
			set { _staffGroupDetail = value; }
		}

		#region Presentation Model

		[ValidateNotNull]
		public string Name
		{
			get { return _staffGroupDetail.Name; }
			set
			{
				if (_staffGroupDetail.Name != value)
				{
					_staffGroupDetail.Name = value;
					this.Modified = true;
					NotifyPropertyChanged("Name");
				}
			}
		}

		public string Description
		{
			get { return _staffGroupDetail.Description; }
			set
			{
				if (_staffGroupDetail.Description != value)
				{
					_staffGroupDetail.Description = value;
					this.Modified = true;
					NotifyPropertyChanged("Description");
				}
			}
		}

		public bool IsElective
		{
			get { return _staffGroupDetail.IsElective; }
			set
			{
				if (_staffGroupDetail.IsElective != value)
				{
					_staffGroupDetail.IsElective = value;
					this.Modified = true;
					NotifyPropertyChanged("IsElective");
				}
			}
		}

		#endregion
	}
}
