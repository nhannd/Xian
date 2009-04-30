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