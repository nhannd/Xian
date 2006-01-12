using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using NUnit.Framework;
using ClearCanvas.Common.Dicom;

namespace ClearCanvas.Dicom.Tests
{
	/// <summary>
	/// Summary description for Test_DCMTKcs.
	/// </summary>
	
	[TestFixture]
	public class DcmDataTest
	{
		DcmFileFormat m_FileFormat;
		DcmDataset m_DataSet;

		public DcmDataTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
			string fileName = "CT.dcm";
			m_FileFormat = new DcmFileFormat();
			OFCondition cond = m_FileFormat.loadFile(fileName, E_TransferSyntax.EXS_Unknown, E_GrpLenEncoding.EGL_noChange, DCMTK.DCM_MaxReadLength, false);

			Assert.IsTrue(cond.good());

			m_DataSet = m_FileFormat.getDataset();
		}

		[Test]
		public void DCMDump()
		{
			DcmObject obj = null;
			obj = m_DataSet.nextInContainer(null);
			DcmTag tag = null;
			string strTagName;
			string strGroupElement;
			string strValue = null;

			while (obj != null)
			{
				tag = obj.getTag();
				strGroupElement = tag.toString();
				strTagName = tag.getTagName();

				DcmElement elem = DCMTK.castToDcmElement(obj);

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

				obj = m_DataSet.nextInContainer(obj);
			}
		}

		[Test]
		public void GetPixelData()
		{
			ushort rows, columns, bitsAllocated, bitsStored;
			m_DataSet.findAndGetUint16(dcm.Rows, out rows);
			m_DataSet.findAndGetUint16(dcm.Columns, out columns);
			m_DataSet.findAndGetUint16(dcm.BitsAllocated, out bitsAllocated);
			m_DataSet.findAndGetUint16(dcm.BitsStored, out bitsStored);

			StringBuilder buf = new StringBuilder(64);
			m_DataSet.findAndGetOFString(dcm.PhotometricInterpretation, buf);
			string photometricInterpretation = buf.ToString();

			IntPtr pPixelData = IntPtr.Zero;
			m_DataSet.findAndGetUint16Array(dcm.PixelData, ref pPixelData);
			
			int stride = columns * bitsAllocated / 8;
			Bitmap bmp = new Bitmap(columns, rows, stride, PixelFormat.Format16bppRgb565, pPixelData);

//			ColorPalette pal = bmp.Palette;

//			System.Console.WriteLine(pal.Entries.Length);

/*			for (int i = 0; i < 255; i++)
			{
				pal.Entries[i] = Color.FromArgb(i,i,i);
			}
*/
//			bmp.Palette = pal;

			bmp.Save("C:\\test.jpg", ImageFormat.Jpeg);
		}
	}
}
