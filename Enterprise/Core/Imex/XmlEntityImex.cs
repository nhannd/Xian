#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Xml;

namespace ClearCanvas.Enterprise.Core.Imex
{
	/// <summary>
	/// Abstract base class for classes that import/export entities in XML format.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TDataContract"></typeparam>
	public abstract class XmlEntityImex<TEntity, TDataContract> : XmlDataImexBase
		where TEntity : Entity
	{
		#region ExportItem class

		private class ExportItem : IExportItem
		{
			private readonly TDataContract _data;

			public ExportItem(TDataContract data)
			{
				_data = data;
			}

			public void Write(XmlWriter writer)
			{
				XmlDataImexBase.Write(writer, _data);
			}
		}

		#endregion

		protected const int ItemsPerReadTransaction = 100;
		protected const int ItemsPerUpdateTransaction = 100;


		#region Absract Methods

		/// <summary>
		/// Called to obtain the list of entities to export.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="firstRow"></param>
		/// <param name="maxRows"></param>
		/// <returns></returns>
		protected abstract IList<TEntity> GetItemsForExport(IReadContext context, int firstRow, int maxRows);

		/// <summary>
		/// Called to export the specified entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		protected abstract TDataContract Export(TEntity entity, IReadContext context);

		/// <summary>
		/// Called to import the specified data.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="context"></param>
		protected abstract void Import(TDataContract data, IUpdateContext context);

		#endregion

		#region Protected overrides

		protected sealed override IEnumerable<IExportItem> ExportCore()
		{
			var more = true;
			for (var row = 0; more; row += ItemsPerReadTransaction)
			{
				using (var scope = new PersistenceScope(PersistenceContextType.Read))
				{
					var context = (IReadContext)PersistenceScope.CurrentContext;
					var items = GetItemsForExport(context, row, ItemsPerReadTransaction);
					foreach (var entity in items)
					{
						var data = Export(entity, context);
						yield return new ExportItem(data);
					}

					// there may be more rows if the last query returned any items
					more = (items.Count > 0);
					scope.Complete();
				}
			}
		}

		protected sealed override void ImportCore(IEnumerable<IImportItem> items)
		{
			var enumerator = items.GetEnumerator();
			for (var more = true; more; )
			{
				using (var scope = new PersistenceScope(PersistenceContextType.Update))
				{
					var context = (IUpdateContext)PersistenceScope.CurrentContext;
					context.ChangeSetRecorder.OperationName = this.GetType().FullName;

					for (var j = 0; j < ItemsPerUpdateTransaction && more; j++)
					{
						more = enumerator.MoveNext();
						if (more)
						{
							var item = enumerator.Current;
							var data = (TDataContract)Read(item.Read(), typeof(TDataContract));
							Import(data, context);
						}
					}
					scope.Complete();
				}
			}
		}

		#endregion

	}
}
