#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	public abstract class Broker
	{
		protected Broker(DicomStoreDataContext context)
		{
			Context = context;	
		}

		protected DicomStoreDataContext Context { get; private set; }
	}
}
