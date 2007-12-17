#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using ClearCanvas.Enterprise.Core;
using System.IO;
using ClearCanvas.Enterprise.Authentication.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;

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
        /// Import authority groups from extensions of <see cref="DefineAuthorityGroupsExtensionPoint"/>.
        /// </summary>
        /// <remarks>
        /// Creates any authority groups that do not already exist.
        /// This method performs an additive import.  It will never remove an existing authority group or
        /// remove authority tokens from an existing group.
        /// </remarks>
        /// <param name="context"></param>
        /// <param name="statusLog"></param>
        public IList<AuthorityGroup> ImportFromPlugins(IUpdateContext context, TextWriter statusLog)
        {
            AuthorityGroupDefinition[] groupDefs = AuthorityGroupSetup.GetDefaultAuthorityGroups();
            return Import(groupDefs, context, statusLog);
        }

        /// <summary>
        /// Imports authority groups from an XML document.
        /// </summary>
        /// <remarks>
        /// Creates any authority groups defined in the document
        /// that do not already exist, and adds any authority tokens specified in the document to the groups.
        /// This method performs an additive import.  It will never remove an existing authority group or
        /// remove authority tokens from an existing group.
        /// </remarks>
        /// <param name="xmlDoc">The document to import</param>
        /// <param name="context">Persistence context</param>
        /// <param name="statusLog">A log to which the import process will write status messages</param>
        public IList<AuthorityGroup> ImportFromXml(XmlDocument xmlDoc, IUpdateContext context, TextWriter statusLog)
        {
            List<AuthorityGroupDefinition> groupDefs = new List<AuthorityGroupDefinition>();

            // process the xml document
            foreach (XmlElement groupNode in xmlDoc.GetElementsByTagName(GroupTag))
            {
                string groupName = groupNode.GetAttribute(NameAttr);
                if (!string.IsNullOrEmpty(groupName))
                {

                    // process all token nodes contained in group
                    string[] tokens = CollectionUtils.Map<XmlElement, string, List<string>>(groupNode.GetElementsByTagName(TokenTag),
                        delegate(XmlElement tokenNode)
                        {
                            return tokenNode.GetAttribute(NameAttr);
                        }).ToArray();

                    groupDefs.Add(new AuthorityGroupDefinition(groupName, tokens));
                }
            }

            return Import(groupDefs, context, statusLog);
        }

        public IList<AuthorityGroup> Import(IEnumerable<AuthorityGroupDefinition> groupDefs, IUpdateContext context, TextWriter statusLog)
        {
            // first load all the existing tokens into memory
            // there should not be that many tokens ( < 500), so this should not be a problem
            IAuthorityTokenBroker tokenBroker = context.GetBroker<IAuthorityTokenBroker>();
            IList<AuthorityToken> existingTokens = tokenBroker.FindAll();

            // load existing groups
            IAuthorityGroupBroker groupBroker = context.GetBroker<IAuthorityGroupBroker>();
            IList<AuthorityGroup> existingGroups = groupBroker.FindAll();

            foreach (AuthorityGroupDefinition groupDef in groupDefs)
            {
                statusLog.WriteLine(string.Format("Processing group {0}...", groupDef.Name));

                AuthorityGroup group = CollectionUtils.SelectFirst<AuthorityGroup>(existingGroups,
                    delegate(AuthorityGroup g) { return g.Name == groupDef.Name; });

                // if group does not exist, create it
                if (group == null)
                {
                    group = new AuthorityGroup();
                    group.Name = groupDef.Name;
                    context.Lock(group, DirtyState.New);
                    existingGroups.Add(group);
                }

                // process all token nodes contained in group
                foreach (string tokenName in groupDef.Tokens)
                {
                    AuthorityToken token = CollectionUtils.SelectFirst<AuthorityToken>(existingTokens,
                        delegate(AuthorityToken t) { return t.Name == tokenName; });

                    // ignore non-existent tokens
                    if (token == null)
                    {
                        statusLog.WriteLine(string.Format("Warning: Group {0} references non-existent token {1}", groupDef.Name, tokenName));
                        continue;
                    }

                    // add the token to the group
                    group.AuthorityTokens.Add(token);
                }
            }

            return existingGroups;
        }

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
            {
                if (args.Length > 0)
                {
                    // assume the first arg is the name of an xml file
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(args[0]);
                    ImportFromXml(xmlDoc, (IUpdateContext)PersistenceScope.Current, Console.Out);
                }
                else
                {
                    ImportFromPlugins((IUpdateContext)PersistenceScope.Current, Console.Out);
                }

                scope.Complete();
            }
        }

        #endregion

    }
}
