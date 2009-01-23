using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyObjects
{
	[ExtensionPoint]
	public class KeyObjectSelectionEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (KeyObjectSelectionEditorComponentViewExtensionPoint))]
	public class KeyObjectSelectionEditorComponent : ApplicationComponent
	{
		private DateTime _datetime;
		private string _description;
		private string _seriesDescription;
		private int _seriesNumber;
		private KeyObjectSelectionDocumentTitle _docTitle;
		private List<Frame> _frames;

		public KeyObjectSelectionEditorComponent()
		{
			InitProps();
		}

		private void InitProps()
		{
			_datetime = DateTime.Now;
			_description = "";
			_seriesDescription = "";
			_seriesNumber = 1;
			_docTitle = KeyObjectSelectionDocumentTitleContextGroup.OfInterest;
			_frames = new List<Frame>();
		}

		public DateTime DateTime
		{
			get { return _datetime; }
			protected set
			{
				if (_datetime != value)
				{
					_datetime = value;
					base.NotifyPropertyChanged("DateTime");
					base.Modified = true;
				}
			}
		}

		public IList<Frame> Frames
		{
			get { return _frames; }
			protected set
			{
				if (_frames != value)
				{
					_frames = (List<Frame>) value;
					base.NotifyPropertyChanged("Images");
					base.Modified = true;
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
					base.NotifyPropertyChanged("DocumentTitle");
					base.Modified = true;
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
					base.NotifyPropertyChanged("Description");
					base.Modified = true;
				}
			}
		}

		public string SeriesDescription
		{
			get { return _seriesDescription; }
			set
			{
				if (_seriesDescription != value)
				{
					_seriesDescription = value;
					base.NotifyPropertyChanged("SeriesDescription");
					base.Modified = true;
				}
			}
		}

		public int SeriesNumber
		{
			get { return _seriesNumber; }
			set
			{
				if (_seriesNumber != value)
				{
					_seriesNumber = value;
					base.NotifyPropertyChanged("SeriesNumber");
					base.Modified = true;
				}
			}
		}

		public void NewDocument()
		{
			InitProps();
			base.NotifyAllPropertiesChanged();
		}

		private DicomFile CreateDocument()
		{
			this.DateTime = DateTime.Now;

			KeyObjectSerializer serializer = new KeyObjectSerializer();
			serializer.DateTime = _datetime;
			serializer.Description = _description;
			serializer.DocumentTitle = _docTitle;
			serializer.SeriesNumber = _seriesNumber;
			serializer.SeriesDescription = _seriesDescription;

			foreach (Frame frame in _frames)
				serializer.Frames.Add(frame);

			return serializer.Serialize();
		}

		public void SaveToFile()
		{
			FileExtensionFilter extDcm = new FileExtensionFilter("*.dcm", "Dicom Files (*.dcm)");
			FileExtensionFilter extAll = new FileExtensionFilter("*.*", "All Files (*.*)");
			FileDialogCreationArgs args = new FileDialogCreationArgs("", null, "dcm", new FileExtensionFilter[] {extDcm, extAll});
			FileDialogResult result = base.Host.DesktopWindow.ShowSaveFileDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				SaveDocument(result.FileName);
			}
		}

		public void SaveDocument(string filename)
		{
			CreateDocument().Save(filename);
		}

		public static IEnumerable StandardDocumentTitles
		{
			get { return KeyObjectSelectionDocumentTitleContextGroup.Values; }
		}
	}
}