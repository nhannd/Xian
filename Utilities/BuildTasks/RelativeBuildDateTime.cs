#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace ClearCanvas.Utilities.BuildTasks
{
    public class RelativeBuildDateTime : Microsoft.Build.Utilities.Task
    {
        //Outputs a Build Datetime that is a relative datetime from the date of ClearCanvas Inc incorporation
        //Build Datatime is the concatenation of <Number of days since November 3, 2005> and <Number of hours since midnight>

        public override bool Execute()
        {
            TimeSpan timeSince = DateTime.Now - _incorporationDate;
            int inHours = (int) timeSince.TotalHours;
            _buildNumber = inHours < UInt16.MaxValue ? inHours.ToString() : "0";

            return true;
        }

        [Output]
        public string BuildNumber
        {
            get { return _buildNumber; }
        }

        private static DateTime _incorporationDate = new DateTime(2005, 11, 3);
        private string _buildNumber;
    }
}
