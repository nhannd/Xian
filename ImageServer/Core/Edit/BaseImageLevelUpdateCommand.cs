#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Xml.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Command;
using ClearCanvas.ImageServer.Common.Command;
using ClearCanvas.ImageServer.Common.Helpers;

namespace ClearCanvas.ImageServer.Core.Edit
{
	/// <summary>
	/// Defines the interface of an <see cref="BaseImageLevelUpdateCommand"/> which modifies a tag in a Dicom file.
	/// </summary>
	public interface IUpdateImageTagCommand
	{
		/// <summary>
		/// Gets the <see cref="ImageLevelUpdateEntry"/> associated with the command
		/// </summary>
		ImageLevelUpdateEntry UpdateEntry { get; }

	}

	/// <summary>
	/// Encapsulates the tag update specification
	/// </summary>
	public class ImageLevelUpdateEntry
	{
		#region Private members

		public ImageLevelUpdateEntry()
		{
			TagPath = new DicomTagPath();
		}

		#endregion

		#region Public Properties

		public DicomTagPath TagPath { get; set; }

		/// <summary>
		/// Gets or sets the value of the tag to be updated
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Gets or sets the original value in the tag to be updated.
		/// </summary>
		public string OriginalValue { get; set; }


		/// <summary>
		/// Gets the value of the tag as a string 
		/// </summary>
		/// <returns></returns>
		public string GetStringValue()
	    {
	    	if (Value == null)
				return null;
	    	return Value.ToString();
	    }

		#endregion

	}

	public abstract class BaseImageLevelUpdateCommand : CommandBase, IUpdateImageTagCommand
	{
		protected BaseImageLevelUpdateCommand()
			: base("ImageLevelUpdateCommand", true)
		{
			UpdateEntry = new ImageLevelUpdateEntry();
		}

		protected BaseImageLevelUpdateCommand(string name)
			: base("ImageLevelUpdateCommand", true)
		{
			UpdateEntry = new ImageLevelUpdateEntry();
			CommandName = name;
		    Description = "Update Dicom Tag";
		}

	    #region IActionItem<DicomFile> Members

		public abstract bool Apply(DicomFile file);

		#endregion

		#region IImageLevelUpdateOperation Members

		[XmlIgnore]
		public string CommandName { get; set; }

		[XmlIgnore]
		public DicomFile File { private get; set; }

		/// <summary>
		/// Gets or sets the <see cref="ImageLevelUpdateEntry"/> for this command.
		/// </summary>
		[XmlIgnore]
		public ImageLevelUpdateEntry UpdateEntry { get; set; }

		#endregion

		protected override void OnExecute(CommandProcessor theProcessor)
		{
			if (File != null)
				Apply(File);
		}

		protected override void OnUndo()
		{
			// NO-OP
		}
	}
}