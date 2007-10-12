#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	public abstract class AnnotationItem : IAnnotationItem
	{
		private string _identifier;
		private string _displayName;
		private string _label;

		protected AnnotationItem(string identifier, string displayName, string label)
		{
			Platform.CheckForEmptyString(identifier, "identifier"); 
			
			_identifier = identifier;
			_displayName = displayName;
			_label = label;
		}

		protected AnnotationItem()
		{ 
		}

		public string Identifier
		{
			get { return _identifier; }
			protected set
			{
				Platform.CheckForEmptyString(value, "value"); 
				_identifier = value;
			}
		}

		public string DisplayName
		{
			get { return _displayName; }
			protected set { _displayName = value; }
		}

		public string Label
		{
			get { return _label; }
			protected set { _label = value; }
		}

		#region IAnnotationItem Members

		public string GetIdentifier()
		{
			return _identifier;
		}

		public string GetDisplayName()
		{
			return _displayName;
		}

		public string GetLabel()
		{
			return _label;
		}

		public abstract string GetAnnotationText(IPresentationImage presentationImage);

		#endregion
	}
}
