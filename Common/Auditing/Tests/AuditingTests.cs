#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

#if	UNIT_TESTS

#pragma warning disable 1591

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using NMock2;
using NUnit.Framework;

namespace ClearCanvas.Common.Auditing.Tests
{
	internal class FakeRawAuditMessage : IAuditMessage
	{
		private string _message;

		public FakeRawAuditMessage(string message)
		{
			_message = message;
		}

		public string GetMessage()
		{
			return _message;
		}
	}

	internal class FakeStudyAccessedAuditMessage : IAuditMessage
	{
		private string _studyInstanceUID;
		private string _userID;

		public string Format
		{
			get { return "Study {0} accessed by {1}"; }
		}

		public FakeStudyAccessedAuditMessage(string studyInstanceUID, string userID)
		{
			_studyInstanceUID = studyInstanceUID;
			_userID = userID;
		}

		public string GetMessage()
		{
			return string.Format(Format, _studyInstanceUID, _userID);
		}
	}

	internal class FakeAuditor : IAuditor
	{
		public FakeAuditor()
		{
		}

		public void Audit(IAuditMessage auditMessage)
		{
			string message = auditMessage.GetMessage();
			//here's where you would log the audit message to your actual auditor service.
		}
	}

	internal class FakeAuditorExtensionPoint : IExtensionPoint
	{
		int _numberFakeAuditors;

		public FakeAuditorExtensionPoint(int numberFakeAuditors)
		{
			_numberFakeAuditors = numberFakeAuditors;
		}

		public object[] CreateExtensions()
		{
			List<IAuditor> list = new List<IAuditor>();
			for (int i = 0; i < _numberFakeAuditors; ++i)
				list.Add(new FakeAuditor());

			return list.ToArray();
		}

		#region Not Implemented

		public IExtensionPoint[] GetExtensions()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public ExtensionInfo[] ListExtensions()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public ExtensionInfo[] ListExtensions(ExtensionFilter filter)
		{
			throw new Exception("The method or operation is not implemented.");
		}

        public ExtensionInfo[] ListExtensions(Predicate<ExtensionInfo> filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

		public object CreateExtension()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public object CreateExtension(ExtensionFilter filter)
		{
			throw new Exception("The method or operation is not implemented.");
		}
        public object CreateExtension(Predicate<ExtensionInfo> filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

		public object[] CreateExtensions(ExtensionFilter filter)
		{
			throw new Exception("The method or operation is not implemented.");
		}
        public object[] CreateExtensions(Predicate<ExtensionInfo> filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
	}

	internal class FakeAuditManager : AuditManager
	{
		private int _numberFakeAuditors;

		public FakeAuditManager(int numberFakeAuditors)
		{
			_numberFakeAuditors = numberFakeAuditors;
		}

		public ICollection Auditors
		{
			get { return new List<IAuditor>(Extensions); }
		}

		protected override IExtensionPoint GetExtensionPoint()
		{
			return new FakeAuditorExtensionPoint(_numberFakeAuditors);
		}
	}
	
	internal class MockAuditorExtensionPoint : IExtensionPoint
	{
		Mockery _mockery;
		int _numberMockAuditors;

		public MockAuditorExtensionPoint(Mockery mockery, int numberMockAuditors)
		{
			_mockery = mockery;
			_numberMockAuditors = numberMockAuditors;
		}

		public object[] CreateExtensions()
		{
			List<IAuditor> list = new List<IAuditor>();
			for (int i = 0; i < _numberMockAuditors; ++i)
			{
				IAuditor auditor = _mockery.NewMock<IAuditor>();
				//have to set expectations after construction and before Audit call, so unfortunately must do it here.
				Expect.Once.On(auditor).Method("Audit");
				list.Add(auditor);
			}

			return list.ToArray();
		}

		#region Not Implemented

        public IExtensionPoint[] GetExtensions()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ExtensionInfo[] ListExtensions()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ExtensionInfo[] ListExtensions(ExtensionFilter filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ExtensionInfo[] ListExtensions(Predicate<ExtensionInfo> filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object CreateExtension()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object CreateExtension(ExtensionFilter filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public object CreateExtension(Predicate<ExtensionInfo> filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object[] CreateExtensions(ExtensionFilter filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public object[] CreateExtensions(Predicate<ExtensionInfo> filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        #endregion
	}

	internal class MockAuditManager : AuditManager
	{
		Mockery _mockery;
		int _numberMockAuditors;

		public MockAuditManager(Mockery mockery, int numberMockAuditors)
		{
			_mockery = mockery;
			_numberMockAuditors = numberMockAuditors;
		}

		public ICollection Auditors
		{
			get { return new List<IAuditor>(Extensions); }
		}

		protected override IExtensionPoint GetExtensionPoint()
		{
			return new MockAuditorExtensionPoint(_mockery, _numberMockAuditors);
		}
	}

	internal class BadAuditorExtensionPoint : IExtensionPoint
	{
		public object[] CreateExtensions()
		{
			//return an extension point that doesn't implement IAuditor.
			return new object[] { new FakeRawAuditMessage("Test") };
		}

		#region Not Implemented

        public IExtensionPoint[] GetExtensions()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ExtensionInfo[] ListExtensions()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ExtensionInfo[] ListExtensions(ExtensionFilter filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ExtensionInfo[] ListExtensions(Predicate<ExtensionInfo> filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object CreateExtension()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object CreateExtension(ExtensionFilter filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public object CreateExtension(Predicate<ExtensionInfo> filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object[] CreateExtensions(ExtensionFilter filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        public object[] CreateExtensions(Predicate<ExtensionInfo> filter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

		#endregion
	}

	//try to use an ExtensionPointManager where the extension's interface is IExtensionPoint.
	internal class BadExtensionPointManager : BasicExtensionPointManager<IExtensionPoint>
	{
		protected override IExtensionPoint GetExtensionPoint()
		{
			return null;
		}

		public void Load()
		{
			LoadExtensions();
		}
	}

	//Try to use an ExtensionPointManager for a class (e.g. not an interface).
	internal class BadExtensionPointManager2 : BasicExtensionPointManager<FakeAuditor>
	{
		protected override IExtensionPoint GetExtensionPoint()
		{
			return null;
		}

		public void Load()
		{
			LoadExtensions();
		}
	}

	//Try to use an ExtensionPointManager where GetExtensionPoint returns something that doesn't implement IExtensionPoint.
	internal class BadExtensionPointManager3 : BasicExtensionPointManager<IAuditor>
	{
		protected override IExtensionPoint GetExtensionPoint()
		{
			//return an object that doesn't implement IExtensionPoint.
			return (IExtensionPoint)new FakeRawAuditMessage("test");
		}

		public void Load()
		{
			LoadExtensions();
		}
	}

	//Try to use an ExtensionPointManager where GetExtensionPoint returns an object that doesn't implement the expected interface (IAuditor).
	internal class BadExtensionPointManager4 : BasicExtensionPointManager<IAuditor>
	{
		protected override IExtensionPoint GetExtensionPoint()
		{
			//return an object that doesn't implement IAuditor.
			return (IExtensionPoint)new BadAuditorExtensionPoint();
		}

		public void Load()
		{
			LoadExtensions();
		}
	}

	[TestFixture]
	internal class AuditorTests
	{
		private Mockery _mockery;
		private int _numberAuditors;

		public AuditorTests()
		{
		}

		[SetUp]
		public void Setup()
		{
			_mockery = new Mockery();
			_numberAuditors = 2;
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void TestMockAuditManager()
		{
			MockAuditManager auditManager = new MockAuditManager(_mockery, _numberAuditors); 
			
			IAuditMessage message = _mockery.NewMock<IAuditMessage>();

			//The auditor has been mocked, so this won't get called.
			Expect.Exactly(0).On(message).Method("GetMessage");

			Assert.IsEmpty(auditManager.Auditors);

			//Auditors are 'lazy created' so the expectations are set in the MockAuditManager.
			auditManager.Audit(message);

			Assert.IsTrue(auditManager.Auditors.Count == _numberAuditors);

			_mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void TestFakeAuditManager()
		{
			FakeAuditManager auditManager = new FakeAuditManager(_numberAuditors);

			IAuditMessage message = _mockery.NewMock<IAuditMessage>();

			//GetMessage will get called once per 'fake' auditor.
			Expect.Exactly(_numberAuditors).On(message).Method("GetMessage");

			Assert.IsEmpty(auditManager.Auditors); // there are no auditors yet.

			auditManager.Audit(message);

			Assert.IsTrue(auditManager.Auditors.Count == _numberAuditors);

			_mockery.VerifyAllExpectationsHaveBeenMet();
		}

		[Test]
		public void TestBasicExtensionPointManager()
		{
			try
			{
				//try to use ExtensionPoint extensions.
				BadExtensionPointManager bad = new BadExtensionPointManager();
			}
			catch (InvalidOperationException)
			{
				return;
			}

			throw new Exception("Expected a InvalidOperationException");
		}

		[Test]
		public void TestBasicExtensionPointManager2()
		{
			try
			{
				//try to implement an ExtensionPointManager using a class.
				BadExtensionPointManager2 bad = new BadExtensionPointManager2();
			}
			catch (InvalidOperationException)
			{
				return;
			}

			throw new Exception("Expected a InvalidOperationException");
		}

		[Test]
		public void TestBasicExtensionPointManager3()
		{
			try
			{
				BadExtensionPointManager3 bad = new BadExtensionPointManager3();
				bad.Load();
				
			}
			catch (InvalidCastException)
			{
				return; //expect an invalid cast exception because GetExtensionPoint returns an invalid type.
			}

			throw new Exception("Expected an InvalidCastException");
		}

		[Test]
		public void TestBasicExtensionPointManager4()
		{
			try
			{
				BadExtensionPointManager4 bad = new BadExtensionPointManager4();
				bad.Load();
			}
			catch (InvalidCastException)
			{
				return; //expect an invalid cast exception because CreateExtensions returns a type that doesn't implement IAuditor.
			}

			throw new Exception("Expected an InvalidCastException");
		}
	}
}

#endif