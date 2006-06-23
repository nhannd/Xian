using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    public interface IStudyFinder
    {
        string Name { get; }
        StudyItemList Query(QueryParameters queryParams);
        StudyItemList Query<T>(T targetServerObject, QueryParameters queryParams);
    }
}
