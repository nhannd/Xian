using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.ImageServer.Database;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Model.Brokers;


namespace ClearCanvas.ImageServer.Model
{
    public class TypeEnum : ServerEnum
    {
        private static Dictionary<short, TypeEnum> _dict = new Dictionary<short, TypeEnum>();

        /// <summary>
        /// One-time load from the database of type enumerated value.
        /// </summary>
        static TypeEnum()
        {
            IEnumBroker<TypeEnum> broker = PersistentStoreRegistry.GetDefaultStore().OpenReadContext().GetBroker<ITypeEnum>();
            IList<TypeEnum> list = broker.Execute();

            foreach (TypeEnum type in list)
            {
                _dict.Add(type.Enum, type);
            }
        }

        #region Constructors
        public TypeEnum()
            : base("TypeEnum")
        {
        }
        #endregion

        public override void SetEnum(short val)
        {
            TypeEnum typeEnum;
            if (false == _dict.TryGetValue(val, out typeEnum))
                throw new PersistenceException("Unknown TypeEnum value: " + val,null);

            Enum = typeEnum.Enum;
            Lookup = typeEnum.Lookup;
            Description = typeEnum.Description;
            LongDescription = typeEnum.LongDescription;
        }

        public static TypeEnum GetEnum(string lookup)
        {
            foreach (TypeEnum type in _dict.Values)
            {
                if (type.Lookup.Equals(lookup))
                    return type;
            }
            throw new PersistenceException("Unknown TypeEnum: " + lookup, null);
        }
    }
}
