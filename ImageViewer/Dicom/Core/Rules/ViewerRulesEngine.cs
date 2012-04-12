#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Rules;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Dicom.Core.Rules
{
    public class ViewerRulesEngine : RulesEngine<ViewerActionContext,RuleActionType>
    {
        /// <summary>
        /// Load the rules engine from the Persistent Store and compile the conditions and actions.
        /// </summary>
        public void Load()
        {
            Statistics.LoadTime.Start();

            // Clearout the current type list.
            _typeList.Clear();

            // May want to make this plugable!
            // Also, may want to insert generic default rules here that are hard coded to a specific length in time.


            using (var context = new DataAccessContext())
            {
                var broker = context.GetRuleBroker();

                var list = broker.GetRules();
                
                
                // Create the specification and action compilers
                // We'll compile the rules right away
                var specCompiler = new XmlSpecificationCompiler("dicom");
                var actionCompiler = new XmlActionCompiler<ViewerActionContext, RuleActionType>();

                foreach (Rule serverRule in list)
                {
                    // _omitList & _includeList could be checked here to only include rules which are in either or, not sure yet, however if we need this for the IV
                    try
                    {
                        

                        // note, Need here to create an Xml Representation of the rule with the generic condition + just the action nodes for Auto-Routing
                        // Then, create a second XmlDocument with the same condition + just the action nodes for deletion, etc., etc.
                        // Note, for testing rules, may want to implement a "No-Op" action, that just fires an event if the "condition" applies to the rule,
                        // and have a special Load() method for a "test" rules engine.
                        var ruleXml = new XmlDocument();


                        RuleActionType ruleType = RuleActionType.AutoRoute;

                        // Prob not needed for this use
                        XmlNode ruleNode =
                            CollectionUtils.SelectFirst<XmlNode>(ruleXml.ChildNodes,
                                                                 delegate(XmlNode child) { return child.Name.Equals("rule"); });


                        var theRule = new Rule<ViewerActionContext, RuleActionType>
                        {
                            Name = serverRule.RuleId,
                            IsDefault = false,
                            IsExempt = false,
                            Description = serverRule.Name
                        };
                    
                        theRule.Compile(ruleNode, RuleActionType.AutoRoute, specCompiler, actionCompiler);

                        RuleTypeCollection<ViewerActionContext, RuleActionType> typeCollection;

                        if (!_typeList.ContainsKey(ruleType))
                        {
                            typeCollection = new RuleTypeCollection<ViewerActionContext, RuleActionType>(ruleType);
                            _typeList.Add(ruleType, typeCollection);
                        }
                        else
                        {
                            typeCollection = _typeList[ruleType];
                        }

                        typeCollection.AddRule(theRule);
                    }
                    catch (Exception e)
                    {
                        // something wrong with the rule...
                        Platform.Log(LogLevel.Warn, e, "Unable to add rule {0} to the engine. It will be skipped", serverRule.Name);
                    }
                }
            }

            Statistics.LoadTime.End();
        }

    }
}
