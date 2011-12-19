#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// DatabaseVersion entity
    /// </summary>	
    public class PersistentStoreVersion : Core.Entity
    {
        #region Private fields
     	
        private string _major;
        private string _minor;
        private string _build;
        private string _revision;
	  	
        #endregion
	  	
        #region Constructors
	  	
        /// <summary>
        /// Default no-args constructor required by NHibernate
        /// </summary>
        public PersistentStoreVersion()
        {
        }

        /// <summary>
        /// All fields constructor
        /// </summary>
        public PersistentStoreVersion(string major1, string minor1, string build1, string revision1)
        {
            _major = major1;
            _minor = minor1;
            _build = build1;		  	
            _revision = revision1;		  	
        }
		
        #endregion
	  	
        #region Public Properties

        [PersistentProperty]
        [Required]
        [Length(5)]
        public virtual string Major
        {
            get { return _major; }
            set { _major = value; }
        }

        [PersistentProperty]
        [Required]
        [Length(5)]
        public virtual string Minor
        {
            get { return _minor; }
            set { _minor = value; }
        }

        [PersistentProperty]
        [Required]
        [Length(5)]
        public virtual string Build
        {
            get { return _build; }
            set { _build = value; }
        }		
		
        [PersistentProperty]
        [Required]
        [Length(5)]
        public virtual string Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }
	  	
        #endregion
    }
}