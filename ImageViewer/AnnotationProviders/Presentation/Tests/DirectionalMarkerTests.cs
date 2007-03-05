#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Annotations;
using System.Drawing;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.AnnotationProviders.Presentation.Tests
{
	internal class JunkProvider : IAnnotationItemProvider
	{
		public JunkProvider()
		{ 
		}

		#region IAnnotationItemProvider Members

		public string GetIdentifier()
		{
			return "junk";
		}

		public string GetDisplayName()
		{
			return "junk";
		}

		public IEnumerable<IAnnotationItem> GetAnnotationItems()
		{
			return null;
		}

		#endregion
	}

	[TestFixture]
	public class DirectionalMarkerTests
	{
		JunkProvider _provider;

		DirectionalMarkerAnnotationItem _leftAnnotation;
		DirectionalMarkerAnnotationItem _topAnnotation;
		DirectionalMarkerAnnotationItem _rightAnnotation;
		DirectionalMarkerAnnotationItem _bottomAnnotation;

		SpatialTransform _transform;

		public DirectionalMarkerTests()
		{ 
		}

		[TestFixtureSetUp]
		public void Initialize()
		{
			_provider = new JunkProvider();
			_leftAnnotation = new DirectionalMarkerAnnotationItem(_provider, DirectionalMarkerAnnotationItem.ImageEdge.Left);
			_topAnnotation = new DirectionalMarkerAnnotationItem(_provider, DirectionalMarkerAnnotationItem.ImageEdge.Top);
			_rightAnnotation = new DirectionalMarkerAnnotationItem(_provider, DirectionalMarkerAnnotationItem.ImageEdge.Right);
			_bottomAnnotation = new DirectionalMarkerAnnotationItem(_provider, DirectionalMarkerAnnotationItem.ImageEdge.Bottom);
		}

		private void NewTransform()
		{
			_transform = new ImageSpatialTransform(null, 10,10,0,0,0,0);
//			_transform.SourceRectangle = new Rectangle(0, 0, 10, 10);
			_transform.ClientRectangle = new Rectangle(0, 0, 15, 25);
		}

		/// <summary>
		/// The directional marker annotation items simply calculate 
		/// </summary>
		private void TestArbitraryImageOrientationPatient
			(	ImageOrientationPatient iop, 
				string leftLabelNoTransform, 
				string topLabelNoTransform,
				string rightLabelNoTransform,
				string bottomLabelNoTransform
			)
		{

			// For an explanation of why this works see the DirectionalMarkerAnnotationItem class.
			// Notice how on 90 degree rotations, the labels roll upward (in this editor).  That is 
			// the nature of the algorithm and is what we expect to happen.
			
			Assert.AreEqual(_leftAnnotation.GetAnnotationTextInternal(_transform, iop), leftLabelNoTransform);
			Assert.AreEqual(_topAnnotation.GetAnnotationTextInternal(_transform, iop), topLabelNoTransform);
			Assert.AreEqual(_rightAnnotation.GetAnnotationTextInternal(_transform, iop), rightLabelNoTransform);
			Assert.AreEqual(_bottomAnnotation.GetAnnotationTextInternal(_transform, iop), bottomLabelNoTransform);

			_transform.RotationXY = -90;

			Assert.AreEqual(_leftAnnotation.GetAnnotationTextInternal(_transform, iop), topLabelNoTransform);
			Assert.AreEqual(_topAnnotation.GetAnnotationTextInternal(_transform, iop), rightLabelNoTransform);
			Assert.AreEqual(_rightAnnotation.GetAnnotationTextInternal(_transform, iop), bottomLabelNoTransform);
			Assert.AreEqual(_bottomAnnotation.GetAnnotationTextInternal(_transform, iop), leftLabelNoTransform);

			_transform.RotationXY = -180;

			Assert.AreEqual(_leftAnnotation.GetAnnotationTextInternal(_transform, iop), rightLabelNoTransform);
			Assert.AreEqual(_topAnnotation.GetAnnotationTextInternal(_transform, iop), bottomLabelNoTransform);
			Assert.AreEqual(_rightAnnotation.GetAnnotationTextInternal(_transform, iop), leftLabelNoTransform);
			Assert.AreEqual(_bottomAnnotation.GetAnnotationTextInternal(_transform, iop), topLabelNoTransform);

			_transform.RotationXY = -270;

			Assert.AreEqual(_leftAnnotation.GetAnnotationTextInternal(_transform, iop), bottomLabelNoTransform);
			Assert.AreEqual(_topAnnotation.GetAnnotationTextInternal(_transform, iop), leftLabelNoTransform);
			Assert.AreEqual(_rightAnnotation.GetAnnotationTextInternal(_transform, iop), topLabelNoTransform);
			Assert.AreEqual(_bottomAnnotation.GetAnnotationTextInternal(_transform, iop), rightLabelNoTransform);

			_transform.RotationXY = -0;
			_transform.FlipY = true;
			Assert.AreEqual(_leftAnnotation.GetAnnotationTextInternal(_transform, iop), rightLabelNoTransform);
			Assert.AreEqual(_topAnnotation.GetAnnotationTextInternal(_transform, iop), topLabelNoTransform);
			Assert.AreEqual(_rightAnnotation.GetAnnotationTextInternal(_transform, iop), leftLabelNoTransform);
			Assert.AreEqual(_bottomAnnotation.GetAnnotationTextInternal(_transform, iop), bottomLabelNoTransform);

			_transform.RotationXY = -90;

			Assert.AreEqual(_leftAnnotation.GetAnnotationTextInternal(_transform, iop), topLabelNoTransform);
			Assert.AreEqual(_topAnnotation.GetAnnotationTextInternal(_transform, iop), leftLabelNoTransform);
			Assert.AreEqual(_rightAnnotation.GetAnnotationTextInternal(_transform, iop), bottomLabelNoTransform);
			Assert.AreEqual(_bottomAnnotation.GetAnnotationTextInternal(_transform, iop), rightLabelNoTransform);

			_transform.RotationXY = -180;

			Assert.AreEqual(_leftAnnotation.GetAnnotationTextInternal(_transform, iop), leftLabelNoTransform);
			Assert.AreEqual(_topAnnotation.GetAnnotationTextInternal(_transform, iop), bottomLabelNoTransform);
			Assert.AreEqual(_rightAnnotation.GetAnnotationTextInternal(_transform, iop), rightLabelNoTransform);
			Assert.AreEqual(_bottomAnnotation.GetAnnotationTextInternal(_transform, iop), topLabelNoTransform);

			_transform.RotationXY = -270;

			Assert.AreEqual(_leftAnnotation.GetAnnotationTextInternal(_transform, iop), bottomLabelNoTransform);
			Assert.AreEqual(_topAnnotation.GetAnnotationTextInternal(_transform, iop), rightLabelNoTransform);
			Assert.AreEqual(_rightAnnotation.GetAnnotationTextInternal(_transform, iop), topLabelNoTransform);
			Assert.AreEqual(_bottomAnnotation.GetAnnotationTextInternal(_transform, iop), leftLabelNoTransform);
		}

		[Test]
		public void TestNoOrientation()
		{
			NewTransform();
			ImageOrientationPatient iop = new ImageOrientationPatient(0, 0, 0, 0, 0, 0);
			TestArbitraryImageOrientationPatient(iop, "", "", "", "");
		}

		[Test]
		public void TestSaggittalView()
		{
			NewTransform();
			ImageOrientationPatient iop = new ImageOrientationPatient(0, 1, 0, 0, 0, -1);
			TestArbitraryImageOrientationPatient(iop, SR.ValueDirectionalMarkersAnterior,
												SR.ValueDirectionalMarkersHead,
												SR.ValueDirectionalMarkersPosterior,
												SR.ValueDirectionalMarkersFoot);
		}

		[Test]
		public void TestCoronalView()
		{
			NewTransform();
			ImageOrientationPatient iop = new ImageOrientationPatient(1, 0, 0, 0, 0, -1);
			TestArbitraryImageOrientationPatient(iop, SR.ValueDirectionalMarkersRight,
												SR.ValueDirectionalMarkersHead,
												SR.ValueDirectionalMarkersLeft,
												SR.ValueDirectionalMarkersFoot);
		}

		[Test]
		public void TestAxialView()
		{
			NewTransform();
			ImageOrientationPatient iop = new ImageOrientationPatient(1, 0, 0, 0, 1, 0);
			TestArbitraryImageOrientationPatient(iop, SR.ValueDirectionalMarkersRight,
												SR.ValueDirectionalMarkersAnterior,
												SR.ValueDirectionalMarkersLeft,
												SR.ValueDirectionalMarkersPosterior);
		}

		[Test]
		public void TestObliqueView()
		{
			NewTransform();
			ImageOrientationPatient iop = new ImageOrientationPatient(0.789570, -0.608964, -0.075781, -0.084157, 0.014871, -0.996342);
			TestArbitraryImageOrientationPatient(iop, SR.ValueDirectionalMarkersRight + SR.ValueDirectionalMarkersPosterior,
												SR.ValueDirectionalMarkersHead + SR.ValueDirectionalMarkersLeft,
												SR.ValueDirectionalMarkersLeft + SR.ValueDirectionalMarkersAnterior,
												SR.ValueDirectionalMarkersFoot + SR.ValueDirectionalMarkersRight);


		}
	}
}

#endif