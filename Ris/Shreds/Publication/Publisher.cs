using System;
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Shreds.Publication
{
    /// <summary>
    /// Defines an extension point for processing publication step
    /// </summary>
    [ExtensionPoint]
    public class PublicationStepProcessorExtensionPoint : ExtensionPoint<IPublicationStepProcessor>
    {
    }

    public class Publisher : IPublisher
    {
        private volatile bool _shouldStop;
        private Thread _sleeper;

        private readonly static int _defaultSleepDuration = 30000;
        private readonly int _sleepDuration;

        private readonly static int _defaultBatchSize = 20;
        private readonly int _batchSize;

        private object[] _publicationStepProcessors;

        public Publisher(int sleepDuration, int batchSize)
        {
            _sleepDuration = sleepDuration;
            _batchSize = batchSize;
            _sleeper = null;
        }


        public Publisher()
            : this(_defaultSleepDuration, _defaultBatchSize)
        {
        }

        #region IPublisher Members

        public void Start()
        {
            _publicationStepProcessors = new PublicationStepProcessorExtensionPoint().CreateExtensions();

            while (_shouldStop == false)
            {
                IList<PublicationStep> publicationSteps = NextBatch();

                // sleep if no outstanding items
                if (publicationSteps.Count == 0 && _shouldStop == false)
                {
                    _sleeper = new Thread(Sleep);
                    _sleeper.Start();
                    _sleeper.Join();
                    _sleeper = null;
                }

                foreach (PublicationStep publicationStep in publicationSteps)
                {
                    PublishStep(publicationStep);
                }
            }
        }

        public void RequestStop()
        {
            _shouldStop = true;
            if (_sleeper != null && _sleeper.ThreadState == ThreadState.WaitSleepJoin)
            {
                _sleeper.Abort();
            }
        }

        #endregion

        /// <summary>
        /// Retrieves next batch of <see cref="PublicationStep"/>
        /// </summary>
        /// <returns></returns>
        private IList<PublicationStep> NextBatch()
        {
            IList<PublicationStep> items;

            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Read))
            {
                // Get scheduled steps, where the "publishing cool-down" has elapsed
                PublicationStepSearchCriteria noFailures = GetCriteria();
                noFailures.LastFailureTime.IsNull();
                noFailures.Scheduling.StartTime.SortAsc(0);

                PublicationStepSearchCriteria failures = GetCriteria();
                failures.LastFailureTime.IsNotNull();
                failures.LastFailureTime.LessThan(Platform.Time.AddMinutes(-5));

                PublicationStepSearchCriteria[] criteria = new PublicationStepSearchCriteria[] { noFailures, failures };
                SearchResultPage page = new SearchResultPage(0, _batchSize);

                items = PersistenceScope.CurrentContext.GetBroker<IPublicationStepBroker>().Find(criteria, page);

                scope.Complete();
            }

            return items;
        }

        private PublicationStepSearchCriteria GetCriteria()
        {
            PublicationStepSearchCriteria criteria = new PublicationStepSearchCriteria();
            criteria.State.EqualTo(ActivityStatus.SC);
            criteria.Scheduling.Performer.Staff.IsNotNull();
            criteria.Scheduling.StartTime.LessThan(Platform.Time);
            return criteria;
        }

        /// <summary>
        /// Dispatches the specified <see cref="PublicationStep"/> to each <see cref="IPublicationStepProcessor"/>
        /// and then completes the it.
        /// </summary>
        /// <param name="publicationStep"></param>
        private void PublishStep(PublicationStep publicationStep)
        {
            using (PersistenceScope scope = new PersistenceScope(PersistenceContextType.Update))
            {
                IUpdateContext context = (IUpdateContext)PersistenceScope.CurrentContext;
                context.ChangeSetRecorder.OperationName = this.GetType().FullName;
                try
                {
                    context.Lock(publicationStep);

                    foreach (IPublicationStepProcessor processor in _publicationStepProcessors)
                    {
                        processor.Process(publicationStep, PersistenceScope.CurrentContext);
                    }

                    publicationStep.Complete(publicationStep.AssignedStaff);
                }
                catch(Exception e)
                {
                    ExceptionLogger.Log("Publisher.PublishStep", e);
                    publicationStep.Fail();
                }

                scope.Complete();
            }
        }

        private void Sleep()
        {
            Thread.Sleep(_sleepDuration);
        }
    }

    //[ExtensionOf(typeof(PublicationStepProcessorExtensionPoint))]
    public class DummyPublisher : IPublicationStepProcessor
    {
        #region IPublicationStepProcessor Members

        public void Process(PublicationStep step, IPersistenceContext context)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
