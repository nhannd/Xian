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
