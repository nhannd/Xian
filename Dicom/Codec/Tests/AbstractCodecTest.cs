#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using System.IO;
using System.Text;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Codec.Tests
{
	[TestFixture]
	public class CodecTest : AbstractCodecTest
	{
		[Test]
		public void Test()
		{
			DicomFile file = CreateFile(256, 256, "MONOCHROME1", 16, 12, true, 1);

			// Little Endian Tests
			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			file.Save();
		}
	}
	public class AbstractCodecTest : AbstractTest
	{

		public static void LosslessImageTest(TransferSyntax syntax, DicomFile theFile)
		{
			if (File.Exists(theFile.Filename))
				File.Delete(theFile.Filename);

			DicomFile saveCopy = new DicomFile(theFile.Filename, theFile.MetaInfo.Copy(), theFile.DataSet.Copy());

			theFile.ChangeTransferSyntax(syntax);

			theFile.Save(DicomWriteOptions.ExplicitLengthSequence);

			DicomFile newFile = new DicomFile(theFile.Filename);

			newFile.Load(DicomReadOptions.Default);

			newFile.ChangeTransferSyntax(saveCopy.TransferSyntax);

			List<DicomAttributeComparisonResult> list = new List<DicomAttributeComparisonResult>();
			bool result = newFile.DataSet.Equals(saveCopy.DataSet, ref list);

			StringBuilder sb = new StringBuilder();
			foreach (DicomAttributeComparisonResult compareResult in list)
				sb.AppendFormat("Comparison Failure: {0}, ", compareResult.Details);

			Assert.IsTrue(result,sb.ToString());
		}

		public static void LossyImageTest(TransferSyntax syntax, DicomFile theFile)
		{
			if (File.Exists(theFile.Filename))
				File.Delete(theFile.Filename);

			DicomFile saveCopy = new DicomFile(theFile.Filename, theFile.MetaInfo.Copy(), theFile.DataSet.Copy());

			theFile.ChangeTransferSyntax(syntax);

			theFile.Save(DicomWriteOptions.ExplicitLengthSequence);

			DicomFile newFile = new DicomFile(theFile.Filename);

			newFile.Load(DicomReadOptions.Default);

			newFile.ChangeTransferSyntax(saveCopy.TransferSyntax);

			Assert.IsFalse(newFile.DataSet.Equals(saveCopy.DataSet));
		}

		public DicomFile CreateFile(ushort rows, ushort columns, string photometricInterpretation, ushort bitsStored, ushort bitsAllocated, bool isSigned, ushort numberOfFrames)
		{
			DicomFile file = new DicomFile("SCTestImage.dcm");

			DicomAttributeCollection dataSet = file.DataSet;

			SetupSecondaryCapture(dataSet);

			dataSet[DicomTags.PixelData] = null;

			DicomUncompressedPixelData pd = new DicomUncompressedPixelData(dataSet)
			                                	{
			                                		ImageWidth = columns,
			                                		ImageHeight = rows,
			                                		PhotometricInterpretation = photometricInterpretation,
			                                		BitsAllocated = bitsAllocated,
			                                		BitsStored = bitsStored,
			                                		HighBit = (ushort) (bitsStored - 1),
			                                		PixelRepresentation = (ushort) (isSigned ? 1 : 0),
			                                		NumberOfFrames = numberOfFrames
			                                	};

			if (photometricInterpretation.Equals("RGB"))
			{
				
			}
			else
				CreateMonochromePixelData(pd);

			pd.UpdateAttributeCollection(dataSet);

			SetupMetaInfo(file);

			file.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

			return file;
		}

		private static void CreateMonochromePixelData(DicomUncompressedPixelData pd)
		{
			int rows = pd.ImageHeight;
			int cols = pd.ImageWidth;

			int minValue = pd.IsSigned ? -(1 << (pd.BitsStored - 1)) : 0;
			int maxValue = (1 << pd.BitsStored) + minValue - 1;

			// Create a small block of pixels in the test pattern in an integer,
			// then copy/tile into the full size frame data

			int smallRows = (rows * 3) / 8;
			int smallColumns = rows / 4;
			int stripSize = rows / 16;

			int[] smallPixels = new int[smallRows * smallColumns];

			float slope = (float)(maxValue - minValue) / smallColumns;

			int pixelOffset = 0;
			for (int i = 0; i < smallRows; i++)
			{
				if (i < stripSize)
				{
					for (int j = 0; j < smallColumns; j++)
					{
						smallPixels[pixelOffset] = (int)((j * slope) + minValue);
						pixelOffset++;
					}
				}
				else if (i > (smallRows - stripSize))
				{
					for (int j = 0; j < smallColumns; j++)
					{
						smallPixels[pixelOffset] = (int)(maxValue - (j * slope));
						pixelOffset++;
					}
				}
				else
				{
					int pixel = minValue + (int)((i - stripSize) * slope);
					if (pixel < minValue) pixel = minValue + 1;
					if (pixel > maxValue) pixel = maxValue - 1;

					int start = (smallColumns / 2) - (i - stripSize) / 2;
					int end = (smallColumns / 2) + (i - stripSize) / 2;

					for (int j = 0; j < smallColumns; j++)
					{
						if (j < start)
							smallPixels[pixelOffset] = minValue;
						else if (j > end)
							smallPixels[pixelOffset] = maxValue;
						else
							smallPixels[pixelOffset] = pixel;

						pixelOffset++;
					}
				}
			}
			// Now create the actual frame
			for (int frame = 0; frame < pd.NumberOfFrames; frame++)
			{

				// Odd length frames are automatically dealt with by DicomUncompressedPixelData
				byte[] frameData = new byte[pd.UncompressedFrameSize];
				pixelOffset = 0;

				if (pd.BitsAllocated == 8)
				{
					if (pd.IsSigned)
					{
						for (int i = 0; i < rows; i++)
						{
							int smallOffset = (i % smallRows) * smallColumns;

							for (int j = 0; j < cols; j++)
							{
								frameData[pixelOffset] = (byte)((sbyte)smallPixels[smallOffset + j % smallColumns]);
								pixelOffset++;
							}
						}
					}
					else
					{
						for (int i = 0; i < rows; i++)
						{
							int smallOffset = (i % smallRows) * smallColumns;

							for (int j = 0; j < cols; j++)
							{
								frameData[pixelOffset] = (byte)smallPixels[smallOffset + j % smallColumns];
								pixelOffset++;
							}
						}
					}
				}
				else
				{
					if (pd.IsSigned)
					{
						for (int i = 0; i < rows; i++)
						{
							int smallOffset = (i % smallRows) * smallColumns;

							for (int j = 0; j < cols; j++)
							{
								short pixel = (short)smallPixels[smallOffset + j % smallColumns];
								frameData[pixelOffset] = (byte)(pixel & 0x00FF);
								pixelOffset++;
								frameData[pixelOffset] = (byte)((pixel & 0xFF00) >> 8);
								pixelOffset++;
							}
						}
					}
					else
					{
						for (int i = 0; i < rows; i++)
						{
							int smallOffset = (i % smallRows) * smallColumns;

							for (int j = 0; j < cols; j++)
							{
								ushort pixel = (ushort)smallPixels[smallOffset + j % smallColumns];
								frameData[pixelOffset] = (byte)(pixel & 0x00FF);
								pixelOffset++;
								frameData[pixelOffset] = (byte)((pixel & 0xFF00) >> 8);
								pixelOffset++;
							}
						}
					}
				}

				pd.AppendFrame(frameData);
			}
		}
	}
}
