using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Dicom.Tests;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.Tests
{
    public class TestBase : AbstractTest
    {
        protected Study CreateTestStudy1()
        {
            var studyUid = "1.2.3";
            var sops = base.SetupMRSeries(4, 5, studyUid);
            var xml = new StudyXml(studyUid);

            var seriesUids = new Dictionary<string, string>();
            var seriesModalities = new Dictionary<string, string>();
            var modalities = new[] { "MR", "MR", "SC", "KO" };

            foreach (var sop in sops)
            {
                //Make the UIDs constant.
                var seriesUid = sop[DicomTags.SeriesInstanceUid].ToString();
                if (!seriesUids.ContainsKey(seriesUid))
                {
                    seriesModalities[seriesUid] = modalities[seriesUids.Count];
                    seriesUids[seriesUid] = string.Format("1.2.3.{0}", seriesUids.Count + 1);
                }

                var modality = seriesModalities[seriesUid];
                seriesUid = seriesUids[seriesUid];

                sop[DicomTags.SeriesInstanceUid].SetString(0, seriesUid);
                sop[DicomTags.Modality].SetString(0, modality);

                var file = new DicomFile("", new DicomAttributeCollection(), sop);
                xml.AddFile(file);
            }

            var study = new Study();
            study.Initialize(xml);
            return study;
        }
    }
}
