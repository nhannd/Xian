using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Represents the root node of an action model.
    /// </summary>
    public class ActionModelRoot : ActionModelNode
    {
        public static ActionModelRoot CreateModel(string namespaze, string modelID, IActionSet actions)
        {
            string qualifiedModelID = string.Format("{0}:{1}", namespaze, modelID);
            ActionModelStore store = new ActionModelStore("actionmodels.xml");
            return store.BuildAndSynchronize(qualifiedModelID, actions);
        }


        private string _modelID;

        /// <summary>
        /// Constructs an action model with the specified ID.
        /// </summary>
        /// <param name="modelID">The ID of the model</param>
        public ActionModelRoot(string modelID)
            :base(null)
        {
            _modelID = modelID;
        }

        /// <summary>
        /// The model ID of this action model.
        /// </summary>
        public string ModelID
        {
            get { return _modelID; }
        }

        /// <summary>
        /// Inserts the specified actions into this model in the specified order.
        /// </summary>
        /// <param name="actions">The actions to insert</param>
        public void InsertActions(IAction[] actions)
        {
            foreach (IAction action in actions)
            {
                InsertAction(action);
            }
        }

        /// <summary>
        /// Insert the specified action into this model.
        /// </summary>
        /// <param name="action">The action to insert</param>
        public void InsertAction(IAction action)
        {
            InsertAction(action, 0);
        }
        
        protected override ActionModelNode CreateNode(PathSegment pathSegment)
        {
            return new ActionModelRoot(_modelID);
        }

    }
}
