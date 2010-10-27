#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Model
{
	public partial class ApplicationLog
	{
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (Exception != null && Exception.Length > 0)
			{
				sb.AppendFormat("{0} {1} [{2}]  {3} - Exception thrown",
				              Host,
				              Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				              Thread,
				              LogLevel);
				sb.AppendLine();
				sb.AppendLine(Message);
				sb.AppendLine();
				sb.AppendLine(Exception);
			}
			else
				sb.AppendFormat("{0} {1} [{2}]  {3} - {4}",
				              Host,
				              Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff"),
				              Thread,
				              LogLevel,
				              Message);

			return sb.ToString();
		}

		public string MessageException
		{
			get
			{
				if (string.IsNullOrEmpty(Exception))
				{
					return Message
						.Replace("<","&lt;")
						.Replace(">","&gt;")
						.Replace(Environment.NewLine, "<br/>");
				}

				StringBuilder sb = new StringBuilder();
				sb.AppendLine(Message);
				sb.AppendLine(Exception);
				return sb.ToString()
						.Replace("<", "&lt;")
						.Replace(">", "&gt;")
						.Replace(Environment.NewLine, "<br/>");
			}
		}
	}
}
