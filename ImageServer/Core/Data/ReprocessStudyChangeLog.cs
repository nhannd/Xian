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