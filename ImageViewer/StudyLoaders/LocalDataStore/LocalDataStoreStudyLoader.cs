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
using System.Text;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom.DataStore;
using ClearCanvas.Dicom;
using System.Collections.ObjectModel;

namespace ClearCanvas.ImageViewer.StudyLoaders.LocalDataStore
{
    [ExtensionOf(typeof(ClearCanvas.ImageViewer.StudyManagement.StudyLoaderExtensionPoint))]
    public class LocalDataStoreStudyLoader : IStudyLoader
    {
		private IEnumerator<ISopInstance> _sops;

        public LocalDataStoreStudyLoader()
        {

        }

        public string Name
        {
            get
            {
                return "DICOM_LOCAL";
            }
        }

		public int Start(string studyInstanceUID)
		{
			using (IDataStoreReader reader = DataAccessLayer.GetIDataStoreReader())
			{
				IStudy study = reader.GetStudy(studyInstanceUID);
				_sops = new List<ISopInstance>(study.GetSopInstances()).GetEnumerator();
				_sops.Reset();

				return study.GetNumberOfSopInstances();
			}
		}

		public ImageSop LoadNextImage()
        {
			ImageSopInstance imageObject;

			while (true)
			{
				bool moreImages = _sops.MoveNext();

				if (!moreImages)
					return null;

				imageObject = _sops.Current as ImageSopInstance;
				
				// TODO: don't just skip non-image sop instances
				if (imageObject != null)
				{
					// Skip non-local images
					if (imageObject.LocationUri.IsFile)
						break;
				}
			}

			LocalDataStoreImageSop localImage = new LocalDataStoreImageSop(imageObject);

			return localImage;
        }
    }
}
