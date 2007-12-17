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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using System.IO;

namespace WSClient
{
    public partial class Form1 : Form
    {
//        ChannelFactory<ITestService> _testServiceFactory;

        public Form1()
        {
            InitializeComponent();
/*
            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;


            _testServiceFactory = new ChannelFactory<ITestService>(
                binding,
                new EndpointAddress("http://localhost:8000/ClearCanvas.Ris.Test.Common.ITestService/"));
            _testServiceFactory.Credentials.UserName.UserName = "me";
            _testServiceFactory.Credentials.UserName.Password = "mmm";
            _testServiceFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust;

 */     
        }

        private void _sendButton_Click(object sender, EventArgs e)
        {
            try
            {
 /*               ITestService testService = _testServiceFactory.CreateChannel();
                using (testService as IDisposable)
                {
                    EntityRef profileRef = testService.FindOnePatientProfile(_number.Text);
                    PatientProfilePreview preview = testService.GetPatientProfilePreview(profileRef);

                    StringWriter text = new StringWriter();
                    text.WriteLine(preview.Name);
                    text.WriteLine(preview.Mrn);
                    text.WriteLine(preview.DateOfBirth);
                    text.WriteLine();
                    foreach (AddressInfo a in preview.Addresses)
                        text.WriteLine(string.Format("{0}: {1}", a.Type, a.DisplayValue));
                    foreach (TelephoneNumberInfo a in preview.PhoneNumbers)
                        text.WriteLine(string.Format("{0}: {1}", a.Type, a.DisplayValue));



                    _result.Text = text.ToString();
                }
  */
            }
            catch (TimeoutException timeProblem)
            {
                Console.WriteLine("The service operation timed out. " + timeProblem.Message);
            }
            catch (FaultException unknownFault)
            {
                Console.WriteLine("An unknown exception was received. " + unknownFault.Message);
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("There was a communication problem. " + commProblem.Message + commProblem.StackTrace);
            }
        }
    }
}