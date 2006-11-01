#if UNIT_TESTS

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using NUnit.Framework;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Dicom.Tests
{
	/// <summary>
	/// Summary description for Test_DCMTKcs.
	/// </summary>
	
	[TestFixture]
	public class DcmDataTest
	{
		DcmFileFormat _fileFormat;
		DcmDataset _dataSet;

		public DcmDataTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
            string fileName = Directory.GetCurrentDirectory() + @"\..\..\..\..\..\UnitTestFiles\ClearCanvas.Dicom.Tests.DcmDataTest.Init\1.2.840.113619.2.55.1.1762652774.2206.1153255770.288.1.dcm";
			_fileFormat = new DcmFileFormat();
            OFCondition cond = _fileFormat.loadFile(fileName, E_TransferSyntax.EXS_Unknown, E_GrpLenEncoding.EGL_noChange, OffisDcm.DCM_MaxReadLength, E_FileReadMode.ERM_autoDetect);

			Assert.IsTrue(cond.good());

			_dataSet = _fileFormat.getDataset();
		}

		[Test]
		public void DCMDump()
		{
			DcmObject obj = null;
			obj = _dataSet.nextInContainer(null);
			DcmTag tag = null;
			string strTagName;
			string strGroupElement;
			string strValue = null;

			while (obj != null)
			{
				tag = obj.getTag();
				strGroupElement = tag.toString();
				strTagName = tag.getTagName();

				DcmElement elem = OffisDcm.castToDcmElement(obj);

				//StringBuilder buffer = new StringBuilder(256);

				IntPtr ptr = IntPtr.Zero;

				if (elem != null)
				{
					//elem.getOFString(buffer,0,true);
					elem.getString(ref ptr);
				}
				
				strValue = Marshal.PtrToStringAnsi(ptr);
				//strValue = buffer.ToString();

				const int length = 20;

				if (strTagName.Length > length)
					strTagName = strTagName.Substring(0, length);
				else
					strTagName = strTagName.PadRight(length, ' ');

				if (false == tag.isUnknownVR())
					Console.WriteLine("{0} {1} {2}", strTagName, strGroupElement, strValue);

				obj = _dataSet.nextInContainer(obj);
			}
		}

		[Test]
		public void GetPixelData()
		{
			ushort rows, columns, bitsAllocated, bitsStored;
			_dataSet.findAndGetUint16(Dcm.Rows, out rows);
			_dataSet.findAndGetUint16(Dcm.Columns, out columns);
			_dataSet.findAndGetUint16(Dcm.BitsAllocated, out bitsAllocated);
			_dataSet.findAndGetUint16(Dcm.BitsStored, out bitsStored);

			StringBuilder buf = new StringBuilder(64);
			_dataSet.findAndGetOFString(Dcm.PhotometricInterpretation, buf);
			string photometricInterpretation = buf.ToString();

			IntPtr pPixelData = IntPtr.Zero;
			_dataSet.findAndGetUint16Array(Dcm.PixelData, ref pPixelData);
			
			int stride = columns * bitsAllocated / 8;
//			Bitmap bmp = new Bitmap(columns, rows, stride, PixelFormat.Format16bppRgb565, pPixelData);

//			ColorPalette pal = bmp.Palette;

//			System.Console.WriteLine(pal.Entries.Length);

/*			for (int i = 0; i < 255; i++)
			{
				pal.Entries[i] = Color.FromArgb(i,i,i);
			}
*/
//			bmp.Palette = pal;

//			bmp.Save("C:\\test.jpg", ImageFormat.Jpeg);
		}
	}
}

#endif