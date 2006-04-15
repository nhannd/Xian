using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    public interface IExtensionPoint
    {
        ExtensionInfo[] ListExtensions();
        ExtensionInfo[] ListExtensions(ExtensionFilter filter);
        object CreateExtension();
        object CreateExtension(ExtensionFilter filter);
        object[] CreateExtensions();
        object[] CreateExtensions(ExtensionFilter filter);
    }

    public abstract class ExtensionPointBase : IExtensionPoint
    {
        public ExtensionInfo[] ListExtensions()
        {
            return ListExtensions(this.GetType(), null).ToArray();
        }

        public ExtensionInfo[] ListExtensions(ExtensionFilter filter)
        {
            return ListExtensions(this.GetType(), filter).ToArray();
        }

        public object CreateExtension()
        {
            return AtLeastOne(CreateExtensionsHelper(this.GetType(), null, true), this.GetType());
        }

        public object CreateExtension(ExtensionFilter filter)
        {
            return AtLeastOne(CreateExtensionsHelper(this.GetType(), filter, true), this.GetType());
        }

        public object[] CreateExtensions()
        {
            return CreateExtensionsHelper(this.GetType(), null, false);
        }
        public object[] CreateExtensions(ExtensionFilter filter)
        {
            return CreateExtensionsHelper(this.GetType(), null, false);
        }

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
                    Platform.Log(string.Format(SR.ExceptionExtensionMustBeConcreteClass,
                        extension.ExtensionClass.FullName), LogLevel.Warn);
                    continue;
                }

                // does the extension implement the required interface?
                if (!InterfaceType.IsAssignableFrom(extension.ExtensionClass))
                {
                    Platform.Log(string.Format(SR.ExceptionExtensionDoesNotImplementRequiredInterface,
                        extension.ExtensionClass.FullName,
                        InterfaceType), LogLevel.Warn);
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
					Platform.Log(e, LogLevel.Error);
				}
            }

            return createdObjects.ToArray();
        }

        protected List<ExtensionInfo> ListExtensions(Type extensionPointClass, ExtensionFilter filter)
        {
            List<ExtensionInfo> extensions = new List<ExtensionInfo>();
            foreach (ExtensionInfo extension in Platform.PluginManager.Extensions)
            {
                if (extension.ExtensionPointClass == extensionPointClass
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
    }

    public abstract class ExtensionPoint<TInterface> : ExtensionPointBase
    {
/*  This stuff won't compile correctly under MONO.
	Better to leave it out for now, can re-introduce it when MONO compiler matures
        public new TInterface CreateExtension()
        {
            return (TInterface)base.CreateExtension();
        }

        public new TInterface CreateExtension(ExtensionFilter filter)
        {
            return (TInterface)base.CreateExtension(filter);
        }

        public new TInterface[] CreateExtensions()
        {
            object[] objs = base.CreateExtensions();
            TInterface[] extensions = new TInterface[objs.Length];
            Array.Copy(objs, extensions, objs.Length);
            return extensions;
        }

        public new TInterface[] CreateExtensions(ExtensionFilter filter)
        {
            object[] objs = base.CreateExtensions(filter);
            TInterface[] extensions = new TInterface[objs.Length];
            Array.Copy(objs, extensions, objs.Length);
            return extensions;
        }
*/
        protected override Type InterfaceType
        {
            get { return typeof(TInterface); }
        }

    }
}
