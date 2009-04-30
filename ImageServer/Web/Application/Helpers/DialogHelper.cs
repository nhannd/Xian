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

using System;
using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Helpers
{
	public class DialogHelper
	{
		public static string createConfirmationMessage(string message)
		{
			return string.Format("<span class=\"ConfirmDialogMessage\">{0}</span>", message);
		}

		public static string createStudyTable(IList<Study> studies)
		{
			string message;

			message =
				"<table cellpadding=\"3\" cellspacing=\"0\" width=\"100%\" class=\"ConfirmDialogTable\"><tr class=\"ConfirmDialogHeadingText\"><td colspan=\"2\">Patient</td><td colspan=\"2\">Accession</td><td>Description</td></tr>";

			int i = 0;
			foreach (Study study in studies)
			{
				String text = String.Format("<tr class=\"ConfirmDialogItemText\"><td>{0}</td><td>&nbsp;</td><td>{1}</td><td>&nbsp;</td><td>{2}</td></tr>",
								 study.PatientsName, study.AccessionNumber, study.StudyDescription);
				message += text;

				i++;
			}
			message += "</table>";

			return message;
		}

		public static string createStudyTable(IList<StudySummary> studies)
		{
			string message;

			message =
				"<table cellpadding=\"3\" cellspacing=\"0\" width=\"100%\" class=\"ConfirmDialogTable\"><tr class=\"ConfirmDialogHeadingText\"><td colspan=\"2\">Patient</td><td colspan=\"2\">Accession</td><td>Description</td></tr>";

			int i = 0;
			foreach (StudySummary study in studies)
			{
				String text = String.Format("<tr class=\"ConfirmDialogItemText\"><td>{0}</td><td>&nbsp;</td><td>{1}</td><td>&nbsp;</td><td>{2}</td></tr>",
								 study.PatientsName, study.AccessionNumber, study.StudyDescription);
				message += text;

				i++;
			}
			message += "</table>";

			return message;
		}
	}
}