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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Dicom.Iod.ContextGroups;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ExtensionPoint]
	public class KeyImageInformationEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> { }

	[AssociateView(typeof(KeyImageInformationEditorComponentViewExtensionPoint))]
	public class KeyImageInformationEditorComponent : ApplicationComponent
	{
		private DateTime _datetime;
		private string _description;
		private string _seriesDescription;
		private string _seriesNumber;
		private KeyObjectSelectionDocumentTitle _docTitle;

		private KeyImageInformationEditorComponent()
		{
		}

		public DateTime DateTime
		{
			get { return _datetime; }
			protected set
			{
				if (_datetime != value)
				{
					_datetime = value;
					NotifyPropertyChanged("DateTime");
				}
			}
		}

		public KeyObjectSelectionDocumentTitle DocumentTitle
		{
			get { return _docTitle; }
			set
			{
				if (_docTitle != value)
				{
					_docTitle = value;
					NotifyPropertyChanged("DocumentTitle");
				}
			}
		}

		public string Description
		{
			get { return _description; }
			set
			{
				if (_description != value)
				{
					_description = value;
					NotifyPropertyChanged("Description");
				}
			}
		}

		[ValidateLength(1, 64, Message = "MessageInvalidSeriesDescription")]
		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set
			{
				if (_seriesDescription != value)
				{
					_seriesDescription = value;
					NotifyPropertyChanged("SeriesDescription");
				}
			}
		}

		public string SeriesNumber
		{
			get { return _seriesNumber; }
			set
			{
				if (_seriesNumber != value)
				{
					_seriesNumber = value;
					NotifyPropertyChanged("SeriesNumber");
				}
			}
		}

		public static IEnumerable<KeyObjectSelectionDocumentTitle> StandardDocumentTitles
		{
			get { return KeyObjectSelectionDocumentTitleContextGroup.Values; }
		}

		public void Accept()
		{
			ExitCode = ApplicationComponentExitCode.Accepted;
			this.Host.Exit();
		}

		public void Cancel()
		{
			ExitCode = ApplicationComponentExitCode.None;
			this.Host.Exit();
		}

		internal static void Launch(IDesktopWindow desktopWindow)
		{
			KeyImageInformation info = KeyImageClipboard.GetKeyImageInformation(desktopWindow);
			if (info == null)
				throw new ArgumentException("There is no valid key image data available for the given window.", "desktopWindow");

			KeyImageInformationEditorComponent component = new KeyImageInformationEditorComponent();
			component.Description = info.Description;
			component.DocumentTitle = info.DocumentTitle;
			component.SeriesDescription = info.SeriesDescription;

			ApplicationComponentExitCode exitCode = LaunchAsDialog(desktopWindow, component, SR.TitleEditKeyImageInformation);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				info.Description = component.Description;
				info.DocumentTitle = component.DocumentTitle;
				info.SeriesDescription = component.SeriesDescription;
			}
		}

		[ValidationMethodFor("SeriesNumber")]
		private ValidationResult ValidateSeriesNumber()
		{
			int value;
			if (String.IsNullOrEmpty(SeriesNumber) || !int.TryParse(SeriesNumber, out value))
				return new ValidationResult(false, "MessageInvalidSeriesNumber");
			else
				return new ValidationResult(true, "");
		}
	}
}