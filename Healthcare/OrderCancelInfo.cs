using System;
using System.Collections;
using System.Text;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// OrderCancelInfo component
    /// </summary>
	public partial class OrderCancelInfo
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="comment"></param>
		public OrderCancelInfo(OrderCancelReasonEnum reason, string comment)
		{
			CustomInitialize();

			_reason = reason;
			_comment = comment;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="cancelledBy"></param>
		public OrderCancelInfo(OrderCancelReasonEnum reason, Staff cancelledBy)
		{
			CustomInitialize();

			_reason = reason;
			_cancelledBy = cancelledBy;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="cancelledBy"></param>
		/// <param name="comment"></param>
		public OrderCancelInfo(OrderCancelReasonEnum reason, Staff cancelledBy, string comment)
		{
			CustomInitialize();

			_reason = reason;
			_cancelledBy = cancelledBy;
			_comment = comment;
		}
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}