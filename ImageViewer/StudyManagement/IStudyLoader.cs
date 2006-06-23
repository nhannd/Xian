using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workstation.Model.StudyManagement
{
    public interface IStudyLoader
    {
		string Name { get; }
        void LoadStudy(string studyUID);
    }
}
