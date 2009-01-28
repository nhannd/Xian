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
			component.SeriesNumber = info.SeriesNumber.ToString();

			ApplicationComponentExitCode exitCode = LaunchAsDialog(desktopWindow, component, SR.TitleEditKeyImageInformation);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				info.Description = component.Description;
				info.DocumentTitle = component.DocumentTitle;
				info.SeriesDescription = component.SeriesDescription;
				info.SeriesNumber = Int32.Parse(component.SeriesNumber);
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