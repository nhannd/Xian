using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Model
{
    public class WebDeleteStudyData
    {
        private string _reason;

        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }
    }
}
