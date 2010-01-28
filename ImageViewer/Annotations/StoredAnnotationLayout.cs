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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	[Cloneable]
	internal sealed class StoredAnnotationLayout : IAnnotationLayout
	{
		private readonly string _identifier;
		private readonly List<StoredAnnotationBoxGroup> _annotationBoxGroups = new List<StoredAnnotationBoxGroup>();
		private bool _visible = true;

		public StoredAnnotationLayout(string identifier)
		{
			Platform.CheckForEmptyString(identifier, "identifier");
			_identifier = identifier;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">This parameter is unused.</param>
		private StoredAnnotationLayout(StoredAnnotationLayout source, ICloningContext context)
		{
			this._identifier = source._identifier;
			this._visible = source._visible;
			foreach (StoredAnnotationBoxGroup group in source._annotationBoxGroups)
			{
				if (group == null)
					continue;
				this._annotationBoxGroups.Add(group.Clone());
			}
		}

		public string Identifier
		{
			get { return _identifier; }
		}

		public IList<StoredAnnotationBoxGroup> AnnotationBoxGroups
		{
			get { return _annotationBoxGroups; }
		}

		#region IAnnotationLayout Members

		public IEnumerable<AnnotationBox> AnnotationBoxes
		{
			get
			{
				foreach (StoredAnnotationBoxGroup group in _annotationBoxGroups)
				{
					foreach (AnnotationBox box in group.AnnotationBoxes)
						yield return box;
				}
			}
		}

		public bool Visible
		{
			get { return _visible; }
			set { _visible = value; }
		}

		#endregion

		public StoredAnnotationLayout Clone()
		{
			return new StoredAnnotationLayout(this, null);
		}
	}
}
