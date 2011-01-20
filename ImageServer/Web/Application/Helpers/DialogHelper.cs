#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
				String text = String.Format("<tr class=\"ConfirmDialogItemText\"><td>{0}</td><td>&nbsp;</td><td>{1}&nbsp;</td><td>&nbsp;</td><td>{2}&nbsp;</td></tr>",
								 study.PatientsName, study.AccessionNumber, study.StudyDescription);
				message += text;

				i++;
			}
			message += "</table>";

			return message;
		}

        public static string createSeriesTable(IList<Series> series)
        {
            string message;

            message =
                "<table cellpadding=\"3\" cellspacing=\"0\" width=\"100%\" class=\"ConfirmDialogTable\"><tr class=\"ConfirmDialogHeadingText\"><td colspan=\"2\">Modality</td><td colspan=\"2\">Description</td><td colspan=\"2\">Related Instances</td><td colspan=\"2\">Instance UID</td></tr>";

            int i = 0;
            foreach (Series s in series)
            {
                String text = String.Format("<tr class=\"ConfirmDialogItemText\"><td>{0}</td><td>&nbsp;</td><td>{1}</td><td>&nbsp;</td><td>{2}</td><td>&nbsp;</td><td>{3}</td></tr>",
                                 s.Modality, s.SeriesDescription, s.NumberOfSeriesRelatedInstances, s.SeriesInstanceUid);
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
				String text = String.Format("<tr class=\"ConfirmDialogItemText\"><td>{0}&nbsp;</td><td>&nbsp;</td><td>{1}&nbsp;</td><td>&nbsp;</td><td>{2}&nbsp;</td></tr>",
								 study.PatientsName, study.AccessionNumber, study.StudyDescription);
				message += text;

				i++;
			}
			message += "</table>";

			return message;
		}
	}
}