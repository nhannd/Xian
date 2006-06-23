using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workstation.Model.StudyManagement
{
    public interface IStudyFinder
    {
        string Name { get; }
        StudyItemList Query(QueryParameters queryParams);
        StudyItemList Query<T>(T targetServerObject, QueryParameters queryParams);
    }
}
