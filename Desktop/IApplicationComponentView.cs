using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public interface IApplicationComponentView : IView
    {
        void SetComponent(IApplicationComponent component);
    }
}
