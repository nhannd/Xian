#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	public class StudyBroker : Broker
	{
		internal StudyBroker(DicomStoreDataContext context)
			: base(context)
		{
		}

		public void AddStudy(Study study)
		{
			this.Context.Studies.InsertOnSubmit(study);
		}

		public Study GetStudy(long oid)
		{	
            var list = (from s in this.Context.Studies
                        where s.Oid == oid
                        select s).ToList();

            if (!list.Any()) return null;

            return list.First();
		}

		public Study GetStudy(string studyInstanceUid)
		{
            var list = (from s in this.Context.Studies
                        where s.StudyInstanceUid == studyInstanceUid
                        select s).ToList();

            if (!list.Any()) return null;

            return list.First();
		}

        public long GetStudyCount()
        {
            var count = (from s in Context.Studies
                        select s).Count();
            return count;
        }

        public List<long> GetStudyOids()
        {
            var oids = from s in Context.Studies select s.Oid;

            return GetSingleColumn<Study, long>(null, s => s.Oid);
        }

        /// <summary>
        /// Delete Study entity.
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Study entity)
        {
            this.Context.Studies.DeleteOnSubmit(entity);
        }

        /// <summary>
        /// Delete Study entity.
        /// </summary>
	    public void DeleteStudy(string studyInstanceUid)
        {
            this.Context.Studies.DeleteOnSubmit(GetStudy(studyInstanceUid));
	    }

	    internal void DeleteAll()
	    {
            Context.Studies.DeleteAllOnSubmit(from s in Context.Studies select s);
	    }
	}
}
