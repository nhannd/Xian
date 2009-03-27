using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
namespace ClearCanvas.Dicom.Audit.Test
{

    [TestFixture]
    public class AuditTest : AbstractTest
    {
        [Test]
        public void DicomQueryAuditTest()
        {
        	AssociationParameters parms = new ClientAssociationParameters("CLIENT", "SERVER",
        	                                                         new IPEndPoint(new IPAddress(new byte[] {2, 2, 2, 2}),
        	                                                                       2));
			parms.LocalEndPoint = new IPEndPoint(new IPAddress(new byte[] {1, 1, 1, 1}),
        	                                                                       1);

        	QueryAuditHelper helper =
        		new QueryAuditHelper(new DicomAuditSource("testApplication"), EventIdentificationTypeEventOutcomeIndicator.Success, parms);

			helper.AddParticipant("testUser","test@test","Test Name");
        	helper.AddPatientParticipant("id1234", "Test Patient");
        	helper.AddStudyParticipant("1.2.3.4.5");
			helper.AddStudyParticipant("1.2.3.4.5","1.2.3","A1234",new AuditSopClass[] {new AuditSopClass("1.2.3",5)});

        	string output = helper.Serialize(true);

        	Assert.IsNotEmpty(output);

        	string failure;
        	bool result = helper.Verify(out failure);

        	Assert.IsTrue(result, failure);

			helper = new QueryAuditHelper(new DicomAuditSource("testApplication2","enterpriseId", AuditSourceTypeCodeEnum.EndUserInterface, NetworkAccessPointTypeEnum.MachineName),EventIdentificationTypeEventOutcomeIndicator.Success, parms);

			output = helper.Serialize(true);

			Assert.IsNotEmpty(output);

			result = helper.Verify(out failure);

			Assert.IsTrue(result, failure);              
        }
	}
}

#endif