#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
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

		public Study GetStudy(int oid)
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

        public void DeleteStudy(string studyInstanceUid)
        {
            //TODO (Marmot): is this how this is even going to work?
            throw new NotImplementedException();
        }

        internal void DeleteAll()
        {
            this.Context.Studies.DeleteAllOnSubmit(from s in Context.Studies select s);
        }
	}
}
