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

using System.Xml.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.CommandProcessor;
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

	public abstract class BaseImageLevelUpdateCommand : ServerCommand, IUpdateImageTagCommand
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

		protected override void OnExecute(ServerCommandProcessor theProcessor)
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