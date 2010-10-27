#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Core.Data
{
    public class ReprocessStudyChangeLog
    {
        #region Private Members

    	#endregion

        #region Public Properties

    	public DateTime TimeStamp { get; set; }

    	public string Reason { get; set; }

    	public string User { get; set; }

    	public string StudyInstanceUid { get; set; }

    	#endregion
    }
}