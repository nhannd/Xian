using System;

using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
    ///<summary>
    ///</summary>
    [ExtensionPoint]
    public sealed class ExceptionPolicyExtensionPoint : ExtensionPoint<IExceptionPolicy> { }

    /// <summary>
    /// Provides Exception specific handling policies
    /// </summary>
    /// <example>
    /// <code>
    /// [ExtensionOf(typeof(ExceptionPolicyExtensionPoint))]
    /// [ExceptionPolicyFor(typeof(FooException))]
    /// public class FooExceptionPolicy : IExceptionPolicy
    /// {
    ///     ...
    /// }
    /// </code>
    /// </example>
    public interface IExceptionPolicy
    {
        ///<summary>
        /// Handles the specified Exception
        ///</summary>
        ///<param name="e"></param>
        ///<param name="exceptonHandlingContext"></param>
        void Handle(Exception e, IExceptionHandlingContext exceptonHandlingContext);
    }

    ///<summary>
    /// Specifies an exception type to which an <see cref="IExceptionPolicy"/> applies
    ///</summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class ExceptionPolicyForAttribute : Attribute
    {
        private readonly Type _exceptionType;

        ///<summary>
        ///</summary>
        ///<param name="exceptionType"></param>
        public ExceptionPolicyForAttribute(Type exceptionType)
        {
            _exceptionType = exceptionType;
        }

        ///<summary>
        ///</summary>
        public Type ExceptionType
        {
            get { return _exceptionType; }
        }
    }
}

