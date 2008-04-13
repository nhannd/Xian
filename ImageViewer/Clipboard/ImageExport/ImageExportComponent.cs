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

using System;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using System.IO;
using ClearCanvas.Desktop.Validation;
using Path=System.IO.Path;

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	[ExtensionPoint]
	public sealed class ImageExportComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	#region ImageExporterInfo class

	public class ImageExporterInfo
	{
		private readonly IImageExporter _imageExporter;

		internal ImageExporterInfo(IImageExporter imageExporter)
		{
			_imageExporter = imageExporter;
		}

		internal IImageExporter ImageExporter
		{
			get { return _imageExporter; }
		}

		public string Description
		{
			get { return _imageExporter.Description; }
		}

		public string FileExtensionFilter
		{
			get
			{
				string filterPortion = StringUtilities.Combine(_imageExporter.FileExtensions, ";",
													delegate(string extension)
													{
														return String.Format("*.{0}", extension);
													});

				return String.Format("{0}|{1}", _imageExporter.Description, filterPortion);
			}
		}

		public string DefaultExtension
		{
			get { return _imageExporter.FileExtensions[0]; }
		}

		public bool IsConfigurable
		{
			get { return _imageExporter is IConfigurableImageExporter; }
		}
	}

	#endregion

	[AssociateView(typeof(ImageExportComponentViewExtensionPoint))]
	public class ImageExportComponent : ApplicationComponent
	{
		#region Path Validation Rule

		private class PathValidationRule : IValidationRule
		{
			public PathValidationRule()
			{
			}

			#region IValidationRule Members

			public string PropertyName
			{
				get { return "ExportFilePath"; }
			}

			public ValidationResult GetResult(IApplicationComponent component)
			{
				ImageExportComponent exportComponent = (ImageExportComponent)component;

				bool valid = true;
				string message = null;

				if (exportComponent.NumberOfImagesToExport == 1)
				{
					string correctedFilename = exportComponent.GetCorrectedExportFilePath();
					string directory = Path.GetDirectoryName(exportComponent.ExportFilePath);

					if (!String.IsNullOrEmpty(directory) && Directory.Exists(directory))
					{
						string fileName = Path.GetFileName(correctedFilename);
						if (String.IsNullOrEmpty(fileName))
						{
							valid = false;
							message = SR.MessageInvalidFilePath;
						}
					}
					else
					{
						valid = false;
						message = SR.MessageInvalidFilePath;
					}
				}
				else
				{
					valid = (!String.IsNullOrEmpty(exportComponent.ExportFilePath) &&
							 Directory.Exists(exportComponent.ExportFilePath));
					
					if (!valid)
					{
						message = SR.MessageDirectoryDoesNotExist;
					}
				}

				if (valid)
					return new ValidationResult(true, "");
				else
					return new ValidationResult(false, message);
			}

			#endregion
		}

		#endregion
		
		private List<ImageExporterInfo> _imageExporters;
		private ImageExporterInfo _selectedImageExporter;
		private readonly int _numberOfImagesToExport;
		private string _exportFilePath;
		private ExportOption _exportOption;

		internal ImageExportComponent(int numberOfImagesToExport)
		{
			Platform.CheckPositive(numberOfImagesToExport, "numberOfImagesToExport");
			_numberOfImagesToExport = numberOfImagesToExport;
		}

		internal IImageExporter SelectedExporter
		{
			get
			{
				if (_selectedImageExporter == null)
					return null;

				return _selectedImageExporter.ImageExporter;
			}
		}

		internal ExportOption ExportOption
		{
			get { return _exportOption; }
		}
		
		public override void Start()
		{
			Validation.Add(new PathValidationRule());

			LoadExporters();
			SetDefaults();

			base.Start();
		}
		
		#region Presentation Model

		public ICollection<ImageExporterInfo> ImageExporters
		{
			get { return _imageExporters; }
		}

		public ImageExporterInfo SelectedImageExporter
		{
			get { return _selectedImageExporter; }
			set
			{
				if (!_imageExporters.Contains(value))
					throw new ArgumentException("The specified image exporter does not exist.");

				_selectedImageExporter = value;

				NotifyPropertyChanged("SelectedImageExporter");
				NotifyPropertyChanged("ConfigureEnabled");
			}
		}

		public int NumberOfImagesToExport
		{
			get { return _numberOfImagesToExport; }
		}

		public bool ExportFilePathEnabled
		{
			get { return false; }	
		}

		public string ExportFilePathLabel
		{
			get { return _numberOfImagesToExport > 1 ? SR.LabelExportPath : SR.LabelExportFile; }
		}

		public string ExportFilePath
		{
			get { return _exportFilePath; }
			set
			{
				_exportFilePath = value;

				Modified = true;
				NotifyPropertyChanged("ExportFilePath");
				NotifyPropertyChanged("AcceptEnabled");
			}
		}

		public bool OptionWysiwyg
		{
			get
			{
				return _exportOption == ExportOption.Wysiwyg;
			}
			set
			{
				if (!value)
					_exportOption = ExportOption.CompleteImage;
			}
		}

		public bool OptionCompleteImage
		{
			get
			{
				return _exportOption == ExportOption.CompleteImage;
			}
			set
			{
				if (!value)
					_exportOption = ExportOption.Wysiwyg;
			}
		}

		public bool AcceptEnabled
		{
			get { return Modified; }
		}

		public bool ConfigureEnabled
		{
			get
			{
				if (_selectedImageExporter == null)
					return false;

				return _selectedImageExporter.IsConfigurable;
			}
		}

		public bool ConfigureVisible
		{
			get
			{
				return CollectionUtils.Contains(_imageExporters,
				                                delegate(ImageExporterInfo info)
				                                	{
				                                		return info.IsConfigurable;
				                                	});
			}	
		}

		public void Configure()
		{
			IConfigurableImageExporter exporter = SelectedExporter as IConfigurableImageExporter;

			if (exporter == null)
				return;

			try
			{
				IApplicationComponent component = exporter.GetConfigurationComponent();
				if (component == null)
					return;

				string title = String.Format("{0} ({1})", SR.ConfigureImageExport, exporter.Description);
				ApplicationComponent.LaunchAsDialog(Host.DesktopWindow, component, title);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				Host.DesktopWindow.ShowMessageBox(SR.MessageErrorLaunchingConfigurationComponent, MessageBoxActions.Ok);
			}
		}

		public void Accept()
		{
			if (HasValidationErrors)
			{
				ShowValidation(true);
			}
			else
			{
				UpdateDefaults();
				_exportFilePath = GetCorrectedExportFilePath();

				ExitCode = ApplicationComponentExitCode.Accepted;
				Host.Exit();
			}
		}

		public void Cancel()
		{
			ExitCode = ApplicationComponentExitCode.None;
			Host.Exit();
		}

		#endregion

		private void LoadExporters()
		{
			_imageExporters = CollectionUtils.Map<object, ImageExporterInfo>
			(
				new ImageExporterExtensionPoint().CreateExtensions(),
				delegate(object extension)
				{
					return new ImageExporterInfo((IImageExporter)extension);
				}
			);

			_imageExporters.AddRange(GetDefaultExporters());

			SortExporters();
		}

		private IEnumerable<ImageExporterInfo> GetDefaultExporters()
		{
			List<IImageExporter> defaultExporters = StandardImageExporterFactory.CreateStandardExporters();
			foreach (IImageExporter defaultExporter in defaultExporters)
			{
				if (!CollectionUtils.Contains(_imageExporters,
					delegate(ImageExporterInfo info)
					{
						return info.ImageExporter.Identifier == defaultExporter.Identifier;
					}))
				{
					yield return new ImageExporterInfo(defaultExporter);
				}
			}
		}

		private void SortExporters()
		{
			_imageExporters.Sort(delegate(ImageExporterInfo x, ImageExporterInfo y)
						{
							return String.Compare(x.Description, y.Description);
						});
		}

		private string GetCorrectedExportFilePath()
		{
			if (_numberOfImagesToExport > 1)
				return _exportFilePath;

			return FileUtilities.CorrectFileNameExtension(_exportFilePath, SelectedExporter.FileExtensions);
		}

		private void SetDefaults()
		{
			_exportOption = (ExportOption)ImageExportSettings.Default.SelectedImageExportOption;

			_selectedImageExporter = CollectionUtils.SelectFirst(_imageExporters,
											delegate(ImageExporterInfo info)
											{
												return info.ImageExporter.Identifier ==
													ImageExportSettings.Default.SelectedImageExporterId;
											});

			if (_selectedImageExporter == null)
				_selectedImageExporter = _imageExporters[0];
		}

		private void UpdateDefaults()
		{
			ImageExportSettings.Default.SelectedImageExportOption = (int)_exportOption;
			ImageExportSettings.Default.SelectedImageExporterId = SelectedExporter.Identifier;
			ImageExportSettings.Default.Save();
		}
	}
}
