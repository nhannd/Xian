#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
        private bool _loaded;
        private bool _loading;

        /// <summary>
        /// Constructs a new <see cref="LocalSopDataSource"/> by loading
        /// the <see cref="DicomFile"/> with the given <paramref name="fileName">file name</paramref>.
        /// </summary>
        /// <param name="fileName">The full path to the file to be loaded.</param>
        public LocalSopDataSource(string fileName)
            : base(new DicomFile(fileName))
        {
            UpdateLoaded();
        }

        /// <summary>
        /// Constructs a new <see cref="LocalSopDataSource"/> with the given <see cref="DicomFile"/>
        /// as it's underlying data.
        /// </summary>
        /// <param name="localFile">The local file.</param>
        public LocalSopDataSource(DicomFile localFile)
            : base(localFile)
        {
           UpdateLoaded();
        }

        #region ILocalSopDataSource Members

        /// <summary>
        /// Gets the source <see cref="DicomFile"/>.
        /// </summary>
        /// <remarks>See the remarks for <see cref="IDicomMessageSopDataSource.SourceMessage"/>.
        /// This property will likely be removed in a future version due to thread-safety concerns.</remarks>
        public DicomFile File
        {
            get { return (DicomFile) GetSourceMessage(true); }
        }

        /// <summary>
        /// Gets the filename of the source <see cref="DicomFile"/>.
        /// </summary>
        public string Filename
        {
            get { return ((DicomFile) GetSourceMessage(false)).Filename; }
        }

        protected override void SetSourceMessage(DicomMessageBase sourceMessage)
        {
            base.SetSourceMessage(sourceMessage);
            UpdateLoaded();
        
        }

        private void UpdateLoaded()
        {
            _loaded = !GetSourceMessage(false).DataSet.IsEmpty();
        }

        protected override void Load()
        {
            lock (SyncLock)
            {
                if (_loaded || _loading)
                    return;
                try
                {
                    _loading = true;
                    File.Load(DicomReadOptions.Default | DicomReadOptions.StorePixelDataReferences);
                    _loaded = true;
                }
                finally
                {
                    _loading = false;
                }
            }
        }

		#endregion

        public override void UnloadAttributes()
        {
           //no-op
        }
    }
}
