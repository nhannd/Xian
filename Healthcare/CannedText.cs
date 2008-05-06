using System;
using System.Collections;
using System.Text;

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare {


    /// <summary>
    /// CannedText entity
    /// </summary>
	public partial class CannedText : ClearCanvas.Enterprise.Core.Entity
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

    	public CannedText(string name1, string path1, string text1)
    		: this()
		{
			_name = name1;
			_path = path1;
			_text = text1;
		}
	}
}