namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Uid : System.IComparable
    {
        private string m_Uid;
        public Uid(string constructionUid)
        {
            m_Uid = constructionUid;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is Uid)
            {
                Uid otherUid = (Uid)obj;
                if (this.m_Uid == otherUid.m_Uid)
                    return 0;
                else
                    return -1;
            }

            throw new Exception("CompareTo object is not of type Uid");
        }

        #endregion
    }
}
