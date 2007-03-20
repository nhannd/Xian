using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Configuration
{
    /// <summary>
    /// Extension point for views onto <see cref="DiskspaceManagerConfigurationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class DiskspaceManagerConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// DiskspaceManagerConfigurationComponent class
    /// </summary>
    [AssociateView(typeof(DiskspaceManagerConfigurationComponentViewExtensionPoint))]
    public class DiskspaceManagerConfigurationComponent : ConfigurationApplicationComponent
    {
        private string _driveName;
        private float _lowWatermark;
        private float _highWatermark;
        private float _spaceUsed;

        /// <summary>
        /// Constructor
        /// </summary>
        public DiskspaceManagerConfigurationComponent()
        {
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        public override void Save()
        {
            // to do
        }

        #region Properties

        public string DriveName
        {
            get { return _driveName; }
            set
            {
                _driveName = value;
                this.Modified = true;
            }
        }

        public float LowWatermark
        {
            get { return _lowWatermark; }
            set
            {
                _lowWatermark = value;
                this.Modified = true;
            }
        }

        public float HighWatermark
        {
            get { return _highWatermark; }
            set
            {
                _highWatermark = value;
                this.Modified = true;
            }
        }

        public float SpaceUsed
        {
            get { return _spaceUsed; }
            set
            {
                _spaceUsed = value;
                this.Modified = true;
            }
        }

        #endregion

    }
}
