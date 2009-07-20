using System;
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Utilities
{
	//public class ValueIndex<T> : IEnumerable<T> where T : class
	//{
	//    private readonly SortedDictionary<TWrapper, int> _index = new SortedDictionary<TWrapper, int>();

	//    public ValueIndex() {}

	//    public ValueIndex(IEnumerable<T> source)
	//    {
	//        foreach (T t in source)
	//            _index.Add(new TWrapper(t), 0);
	//    }

	//    public void Index(T value, StudyItem entry)
	//    {
	//        TWrapper t = new TWrapper(value);
	//        if (!_index.ContainsKey(t))
	//            _index.Add(t, 0);
	//    }

	//    public IEnumerator<T> GetEnumerator()
	//    {
	//        if (_nullCount >= 0)
	//            yield return null;
	
	//        foreach (T t in _index.Keys)
	//        {
	//            yield return t;
	//        }
	//    }

	//    IEnumerator IEnumerable.GetEnumerator()
	//    {
	//        return this.GetEnumerator();
	//    }

	//    private struct TWrapper
	//    {
	//        public readonly T T;

	//        public TWrapper (T t)
	//        {
	//            this.T = t;
	//        }


	//    }
	//}
}