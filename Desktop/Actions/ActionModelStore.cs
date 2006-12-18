using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Provides services for storing an action model to an XML file, and rebuilding that action model
    /// from the file.
    /// </summary>
    internal class ActionModelStore
    {
        private string _filename;
        private XmlDocument _xmlDoc;
        private XmlElement _xmlActionModelsNode;

        /// <summary>
        /// Constructs an object on the specified filename.
        /// </summary>
        /// <param name="filename">The file that acts a store</param>
        internal ActionModelStore(string filename)
        {
            _filename = filename;
            _xmlDoc = new XmlDocument();

            try
            {
                _xmlDoc.Load(_filename);
            }
            catch (Exception)
            {
                // doesn't exist
            }

            // find or create the "action-models" node
            if (_xmlDoc.GetElementsByTagName("action-models").Count == 0)
            {
                _xmlActionModelsNode = _xmlDoc.CreateElement("action-models");
                _xmlDoc.AppendChild(_xmlActionModelsNode);
            }
            else
            {
                _xmlActionModelsNode = (XmlElement)_xmlDoc.GetElementsByTagName("action-models").Item(0);
            }
        }

        /// <summary>
        /// Builds an in-memory action model from the specified XML model and the specified set of actions.
        /// The actions will be ordered according to the XML model.  Any actions that are not a part of the
        /// XML model will be appended to memory model and appended to the XML model, and the XML model
        /// automatically persisted.  Hence a new model that has never before been persisted will be
        /// added to the store.
        /// </summary>
        /// <param name="namespaze">A namespace to qualify the site</param>
        /// <param name="site">The site</param>
        /// <param name="actions">The set of actions to include</param>
        /// <returns>an <see cref="ActionModelNode"/> representing the root of the action model</returns>
        public ActionModelRoot BuildAndSynchronize(string namespaze, string site, IActionSet actions)
        {
            string id = string.Format("{0}:{1}", namespaze, site);

			XmlElement xmlActionModel = FindXmlActionModel(id);
			bool modelExists = (xmlActionModel != null);
			if (!modelExists)
				xmlActionModel = CreateXmlActionModel(id);

			int numberModelActions = xmlActionModel.ChildNodes.Count;
			ActionModelRoot modelRoot = BuildAndSynchronize(site, xmlActionModel, actions);

			//Some actions are generated, and so are not persistent.  We don't need to save those to the store.
			if (!modelExists && xmlActionModel.ChildNodes.Count > 0)
				_xmlActionModelsNode.AppendChild(xmlActionModel);

			// Save the file if anything was added.
			if (numberModelActions != xmlActionModel.ChildNodes.Count)
				_xmlDoc.Save(_filename);

			return modelRoot;
		}

        /// <summary>
        /// Builds an in-memory action model from the specified XML model and the specified set of actions.
        /// The actions will be ordered according to the XML model.  Any actions that are not a part of the
        /// XML model will be appended to memory model and appended to the XML model.
        /// </summary>
        /// <param name="site">the action model site</param>
        /// <param name="xmlActionModel">an XML "action-model" node</param>
        /// <param name="actions">the set of that the model should contain</param>
        /// <returns>an <see cref="ActionModelNode"/> representing the root of the action model</returns>
        private ActionModelRoot BuildAndSynchronize(string site, XmlElement xmlActionModel, IActionSet actions)
        {
            ActionModelRoot model = new ActionModelRoot(site);
            
            // easier to work with the actions in a map
            Dictionary<string, IAction> actionMap = new Dictionary<string, IAction>();
            foreach (IAction action in actions)
            {
                actionMap[action.ActionID] = action;
            }

            // process xml model, inserting actions in order
            foreach (XmlElement xmlAction in xmlActionModel.GetElementsByTagName("action"))
            {
                string actionID = xmlAction.GetAttribute("id");
                if (actionMap.ContainsKey(actionID))
                {
                    IAction action = actionMap[actionID];
                    actionMap.Remove(actionID);

                    // update the action path from the xml
                    string path = xmlAction.GetAttribute("path");
                    action.Path = new ActionPath(path, action.ResourceResolver);

                    // insert the action into the model
                    model.InsertAction(action);
                }
            }

			// insert any *persistent* actions that were not listed in the xml, and add them to the xml model
			foreach (IAction action in actionMap.Values)
			{
				if (!action.Persistent)
					continue;

				model.InsertAction(action);
				AppendActionToXmlModel(xmlActionModel, action);
			}

			// by default, non-persistent actions with no placeholders in the xml model are pushed to the end
			foreach (IAction action in actionMap.Values)
			{
				if (action.Persistent)
					continue;

				model.InsertAction(action);
			}

			return model;
		}

		///// <summary>
		///// Finds a stored model in the XML doc with the specified model ID.
		///// </summary>
		///// <param name="id">The model ID</param>
		///// <returns>An "action-model" element, or null if not found</returns>
		private XmlElement FindXmlActionModel(string id)
		{
			return (XmlElement)_xmlActionModelsNode.SelectSingleNode(String.Format("/action-models/action-model[@id='{0}']", id));
		}

        /// <summary>
        /// Creates the specified action model, but *does not* immediately append it to the xmlDoc.
		/// Since not all actions are persistent (e.g. some could be generated), we need to figure
		/// out how many actions (if any) belonging to the node will be persisted in the store.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An "action-model" element</returns>
        private XmlElement CreateXmlActionModel(string id)
        {
            XmlElement xmlActionModel = _xmlDoc.CreateElement("action-model");
            xmlActionModel.SetAttribute("id", id);
            return xmlActionModel;
        }

        /// <summary>
        /// Appends the specified action to the specified XML action model.
        /// </summary>
        /// <param name="xmlActionModel"></param>
        /// <param name="action"></param>
        private void AppendActionToXmlModel(XmlElement xmlActionModel, IAction action)
        {
			XmlElement xmlAction = _xmlDoc.CreateElement("action");
            xmlAction.SetAttribute("id", action.ActionID);
            xmlAction.SetAttribute("path", action.Path.ToString());
            xmlActionModel.AppendChild(xmlAction);
        }
    }
}
