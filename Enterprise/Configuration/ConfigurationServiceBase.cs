#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using System.Threading;

namespace ClearCanvas.Enterprise.Configuration
{
	/// <summary>
	/// Contains code shared by different configuration service implementations.
	/// </summary>
	public abstract class ConfigurationServiceBase : CoreServiceLayer
	{
		/// <summary>
		/// Gets the specified configuration document.
		/// </summary>
		/// <param name="documentKey"></param>
		/// <returns></returns>
		protected GetConfigurationDocumentResponse GetConfigurationDocumentHelper(ConfigurationDocumentKey documentKey)
		{
			CheckReadAccess(documentKey);

			var broker = PersistenceContext.GetBroker<IConfigurationDocumentBroker>();
			var criteria = BuildCurrentVersionCriteria(documentKey);
			var documents = broker.Find(criteria, new SearchResultPage(0, 1), new EntityFindOptions { Cache = true });

			var document = CollectionUtils.FirstElement(documents);
			return new GetConfigurationDocumentResponse(documentKey, document == null ? null : document.Body.DocumentText);
		}

		protected ConfigurationDocumentSearchCriteria BuildCurrentVersionCriteria(ConfigurationDocumentKey key)
		{
			var criteria = BuildUnversionedCriteria(key);
			criteria.DocumentVersionString.EqualTo(VersionUtils.ToPaddedVersionString(key.Version, false, false));
			return criteria;
		}

		protected ConfigurationDocumentSearchCriteria BuildUnversionedCriteria(ConfigurationDocumentKey key)
		{
			var criteria = new ConfigurationDocumentSearchCriteria();
			criteria.DocumentName.EqualTo(key.DocumentName);

			if (!string.IsNullOrEmpty(key.InstanceKey))
			{
				criteria.InstanceKey.EqualTo(key.InstanceKey);
			}
			else
			{
				criteria.InstanceKey.IsNull();
			}

			if (!string.IsNullOrEmpty(key.User))
			{
				criteria.User.EqualTo(key.User);
			}
			else
			{
				criteria.User.IsNull();
			}

			return criteria;
		}

		/// <summary>
		/// This method is called automatically by response caching framework
		/// to provide caching directive for configuration documents.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected ResponseCachingDirective GetDocumentCachingDirective(GetConfigurationDocumentRequest request)
		{
			// if the request is for ConfigurationStoreSettings, we cannot try to load 
			// these settings to read the values, or we'll get into an infinite recursion
			// therefore, we assume ConfigurationStoreSettings are simply never cached.
			// a better solution would be to allow each settings group to specify its own
			// cacheability, and store this in the db with the settings meta-data
			// but this is not currently implemented
			if (request.DocumentKey.DocumentName == typeof(ConfigurationStoreSettings).FullName)
			{
				return ResponseCachingDirective.DoNotCacheDirective;
			}

			var settings = new ConfigurationStoreSettings();
			return new ResponseCachingDirective(
				settings.ConfigurationCachingEnabled,
				TimeSpan.FromSeconds(settings.ConfigurationCachingTimeToLiveSeconds),
				ResponseCachingSite.Client);
		}

		/// <summary>
		/// This method is called automatically by response caching framework
		/// to provide caching directive for settings meta-data.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected ResponseCachingDirective GetSettingsMetadataCachingDirective(object request)
		{
			var settings = new ConfigurationStoreSettings();
			return new ResponseCachingDirective(
				settings.SettingsMetadataCachingEnabled,
				TimeSpan.FromSeconds(settings.SettingsMetadataCachingTimeToLiveSeconds),
				ResponseCachingSite.Client);
		}

		protected void CheckReadAccess(ConfigurationDocumentKey key)
		{
			var user = key.User;
			if (string.IsNullOrEmpty(user))
			{
				// all users can read application configuration docs
			}
			else
			{
				// user can only read their own configuration docs
				if (user != Thread.CurrentPrincipal.Identity.Name)
					ThrowNotAuthorized();
			}
		}

		protected void CheckWriteAccess(ConfigurationDocumentKey key)
		{
			var user = key.User;
			if (string.IsNullOrEmpty(user))
			{
				// this is an application configuration doc - need admin permission
				if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Admin.System.Configuration))
					ThrowNotAuthorized();
			}
			else
			{
				// user can only save their own configuration docs
				if (user != Thread.CurrentPrincipal.Identity.Name)
					ThrowNotAuthorized();
			}
		}

		protected static void ThrowNotAuthorized()
		{
			throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
		}
	}
}
