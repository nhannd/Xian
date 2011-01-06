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
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Core.Data
{
    public class ProcessDuplicateChangeLog
    {
        #region Constructors

    	public ProcessDuplicateChangeLog()
    	{
    		TimeStamp = Platform.Time;
    	}

    	#endregion

        #region Public Properties

    	public DateTime TimeStamp { get; set; }

    	public ProcessDuplicateAction Action { get; set; }

    	public ImageSetDetails DuplicateDetails { get; set; }

    	public StudyInformation StudySnapShot { get; set; }

    	[XmlArray("StudyUpdateCommands")]
    	[XmlArrayItem("Command", Type = typeof (AbstractProperty<BaseImageLevelUpdateCommand>))]
    	public List<BaseImageLevelUpdateCommand> StudyUpdateCommands { get; set; }

        public string UserName { get; set; }

    	#endregion
    }
}