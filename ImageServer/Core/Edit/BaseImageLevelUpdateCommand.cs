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
		private DicomTagPath _tagPath = new DicomTagPath();
		private object _value;
	    private string _originalValue;
		#endregion

		#region Public Properties

		public DicomTagPath TagPath
		{
			get { return _tagPath; }
			set { _tagPath = value; }
		}

		/// <summary>
		/// Gets or sets the value of the tag to be updated
		/// </summary>
		public object Value
		{
			get { return _value; }
			set { _value = value; }
		}

	    /// <summary>
		/// Gets or sets the original value in the tag to be updated.
		/// </summary>
		public string OriginalValue
	    {
	        get { return _originalValue; }
	        set { _originalValue = value; }
	    }


	    /// <summary>
		/// Gets the value of the tag as a string 
		/// </summary>
		/// <returns></returns>
		public string GetStringValue()
		{
			if (_value == null)
				return null;
			else
				return _value.ToString();
		}

		#endregion

	}

	public abstract class BaseImageLevelUpdateCommand : ServerCommand, IUpdateImageTagCommand
	{
		private DicomFile _file;
		private string _name;
	    protected ImageLevelUpdateEntry _updateEntry = new ImageLevelUpdateEntry();


	    public BaseImageLevelUpdateCommand()
			: base("ImageLevelUpdateCommand", true)
		{
		}

		public BaseImageLevelUpdateCommand(string name)
			: base("ImageLevelUpdateCommand", true)
		{
		    _name = name;
		    Description = "Update Dicom Tag";
		}

	    #region IActionItem<DicomFile> Members

		public abstract bool Apply(DicomFile file);

		#endregion

		#region IImageLevelUpdateOperation Members

		[XmlIgnore]
		public string CommandName
		{
			get { return _name; }
			set { _name = value; }
		}

		[XmlIgnore]
		public DicomFile File
		{
			set { _file = value; }
		}

	    /// <summary>
	    /// Gets or sets the <see cref="ImageLevelUpdateEntry"/> for this command.
	    /// </summary>
	    [XmlIgnore]
	    public ImageLevelUpdateEntry UpdateEntry
	    {
	        get { return _updateEntry; }
	        set { _updateEntry = value; }
	    }

	    #endregion

		protected override void OnExecute(ServerCommandProcessor theProcessor)
		{
			if (_file != null)
				Apply(_file);
		}

		protected override void OnUndo()
		{
			// NO-OP
		}

		#region IImageLevelUpdateCommand Members

       
		#endregion
	}
}