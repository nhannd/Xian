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
        /// <summary>
        /// Creates the action model with the specified namespace and modelID, using the specified
        /// set of actions as input.  If an action model specification for the namespace/modelID
        /// does not exist, it will be created.  If it does it exist, it will be used as guidance
        /// in constructing the action model tree.
        /// </summary>
        /// <param name="namespaze">A namespace to qualify the model ID, typically the class name of the calling class is a good choice</param>
        /// <param name="modelID">The model ID, typically the <see cref="ActionPath.Site"/> value is a good choice</param>
        /// <param name="actions">The set of actions from which to construct the model</param>
        /// <returns>An action model tree</returns>
        public static ActionModelRoot CreateModel(string namespaze, string modelID, IActionSet actions)
        {
            string qualifiedModelID = string.Format("{0}:{1}", namespaze, modelID);
            ActionModelStore store = new ActionModelStore("actionmodels.xml");
            return store.BuildAndSynchronize(qualifiedModelID, actions);
        }


        /// <summary>
        /// Constructor
        /// </summary>
        public ActionModelRoot()
            :base(null)
        {
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
            return new ActionModelRoot();
        }

    }
}
