#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Tools.Standard.Configuration
{
	public sealed class ToolModalityBehavior : INotifyPropertyChanged, IXmlSerializable
	{
		private event PropertyChangedEventHandler _propertyChanged;
		private bool _selectedImageWindowLevelTool;
		private bool _selectedImageWindowLevelPresetsTool;
		private bool _selectedImageColorMapTool;
		private bool _selectedImageInvertTool;
		private bool _selectedImageZoomTool;
		private bool _selectedImagePanTool;
		private bool _selectedImageFlipTool;
		private bool _selectedImageRotateTool;
		private bool _selectedImageResetTool;

		public ToolModalityBehavior() {}

		public ToolModalityBehavior(ToolModalityBehavior source)
		{
			if (source == null)
				return;

			SelectedImageColorMapTool = source.SelectedImageColorMapTool;
			SelectedImageFlipTool = source.SelectedImageFlipTool;
			SelectedImageInvertTool = source.SelectedImageInvertTool;
			SelectedImagePanTool = source.SelectedImagePanTool;
			SelectedImageResetTool = source.SelectedImageResetTool;
			SelectedImageRotateTool = source.SelectedImageRotateTool;
			SelectedImageWindowLevelPresetsTool = source.SelectedImageWindowLevelPresetsTool;
			SelectedImageWindowLevelTool = source.SelectedImageWindowLevelTool;
			SelectedImageZoomTool = source.SelectedImageZoomTool;
		}

		/// <summary>
		/// Configures whether or not the <see cref="WindowLevelTool"/> is only applied to the selected image.
		/// </summary>
		public bool SelectedImageWindowLevelTool
		{
			get { return _selectedImageWindowLevelTool; }
			set
			{
				if (_selectedImageWindowLevelTool != value)
				{
					_selectedImageWindowLevelTool = value;
					NotifyPropertyChanged("SelectedImageWindowLevelTool");
				}
			}
		}

		/// <summary>
		/// Configures whether or not the preset functions of the <see cref="WindowLevelTool"/> are only applied to the selected image.
		/// </summary>
		public bool SelectedImageWindowLevelPresetsTool
		{
			get { return _selectedImageWindowLevelPresetsTool; }
			set
			{
				if (_selectedImageWindowLevelPresetsTool != value)
				{
					_selectedImageWindowLevelPresetsTool = value;
					NotifyPropertyChanged("SelectedImageWindowLevelPresetsTool");
				}
			}
		}

		/// <summary>
		/// Configures whether or not the preset functions of the <see cref="ColorMapTool"/> are only applied to the selected image.
		/// </summary>
		public bool SelectedImageColorMapTool
		{
			get { return _selectedImageColorMapTool; }
			set
			{
				if (_selectedImageColorMapTool != value)
				{
					_selectedImageColorMapTool = value;
					NotifyPropertyChanged("SelectedImageColorMapTool");
				}
			}
		}

		/// <summary>
		/// Configures whether or not the <see cref="InvertTool"/> is only applied to the selected image.
		/// </summary>
		public bool SelectedImageInvertTool
		{
			get { return _selectedImageInvertTool; }
			set
			{
				if (_selectedImageInvertTool != value)
				{
					_selectedImageInvertTool = value;
					NotifyPropertyChanged("SelectedImageInvertTool");
				}
			}
		}

		/// <summary>
		/// Configures whether or not the <see cref="ZoomTool"/> is only applied to the selected image.
		/// </summary>
		public bool SelectedImageZoomTool
		{
			get { return _selectedImageZoomTool; }
			set
			{
				if (_selectedImageZoomTool != value)
				{
					_selectedImageZoomTool = value;
					NotifyPropertyChanged("SelectedImageZoomTool");
				}
			}
		}

		/// <summary>
		/// Configures whether or not the <see cref="PanTool"/> is only applied to the selected image.
		/// </summary>
		public bool SelectedImagePanTool
		{
			get { return _selectedImagePanTool; }
			set
			{
				if (_selectedImagePanTool != value)
				{
					_selectedImagePanTool = value;
					NotifyPropertyChanged("SelectedImagePanTool");
				}
			}
		}

		/// <summary>
		/// Configures whether or not <see cref="FlipHorizontalTool"/> and <see cref="FlipVerticalTool"/> are only applied to the selected image.
		/// </summary>
		public bool SelectedImageFlipTool
		{
			get { return _selectedImageFlipTool; }
			set
			{
				if (_selectedImageFlipTool != value)
				{
					_selectedImageFlipTool = value;
					NotifyPropertyChanged("SelectedImageFlipTool");
				}
			}
		}

		/// <summary>
		/// Configures whether or not <see cref="RotateLeftTool"/> and <see cref="RotateRightTool"/> are only applied to the selected image.
		/// </summary>
		public bool SelectedImageRotateTool
		{
			get { return _selectedImageRotateTool; }
			set
			{
				if (_selectedImageRotateTool != value)
				{
					_selectedImageRotateTool = value;
					NotifyPropertyChanged("SelectedImageRotateTool");
				}
			}
		}

		/// <summary>
		/// Configures whether or not the <see cref="ResetTool"/> is only applied to the selected image.
		/// </summary>
		public bool SelectedImageResetTool
		{
			get { return _selectedImageResetTool; }
			set
			{
				if (_selectedImageResetTool != value)
				{
					_selectedImageResetTool = value;
					NotifyPropertyChanged("SelectedImageResetTool");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged
		{
			add { _propertyChanged += value; }
			remove { _propertyChanged -= value; }
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			EventsHelper.Fire(_propertyChanged, this, new PropertyChangedEventArgs(propertyName));
		}

		#region Implementation of IXmlSerializable

		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			var empty = reader.IsEmptyElement;
			reader.ReadStartElement();
			if (!empty)
			{
				while (reader.MoveToContent() == XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case @"SelectedImageWindowLevelTool":
							SelectedImageWindowLevelTool = ReadXmlBooleanAttribute(reader, false);
							break;
						case @"SelectedImageWindowLevelPresetsTool":
							SelectedImageWindowLevelPresetsTool = ReadXmlBooleanAttribute(reader, false);
							break;
						case @"SelectedImageColorMapTool":
							SelectedImageColorMapTool = ReadXmlBooleanAttribute(reader, false);
							break;
						case @"SelectedImageInvertTool":
							SelectedImageInvertTool = ReadXmlBooleanAttribute(reader, false);
							break;
						case @"SelectedImagePanTool":
							SelectedImagePanTool = ReadXmlBooleanAttribute(reader, false);
							break;
						case @"SelectedImageZoomTool":
							SelectedImageZoomTool = ReadXmlBooleanAttribute(reader, false);
							break;
						case @"SelectedImageFlipTool":
							SelectedImageFlipTool = ReadXmlBooleanAttribute(reader, false);
							break;
						case @"SelectedImageRotateTool":
							SelectedImageRotateTool = ReadXmlBooleanAttribute(reader, false);
							break;
						case @"SelectedImageResetTool":
							SelectedImageResetTool = ReadXmlBooleanAttribute(reader, false);
							break;
						default:
							// consume the bad element and skip to next sibling or the parent end element tag
							reader.ReadOuterXml();
							break;
					}
				}
				reader.MoveToContent();
				reader.ReadEndElement();
			}
		}

		public void WriteXml(XmlWriter writer)
		{
			WriteXmlBooleanAttribute(writer, @"SelectedImageWindowLevelTool", SelectedImageWindowLevelTool);
			WriteXmlBooleanAttribute(writer, @"SelectedImageWindowLevelPresetsTool", SelectedImageWindowLevelPresetsTool);
			WriteXmlBooleanAttribute(writer, @"SelectedImageColorMapTool", SelectedImageInvertTool);
			WriteXmlBooleanAttribute(writer, @"SelectedImageInvertTool", SelectedImageInvertTool);
			WriteXmlBooleanAttribute(writer, @"SelectedImagePanTool", SelectedImagePanTool);
			WriteXmlBooleanAttribute(writer, @"SelectedImageZoomTool", SelectedImageZoomTool);
			WriteXmlBooleanAttribute(writer, @"SelectedImageFlipTool", SelectedImageFlipTool);
			WriteXmlBooleanAttribute(writer, @"SelectedImageRotateTool", SelectedImageRotateTool);
			WriteXmlBooleanAttribute(writer, @"SelectedImageResetTool", SelectedImageResetTool);
		}

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		private static bool ReadXmlBooleanAttribute(XmlReader reader, bool defaultValue)
		{
			bool result;
			return bool.TryParse(reader.ReadElementString(), out result) ? result : defaultValue;
		}

		private static void WriteXmlBooleanAttribute(XmlWriter writer, string name, bool value)
		{
			writer.WriteElementString(name, value.ToString(CultureInfo.InvariantCulture));
		}

		#endregion
	}
}