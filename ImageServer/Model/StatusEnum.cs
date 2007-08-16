using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Model
{
    public class StatusEnum : ServerEnum
    {
        private static Dictionary<short, StatusEnum> _dict = new Dictionary<short, StatusEnum>();

        /// <summary>
        /// One-time load of status values from the database.
        /// </summary>
        static StatusEnum()
        {
            IEnumBroker<StatusEnum> broker = PersistentStoreRegistry.GetDefaultStore().OpenReadContext().GetBroker<IStatusEnum>();
            IList<StatusEnum> list = broker.Execute();

            foreach (StatusEnum type in list)
            {
                _dict.Add(type.Enum, type);
            }
        }

        #region Constructors
        public StatusEnum()
            : base("StatusEnum")
        {
        }
        #endregion

        public override void SetEnum(short val)
        {
            StatusEnum statusEnum;
            if (false == _dict.TryGetValue(val, out statusEnum))
                throw new PersistenceException("Unknown TypeEnum value: " + val, null);

            Enum = statusEnum.Enum;
            Lookup = statusEnum.Lookup;
            Description = statusEnum.Description;
            LongDescription = statusEnum.LongDescription;
        }

        public static StatusEnum GetEnum(string lookup)
        {
            foreach (StatusEnum status in _dict.Values)
            {
                if (status.Lookup.Equals(lookup))
                    return status;
            }
            throw new PersistenceException("Unknown StatusEnum: " + lookup, null);
        }
    }
}
