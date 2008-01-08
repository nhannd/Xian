using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Model.EntityBrokers
{
    public partial class StudySelectCriteria
    {
        /// <summary>
        /// Used for EXISTS or NOT EXISTS subselects against the Series table.
        /// </summary>
        /// <remarks>
        /// A <see cref="SeriesSelectCriteria"/> instance is created with the subselect parameters, 
        /// and assigned to this Sub-Criteria.  Note that the link between the <see cref="Study"/>
        /// and <see cref="Series"/> tables is automatically added into the <see cref="SeriesSelectCriteria"/>
        /// instance by the broker.
        /// </remarks>
        public IRelatedEntityCondition<EntitySelectCriteria> SeriesRelatedEntityCondition
        {
            get
            {
                if (!this.SubCriteria.ContainsKey("SeriesRelatedEntityCondition"))
                {
                    this.SubCriteria["SeriesRelatedEntityCondition"] = new RelatedEntityCondition<EntitySelectCriteria>("SeriesRelatedEntityCondition", "Key", "StudyKey");
                }
                return (IRelatedEntityCondition<EntitySelectCriteria>)this.SubCriteria["SeriesRelatedEntityCondition"];
            }
        }
    }
}
