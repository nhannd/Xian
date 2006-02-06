namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Uid : System.IComparable
    {
        private string _uid;
        public Uid(string constructionUid)
        {
            _uid = constructionUid;
        }

        public override string ToString()
        {
            return _uid;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is Uid)
            {
                Uid otherUid = (Uid)obj;
                if (this._uid == otherUid._uid)
                    return 0;
                else
                    return -1;
            }

            throw new Exception("CompareTo object is not of type Uid");
        }

        #endregion
    }
}
