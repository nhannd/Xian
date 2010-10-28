#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;

namespace ClearCanvas.ImageServer.Core.Reconcile
{
	public class StudyReconcileDescriptorParser
	{
		public StudyReconcileDescriptor Parse(XmlDocument doc)
		{
			if (doc.DocumentElement != null)
			{
				if (doc.DocumentElement.Name == "Reconcile")
				{
					return XmlUtils.Deserialize<StudyReconcileDescriptor>(doc.DocumentElement);
				}
				// Note, the prior software versions had "MergeStudy", "CreateStudy"
				// and "Discard" Document Elements.  With 1.5, they were all changed
				// to "Reconcile".
				if (doc.DocumentElement.Name == "MergeStudy"
				    ||doc.DocumentElement.Name == "CreateStudy"
				    ||doc.DocumentElement.Name == "Discard")
					throw new NotSupportedException(String.Format("ReconcileStudy Command from prior version no longer supported: {0}", doc.DocumentElement.Name));
                
				throw new NotSupportedException(String.Format("ReconcileStudy Command: {0}", doc.DocumentElement.Name));
			}

			return null;
		}
	}
}