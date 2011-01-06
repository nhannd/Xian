#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
    [Serializable]
    public class PersistentStoreVersion: ServerEntity
    {
        #region Constructors
        public PersistentStoreVersion():base("DatabaseVersion_")
        {}
		public PersistentStoreVersion(
             String _build__
            ,String _major__
            ,String _minor__
            ,String _revision__
            ):base("DatabaseVersion_")
        {
            _build_ = _build__;
            _major_ = _major__;
            _minor_ = _minor__;
            _revision_ = _revision__;
        }
        #endregion

        #region Private Members
        private String _build_;
        private String _major_;
        private String _minor_;
        private String _revision_;
        #endregion

        #region Public Properties
        [EntityFieldDatabaseMappingAttribute(TableName="DatabaseVersion_", ColumnName="Build_")]
        public String Build
        {
        get { return _build_; }
        set { _build_ = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="DatabaseVersion_", ColumnName="Major_")]
        public String Major
        {
        get { return _major_; }
        set { _major_ = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="DatabaseVersion_", ColumnName="Minor_")]
        public String Minor
        {
        get { return _minor_; }
        set { _minor_ = value; }
        }
        [EntityFieldDatabaseMappingAttribute(TableName="DatabaseVersion_", ColumnName="Revision_")]
        public String Revision
        {
        get { return _revision_; }
        set { _revision_ = value; }
        }
        #endregion

		static public PersistentStoreVersion Insert(PersistentStoreVersion entity)
        {
            using (IUpdateContext update = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
				PersistentStoreVersion newEntity = Insert(update, entity);
                update.Commit();
                return newEntity;
            }
        }
		static public PersistentStoreVersion Insert(IUpdateContext update, PersistentStoreVersion entity)
        {
			IPersistentStoreVersionEntityBroker broker = update.GetBroker<IPersistentStoreVersionEntityBroker>();
            PersistentStoreVersionUpdateColumns updateColumns = new PersistentStoreVersionUpdateColumns();
            updateColumns.Build = entity.Build;
            updateColumns.Major = entity.Major;
            updateColumns.Minor = entity.Minor;
            updateColumns.Revision = entity.Revision;
			PersistentStoreVersion newEntity = broker.Insert(updateColumns);
            return newEntity;
        }
    }
}
