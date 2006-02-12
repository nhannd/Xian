namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public struct Uid// : System.IComparable
    {
        private string _uid;
        public Uid(string constructionUid)
        {
            // validate the input
            if (null == constructionUid)
                throw new System.ArgumentNullException("constructionUid", SR.ExceptionGeneralUidNull);

            if (0 == constructionUid.Length)
                throw new System.ArgumentOutOfRangeException("constructionUid", SR.ExceptionGeneralUidZeroLength);

            _uid = constructionUid;
        }

        public override string ToString()
        {
            return _uid;
        }

        //#region IComparable Members

        //public int CompareTo(object obj)
        //{
        //    Uid otherUid = obj as Uid;
        //    if (null != otherUid)
        //    {
        //        if (this._uid == otherUid._uid)
        //            return 0;
        //        else
        //            return 1;
        //    }

        //    throw new System.ArgumentException(SR.ExceptionGeneralUidCompareToTypeIncorrect, "obj");
        //}

        //#endregion
    }
}
