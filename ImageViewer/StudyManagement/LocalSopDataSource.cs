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

using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// An <see cref="ISopDataSource"/> whose underlying data resides in a <see cref="DicomFile"/>
	/// on the local file system.
	/// </summary>
	/// <remarks>
	/// Pixel data is always loaded from the source file on-demand.
	/// </remarks>
	public class LocalSopDataSource : DicomMessageSopDataSource, ILocalSopDataSource
	{
		/// <summary>
		/// Constructs a new <see cref="LocalSopDataSource"/> by loading
		/// the <see cref="DicomFile"/> with the given <paramref name="fileName">file name</paramref>.
		/// </summary>
		/// <param name="fileName">The full path to the file to be loaded.</param>
		public LocalSopDataSource(string fileName)
			: base(new DicomFile(fileName))
		{
		}

		/// <summary>
		/// Constructs a new <see cref="LocalSopDataSource"/> with the given <see cref="DicomFile"/>
		/// as it's underlying data.
		/// </summary>
		/// <param name="localFile">The local file.</param>
		public LocalSopDataSource(DicomFile localFile)
			: base(localFile)
		{
		}

		#region ILocalSopDataSource Members

		/// <summary>
		/// Gets the source <see cref="DicomFile"/>.
		/// </summary>
		/// <remarks>See the remarks for <see cref="IDicomMessageSopDataSource.SourceMessage"/>.
		/// This property will likely be removed in a future version due to thread-safety concerns.</remarks>
		public DicomFile File
		{
			get { return (DicomFile)SourceMessage; }
		}

		/// <summary>
		/// Gets the filename of the source <see cref="DicomFile"/>.
		/// </summary>
		public string Filename
		{
			get { return File.Filename; }
		}

		/// <summary>
		/// Called by the base class to ensure that all DICOM data attributes are loaded.
		/// </summary>
		protected override void EnsureLoaded()
		{
			File.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences);
		}

		#endregion
	}
}
