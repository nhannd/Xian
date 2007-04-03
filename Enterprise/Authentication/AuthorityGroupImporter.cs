using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Enterprise.Core;
using System.IO;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Authentication
{
    /// <summary>
    /// Imports authority groups from an XML document.
    /// </summary>
    /// <remarks>
    /// This class implements <see cref="IApplicationRoot"/> so that it may be run stand-alone from a console.  However,
    /// it may also be used as a utility class to be invoked by other means.
    /// </remarks>
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class AuthorityGroupImporter : IApplicationRoot
    {
        private const string GroupTag = "group";
        private const string TokenTag = "token";
        private const string NameAttr = "name";

        /// <summary>
        /// Imports the specified XML document.  Creates any authority groups defined in the document
        /// that do not already exist, and adds any authority tokens specified in the document to the groups.
        /// </summary>
        /// <remarks>
        /// This method performs an additive import.  It will never remove an existing authority group or
        /// remove authority tokens from an existing group.
        /// </remarks>
        /// <param name="xmlDoc">The document to import</param>
        /// <param name="context">Persistence context</param>
        /// <param name="statusLog">A log to which the import process will write status messages</param>
        public void ImportFromXml(XmlDocument xmlDoc, IUpdateContext context, TextWriter statusLog)
        {
            statusLog.WriteLine("Loading existing tokens...");

            // first load all the existing tokens into memory
            // there should not be that many tokens ( < 500), so this should not be a problem
            IAuthorityTokenBroker tokenBroker = context.GetBroker<IAuthorityTokenBroker>();
            IList<AuthorityToken> existingTokens = tokenBroker.FindAll();

            // load existing groups
            IAuthorityGroupBroker groupBroker = context.GetBroker<IAuthorityGroupBroker>();
            IList<AuthorityGroup> existingGroups = groupBroker.FindAll();

            // process the xml document
            foreach (XmlElement groupNode in xmlDoc.GetElementsByTagName(GroupTag))
            {
                string groupName = groupNode.GetAttribute(NameAttr);
                if (!string.IsNullOrEmpty(groupName))
                {
                    statusLog.WriteLine(string.Format("Processing group {0}...", groupName));

                    AuthorityGroup group = CollectionUtils.SelectFirst<AuthorityGroup>(existingGroups,
                        delegate(AuthorityGroup g) { return g.Name == groupName; });

                    // if group does not exist, create it
                    if (group == null)
                    {
                        group = new AuthorityGroup();
                        group.Name = groupName;
                        context.Lock(group, DirtyState.New);
                        existingGroups.Add(group);
                    }

                    // process all token nodes contained in group
                    foreach (XmlElement tokenNode in groupNode.GetElementsByTagName(TokenTag))
                    {
                        string tokenName = tokenNode.GetAttribute(NameAttr);
                        AuthorityToken token = CollectionUtils.SelectFirst<AuthorityToken>(existingTokens,
                            delegate(AuthorityToken t) { return t.Name == tokenName; });

                        // ignore non-existent tokens
                        if (token == null)
                        {
                            statusLog.WriteLine(string.Format("Warning: Group {0} references non-existent token {1}", groupName, tokenName));
                            continue;
                        }

                        // add the token to the group
                        group.AuthorityTokens.Add(token);
                    }

                }
            }

        }

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Error: must specify XML file to import from");
                return;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(args[0]);

                using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
                {
                    ImportFromXml(xmlDoc, (IUpdateContext)PersistenceScope.Current, Console.Out);

                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Error: {0}", e.Message));
            }
        }

        #endregion
    }
}
