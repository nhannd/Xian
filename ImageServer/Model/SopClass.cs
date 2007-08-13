using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Model
{
    public class SopClass : ServerEntity
    {
        #region Constructors
        public SopClass()
            : base("SopClass")
        {
        }
        #endregion

        #region Private Members
        private String _sopClassUid;
        private String _description;
        private bool _nonImage;
        #endregion

        public String SopClassUid
        {
            get { return _sopClassUid; }
            set { _sopClassUid = value; }
        }
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public bool NonImage
        {
            get { return _nonImage; }
            set { _nonImage = value; }
        }
    }
}
