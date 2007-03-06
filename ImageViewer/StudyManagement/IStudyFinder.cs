using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Defines a study finder.
	/// </summary>
	/// <remarks>
	/// <see cref="IStudyFinder"/> abstracts the finding of studies,
	/// allowing different many means of finding studies (e.g., local database,
	/// DICOM query, DICOMDIR, etc.) to be treated in the same way..
	/// </remarks>
    public interface IStudyFinder
    {
		/// <summary>
		/// Gets the name of the study finder.
		/// </summary>
        string Name { get; }

		/// <summary>
		/// Queries for a study matching the specified query parameters.
		/// </summary>
		/// <param name="queryParams"></param>
		/// <returns></returns>
        StudyItemList Query(QueryParameters queryParams);

		/// <summary>
		/// Queries for a study on a target server matching the specified query parameters.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="targetServerObject"></param>
		/// <param name="queryParams"></param>
		/// <returns></returns>
        StudyItemList Query<T>(T targetServerObject, QueryParameters queryParams);
    }
}
