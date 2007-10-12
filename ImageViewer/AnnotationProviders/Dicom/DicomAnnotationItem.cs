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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Annotations;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Dicom
{
	public delegate T SopDataRetrieverDelegate<T>(ImageSop imageSop);

	public class DicomAnnotationItem <T>: ResourceResolvingAnnotationItem
	{
		private SopDataRetrieverDelegate<T> _sopDataRetrieverDelegate;
		private ResultFormatterDelegate<T> _resultFormatterDelegate;

		public DicomAnnotationItem
			(
				string identifier,
				IAnnotationItemProvider ownerProvider,
				SopDataRetrieverDelegate<T> sopDataRetrieverDelegate,
				ResultFormatterDelegate<T> resultFormatterDelegate
			)
			: this(identifier, ownerProvider, sopDataRetrieverDelegate, resultFormatterDelegate, false)
		{
		}

		public DicomAnnotationItem
			(
				string identifier,
				IAnnotationItemProvider ownerProvider,
				SopDataRetrieverDelegate<T> sopDataRetrieverDelegate,
				ResultFormatterDelegate<T> resultFormatterDelegate,
				bool allowEmptyLabel
			)
			: this(identifier, ownerProvider, sopDataRetrieverDelegate, resultFormatterDelegate, allowEmptyLabel, null)
		{
		}

		public DicomAnnotationItem
			(
				string identifier,
				IAnnotationItemProvider ownerProvider,
				SopDataRetrieverDelegate<T> sopDataRetrieverDelegate,
				ResultFormatterDelegate<T> resultFormatterDelegate,
				bool allowEmptyLabel,
				IAnnotationResourceResolver resolver
			)
			: base(identifier, ownerProvider, allowEmptyLabel, resolver)
		{
			Platform.CheckForNullReference(sopDataRetrieverDelegate, "sopDataRetrieverDelegate");
			Platform.CheckForNullReference(resultFormatterDelegate, "resultFormatterDelegate");

			_sopDataRetrieverDelegate = sopDataRetrieverDelegate;
			_resultFormatterDelegate = resultFormatterDelegate;
		}

		public override string GetAnnotationText(IPresentationImage presentationImage)
		{
			IImageSopProvider associatedDicom = presentationImage as IImageSopProvider;
			if (associatedDicom == null)
				return "";

			return _resultFormatterDelegate(_sopDataRetrieverDelegate(associatedDicom.ImageSop));
		}
	}
}
