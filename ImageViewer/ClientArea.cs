using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Summary description for ClientArea.
	/// </summary>
	public class ClientArea 
	{
		// Private attributes
		private Rectangle _clientRectangle = new Rectangle(0, 0, 0, 0);
		private Rectangle _parentRectangle = new Rectangle(0, 0, 0, 0);
		private RectangleF _normalizedRectangle = new RectangleF(0.0f, 0.0f, 0.0f, 0.0f);

		// Constructor
		public ClientArea()
		{
		}

		// Properties
		public Rectangle ClientRectangle
		{
			get { return _clientRectangle; }
		}

		public Rectangle ParentRectangle
		{
			get { return _parentRectangle; }
			set
			{
				/*if (value.Left < 0 || value.Right < 0 ||
					value.Top < 0 || value.Bottom < 0 ||
					value.Left > value.Right ||
					value.Top > value.Bottom)
				{
					throw new ArgumentException(SR.ExceptionInvalidParentRectangle(value.Top, value.Left, value.Right, value.Bottom));
				}*/

				_parentRectangle = value;
				CalculateClientRectangle();
			}
		}

		public RectangleF NormalizedRectangle
		{
			get { return _normalizedRectangle; }
			set
			{

				if (FloatComparer.IsLessThan(value.Left, 0.0f) || 
					FloatComparer.IsGreaterThan(value.Left, 1.0f) ||
					FloatComparer.IsLessThan(value.Right, 0.0f) || 
					FloatComparer.IsGreaterThan(value.Right, 1.0f) ||
					FloatComparer.IsLessThan(value.Top, 0.0f) || 
					FloatComparer.IsGreaterThan(value.Top, 1.0f) ||
					FloatComparer.IsLessThan(value.Bottom, 0.0f) || 
					FloatComparer.IsGreaterThan(value.Bottom, 1.0f) ||
					FloatComparer.IsGreaterThan(value.Left, value.Right) ||
					FloatComparer.IsGreaterThan(value.Top, value.Bottom))
				{
					throw new ArgumentException(String.Format(SR.ExceptionInvalidNormalizedRectangle, value.Top.ToString(), value.Left.ToString(), value.Bottom.ToString(), value.Right.ToString()));
				}

				_normalizedRectangle = value;
				CalculateClientRectangle();
			}
		}

		private void CalculateClientRectangle()
		{
			// Calculate client rectangle
			int left = _parentRectangle.Left + (int) (_normalizedRectangle.Left * _parentRectangle.Width);
			int right = _parentRectangle.Left + (int) (_normalizedRectangle.Right * _parentRectangle.Width);
			int top = _parentRectangle.Top + (int) (_normalizedRectangle.Top * _parentRectangle.Height);
			int bottom = _parentRectangle.Top + (int) (_normalizedRectangle.Bottom * _parentRectangle.Height);

			_clientRectangle = new Rectangle(left, top, right - left, bottom - top);
		}
	}
}
