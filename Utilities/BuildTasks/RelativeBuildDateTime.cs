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
            _buildNumber = timeSince.Days.ToString() + DateTime.Now.Hour.ToString();

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
