using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Type;
using NHibernate.SqlTypes;
using System.Data;
using NHibernate.Engine;
using NHibernate;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// This class provides NHibernate mapping for <see cref="CodedValue"> types.  It is based on the class
    /// NHibernate.Type.EnumStringType, which can be found in the NHibernate source.
    /// </summary>
    /// <typeparam name="TCodedValue"></typeparam>
    [Serializable]
    public abstract class CodedValueHbm<TCodedValue> : ImmutableType, IDiscriminatorType
        where TCodedValue : CodedValue<TCodedValue>, new()
    {
        private readonly CodedValueDictionary<TCodedValue> _dictionary;

        /// <summary>
        /// Hardcoding of <c>255</c> for the maximum length
        /// of the Enum name that will be saved to the db.
        /// </summary>
        /// <value>
        /// <c>255</c> because that matches the default length that hbm2ddl will
        /// use to create the column.
        /// </value>
        public const int MaxLengthForEnumString = 255;

        /// <summary>
        /// Initializes a new instance of <see cref="EnumStringType"/>.
        /// </summary>
        /// <param name="enumClass">The <see cref="System.Type"/> of the Enum.</param>
        protected CodedValueHbm(CodedValueDictionary<TCodedValue> dictionary)
            : this(dictionary, MaxLengthForEnumString)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="EnumStringType"/>.
        /// </summary>
        /// <param name="enumClass">The <see cref="System.Type"/> of the Enum.</param>
        /// <param name="length">The length of the string that can be written to the column.</param>
        protected CodedValueHbm(CodedValueDictionary<TCodedValue> dictionary, int length)
            : base(SqlTypeFactory.GetString(length))
        {
            _dictionary = dictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual object GetInstance(object code)
        {
            //code is an named constants defined for the enumeration.
            try
            {
                return _dictionary.LookupByCode(code as string);
            }
            catch (ArgumentException ae)
            {
                throw new HibernateException(string.Format("Can't Parse {0} as {1}", code, typeof(TCodedValue).FullName), ae);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public virtual object GetValue(object codedValue)
        {
            //code is an enum instance.
            return codedValue == null ? string.Empty : (codedValue as TCodedValue).Code;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool Equals(object x, object y)
        {
            // TCodedValue has defined == operator, so this simple comparison will work
            return (x as TCodedValue) == (y as TCodedValue);
        }

        /// <summary>
        /// 
        /// </summary>
        public override System.Type ReturnedClass
        {
            get { return typeof(TCodedValue); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        public override void Set(IDbCommand cmd, object value, int index)
        {
            IDataParameter par = (IDataParameter)cmd.Parameters[index];
            if (value == null)
            {
                par.Value = DBNull.Value;
            }
            else
            {
                par.Value = ((TCodedValue)value).Code;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public override object Get(IDataReader rs, int index)
        {
            object code = rs[index];
            if (code == DBNull.Value || code == null)
            {
                return null;
            }
            else
            {
                return GetInstance(code);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rs"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public override object Get(IDataReader rs, string name)
        {
            return Get(rs, rs.GetOrdinal(name));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This appends <c>enumstring - </c> to the beginning of the underlying
        /// enums name so that <see cref="System.Enum"/> could still be stored
        /// using the underlying value through the <see cref="PersistentEnumType"/>
        /// also.
        /// </remarks>
        public override string Name
        {
            get { return "enumstring - " + typeof(TCodedValue).FullName; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override string ToString(object value)
        {
            return (value == null) ? null : GetValue(value).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cached"></param>
        /// <param name="session"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public override object Assemble(object cached, ISessionImplementor session, object owner)
        {
            if (cached == null)
            {
                return null;
            }
            else
            {
                return GetInstance(cached);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public override object Disassemble(object value, ISessionImplementor session)
        {
            return (value == null) ? null : GetValue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ObjectToSQLString(object value)
        {
            return GetValue(value).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public object StringToObject(string xml)
        {
            return FromString(xml);
        }

        public override object FromStringValue(string xml)
        {
            return GetInstance(xml);
        }

    }
}
