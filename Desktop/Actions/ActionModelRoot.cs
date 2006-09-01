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
        /// Creates the action model with the specified namespace and site, using the specified
        /// set of actions as input.  If an action model specification for the namespace/site
        /// does not exist, it will be created.  If it does it exist, it will be used as guidance
        /// in constructing the action model tree.
        /// </summary>
        /// <param name="namespaze">A namespace to qualify the site, typically the class name of the calling class is a good choice</param>
        /// <param name="site">The site (<see cref="ActionPath.Site"/>)</param>
        /// <param name="actions">The set of actions from which to construct the model</param>
        /// <returns>An action model tree</returns>
        public static ActionModelRoot CreateModel(string namespaze, string site, IActionSet actions)
        {
            ActionModelStore store = new ActionModelStore("actionmodels.xml");
            return store.BuildAndSynchronize(namespaze, site, actions.Select(delegate(IAction action) { return action.Path.Site == site; }));
        }

        private string _site;


        /// <summary>
        /// Constructor
        /// </summary>
        public ActionModelRoot()
            :this(null)
        {
        }

        /// <summary>
        /// Protected constructor
        /// </summary>
        /// <param name="site">The site to which this model corresponds</param>
        public ActionModelRoot(string site)
            : base(null)
        {
            _site = site;
        }

        public string Site
        {
            get { return _site; }
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
            InsertAction(action, 1);
        }
        
        protected override ActionModelNode CreateNode(PathSegment pathSegment)
        {
            return new ActionModelRoot();
        }

    }
}
