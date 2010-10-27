#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

namespace ClearCanvas.Common.Specifications
{
	public class TestResultReason
	{
		private readonly string _message;
		private readonly TestResultReason[] _reasons;

		public TestResultReason(string message)
			: this(message, new TestResultReason[] { })
		{
		}

		public TestResultReason(string message, TestResultReason reason)
			: this(message, new [] { reason })
		{
		}

		public TestResultReason(string message, TestResultReason[] reasons)
		{
			_message = message;
			_reasons = reasons;
		}

		public string Message
		{
			get { return _message; }
		}

		public TestResultReason[] Reasons
		{
			get { return _reasons; }
		}
	}
}
