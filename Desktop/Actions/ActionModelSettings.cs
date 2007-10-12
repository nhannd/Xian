#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Diagnostics;
using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
    /// Provides services for storing an action model to an XML document, and rebuilding that action model from the document.
    /// </summary>
	[SettingsGroupDescription("Stores the action model settings for each user")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	internal sealed partial class ActionModelSettings
    {
		private ActionModelSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		#region Public Methods

		/// <summary>
        /// Builds an in-memory action model from the specified XML model and the specified set of actions.
        /// The actions will be ordered according to the XML model.  Any actions that are not a part of the
        /// XML model will be added to the memory model and inserted into the XML model based on a 'group hint'.
		/// The XML model is automatically persisted, and new models that have never before been persisted
		/// will be added to the store.
        /// </summary>
        /// <param name="namespaze">A namespace to qualify the site</param>
        /// <param name="site">The site</param>
        /// <param name="actions">The set of actions to include</param>
        /// <returns>an <see cref="ActionModelNode"/> representing the root of the action model</returns>
        public ActionModelRoot BuildAndSynchronize(string namespaze, string site, IActionSet actions)
        {
			string actionModelID = string.Format("{0}:{1}", namespaze, site);

			IDictionary<string, IAction> actionMap = BuildActionMap(actions);

			XmlElement xmlActionModel = Synchronize(actionModelID, actionMap);
			ActionModelRoot modelRoot = Build(site, xmlActionModel, actionMap);

			return modelRoot;
		}

		#endregion

		#region Private Methods
		
		#region Utility Methods

		/// <summary>
		/// Builds a map of action IDs to actions.
		/// </summary>
		/// <param name="actions">the set of actions from which to build a map</param>
		/// <returns>a map of action IDs to actions</returns>
		private IDictionary<string, IAction> BuildActionMap(IActionSet actions)
		{
			Dictionary<string, IAction> actionMap = new Dictionary<string, IAction>();

			foreach (IAction action in actions)
				actionMap[action.ActionID] = action;

			return actionMap;
		}

		/// <summary>
		/// Creates the specified action model, but *does not* immediately append it to the xmlDoc.
		/// Since not all actions are persistent (e.g. some could be generated), we need to figure
		/// out how many actions (if any) belonging to the node will be persisted in the store
		/// before adding the action to the store.
		/// </summary>
		/// <param name="id">the id of the "action-model" to create</param>
		/// <returns>An "action-model" element</returns>
		private XmlElement CreateXmlActionModel(string id)
		{
			XmlElement xmlActionModel = this.GetXmlDocument().CreateElement("action-model");
			xmlActionModel.SetAttribute("id", id);
			return xmlActionModel;
		}

		/// <summary>
		/// Creates an "action" node for insertion into an "action-model" node in the Xml store.
		/// </summary>
		/// <param name="action">the action whose relevant properties are to be used to create the node</param>
		/// <returns>an "action" element</returns>
		private XmlElement CreateXmlAction(IAction action)
		{
			XmlElement xmlAction = this.GetXmlDocument().CreateElement("action");

			xmlAction.SetAttribute("id", action.ActionID);
			xmlAction.SetAttribute("path", action.Path.ToString());
			xmlAction.SetAttribute("group-hint", action.GroupHint.Hint);
			
			return xmlAction;
		}

		///// <summary>
		///// Finds a stored model in the XML doc with the specified model ID.
		///// </summary>
		///// <param name="id">The model ID</param>
		///// <returns>An "action-model" element, or null if not found</returns>
		private XmlElement FindXmlActionModel(string id)
		{
			return (XmlElement)this.GetActionModelsNode().SelectSingleNode(String.Format("/action-models/action-model[@id='{0}']", id));
		}

		/// <summary>
		/// Finds an action with the specified id in the specified "action-model" node.
		/// </summary>
		/// <param name="id">the id of the action to find</param>
		/// <param name="xmlActionModel">the "action-model" node to search in</param>
		/// <returns>the XmlElement of the action if found, otherwise null</returns>
		private XmlElement FindXmlAction(string id, XmlElement xmlActionModel)
		{
			return (XmlElement)xmlActionModel.SelectSingleNode(String.Format("action[@id='{0}']", id));
		}

		#endregion

		/// <summary>
		/// Synchronizes persistent actions with the xml store.
		/// Refer to <see cref="ActionModelStore.BuildAndSynchronize"/> for more details.
		/// </summary>
		/// <param name="actionModelID">the ID of the action model</param>
		/// <param name="actionMap">the actions that are to be synchronized/added to the store</param>
		/// <returns>the "action-model" node with the specified actionModelID</returns>
		private XmlElement Synchronize(string actionModelID, IDictionary<string, IAction> actionMap)
		{
			bool changed = false;

			XmlElement xmlActionModel = FindXmlActionModel(actionModelID);
			bool modelExists = (xmlActionModel != null);
			if (!modelExists)
				xmlActionModel = CreateXmlActionModel(actionModelID);

			if (ValidateXmlActionModel(xmlActionModel, actionMap))
				changed = true;

			//make sure every action has a pre-determined spot in the store, inserting
			//actions appropriately based on their 'group hint'.  The algorithm guarantees 
			//that each action will get put somewhere in the store.  Only persistent actions
			//are added to the xml store; otherwise, the non-persistent actions would 
			//be determining the positions of persistent actions in the store,
			//which is clearly the reverse of what should happen.
			foreach (IAction action in actionMap.Values)
			{
				if (action.Persistent)
				{
					if (AppendActionToXmlModel(xmlActionModel, action))
						changed = true;
				}
			}

			if (changed)
			{
				if (!modelExists)
					this.GetActionModelsNode().AppendChild(xmlActionModel);

				this.Save();
			}
			
			XmlElement xmlActionModelClone = (XmlElement)xmlActionModel.CloneNode(true);
	
			foreach (IAction action in actionMap.Values)
			{
				if (!action.Persistent)
					AppendActionToXmlModel(xmlActionModelClone, action);
			}

			return xmlActionModelClone;
		}

		/// <summary>
		/// Validates the entries in the xmlActionModel against the input set of actions.  If an entry
		/// in the xml model does not have a 'group-hint' attribute, the default one from the corresponding
		/// action is automatically inserted.
		/// </summary>
		/// <param name="xmlActionModel">the "action-model" to validate</param>
		/// <param name="actionMap">the set of actions against which to validate the "action-model"</param>
		/// <returns>a boolean indicating whether anything was modified</returns>
		private bool ValidateXmlActionModel(XmlElement xmlActionModel, IDictionary<string, IAction> actionMap)
		{
			bool changed = false;

			foreach (XmlElement xmlAction in xmlActionModel.GetElementsByTagName("action"))
			{
				XmlAttribute groupHintNode = xmlAction.GetAttributeNode("group-hint");
				string id = xmlAction.GetAttribute("id");

				if (groupHintNode == null)
				{
					//Only automatically add the group-hint to the xml if a corresponding action is currently in memory.
					//otherwise, we don't know what it should be.
					if (actionMap.ContainsKey(id))
					{
						xmlAction.SetAttribute("group-hint", actionMap[id].GroupHint.Hint);
						changed = true;
					}
				}
			}

			return changed;
		}

        /// <summary>
        /// Builds an in-memory action model from the specified XML model and the specified set of actions.
        /// The actions will be ordered according to the XML model.
        /// </summary>
        /// <param name="site">the action model site</param>
        /// <param name="xmlActionModel">an XML "action-model" node</param>
        /// <param name="actions">the set of actions that the model should contain</param>
        /// <returns>an <see cref="ActionModelNode"/> representing the root of the action model</returns>
		private ActionModelRoot Build(string site, XmlElement xmlActionModel, IDictionary<string, IAction> actions)
        {
			ActionModelRoot model = new ActionModelRoot(site);
            
			// process xml model, inserting actions in order
			foreach (XmlElement xmlAction in xmlActionModel.GetElementsByTagName("action"))
            {
                string actionID = xmlAction.GetAttribute("id");
				if (actions.ContainsKey(actionID))
				{
					IAction action = actions[actionID];
					actions.Remove(actionID);

					// update the action path from the xml
					string path = xmlAction.GetAttribute("path");
					string grouphint = xmlAction.GetAttribute("group-hint");

					action.Path = new ActionPath(path, action.ResourceResolver);
					action.GroupHint = new GroupHint(grouphint);

					// insert the action into the model
					model.InsertAction(action);
				}
            }

			if (actions.Count > 0)
				Debug.Assert(false);

			return model;
		}

        /// <summary>
        /// Appends the specified action to the specified XML action model.  The "group-hint"
		/// attribute of the action to be inserted is compared with the "group-hint" of the
		/// actions in the xml model and an appropriate place to insert the action is determined
		/// based on the MatchScore method of the <see cref="GroupHint"/>.
        /// </summary>
        /// <param name="xmlActionModel">the "action-model" node to insert an action into</param>
        /// <param name="action">the action to be inserted</param>
		/// <returns>a boolean indicating whether anything was added/removed/modified</returns>
        private bool AppendActionToXmlModel(XmlElement xmlActionModel, IAction action)
        {
			if (null != FindXmlAction(action.ActionID, xmlActionModel))
				return false;
			
			XmlNode insertionPoint = null;
			int currentGroupScore = 0;

			foreach (XmlElement xmlAction in xmlActionModel.GetElementsByTagName("action"))
			{
				string hint = xmlAction.GetAttribute("group-hint");
				GroupHint groupHint = new GroupHint(hint);

				int groupScore = action.GroupHint.MatchScore(groupHint);
				if (Math.Abs(groupScore) >= Math.Abs(currentGroupScore))
				{
					insertionPoint = xmlAction;
					currentGroupScore = groupScore;
				}
			}
						
			XmlElement newXmlAction = CreateXmlAction(action);
			
			if (insertionPoint != null)
				xmlActionModel.InsertAfter(newXmlAction, insertionPoint);
			else
				xmlActionModel.AppendChild(newXmlAction);

			return true;
		}

		private XmlDocument GetXmlDocument()
		{
			try
			{
				return this.ActionModelsXml;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
				this.Reset();
				return this.ActionModelsXml;
			}
		}

		private XmlElement GetActionModelsNode()
		{
			try
			{
				return (XmlElement)this.GetXmlDocument().GetElementsByTagName("action-models")[0];
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
				this.Reset();
				return (XmlElement)this.GetXmlDocument().GetElementsByTagName("action-models")[0];
			}
		}

		#endregion
	}
}
