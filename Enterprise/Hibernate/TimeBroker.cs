using System;
using System.Data;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
    [ClearCanvas.Common.ExtensionOf(typeof(BrokerExtensionPoint))]
    public class TimeBroker : Broker, ITimeBroker
    {
        #region ITimeBroker Members

        public DateTime GetTime()
        {
            DateTime currentTime;

            string sql = @"select getDate()";
            IDbCommand dbCommand;
            IDataReader reader;

            try
            {
                using (dbCommand = this.Context.CreateSqlCommand(sql))
                {
                    using (reader = dbCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentTime = reader.GetDateTime(0);
                            return currentTime;
                        }
                        else
                        {
                            throw new Exception("Database returned no results");
                        }
                    }                    
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unable to query database for current time", e);
            }
        }

        #endregion
    }    
}