using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Common;
using System.Reflection;
using NHibernate;
using ClearCanvas.Enterprise.Hibernate.Hql;

namespace ClearCanvas.Enterprise.Hibernate
{
    [ExtensionOf(typeof(BrokerExtensionPoint))]
    public class UniqueConstraintValidationBroker : Broker, IUniqueConstraintValidationBroker
    {
        #region IUniqueConstraintValidationBroker Members

        public bool IsUnique(DomainObject obj, string[] uniqueConstraintMembers)
        {
            Platform.CheckForNullReference(obj, "obj");
            Platform.CheckForNullReference(uniqueConstraintMembers, "uniqueConstraintMembers");

            if (uniqueConstraintMembers.Length == 0)
                throw new InvalidOperationException("uniqueConstraintMembers must contain at least one entry.");

            HqlQuery hqlQuery = BuildQuery(obj, uniqueConstraintMembers);

            // create a new session to do the validation query
            // this is a bit of a HACK, but we know that this may occur during an interceptor callback
            // on the originating session, and we don't want to modify the state of that session
            // ideally the new session should share the connection and transaction of the main session,
            // but this isn't possible with NHibernate 1.0.3
            using (ISession auxSession = this.Context.PersistentStore.SessionFactory.OpenSession())
            {
                auxSession.FlushMode = FlushMode.Never;
                IQuery query = auxSession.CreateQuery(hqlQuery.Hql);
                int i = 0;
                foreach (HqlCondition condition in hqlQuery.Conditions)
                {
                    foreach(object paramVal in condition.Parameters)
                        query.SetParameter(i++, paramVal);
                }

                int count = (int)query.UniqueResult();

                // if count == 0, there are no other objects with this particular set of values
                return count == 0;
            }
        }

        #endregion

        private HqlQuery BuildQuery(DomainObject obj, string[] uniqueConstraintMembers)
        {
            // get the id of the object being validated
            // if the object is unsaved, this id may be null
            object id = (obj is Entity) ? (obj as Entity).OID : (obj as EnumValue).Code;

            HqlQuery query = new HqlQuery(string.Format("select count(*) from {0} x", obj.GetType().Name));
            if (id != null)
            {
                // this object may have already been saved (i.e. this is an update) and therefore should be
                // excluded
                query.Conditions.Add(new HqlCondition("x.id <> ?", new object[] { id }));
            }

            // add other conditions 
            foreach (string propertyExpression in uniqueConstraintMembers)
            {
                object value = EvaluatePropertyExpression(obj, propertyExpression);
                if (value == null)
                    query.Conditions.Add(new HqlCondition(string.Format("x.{0} is null", propertyExpression), new object[] { }));
                else
                    query.Conditions.Add(new HqlCondition(string.Format("x.{0} = ?", propertyExpression), new object[] { value }));
            }

            return query;
       }

        private object EvaluatePropertyExpression(object subject, string propertyExpression)
        {
            if (string.IsNullOrEmpty(propertyExpression))
                throw new InvalidOperationException("propertyExpression must be non-empty.");

            string[] parts = propertyExpression.Split('.');
            return EvaluatePropertyExpression(subject, parts, 0);
        }

        private object EvaluatePropertyExpression(object subject, string[] parts, int partIndex)
        {
            // find property
            Type subjectType = subject.GetType();
            PropertyInfo propertyInfo = subjectType.GetProperty(parts[partIndex]);
            if(propertyInfo == null)
                throw new InvalidOperationException(string.Format("{0} is not a property of type {1}.",
                    propertyInfo.Name, subjectType.FullName));

            // find get method
            MethodInfo getter = propertyInfo.GetGetMethod(true);
            if (getter == null)
                throw new InvalidOperationException(string.Format("Property {0} of type {1} has no accessible get method.",
                    propertyInfo.Name, subjectType.FullName));

            // evaluate 
            object result = getter.Invoke(subject, null);

            // if null, can't go any further with the expression
            if (result == null)
                return null;

            // return the result if this is the end of the expression, otherwise recur
            return (++partIndex == parts.Length) ? result : EvaluatePropertyExpression(result, parts, partIndex);
        }

    }
}
