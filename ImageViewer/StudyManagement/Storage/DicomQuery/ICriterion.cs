using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery
{
    public interface IStringPropertyFilter
    {
        string[] CriterionValues { get; }
        bool IsWildcard { get; }

    }
}
