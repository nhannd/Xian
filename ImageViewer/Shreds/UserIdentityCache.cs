#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Globalization;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.StudyManagement.Core;

namespace ClearCanvas.ImageViewer.Shreds
{
    internal static class UserIdentityCache
    {
        private static readonly object SyncObject = new object();
        private static ICache<UserIdentityContext> _identityContextCache;

        public static void Put(long identifier, UserIdentityContext context)
        {
            lock (SyncObject)
            {
               Initialize();

                _identityContextCache.Put(identifier.ToString(CultureInfo.InvariantCulture), context);
            }
        }

        public static void Remove(long identifier)
        {
            lock (SyncObject)
            {
                Initialize();
                _identityContextCache.Remove(identifier.ToString(CultureInfo.InvariantCulture));
            }
        }

        public static UserIdentityContext Get(long identifier)
        {
            lock (SyncObject)
            {
                Initialize();
                var context = _identityContextCache.Get(identifier.ToString(CultureInfo.InvariantCulture)) ??
                              new UserIdentityContext();

                return context;
            }
        }

        private static void Initialize()
        {
            if (_identityContextCache == null)
            {
                _identityContextCache = Cache<UserIdentityContext>.Create(typeof (UserIdentityContext).FullName);
                (_identityContextCache as Cache<UserIdentityContext>).Expiration = TimeSpan.FromMinutes(60);
            }
        }
    }
}
