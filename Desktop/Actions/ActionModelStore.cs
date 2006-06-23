using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Provides services for storing an action model to a file, and rebuilding that action model
    /// from the file.
    /// </summary>
    internal class ActionModelStore : IDisposable
    {
        private string _filename;
        private XmlDocument _xmlDoc;
        private bool _modified;

        /// <summary>
        /// Constructs an object on the specified filename.
        /// </summary>
        /// <param name="filename">The file that acts a store</param>
        /// <param name="throwIfNotExist">Specify true to throw an exception if the file does not exist</param>
        internal ActionModelStore(string filename, bool throwIfNotExist)
        {
            _filename = filename;
            _xmlDoc = new XmlDocument();

            try
            {
                _xmlDoc.Load(_filename);
            }
            catch (Exception e)
            {
                if (throwIfNotExist)
                    throw e;
            }
        }

        /// <summary>
        /// Loads the model with specified model ID, and inserting the specified set of
        /// actions into the model.
        /// </summary>
        /// <param name="id">The ID of the model to load</param>
        /// <param name="actions">A set of actions to insert into the model</param>
        /// <returns>The specified action model</returns>
        public ActionModelRoot Load(string id, IAction[] actions)
        {
            ActionModelRoot actionModel = new ActionModelRoot(id);
            XmlNodeList xmlActionModels = _xmlDoc.GetElementsByTagName("action-model");
            foreach (XmlElement xmlActionModel in xmlActionModels)
            {
                if (xmlActionModel.GetAttribute("id") == id)
                {
                    Load(actionModel, xmlActionModel, actions);
                    return actionModel;
                }
            }

            // no stored model, so pass null to the helper method
            Load(actionModel, null, actions);
            return actionModel;
        }

        /// <summary>
        /// Saves the specified action model to the store.
        /// </summary>
        /// <param name="model">The model to save</param>
        public void Save(ActionModelRoot model)
        {
            _modified = true;

            // find or create the "action-models" node
            XmlElement xmlActionModelsNode;
            if(_xmlDoc.GetElementsByTagName("action-models").Count == 0)
            {
                xmlActionModelsNode = _xmlDoc.CreateElement("action-models");
                _xmlDoc.AppendChild(xmlActionModelsNode);
            }
            xmlActionModelsNode = (XmlElement)_xmlDoc.GetElementsByTagName("action-models").Item(0);

            // retrieve the set of action-model nodes
            XmlNodeList xmlActionModels = xmlActionModelsNode.ChildNodes;    

            // find this model, if it already exists
            XmlElement xmlThisModel = null;
            foreach (XmlElement xmlModel in xmlActionModels)
            {
                if (xmlModel.GetAttribute("id") == model.ModelID)
                {
                    // found this model - remove its child nodes, so that we overwrite it
                    xmlModel.RemoveAll();
                    xmlThisModel = xmlModel;
                    break;
                }
            }

            // model was not found - create it
            if (xmlThisModel == null)
            {
                xmlThisModel = _xmlDoc.CreateElement("action-model");
                xmlActionModelsNode.AppendChild(xmlThisModel);
            }

            // write this model
            xmlThisModel.SetAttribute("id", model.ModelID);
            Save(model, xmlThisModel);
        }

        /// <summary>
        /// Ensures that any previous calls to the <see cref="Save"/> method are written to the file.
        /// </summary>
        public void Flush()
        {
            if (_modified)
            {
                _xmlDoc.Save(_filename);
                _modified = false;
            }
        }

        public void Dispose()
        {
            Flush();
        }


        private void Save(ActionModelRoot model, XmlElement xmlActionModel)
        {
            IAction[] actions = model.GetActionsInOrder();
            foreach(IAction action in actions)
            {
                XmlElement xmlAction = _xmlDoc.CreateElement("action");
                xmlAction.SetAttribute("id", action.ActionID);
                xmlAction.SetAttribute("path", action.Path.ToString());
                xmlActionModel.AppendChild(xmlAction);
            }
        }

        private void Load(ActionModelRoot model, XmlElement xmlActionModel, IAction[] actions)
        {
            // easier to work with the actions in a map
            Dictionary<string, IAction> actionMap = new Dictionary<string, IAction>();
            foreach (IAction action in actions)
            {
                actionMap[action.ActionID] = action;
            }

            // process xml model, inserting actions in order
            if (xmlActionModel != null)
            {
                foreach (XmlElement xmlAction in xmlActionModel.GetElementsByTagName("action"))
                {
                    string actionID = xmlAction.GetAttribute("id");
                    if (actionMap.ContainsKey(actionID))
                    {
                        IAction action = actionMap[actionID];
                        actionMap.Remove(actionID);

                        // update the action path from the xml
                        string path = xmlAction.GetAttribute("path");
                        action.Path = ActionPath.ParseAndLocalize(path, new ActionResourceResolver(action.Target));

                        // insert the action into the model
                        model.InsertAction(action);
                    }
                }
            }

            // insert any actions that were not listed in the xml
            foreach (IAction action in actionMap.Values)
            {
                model.InsertAction(action);
            }
        }
    }
}
