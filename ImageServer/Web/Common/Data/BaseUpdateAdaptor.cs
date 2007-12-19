using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class BaseUpdateAdaptor<TServerEntity, TCriteria, TISelect, TIUpdate, TParam> : BaseAdaptor<TServerEntity, TCriteria, TISelect>
             where TServerEntity : ServerEntity, new()
        where TCriteria : SelectCriteria, new()
        where TISelect : ISelectBroker<TCriteria, TServerEntity>
        where TIUpdate : IUpdateBroker<TServerEntity, TParam>
        where TParam : UpdateBrokerParameters
    {
        public bool Add(TParam param)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIUpdate update = context.GetBroker<TIUpdate>();

                    update.Insert(param);

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception adding {0}", typeof(TServerEntity));
                return false;
            }
        }

        public bool Update(ServerEntityKey key, TParam param)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIUpdate update = context.GetBroker<TIUpdate>();

                    update.Update(key, param);

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception updating {0}", typeof(TServerEntity));
                return false;
            }
        }

        public bool Delete(ServerEntityKey key)
        {
            try
            {
                using (IUpdateContext context = PersistentStore.OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    TIUpdate update = context.GetBroker<TIUpdate>();

                    if (!update.Delete(key))
                        return false;

                    context.Commit();
                }
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception updating {0}", typeof(TServerEntity));
                return false;
            }
        }
    }
}
