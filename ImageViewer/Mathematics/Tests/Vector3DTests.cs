#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using NUnit.Framework;
using System;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{
	[TestFixture]
	public class Vector3DTests
	{
		public Vector3DTests()
		{
		}

		[Test]
		public void TestAdd()
		{
			Vector3D v1 = new Vector3D(2.2F, 6.1F, 7.4F);
			Vector3D v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			Vector3D result = new Vector3D(6F, 9.8F, 11.5F);

			Assert.IsTrue(Vector3D.AreEqual(v1 + v2, result));

			v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			v2 = new Vector3D(-3.8F, 3.7F, -4.1F);
			result = new Vector3D(-1.6F, -2.4F, 3.3F);

			Assert.IsTrue(Vector3D.AreEqual(v1 + v2, result));
		}

		[Test]
		public void TestSubtract()
		{
			Vector3D v1 = new Vector3D(2.2F, 6.1F, 7.4F);
			Vector3D v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			Vector3D result = new Vector3D(-1.6F, 2.4F, 3.3F);

			Assert.IsTrue(Vector3D.AreEqual(v1 - v2, result));

			v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			v2 = new Vector3D(-3.8F, 3.7F, -4.1F);
			result = new Vector3D(6F, -9.8F, 11.5F);

			Assert.IsTrue(Vector3D.AreEqual(v1 - v2, result));
		}

		[Test]
		public void TestMultiply()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D result = new Vector3D(6.82F, -18.91F, 22.94f);
			v1 = 3.1F*v1;

			Assert.IsTrue(Vector3D.AreEqual(v1, result));
		}

		[Test]
		public void TestDivide()
		{
			Vector3D result = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D v1 = new Vector3D(6.82F, -18.91F, 22.94f);
			v1 = 3.1F / v1;

			Assert.IsTrue(Vector3D.AreEqual(v1, result));
		}

		[Test]
		public void TestNormalize()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Assert.IsTrue(FloatComparer.AreEqual(v1.Magnitude, 9.8392072851F));

			Vector3D normalized = v1.Normalize();
			Assert.IsTrue(FloatComparer.AreEqual(normalized.Magnitude, 1.0F));
		}

		[Test]
		public void TestDot()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D v2 = new Vector3D(3.8F, 3.7F, 4.1F);
			
			Assert.IsTrue(FloatComparer.AreEqual(v1.Dot(v2), 16.13F));
		}

		[Test]
		public void TestCross()
		{
			Vector3D v1 = new Vector3D(2.2F, -6.1F, 7.4F);
			Vector3D v2 = new Vector3D(-3.8F, 3.7F, 4.1F);
			Vector3D result = new Vector3D(-52.39F, -37.14F, -15.04F);

			Assert.IsTrue(Vector3D.AreEqual(v1.Cross(v2), result));
		}
	}
}

#endif