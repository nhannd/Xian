using System;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.SeriesDetails
{
    static public class SeriesDetailsAssembler
    {
        /// <summary>
        /// Returns an instance of <see cref="SeriesDetails"/> for a <see cref="Series"/>.
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        /// <remark>
        /// 
        /// </remark>
        static public SeriesDetails CreateSeriesDetails(Model.Series series)
        {
            SeriesDetails details = new SeriesDetails();

            details.Modality = series.Modality;
            details.NumberOfSeriesRelatedInstances = series.NumberOfSeriesRelatedInstances;
            details.PerformedDateTime = DateTime.Now;
            details.SeriesDescription = series.SeriesDescription;
            details.SeriesInstanceUid = series.SeriesInstanceUid;
            details.SeriesNumber = series.SeriesNumber;
            details.SourceApplicationEntityTitle = series.SourceApplicationEntityTitle;

            return details;
        }
    }
}
