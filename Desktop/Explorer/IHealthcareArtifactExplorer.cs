using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Explorer
{
	public interface IHealthcareArtifactExplorer
	{
		string Name
		{
			get;
		}

		IApplicationComponent Component 
		{ 
			get; 
		}
	}
}
