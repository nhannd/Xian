using System.Web.UI;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Defines the interface of the class that returns the customize
    /// control to display information in different types of
    /// <see cref="StudyHistory"/> record.
    /// 
    /// </summary>
    internal interface IStudyHistoryColumnRendererFactory
    {
        /// <summary>
        /// Returns the <see cref="Control"/> that displays the content of 
        /// the ChangeDescription column of the specified <see cref="StudyHistory"/> record.
        /// </summary>
        /// <param name="historyRecord"></param>
        /// <returns></returns>
        Control GetChangeDescColumnControl(StudyHistory historyRecord);
    }
}