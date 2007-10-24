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

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Annotations;

namespace ClearCanvas.ImageViewer.AnnotationProviders
{
	public class AnnotationResourceResolver : ResourceResolver, IAnnotationResourceResolver
	{
		protected char replaceChar = '.';
		protected char replaceWithChar = '_';

		public AnnotationResourceResolver(object target)
			: base(new Assembly[] { target.GetType().Assembly, typeof(AnnotationResourceResolver).Assembly })
		{
		}

		public AnnotationResourceResolver(Assembly assembly)
			: base(assembly)
		{
		}

		public virtual string ResolveLabel(string annotationIdentifier)
		{
			Platform.CheckForEmptyString(annotationIdentifier, "annotationIdentifier"); 
			
			string resourceString = String.Format("{0}{1}{2}", annotationIdentifier, replaceChar, "Label");
			resourceString = resourceString.Replace(replaceChar, replaceWithChar);

			return base.LocalizeString(resourceString) ?? "";
		}

		public virtual string ResolveDisplayName(string annotationIdentifier)
		{
			Platform.CheckForEmptyString(annotationIdentifier, "annotationIdentifier"); 
			
			string resourceString = String.Format("{0}{1}{2}", annotationIdentifier, replaceChar, "DisplayName");
			resourceString = resourceString.Replace(replaceChar, replaceWithChar);

			string displayName = base.LocalizeString(resourceString);

			if (displayName == resourceString)
				throw new InvalidOperationException(String.Format(SR.ExceptionFormatAnnotationItemHasNoDisplayName, annotationIdentifier));

			return displayName;
		}
	}
}
