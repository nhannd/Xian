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

using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow
{
	public class DiscontinueOrderOperation
	{
        /// <summary>
        /// Executes Discontinue Order operation.
        /// Checks if order is in progress, then executes if it is.
        /// Otherwise, throws a WorkflowException.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="info"></param>
		public void Execute(Order order, OrderCancelInfo info)
		{
			if (order.Status == OrderStatus.IP)
				order.Discontinue(info);
			else
				throw new WorkflowException(string.Format("Order with status {0} cannot be discontinued.", order.Status));
		}

        /// <summary>
        /// Determines if discontinuing an order is possible.
        /// The order needs to currently be in progress.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
		public bool CanExecute(Order order)
		{
			return order.Status == OrderStatus.IP;
		}
	}
}
