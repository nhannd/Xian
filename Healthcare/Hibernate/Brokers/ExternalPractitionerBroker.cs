using System.Collections.Generic;
using System.Data;
using System;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Healthcare.Hibernate.Brokers
{
	public partial class ExternalPractitionerBroker
	{
		protected static readonly HqlSelect SelectOrder = new HqlSelect("o");
		protected static readonly HqlSelect SelectVisit = new HqlSelect("v");

		protected static readonly HqlFrom fromVisit = new HqlFrom(typeof(Visit).Name, "v");

		protected static readonly HqlJoin JoinOrder = new HqlJoin("n.Order", "o");
		protected static readonly HqlJoin JoinPatient = new HqlJoin("o.Patient", "p");
		protected static readonly HqlJoin JoinPatientProfile = new HqlJoin("p.Profiles", "pp");
		protected static readonly HqlJoin JoinNotePostings = new HqlJoin("n.Postings", "np");


		#region IExternalPractitionerBroker Members

		public void MergePractitioners(ExternalPractitioner duplicate, ExternalPractitioner original)
		{
			string duplciateOID = duplicate.GetRef().ToString(false, false);
			string originalOID = original.GetRef().ToString(false, false);

			// Move all related orders to reference the new practitioner
			ReplaceValueInRow("Order_", "OrderingPractitionerOID_", duplciateOID, originalOID);

			// Move all related visits to reference the new practitioner
			ReplaceValueInRow("VisitPractitioner_", "PractitionerOID_", duplciateOID, originalOID);

			// Transfer all contact points to the new practitioner
			ReplaceValueInRow("ExternalPractitionerContactPoint_", "PractitionerOID_", duplciateOID, originalOID);

			// Delete extended properties of the duplicate practitioner
			DeleteRow("ExternalPractitionerExtendedProperties_", "ExternalPractitionerOID_", duplciateOID);

			// Delete the duplicate practitioner
			DeleteRow("ExternalPractitioner_", "OID_", duplciateOID);
		}

		public IList<Order> GetRelatedOrders(ExternalPractitioner practitioner)
		{
			HqlFrom hqlFrom = new HqlFrom(typeof(Order).Name, "o");
			HqlProjectionQuery query = new HqlProjectionQuery(hqlFrom);

			OrderSearchCriteria criteria = new OrderSearchCriteria();
			criteria.OrderingPractitioner.EqualTo(practitioner);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("o", criteria));

			return ExecuteHql<Order>(query);
		}

		public IList<Visit> GetRelatedVisits(ExternalPractitioner practitioner)
		{
			HqlFrom hqlFrom = new HqlFrom(typeof(Visit).Name, "v");
			hqlFrom.Joins.Add(new HqlJoin("v.Practitioners", "p"));
			HqlProjectionQuery query = new HqlProjectionQuery(hqlFrom);

			VisitPractitionerSearchCriteria criteria = new VisitPractitionerSearchCriteria();
			criteria.Practitioner.EqualTo(practitioner);
			query.Conditions.AddRange(HqlCondition.FromSearchCriteria("p", criteria));

			return ExecuteHql<Visit>(query);
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
