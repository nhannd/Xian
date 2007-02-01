using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Services
{
	public static class LocalApplicationEntity
	{
		public static string AETitle
		{
			get { return LocalAESettings.Default.AETitle; }
		}

		public static int Port
		{
			get { return LocalAESettings.Default.Port; }
		}

		public static string DicomStoragePath
		{
			get { return LocalAESettings.Default.DicomStoragePath; }
		}
	}
}
