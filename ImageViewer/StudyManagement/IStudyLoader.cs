using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    public interface IStudyLoader
    {
		string Name { get; }
		void Start(string studyInstanceUID);
		ImageSop LoadNextImage();
		void Stop();
    }
}
