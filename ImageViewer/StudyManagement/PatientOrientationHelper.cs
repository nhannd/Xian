using System;
using System.Drawing;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
    public class PatientOrientationHelper
    {
        private readonly SpatialTransform _imageTransform;
        private readonly ImageOrientationPatient _imageOrientationPatient;
        private readonly PatientOrientation _patientOrientation;

        public enum ImageEdge { Left = 0, Top = 1, Right = 2, Bottom = 3 };
        private static readonly SizeF[] _edgeVectors = new SizeF[] { new SizeF(-1, 0), new SizeF(0, -1), new SizeF(1, 0), new SizeF(0, 1) };

        public PatientOrientationHelper(SpatialTransform imageTransform, ImageOrientationPatient imageOrientationPatient)
        {
            Platform.CheckForNullReference(imageTransform, "imageTransform");
            Platform.CheckForNullReference(imageOrientationPatient, "imageOrientationPatient");

            _imageTransform = imageTransform;
            _imageOrientationPatient = imageOrientationPatient;
        }

        public PatientOrientation GetEffectivePatientOrientation()
        {
            return new PatientOrientation(GetEdgeDirection(ImageEdge.Right), GetEdgeDirection(ImageEdge.Bottom));
        }

        public PatientDirection GetEdgeDirection(ImageEdge viewportEdge)
        {
            var direction = GetPrimaryEdgeDirection(viewportEdge);
            if (!direction.IsEmpty)
            {
                var secondaryDirection = GetSecondaryEdgeDirection(viewportEdge);
                direction += secondaryDirection;
                //TODO (CR June 2011): Tertiary?
            }

            return direction;
        }

        public PatientDirection GetPrimaryEdgeDirection(ImageEdge viewportEdge)
        {
            var destinationEdgeVectors = GetDestinationEdgeVectors();
            //find out which source image edge got transformed to coincide with this viewport edge.
            var transformedEdge = GetTransformedEdge(destinationEdgeVectors, viewportEdge);
            return GetEdgeComponentDirection(transformedEdge, PatientDirection.Component.Primary);
        }

        public PatientDirection GetSecondaryEdgeDirection(ImageEdge viewportEdge)
        {
            var destinationEdgeVectors = GetDestinationEdgeVectors();
            //find out which source image edge got transformed to coincide with this viewport edge.
            var transformedEdge = GetTransformedEdge(destinationEdgeVectors, viewportEdge);
            return GetEdgeComponentDirection(transformedEdge, PatientDirection.Component.Secondary);
        }

        private PatientDirection GetEdgeComponentDirection(ImageEdge imageEdge, PatientDirection.Component component)
		{
			bool negativeDirection = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Top);
			bool rowValues = (imageEdge == ImageEdge.Left || imageEdge == ImageEdge.Right);

            var direction = PatientDirection.Empty;
            if (rowValues)
			{
                //TODO (CR June 2011): tertiary?
                if (component == PatientDirection.Component.Primary)
                    direction = GetPrimaryRowDirection();
                else if (component == PatientDirection.Component.Secondary)
                    direction = GetSecondaryRowDirection();
			}
			else
			{
                //TODO (CR June 2011): tertiary?
                if (component == PatientDirection.Component.Primary)
                    direction = GetPrimaryColumnDirection();
                else if (component == PatientDirection.Component.Secondary)
                    direction = GetSecondaryColumnDirection();
            }

            return negativeDirection ? direction.OpposingDirection : direction;
		}

        private PatientDirection GetPrimaryRowDirection()
        {
            if (_imageOrientationPatient != null)
                return _imageOrientationPatient.GetPrimaryRowDirection(false);
            
            if (_patientOrientation != null)
                return _patientOrientation.PrimaryRow;

            return PatientDirection.Empty;
        }

        private PatientDirection GetPrimaryColumnDirection()
        {
            if (_imageOrientationPatient != null)
                return _imageOrientationPatient.GetPrimaryColumnDirection(false);

            if (_patientOrientation != null)
                return _patientOrientation.PrimaryColumn;

            return PatientDirection.Empty;
        }

        private PatientDirection GetSecondaryRowDirection()
        {
            if (_imageOrientationPatient != null)
                return _imageOrientationPatient.GetSecondaryRowDirection(false);

            if (_patientOrientation != null)
                return _patientOrientation.SecondaryRow;

            return PatientDirection.Empty;
        }

        private PatientDirection GetSecondaryColumnDirection()
        {
            if (_imageOrientationPatient != null)
                return _imageOrientationPatient.GetSecondaryColumnDirection(false);

            if (_patientOrientation != null)
                return _patientOrientation.SecondaryColumn;

            return PatientDirection.Empty;
        }

        private SizeF[] GetDestinationEdgeVectors()
        {
            SizeF[] imageEdgeVectors = new SizeF[4];
            for (int i = 0; i < 4; ++i)
                imageEdgeVectors[i] = _imageTransform.ConvertToDestination(_edgeVectors[i]);
            return imageEdgeVectors;
        }
        
        private static ImageEdge GetTransformedEdge(SizeF[] transformedVectors, ImageEdge viewportEdge)
        {
            //the original (untransformed) vector for this viewport edge.
            SizeF thisViewportEdge = _edgeVectors[(int)viewportEdge];

            //find out which edge in the source image has moved to this edge of the viewport.
            for (int index = 0; index < transformedVectors.Length; ++index)
            {
                //normalize the vector before comparing.
                SizeF transformedVector = transformedVectors[index];
                double magnitude = Math.Sqrt(transformedVector.Width * transformedVector.Width +
                                                transformedVector.Height * transformedVector.Height);

                transformedVector.Width = (float)Math.Round(transformedVector.Width / magnitude);
                transformedVector.Height = (float)Math.Round(transformedVector.Height / magnitude);

                //is it the same as the original vector for this edge?
                if (transformedVector == thisViewportEdge)
                {
                    //return the image edge that has now moved to this edge of the viewport.
                    return (ImageEdge)index;
                }
            }

            //this should never happen.
            throw new IndexOutOfRangeException("The transformed edge does not have a corresponding value.");
        }
    }
}
