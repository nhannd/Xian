#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Manages instantiation of noteboxs.
    /// </summary>
    public class NoteboxFactory
    {
        #region Static members

        private static readonly NoteboxFactory _theInstance = new NoteboxFactory();

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static NoteboxFactory Instance
        {
            get { return _theInstance; }
        }

        #endregion

        private readonly List<Type> _noteboxClasses;

        private NoteboxFactory()
        {
            _noteboxClasses = CollectionUtils.Map<ExtensionInfo, Type>(new NoteboxExtensionPoint().ListExtensions(),
                delegate(ExtensionInfo info) { return info.ExtensionClass; });
        }

        /// <summary>
        /// Creates an instance of the notebox class as specified by the class name, which may be fully or
        /// only partially qualified.
        /// </summary>
        /// <param name="noteboxClassName"></param>
        /// <returns></returns>
        public Notebox CreateNotebox(string noteboxClassName)
        {
            return CreateNotebox(ResolvePartialClassName(noteboxClassName));
        }

        /// <summary>
        /// Creates an instance of the specified notebox class.
        /// </summary>
        /// <param name="noteboxClass"></param>
        /// <returns></returns>
        public Notebox CreateNotebox(Type noteboxClass)
        {
            return (Notebox)Activator.CreateInstance(noteboxClass);
        }


        private Type ResolvePartialClassName(string noteboxClassName)
        {
            Type noteboxClass = CollectionUtils.SelectFirst(_noteboxClasses,
                delegate(Type t) { return t.FullName.Contains(noteboxClassName); });

            if(noteboxClass == null)
                throw new ArgumentException(string.Format("{0} is not a valid notebox class name.", noteboxClassName));

            return noteboxClass;
        }
    }
}