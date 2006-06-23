using System;
using System.Drawing;
using ClearCanvas.Common;

namespace ClearCanvas.Workstation.Model
{
	/// <summary>
	/// Summary description for ClientArea.
	/// </summary>
	internal class ClientArea : ICloneable
	{
		// Private attributes
		private Rectangle m_ClientRectangle = new Rectangle(0, 0, 0, 0);
		private Rectangle m_ParentRectangle = new Rectangle(0, 0, 0, 0);
		private RectangleF m_NormalizedRectangle = new RectangleF(0.0f, 0.0f, 0.0f, 0.0f);

		// Constructor
		public ClientArea()
		{
		}

		// Properties
		public Rectangle ClientRectangle
		{
			get 
			{ 
				return m_ClientRectangle; 
			}
		}

		public Rectangle ParentRectangle
		{
			get 
			{ 
				return m_ParentRectangle; 
			}
			set
			{
				/*if (value.Left < 0 || value.Right < 0 ||
					value.Top < 0 || value.Bottom < 0 ||
					value.Left > value.Right ||
					value.Top > value.Bottom)
				{
					throw new ArgumentException(SR.ExceptionInvalidParentRectangle(value.Top, value.Left, value.Right, value.Bottom));
				}*/

				m_ParentRectangle = value;
				CalculateClientRectangle();
			}
		}

		public RectangleF NormalizedRectangle
		{
			get 
			{ 
				return m_NormalizedRectangle; 
			}
			set
			{
				if (value.Left < 0.0 || value.Left > 1.0 ||
					value.Right < 0.0 || value.Right > 1.0 ||
					value.Top < 0.0 || value.Top > 1.0 ||
					value.Bottom < 0.0 || value.Bottom > 1.0 ||
					value.Left > value.Right ||
					value.Top > value.Bottom)
				{
					throw new ArgumentException(String.Format(SR.ExceptionInvalidNormalizedRectangle, value.Top.ToString(), value.Left.ToString(), value.Bottom.ToString(), value.Right.ToString()));
				}

				m_NormalizedRectangle = value;
				CalculateClientRectangle();
			}
		}

		private void CalculateClientRectangle()
		{
			// Calculate client rectangle
			int left = m_ParentRectangle.Left + (int) (m_NormalizedRectangle.Left * m_ParentRectangle.Width);
			int right = m_ParentRectangle.Left + (int) (m_NormalizedRectangle.Right * m_ParentRectangle.Width);
			int top = m_ParentRectangle.Top + (int) (m_NormalizedRectangle.Top * m_ParentRectangle.Height);
			int bottom = m_ParentRectangle.Top + (int) (m_NormalizedRectangle.Bottom * m_ParentRectangle.Height);

			m_ClientRectangle = new Rectangle(left, top, right - left, bottom - top);
		}

		#region ICloneable Members

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion
	}
}
