#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

#if UNIT_TESTS

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace ClearCanvas.Common.Utilities.Tests
{
	[TestFixture]
	public class ObjectWalkerTests
	{
		public class Foo
		{
			public static readonly object SomeObject = new object();

			private bool PrivateField = true;
			public string PublicField = "hello";

			private int _privatePropertyField = 10;
			private object _publicPropertyField = SomeObject;

			private int PrivateProperty
			{
				get { return _privatePropertyField; }
				set { _privatePropertyField = value;  }
			}

			public object PublicProperty
			{
				get { return _publicPropertyField; }
				set { _publicPropertyField = value; }
			}

		}



		public ObjectWalkerTests()
		{

		}

		[Test]
		public void TestPrivatePublic()
		{
			ObjectWalker walker = new ObjectWalker();

			// public properties
			walker.IncludeNonPublicFields = false;
			walker.IncludeNonPublicProperties = false;
			walker.IncludePublicFields = false;
			walker.IncludePublicProperties = true;

			List<IObjectMemberContext> members = new List<IObjectMemberContext>(walker.Walk(typeof (Foo)));
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("PublicProperty", members[0].Member.Name);

			// public fields
			walker.IncludeNonPublicFields = false;
			walker.IncludeNonPublicProperties = false;
			walker.IncludePublicFields = true;
			walker.IncludePublicProperties = false;

			members = new List<IObjectMemberContext>(walker.Walk(typeof(Foo)));
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("PublicField", members[0].Member.Name);

			// private property
			walker.IncludeNonPublicFields = false;
			walker.IncludeNonPublicProperties = true;
			walker.IncludePublicFields = false;
			walker.IncludePublicProperties = false;

			members = new List<IObjectMemberContext>(walker.Walk(typeof(Foo)));
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("PrivateProperty", members[0].Member.Name);

			// private field
			walker.IncludeNonPublicFields = true;
			walker.IncludeNonPublicProperties = false;
			walker.IncludePublicFields = false;
			walker.IncludePublicProperties = false;

			members = new List<IObjectMemberContext>(walker.Walk(typeof(Foo)));
			Assert.AreEqual(1, members.Count);
			Assert.AreEqual("PrivateField", members[0].Member.Name);

			// all
			walker.IncludeNonPublicFields = true;
			walker.IncludeNonPublicProperties = true;
			walker.IncludePublicFields = true;
			walker.IncludePublicProperties = true;

			members = new List<IObjectMemberContext>(walker.Walk(typeof(Foo)));
			Assert.AreEqual(4, members.Count);
		}

	}
}

#endif
