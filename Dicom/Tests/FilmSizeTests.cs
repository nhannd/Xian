#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using NUnit.Framework;
using ClearCanvas.Dicom.Iod.Modules;

namespace ClearCanvas.Dicom.Tests
{
	[TestFixture]
	public class FilmSizeTests
	{
		[Test]
		public void Null_DicomString_Test()
		{
			var filmSize = FilmSize.FromDicomString(null);
			Assert.IsNull(filmSize);
		}

		[Test]
		public void Empty_DicomString_Test()
		{
			var filmSize = FilmSize.FromDicomString(string.Empty);
			Assert.IsNull(filmSize);
		}

		[Test]
		public void Supported_FilmSizes_Test()
		{
			var supportedFilmSizes = FilmSize.StandardFilmSizes;
			Assert.IsNotEmpty(supportedFilmSizes);
		}

		[Test]
		public void Valid_FilmSize_Test()
		{
			var filmSize = FilmSize.FromDicomString("11INX22IN");
			Assert.AreEqual(filmSize.DicomString, "11INX22IN");
			Assert.AreEqual(filmSize.GetWidth(FilmSize.FilmSizeUnit.Inch), 11);
			Assert.AreEqual(filmSize.GetHeight(FilmSize.FilmSizeUnit.Inch), 22);
			Assert.AreEqual(filmSize.GetWidth(FilmSize.FilmSizeUnit.Millimeter), 11 * LengthInMillimeter.Inch);
			Assert.AreEqual(filmSize.GetHeight(FilmSize.FilmSizeUnit.Millimeter), 22 * LengthInMillimeter.Inch);
		}

		[Test]
		[ExpectedException(typeof(FilmSize.InvalidFilmSizeException))]
		public void Lower_Case_X_Test()
		{
			FilmSize.FromDicomString("11MMx22MM");
		}

		[Test]
		[ExpectedException(typeof(FilmSize.InvalidFilmSizeException))]
		public void Invalid_Units_Test()
		{
			FilmSize.FromDicomString("11MMX22MM");
		}

		[Test]
		[ExpectedException(typeof(FilmSize.InvalidFilmSizeException))]
		public void Two_Different_Units_Test()
		{
			FilmSize.FromDicomString("11INX22CM");
		}

	}
}


#endif