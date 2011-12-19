#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Text;
using System.Net.Mail;
using Microsoft.Build.Framework;
using System.Net;

namespace ClearCanvas.Utilities.BuildTasks
{
    public class SendEmail : Microsoft.Build.Utilities.Task
    {
        //Sends out an email
        //Was using Mail task from the MsBuild Community Tasks but that task does not allow us to specify an SMTP Port

        #region properties

        [Required]
        public string SmtpServer
        {
            get { return _smtpServer; }
            set { _smtpServer = value; }
        }

        public string SmtpUsername
        {
            get { return _smtpUsername; }
            set { _smtpUsername = value; }
        }

        public string SmtpPassword
        {
            get { return _smtpPassword; }
            set { _smtpPassword = value; }
        }

        public string SmtpPort
        {
            get { return _smtpPort; }
            set { _smtpPort = value; }
        }

        [Required]
        public string ToAddress
        {
            get { return _toAddress; }
            set { _toAddress = value; }
        }

        [Required]
        public string FromAddress
        {
            get { return _fromAddress; }
            set { _fromAddress = value; }
        }

        public string Subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        [Required]
        public string Body
        {
            get { return _body; }
            set { _body = value; }
        }

        public string Attachments
        {
            get { return _attachments; }
            set { _attachments = value; }
        }
        #endregion

        public override bool Execute()
        {
            MailMessage mail = new MailMessage();

            string[] toAddresses = _toAddress.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string address in toAddresses)
            {
                mail.To.Add(new MailAddress(address));
            }

            mail.From = new MailAddress(_fromAddress);
            mail.Subject = _subject;
            mail.Body = _body;

            if (_attachments != null && _attachments.Length > 0)
            {
                string[] attachments = _attachments.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string attachment in attachments)
                {
                    mail.Attachments.Add(new Attachment(attachment));
                }
            }

            try
            {
                SmtpClient client = new SmtpClient();
                client.Host = _smtpServer;
                if (!string.IsNullOrEmpty(_smtpUsername))
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                else
                    client.UseDefaultCredentials = true;

                client.Port = Int32.Parse(_smtpPort);

                client.Send(mail);
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(ex);
                return false;
            }
            finally
            {
                if (mail == null)
                    mail.Dispose();
            }

            return true;
        }
  
        private string _smtpServer;
        private string _smtpUsername;
        private string _smtpPassword;
        private string _smtpPort;
        private string _toAddress;
        private string _fromAddress;
        private string _subject;
        private string _body;
        private string _attachments;


    }
}
