using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;

namespace ClearCanvas.ImageServer.Model
{
    public class FilesystemTier : ServerEntity
    {
        #region Constructors
        public FilesystemTier()
            : base("FilesystemTier")
        {
        }
        #endregion

        #region Private Members
        private String _description;
        private int _tierId;
        #endregion

        #region Public Properties
        private String Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private int TierId
        {
            get { return _tierId; }
            set { _tierId = value; }
        }
        #endregion
    }
}
