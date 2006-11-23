using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Facility entity
    /// </summary>
	public partial class Facility : Entity
	{
        private void CustomInitialize()
        {
        }

        public virtual string Format()
        {
            return this.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}