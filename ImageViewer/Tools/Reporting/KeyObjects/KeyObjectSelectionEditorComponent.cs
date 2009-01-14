using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Dicom.Iod.ContextGroups;
using ClearCanvas.ImageViewer.KeyObjects;
using ClearCanvas.ImageViewer.StudyManagement;

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
		private List<ImageSop> _images;

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
			_images = new List<ImageSop>();
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

		public IList<ImageSop> Images
		{
			get { return _images; }
			protected set
			{
				if (_images != value)
				{
					_images = (List<ImageSop>) value;
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

		private KeyObjectSelection MakeDocument()
		{
			this.DateTime = DateTime.Now;

			KeyObjectSelection koSelection = new KeyObjectSelection();
			koSelection.DateTime = _datetime;
			koSelection.Description = _description;
			koSelection.DocumentTitle = _docTitle;
			koSelection.SeriesNumber = _seriesNumber;
			koSelection.SeriesDescription = _seriesDescription;

			foreach (ImageSop imageSop in _images)
			{
				koSelection.Images.Add(imageSop);
			}

			return koSelection;
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
			MakeDocument().Save(filename);
		}

		public static IEnumerable StandardDocumentTitles
		{
			get { return KeyObjectSelectionDocumentTitleContextGroup.Values; }
		}
	}
}