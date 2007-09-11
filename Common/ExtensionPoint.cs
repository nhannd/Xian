using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Base class for all extension points.
    /// </summary>
    /// <typeparam name="TInterface">The interface that extensions are expected to implement.</typeparam>
    /// <remarks>
    /// <para>
    /// To define an extension point, create a dedicated subclass of this class, specifying the interface
    /// that extensions are expected to implement.  The name of the subclass should be chosen
    /// with care, as the name effectively acts as a unique identifier which all extensions
    /// will reference.  Once chosen, the name should not be changed, as doing so will break all
    /// existing extensions to this extension point.  There is no need to add any methods to the subclass,
    /// and it is recommended that the class be left empty, such that it serves as a dedicated
    /// factory for creating extensions of this extension point.
    /// </para>
    /// <para>The subclass must also be marked with the <see cref="ExtensionPointAttribute" /> in order
    /// for the framework to discover it at runtime.
    /// </para>
    /// </remarks>
    public abstract class ExtensionPoint<TInterface> : ExtensionPointBase
    {
        protected override Type InterfaceType
        {
            get { return typeof(TInterface); }
        }
    }

    public abstract class ExtensionPointBase : IExtensionPoint
    {
        class PredicateExtensionFilter : ExtensionFilter
        {
            private Predicate<ExtensionInfo> _predicate;

            public PredicateExtensionFilter(Predicate<ExtensionInfo> predicate)
            {
                _predicate = predicate;
            }

            public override bool Test(ExtensionInfo extension)
            {
                return _predicate(extension);
            }
        }

        #region IExtensionPoint methods

        /// <summary>
        /// Lists meta-data for all extensions of this point.
        /// </summary>
        /// <returns></returns>
        public ExtensionInfo[] ListExtensions()
        {
            return ListExtensions(this.GetType(), null).ToArray();
        }

        /// <summary>
        /// List meta-data for extensions of this point that match the supplied filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ExtensionInfo[] ListExtensions(ExtensionFilter filter)
        {
            return ListExtensions(this.GetType(), filter).ToArray();
        }

        /// <summary>
        /// List meta-data for extensions of this point that match the supplied filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ExtensionInfo[] ListExtensions(Predicate<ExtensionInfo> filter)
        {
            return ListExtensions(new PredicateExtensionFilter(filter));
        }

        /// <summary>
        /// Instantiates one extension of this point.
        /// </summary>
        /// <returns></returns>
        public object CreateExtension()
        {
            return AtLeastOne(CreateExtensionsHelper(this.GetType(), null, true), this.GetType());
        }

        /// <summary>
        /// Instantiates one extension of this point that matches the specified filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public object CreateExtension(ExtensionFilter filter)
        {
            return AtLeastOne(CreateExtensionsHelper(this.GetType(), filter, true), this.GetType());
        }

        /// <summary>
        /// Instantiates one extension of this point that matches the specified filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public object CreateExtension(Predicate<ExtensionInfo> filter)
        {
            return CreateExtension(new PredicateExtensionFilter(filter));
        }

        /// <summary>
        /// Instantiates all extensions of this point.
        /// </summary>
        /// <returns></returns>
        public object[] CreateExtensions()
        {
            return CreateExtensionsHelper(this.GetType(), null, false);
        }

        /// <summary>
        /// Instantiates all extensions of this point that match the supplied filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public object[] CreateExtensions(ExtensionFilter filter)
        {
            return CreateExtensionsHelper(this.GetType(), filter, false);
        }

        /// <summary>
        /// Instantiates all extensions of this point that match the supplied filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public object[] CreateExtensions(Predicate<ExtensionInfo> filter)
        {
            return CreateExtensionsHelper(this.GetType(), new PredicateExtensionFilter(filter), false);
        }

        #endregion

        #region Protected methods

        protected abstract Type InterfaceType { get; }

        protected object[] CreateExtensionsHelper(Type extensionPointClass, ExtensionFilter filter, bool justOne)
        {
            // get subset of applicable extensions
            List<ExtensionInfo> extensions = ListExtensions(extensionPointClass, filter);

            // attempt to instantiate the extension classes
            List<object> createdObjects = new List<object>();
            foreach (ExtensionInfo extension in extensions)
            {
                if (justOne && createdObjects.Count > 0)
                    break;

                // is the extension a concrete class?
                if (!IsConcreteClass(extension.ExtensionClass))
                {
                    Platform.Log(LogLevel.Warn, SR.ExceptionExtensionMustBeConcreteClass,
                        extension.ExtensionClass.FullName);
                    continue;
                }

                // does the extension implement the required interface?
                if (!InterfaceType.IsAssignableFrom(extension.ExtensionClass))
                {
                    Platform.Log(LogLevel.Warn, SR.ExceptionExtensionDoesNotImplementRequiredInterface,
                        extension.ExtensionClass.FullName,
                        InterfaceType);

                    continue;
                }

                try
                {
                    // instantiate
                    object o = Activator.CreateInstance(extension.ExtensionClass);
                    createdObjects.Add(o);
                }
                catch (Exception e)
                {
                    // instantiation failed
					Platform.Log(LogLevel.Error, e);
				}
            }

            return createdObjects.ToArray();
        }

        protected List<ExtensionInfo> ListExtensions(Type extensionPointClass, ExtensionFilter filter)
        {
            List<ExtensionInfo> extensions = new List<ExtensionInfo>();
            foreach (ExtensionInfo extension in Platform.PluginManager.Extensions)
            {
                if (extension.PointExtended == extensionPointClass
                    && (filter == null || filter.Test(extension)))
                {
                    extensions.Add(extension);
                }
            }
            return extensions;
        }

        protected object AtLeastOne(object[] objs, Type extensionPointType)
        {
            if (objs.Length > 0)
            {
                return objs[0];
            }
            else
            {
                throw new NotSupportedException(
                    string.Format(SR.ExceptionNoExtensionsCreated, extensionPointType.FullName));
            }
        }

        protected bool IsConcreteClass(Type type)
        {
            return !type.IsAbstract && type.IsClass;
        }

        #endregion
    }

}
