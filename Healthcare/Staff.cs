using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// Staff entity
    /// </summary>
	public partial class Staff : Entity
	{
        private void CustomInitialize()
        {
        }

        public override string ToString()
        {
            return this._name.Format();
        }
    }
}