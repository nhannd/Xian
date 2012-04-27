using System.IO;

namespace ClearCanvas.ImageViewer.Common
{
    public class DriveInfo
    {
        private readonly System.IO.DriveInfo _real;

        private long? _totalSize;
        private long? _totalFreeSpace;
        private long? _availableFreeSpace;

        public DriveInfo(string name)
            :this (new System.IO.DriveInfo(name))
        {
        }

        private DriveInfo(System.IO.DriveInfo real)
        {
            _real = real;
        }

        /// <summary>
        /// For unit tests.
        /// </summary>
        internal DriveInfo()
        {
        }

        public void Refresh()
        {
            _totalSize = null;
            _totalFreeSpace = null;
            _availableFreeSpace = null;
        }

        public bool IsReady
        {
            get { return _real.IsReady; }
        }

        public string Name
        {
            get { return _real.Name; }
        }

        public DriveType DriveType
        {
            get { return _real.DriveType; }
        }

        public string DriveFormat
        {
            get { return _real.DriveFormat; }
        }

        public string VolumeLabel
        {
            get { return _real.VolumeLabel; }
        }

        //private long UnavailableFreeSpace
        //{
        //    get { return TotalFreeSpace - AvailableFreeSpace; }
        //}

        //public long EffectiveSize
        //{
        //    get { return TotalSize - UnavailableFreeSpace; }
        //}

        public long TotalSize
        {
            get { return _totalSize.HasValue ? _totalSize.Value : (_totalSize = _real.TotalSize).Value; }
            internal set { _totalSize = value; }
        }

        public long TotalUsedSpace
        {
            get { return TotalSize - TotalFreeSpace; }
        }

        public double TotalUsedSpacePercent
        {
            get
            {
                return (double)TotalUsedSpace / TotalSize * 100;
            }
        }

        public long TotalFreeSpace
        {
            get { return _totalFreeSpace.HasValue ? _totalFreeSpace.Value : (_totalFreeSpace = _real.TotalFreeSpace).Value; }
            internal set { _totalFreeSpace = value; }
        }

        public double TotalFreeSpacePercent
        {
            get
            {
                return (double)TotalFreeSpace / TotalSize * 100;
            }
        }

        public long AvailableFreeSpace
        {
            get { return _availableFreeSpace.HasValue ? _availableFreeSpace.Value : (_availableFreeSpace = _real.AvailableFreeSpace).Value; }
            internal set { _availableFreeSpace = value; }
        }

        public double AvailableFreeSpacePercent
        {
            get 
            {
                return (double)AvailableFreeSpace / TotalSize * 100;
            }
        }

        public static implicit operator System.IO.DriveInfo(DriveInfo info)
        {
            return info._real;
        }

        public static implicit operator DriveInfo(System.IO.DriveInfo info)
        {
            return new DriveInfo(info);
        }
    }
}
