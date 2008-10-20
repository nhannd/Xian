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
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A local, file-based implementation of <see cref="ImageSop"/>.
	/// </summary>
	public class LocalImageSop : ImageSop
	{
		/// <summary>
		/// Initializes a new instance of <see cref="LocalImageSop"/> with
		/// a specified filename.
		/// </summary>
		/// <param name="filename"></param>
		public LocalImageSop(string filename) : base(new DicomFile(filename))
		{
		}

		/// <summary>
		/// Gets the filename this <see cref="LocalImageSop"/> was constructed with.
		/// </summary>
		public string Filename
		{
			get
			{
				return DicomFile.Filename;
			}
		}

		private DicomFile DicomFile
		{
			get { return NativeDicomObject as DicomFile; }
		}

		/// <summary>
		/// Called to ensure that all DICOM data is loaded into memory.
		/// </summary>
		/// <remarks>
		/// <para>Subclasses of <see cref="Sop"/> should override this method to provide any necessary additional logic to load all
		/// DICOM data into memory.</para>
		/// </remarks>
		protected override void EnsureLoaded()
		{
            DicomFile.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences);
		}
	}
}
