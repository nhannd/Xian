using System.Collections.Generic;
using System;
using System.Data;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class ExternalPractitionerContactPointBroker
	{
		#region IExternalPractitionerContactPointBroker Members

		public void MergeContactPoints(ExternalPractitionerContactPoint duplicate, ExternalPractitionerContactPoint original)
		{
			string duplciateOID = duplicate.GetRef().ToString(false, false);
			string originalOID = original.GetRef().ToString(false, false);

			// Transfer all order result recipient to the new contact point
			ReplaceValueInRow("OrderResultRecipient_", "PractitionerContactPointOID_", duplciateOID, originalOID);

			// Delete all addresses of the duplicate contact point
			DeleteRow("ExternalPractitionerContactPointAddress_", "ExternalPractitionerContactPointOID_", duplciateOID);

			// Delete all emails of the duplicate contact point
			DeleteRow("ExternalPractitionerContactPointEmailAddress_", "ExternalPractitionerContactPointOID_", duplciateOID);

			// Delete all phone numbers of the duplicate contact point
			DeleteRow("ExternalPractitionerContactPointTelephoneNumber_", "ExternalPractitionerContactPointOID_", duplciateOID);

			// Delete the duplicate contact point
			DeleteRow("ExternalPractitionerContactPoint_", "OID_", duplciateOID);
		}

		public IList<Order> GetRelatedOrders(ExternalPractitionerContactPoint contactPoint)
		{
			HqlFrom hqlFrom = new HqlFrom(typeof(Order).Name, "o");
			hqlFrom.Joins.Add(new HqlJoin("o.ResultRecipients", "rr"));

			HqlProjectionQuery query = new HqlProjectionQuery(hqlFrom);

			ResultRecipientSearchCriteria criteria = new ResultRecipientSearchCriteria();
			criteria.PractitionerContactPoint.EqualTo(contactPoint);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("rr", criteria));

			return ExecuteHql<Order>(query);
		}

		#endregion

		private int DeleteRow(string tableName, string columnName, object value)
		{
			try 
			{
				string deleteSql = string.Format("DELETE FROM {0} WHERE {1} = @value", tableName, columnName);
				IDbCommand delete = this.Context.CreateSqlCommand(deleteSql);
				AddParameter(delete, "value", value);

				return delete.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				throw new PersistenceException(SR.ErrorFailedToDeleteRow, e);
			}
		}

		private int ReplaceValueInRow(string tableName, string columnName, object oldValue, object newValue)
		{
			try 
			{
				string updateSql = string.Format("UPDATE {0} SET {1} = @newValue WHERE {1} = @oldValue", tableName, columnName);
				IDbCommand update = this.Context.CreateSqlCommand(updateSql);
				AddParameter(update, "oldValue", oldValue);
				AddParameter(update, "newValue", newValue);

				return update.ExecuteNonQuery();
			}
			catch (Exception e)
			{
				throw new PersistenceException(SR.ErrorFailedToReplaceValue, e);
			}
		}

		private void AddParameter(IDbCommand cmd, string name, object value)
		{
			IDbDataParameter p = cmd.CreateParameter();
			p.ParameterName = name;
			p.Value = value;
			cmd.Parameters.Add(p);
		}
	}
}
