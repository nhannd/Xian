using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Services.Configuration;
using ClearCanvas.ImageViewer.Services.ServerTree;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	public sealed class KeyImageInformation : IDisposable
	{
		private string _description;
		private string _seriesDescription;
		
		//TODO: dynamically calculate series# until edited by user.
		private int _seriesNumber;
		private KeyObjectSelectionDocumentTitle _docTitle;

		internal readonly BindingList<IClipboardItem> ClipboardItems;

		public KeyImageInformation()
		{
			_description = "";
			_seriesDescription = "KEY IMAGES";
			_seriesNumber = 1;
			_docTitle = KeyObjectSelectionDocumentTitleContextGroup.OfInterest;

			ClipboardItems = new BindingList<IClipboardItem>();
		}

		public DateTime DateTime
		{
			get { return Platform.Time; }
		}

		public KeyObjectSelectionDocumentTitle DocumentTitle
		{
			get { return _docTitle; }
			set { _docTitle = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set { _seriesDescription = value; }
		}

		public int SeriesNumber
		{
			get { return _seriesNumber; }
			set { _seriesNumber = value; }
		}

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			foreach (IClipboardItem item in ClipboardItems)
				((IDisposable) item).Dispose();

			ClipboardItems.Clear();
		}

		#endregion
	}
}
