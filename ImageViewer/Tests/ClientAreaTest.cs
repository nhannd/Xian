#if	UNIT_TESTS

using System;
using NUnit.Framework;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class ClientAreaTest
	{
		public ClientAreaTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void NestedRectangles()
		{
			ClientArea clientArea1 = new ClientArea();
			clientArea1.NormalizedRectangle = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
			clientArea1.ParentRectangle = new Rectangle(0, 0, 800, 600);
			Rectangle resultRect1 = new Rectangle(0, 0, 800, 600);
			Assert.AreEqual(resultRect1, clientArea1.ClientRectangle);

			// Nest clientArea2 in clientArea1
			ClientArea clientArea2 = new ClientArea();
			clientArea2.NormalizedRectangle = new RectangleF(0.25f, 0.3333333333f, 0.25f, 0.3333333333f);
			clientArea2.ParentRectangle = clientArea1.ClientRectangle;
			Rectangle resultRect2 = new Rectangle(200, 200, 200, 200);
			Assert.AreEqual(resultRect2, clientArea2.ClientRectangle);

			// Nest clientArea3 in clientArea2
			ClientArea clientArea3 = new ClientArea();
			clientArea3.NormalizedRectangle = new RectangleF(0.25f, 0.25f, 0.5f, 0.5f);
			clientArea3.ParentRectangle = clientArea2.ClientRectangle;
			Rectangle resultRect3 = new Rectangle(250, 250, 100, 100);
			Assert.AreEqual(resultRect3, clientArea3.ClientRectangle);
		}

		[Test]
		public void EmptyRectangle()
		{
			// Make sure it works with a zero area rectangle
			ClientArea clientArea1 = new ClientArea();
			clientArea1.NormalizedRectangle = new RectangleF(0.0f, 0.0f, 0.0f, 0.0f);
			clientArea1.ParentRectangle = new Rectangle(0, 0, 0, 0);
		}

		[Test]
		public void DifferentSetSequences()
		{
			// Set the normalized rectangle, then the parent rectangle
			ClientArea clientArea1 = new ClientArea();
			clientArea1.NormalizedRectangle = new RectangleF(0.0f, 0.0f, 0.5f, 0.5f);
			clientArea1.ParentRectangle = new Rectangle(0, 0, 800, 600);
			Rectangle resultRect = new Rectangle(0, 0, 400, 300);
			Assert.AreEqual(resultRect, clientArea1.ClientRectangle);

			// Set the parent rectangle, then the normalized rectangle
			ClientArea clientArea2 = new ClientArea();
			clientArea2.ParentRectangle = new Rectangle(0, 0, 800, 600);
			clientArea2.NormalizedRectangle = new RectangleF(0.0f, 0.0f, 0.5f, 0.5f);
			Assert.AreEqual(resultRect, clientArea2.ClientRectangle);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidNormalizedRectangle01()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.NormalizedRectangle = new RectangleF(-1.0f, 0.0f, 1.0f, 1.0f);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidNormalizedRectangle02()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.NormalizedRectangle = new RectangleF(2.0f, 0.0f, 1.0f, 1.0f);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidNormalizedRectangle03()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.NormalizedRectangle = new RectangleF(0.0f, 0.0f, -1.0f, 1.0f);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidNormalizedRectangle04()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.NormalizedRectangle = new RectangleF(0.0f, 0.0f, 2.0f, 1.0f);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidNormalizedRectangle05()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.NormalizedRectangle = new RectangleF(0.0f, -1.0f, 1.0f, 1.0f);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidNormalizedRectangle06()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.NormalizedRectangle = new RectangleF(0.0f, 2.0f, 1.0f, 1.0f);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidNormalizedRectangle07()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.NormalizedRectangle = new RectangleF(0.0f, 0.0f, 1.0f, -1.0f);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidNormalizedRectangle08()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.NormalizedRectangle = new RectangleF(0.0f, 0.0f, 1.0f, 2.0f);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidNormalizedRectangle09()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.NormalizedRectangle = new RectangleF(1.0f, 0.0f, -1.0f, 1.0f);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidNormalizedRectangle10()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.NormalizedRectangle = new RectangleF(0.0f, 1.0f, 1.0f, -1.0f);
		}

		[Test]
		public void NormalizedRectangleBoundaryConditions()
		{
			// We just want to make sure an exception is never thrown
			for (int n = 1; n < 50; n++)
			{
				float x = 1.0f / n;

				RectangleF rect = new RectangleF(x, 0.0f, 1.0f - x, 1.0f);

				ClientArea clientArea = new ClientArea();
				clientArea.NormalizedRectangle = rect;
			}
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidParentRectangle01()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.ParentRectangle = new Rectangle(-1, 0, 1, 1);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidParentRectangle02()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.ParentRectangle = new Rectangle(0, 0, -1, 1);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidParentRectangle03()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.ParentRectangle = new Rectangle(0, -1, 1, 1);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidParentRectangle04()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.ParentRectangle = new Rectangle(0, 0, 1, -1);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidParentRectangle05()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.ParentRectangle = new Rectangle(1, 0, -1, 1);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void InvalidParentRectangle06()
		{
			ClientArea clientArea = new ClientArea();
			clientArea.ParentRectangle = new Rectangle(0, 1, 1, -1);
		}

	}
}

#endif