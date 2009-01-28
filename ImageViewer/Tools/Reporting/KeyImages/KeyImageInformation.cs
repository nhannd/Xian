using System;
using System.Collections.Generic;
using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.Clipboard;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.StudyManagement;

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

		private List<DicomFile> CreateDocuments()
		{
			KeyObjectSerializer serializer = new KeyObjectSerializer();
			serializer.DateTime = DateTime;
			serializer.Description = _description;
			serializer.DocumentTitle = _docTitle;
			serializer.SeriesNumber = _seriesNumber;
			serializer.SeriesDescription = _seriesDescription;

			foreach (Frame frame in ExtractFrames())
				serializer.Frames.Add(frame);

			return serializer.Serialize();
		}

		private IEnumerable<Frame> ExtractFrames()
		{
			foreach (IClipboardItem item in ClipboardItems)
			{
				IImageSopProvider provider = item.Item as IImageSopProvider;
				if (provider != null)
					yield return provider.Frame;
			}
		}

		internal void Publish()
		{
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
