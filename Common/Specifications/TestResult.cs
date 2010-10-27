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
	public class TestResult
	{
		private readonly bool _success;
		private readonly TestResultReason[] _reasons;

		public TestResult(bool success)
			: this(success, new TestResultReason[] { })
		{
		}

		public TestResult(bool success, string reason)
			: this(success, new[] { new TestResultReason(reason) })
		{
		}

		public TestResult(bool success, TestResultReason reason)
			: this(success, new[] { reason })
		{
		}

		public TestResult(bool success, TestResultReason[] reasons)
		{
			_success = success;
			_reasons = reasons;
		}

		public bool Success
		{
			get { return _success; }
		}

		public bool Fail
		{
			get { return !_success; }
		}

		public TestResultReason[] Reasons
		{
			get { return _reasons; }
		}
	}
}
