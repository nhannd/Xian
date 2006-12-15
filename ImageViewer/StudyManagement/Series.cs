using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Collections;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class Series
	{
		private Study _parentStudy;
		private string _seriesInstanceUID;
		private SopCollection _sops = new SopCollection();

		internal Series(string seriesInstanceUID, Study parentStudy)
		{
			_seriesInstanceUID = seriesInstanceUID;
			_parentStudy = parentStudy;
		}

		public Study ParentStudy
		{
			get { return _parentStudy; }
		}

		public string SeriesInstanceUID
		{
			get { return _seriesInstanceUID; }
			set { _seriesInstanceUID = value; }
		}

		private string _seriesDescription;

		public string SeriesDescription
		{
			get 
			{
				IEnumerator<KeyValuePair<string, Sop>> pair = this.Sops.GetEnumerator();
				
				if (!pair.MoveNext())
					throw new ApplicationException(SR.ExceptionNoSopsExistInSeries);

				ImageSop imageSop = pair.Current.Value as ImageSop;

				if (imageSop == null)
					return String.Empty;
				else
					return imageSop.SeriesDescription;

			}
			set { _seriesDescription = value; }
		}
	

		public SopCollection Sops
		{
			get { return _sops; }
		}

		public override string ToString()
		{
			return this.SeriesInstanceUID;
		}
	}
}
