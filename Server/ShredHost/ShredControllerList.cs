using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
    internal class ShredControllerList : MarshallableList<ShredController>
    {
        public ShredControllerList()
        {

        }

        public ReadOnlyCollection<ShredController> AllShredInfo
        {
            get { return this.ContainedObjects; }
        }

        public ShredController this[int index]
        {
            get
            {
                foreach (ShredController shredController in this.ContainedObjects)
                {
                    if (shredController.Id == index)
                    {
                        return shredController;
                    }
                }

                string message = "Could not find ShredController object with Id = " + index.ToString();
                throw new System.IndexOutOfRangeException(message);
            }
        }

        public WcfDataShred[] WcfDataShredCollection
        {
            get
            {
                WcfDataShred[] shreds = new WcfDataShred[this.ContainedObjects.Count];

                int i = 0;
                foreach (ShredController shredController in this.ContainedObjects)
                {
                    shreds[i++] = shredController.WcfDataShred;
                }

                return shreds;
            }
        }
    }
}
