using System;
using System.Drawing;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
    public class Formula
    {
        public static double AreaOfRectangle(double width, double height)
        {
            return Math.Abs(width*height);
        }

        public static double AreaOfEllipse(double width, double height)
        {
            // pi/4 = 0.7853981633
            return Math.Abs(0.7853981633 * width * height);
        }

		/// <summary>
		/// Calculates the angle subtended by two line segments.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="vertex"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		/// <remarks>
		/// Line segment 1 is defined by points <paramref name="start"/> and <paramref name="vertex"/>.
		/// Line segment 2 is defined by points <paramref name="end"/> and <paramref name="vertex"/>.
		/// </remarks>
		public static double SubtendedAngle(PointF start, PointF vertex, PointF end)
		{
			Vector3D vertexPositionVector = new Vector3D(vertex.X, vertex.Y, 0);
			Vector3D a = new Vector3D(start.X, start.Y, 0) - vertexPositionVector;
			Vector3D b = new Vector3D(end.X, end.Y, 0) - vertexPositionVector;

			float dotProduct = a.Dot(b);

			Vector3D crossProduct = a.Cross(b);

			float magA = a.Magnitude;
			float magB = b.Magnitude;

			if (magA == 0 || magB == 0)
				return 0;

			double cosTheta = dotProduct / magA / magB;

			// Make sure cosTheta is within bounds so we don't
			// get any errors when we take the acos.
			if (cosTheta > 1.0f)
				cosTheta = 1.0f;

			if (cosTheta < -1.0f)
				cosTheta = -1.0f;

			double theta = Math.Acos(cosTheta)*(crossProduct.Z == 0 ? 1 : -Math.Sign(crossProduct.Z));
			double thetaInDegrees = theta / Math.PI * 180;

			return thetaInDegrees;
		}
    }
}
