using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// DiagnosticServiceTreeNode entity
    /// </summary>
	public partial class DiagnosticServiceTreeNode : ClearCanvas.Enterprise.Core.Entity
	{

        public DiagnosticServiceTreeNode(DiagnosticServiceTreeNode parent)
        {
            if (parent != null)
            {
                parent.AddChild(this);
            }

            _children = new ArrayList();

            CustomInitialize();
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public virtual void AddChild(DiagnosticServiceTreeNode child)
        {
            if (child.Parent != null)
            {
                child.Parent.Children.Remove(child);
            }
            child.Parent = this;
            this.Children.Add(child);
        }
		
		#region Object overrides
		
		public override bool Equals(object that)
		{
			// TODO: implement a test for business-key equality
			return base.Equals(that);
		}
		
		public override int GetHashCode()
		{
			// TODO: implement a hash-code based on the business-key used in the Equals() method
			return base.GetHashCode();
		}
		
		#endregion

	}
}