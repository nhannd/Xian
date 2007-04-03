using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Authentication.Brokers;
using System.IO;
using ClearCanvas.Common;
using System.Reflection;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Authentication
{
    /// <summary>
    /// Scans the entire set of installed plugins for declared authority tokens (const string fields marked
    /// with the <see cref="AuthorityTokenAttribute"/>), and imports them into the database.
    /// </summary>
    /// <remarks>
    /// This class implements <see cref="IApplicationRoot"/> so that it may be run stand-alone from a console.  However,
    /// it may also be used as a utility class to be invoked by other means.
    /// </remarks>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class AuthorityTokenImporter : IApplicationRoot
    {
        /// <summary>
        /// Imports authority tokens from all installed plugins.
        /// </summary>
        /// <param name="context">Persistence context</param>
        /// <param name="statusLog">A log to which the importer will write status messages</param>
        public void ImportFromPlugins(IUpdateContext context, TextWriter statusLog)
        {
            statusLog.WriteLine("Loading existing tokens...");

            // first load all the existing tokens into memory
            // there should not be that many tokens ( < 500), so this should not be a problem
            IAuthorityTokenBroker broker = context.GetBroker<IAuthorityTokenBroker>();
            IList<AuthorityToken> existingTokens = broker.FindAll();

            // scan all plugins for token definitions
            foreach (PluginInfo plugin in Platform.PluginManager.Plugins)
            {
                statusLog.WriteLine(string.Format("Processing plugin {0}...", plugin.FormalName));
                foreach (Type type in plugin.Assembly.GetTypes())
                {
                    // look at public fields
                    foreach (FieldInfo field in type.GetFields())
                    {
                        object[] attrs = field.GetCustomAttributes(typeof(AuthorityTokenAttribute), false);
                        if (attrs.Length == 1)
                        {
                            ProcessToken(field, (AuthorityTokenAttribute)attrs[0], existingTokens, context);
                        }
                    }
                }
            }

        }

        private static void ProcessToken(FieldInfo field, AuthorityTokenAttribute a, IList<AuthorityToken> existingTokens, IUpdateContext context)
        {
            string tokenName = (string)field.GetValue(null);

            AuthorityToken token = CollectionUtils.SelectFirst<AuthorityToken>(existingTokens,
                delegate(AuthorityToken t) { return t.Name == tokenName; });

            // if token does not already exist, create it
            if (token == null)
            {
                token = new AuthorityToken();
                token.Name = tokenName;

                context.Lock(token, DirtyState.New);
            }

            // update the description
            token.Description = a.Description;
        }

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
            {
                ImportFromPlugins((IUpdateContext)PersistenceScope.Current, Console.Out);

                scope.Complete();
            }
        }

        #endregion
    }
}
