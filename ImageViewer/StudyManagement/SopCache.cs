using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	internal sealed class SopCache
	{
		private Dictionary<string, ICacheableSop> _sops = new Dictionary<string,ICacheableSop>();

		public SopCache()
		{

		}

		public void Add(ICacheableSop sop)
		{
			if (_sops.ContainsKey(sop.SopInstanceUID))
			{
				_sops[sop.SopInstanceUID].IncrementReferenceCount();
			}
			else
			{
				sop.IncrementReferenceCount();
				_sops.Add(sop.SopInstanceUID, sop);
			}
		}

		public void Remove(string sopInstanceUID)
		{
			if (_sops.ContainsKey(sopInstanceUID))
			{
				ICacheableSop cachedSop = _sops[sopInstanceUID];
				cachedSop.DecrementReferenceCount();

				if (cachedSop.IsReferenceCountZero)
				{
					_sops.Remove(cachedSop.SopInstanceUID);
					cachedSop.Dispose();
				}
			}
		}

		public ICacheableSop this[string sopInstanceUID]
		{
			get
			{
				if (_sops.ContainsKey(sopInstanceUID))
					return _sops[sopInstanceUID];
				else
					return null;
			}
		}
	}
}
