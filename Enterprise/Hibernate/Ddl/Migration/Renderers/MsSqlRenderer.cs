using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Hibernate.Ddl.Migration;
using NHibernate.Dialect;

namespace ClearCanvas.Enterprise.Hibernate.Ddl.Migration.Renderers
{
    class MsSqlRenderer : Renderer
    {
		public MsSqlRenderer(Dialect dialect)
			:base(dialect)
		{
		}
    }
}
