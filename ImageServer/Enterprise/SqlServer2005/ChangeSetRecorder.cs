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
using ClearCanvas.Common;
using ClearCanvas.Common.Audit;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.ImageServer.Enterprise.SqlServer2005
{
	public class ChangeSetRecorder : IEntityChangeSetRecorder
	{
		private string _operationName;
		public string OperationName
		{
			get { return _operationName; }
			set { _operationName = value; }
		}

		public void WriteLogEntry(IEnumerable<EntityChange> changeSet, AuditLog auditLog)
		{
            AuditLogEntry entry = null;
            try
			{
                entry = CreateLogEntry(changeSet);
                auditLog.WriteEntry(entry.Category, entry.Details);
            }
			catch(Exception ex)
			{
                // Error saving the audit log repository. Write to log file instead.

                Platform.Log(LogLevel.Error, ex, "Error occurred when writing audit log");
                
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Audit log entry failed to save:");

                if (entry!=null)
                {
                    sb.AppendLine(String.Format("Operation: {0}", entry.Operation));
                    sb.AppendLine(String.Format("Details: {0}", entry.Details));
                }
                else
                {
                    foreach (EntityChange change in changeSet)
                    {
                        sb.AppendLine(String.Format("Changeset: {0} on entity: {1}", change.ChangeType, change.EntityRef));
                        if (change.PropertyChanges != null)
                        {
                            foreach (PropertyChange property in change.PropertyChanges)
                            {
                                sb.AppendLine(String.Format("{0} : Old Value: {1}\tNew Value: {2}",
                                                            property.PropertyName, property.OldValue, property.NewValue));

                            }
                        }
                    }
                }
                
			    Platform.Log(LogLevel.Info, sb.ToString());
			}

		}

		public AuditLogEntry CreateLogEntry(IEnumerable<EntityChange> changeSet)
		{
			string details = string.Empty;
			string type = string.Empty;
			foreach (EntityChange change in changeSet)
			{
				if (change.ChangeType == EntityChangeType.Create)
				{
					type = "Create";
				}
				else if (change.ChangeType == EntityChangeType.Delete)
				{
					type = "Delete";
				}
				else if (change.ChangeType == EntityChangeType.Update)
				{
					type = "Update";
				}
			}
			return new AuditLogEntry("ImageServer", type, details);			
		}
	}
}
