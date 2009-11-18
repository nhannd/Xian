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
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer
{
	//NOTE: keep this internal for now, as I'm not too sure of their usefulness right now.

	internal interface IImageSetFactory
	{
		void SetStudyTree(StudyTree studyTree);

		IImageSet CreateImageSet(Study study);
	}

	internal abstract class ImageSetFactory : IImageSetFactory
	{
		private StudyTree _studyTree;
		private readonly IDisplaySetFactory _displaySetFactory;

		public ImageSetFactory()
			: this(new BasicDisplaySetFactory())
		{
		}

		public ImageSetFactory(IDisplaySetFactory displaySetFactory)
		{
			_displaySetFactory = displaySetFactory;
		}

		#region IImageSetFactory Members

		void IImageSetFactory.SetStudyTree(StudyTree studyTree)
		{
			_studyTree = studyTree;
			_displaySetFactory.SetStudyTree(studyTree);
		}

		IImageSet IImageSetFactory.CreateImageSet(Study study)
		{
			Platform.CheckForNullReference(study, "study");
			Platform.CheckMemberIsSet(_studyTree, "_studyTree");

			return CreateImageSet(study);
		}

		#endregion

		protected virtual IImageSet CreateImageSet(Study study)
		{
			ImageSet imageSet = null;
			List<IDisplaySet> displaySets = CreateDisplaySets(study);

			if (displaySets.Count > 0)
			{
				imageSet = new ImageSet(CreateImageSetDescriptor(study.GetIdentifier()));
				
				foreach (IDisplaySet displaySet in displaySets)
					imageSet.DisplaySets.Add(displaySet);
			}

			return imageSet;
		}

		protected virtual List<IDisplaySet> CreateDisplaySets(Study study)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();

			foreach (Series series in study.Series)
				displaySets.AddRange(_displaySetFactory.CreateDisplaySets(series));

			return displaySets;
		}

		protected virtual DicomImageSetDescriptor CreateImageSetDescriptor(IStudyRootStudyIdentifier studyIdentifier)
		{
			return new DicomImageSetDescriptor(studyIdentifier);
		}
	}
}