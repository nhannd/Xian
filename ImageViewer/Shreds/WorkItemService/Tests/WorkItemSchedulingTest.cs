#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Tests;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.DeleteStudy;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.DicomSend;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.ProcessStudy;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.ReapplyRules;
using ClearCanvas.ImageViewer.Shreds.WorkItemService.Reindex;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;
using ClearCanvas.ImageViewer.StudyManagement.Core.WorkItemProcessor;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Shreds.WorkItemService.Tests
{
    internal class SchedulingTest
    {
        public IWorkItemProcessor Processor { get; set; }
        public bool CanStart { get; set; }
        public string Message { get; set; }
    }

    [TestFixture]
    public class WorkItemSchedulingTest : AbstractTest
    {

        private IWorkItemProcessor InsertStudyDelete(DicomMessageBase msg)
        {
            var rq = new WorkItemInsertRequest
                         {
                             Request = new DeleteStudyRequest
                                           {
                                               Patient = new WorkItemPatient(msg.DataSet),
                                               Study = new WorkItemStudy(msg.DataSet),
                                           }
                         };
            var rsp = WorkItemService.Instance.Insert(rq);

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();

                var d = new DeleteStudyItemProcessor();
                d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
                return d;
            }
        }

        private IWorkItemProcessor InsertStudyProcess(DicomMessageBase msg)
        {
            var rq = new WorkItemInsertRequest
                         {
                             Request = new DicomReceiveRequest
                                           {
                                               Patient = new WorkItemPatient(msg.DataSet),
                                               Study = new WorkItemStudy(msg.DataSet),
                                               SourceServerName = "TEST"
                                           }
                         };
            var rsp = WorkItemService.Instance.Insert(rq);

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                var d = new StudyProcessProcessor();
                d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
                return d;
            }
        }

        private IWorkItemProcessor InsertSendStudy(DicomMessageBase msg)
        {
            var rq = new WorkItemInsertRequest
                         {
                             Request = new DicomSendStudyRequest
                                           {
                                               Patient = new WorkItemPatient(msg.DataSet),
                                               Study = new WorkItemStudy(msg.DataSet),
                                               DestinationServerName = "Dest AE"
                                           }
                         };
            var rsp = WorkItemService.Instance.Insert(rq);

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                var d = new DicomSendItemProcessor();
                d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
                return d;
            }
        }


        private IWorkItemProcessor InsertReindex()
        {
            var rq = new WorkItemInsertRequest
                         {
                             Request = new ReindexRequest()
                         };
            var rsp = WorkItemService.Instance.Insert(rq);

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                var d = new ReindexItemProcessor();
                d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
                return d;
            }
        }

        private IWorkItemProcessor InsertReapplyRules()
        {
            var rq = new WorkItemInsertRequest
                         {
                             Request = new ReapplyRulesRequest
                                           {
                                               ApplyDeleteActions = true,
                                               ApplyRouteActions = true
                                           }
                         };
            var rsp = WorkItemService.Instance.Insert(rq);

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                var d = new ReapplyRulesItemProcessor();
                d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
                return d;
            }
        }

        private void DeleteWorkItems(IEnumerable<SchedulingTest> list)
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();

                foreach (var test in list)
                {
                    var item = broker.GetWorkItem(test.Processor.Proxy.Item.Oid);

                    broker.Delete(item);
                }

                context.Commit();
            }
        }

        private void DoTest(IList<SchedulingTest> list)
        {
            foreach (var test in list)
            {
                string reason;
                if (test.CanStart)
                    Assert.IsTrue(WorkItemQuery.UnitTestCanStart(test.Processor.Proxy.Item, out reason), "Scheduling test failed for: " + test.Message);
                else
                    Assert.IsFalse(WorkItemQuery.UnitTestCanStart(test.Processor.Proxy.Item, out reason), "Scheduling test failed for: " + test.Message);
            }

            DeleteWorkItems(list);
        }

        [Test]
        public void Test1()
        {
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg),
                             Message = "Study Delete",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg),
                             Message = "Study Process",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg),
                             Message = "Study Send",
                             CanStart = false
                         });

            DoTest(list);
        }

        [Test]
        public void Test2()
        {
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg),
                             Message = "Study Send",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg),
                             Message = "Study Process",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg),
                             Message = "Study Delete",
                             CanStart = false
                         });

            DoTest(list);
        }

        [Test]
        public void Test3()
        {
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg),
                             Message = "Study Send 1",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg),
                             Message = "Study Send 2",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg),
                             Message = "Study Send 3",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg),
                             Message = "Study Process",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg),
                             Message = "Study Send 4",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg),
                             Message = "Study Delete",
                             CanStart = false
                         });


            DoTest(list);
        }

        [Test]
        public void Test4()
        {
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg),
                             Message = "Study Process",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg),
                             Message = "Study Process",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg),
                             Message = "Study Send",
                             CanStart = false
                         });


            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg),
                             Message = "Study Delete",
                             CanStart = false
                         });

            DoTest(list);
        }

        [Test]
        public void TestReindex5()
        {
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);


            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg),
                             Message = "Study Send",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg),
                             Message = "Study Process",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReindex(),
                             Message = "Reindex",
                             CanStart = true
                         });

            DoTest(list);
        }

        [Test]
        public void TestReapply6()
        {
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg),
                             Message = "Study Process",
                             CanStart = true
                         });

            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg),
                             Message = "Study Send",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg),
                             Message = "Study Delete",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReapplyRules(),
                             Message = "Reapply Rules",
                             CanStart = true
                         });

            DoTest(list);
        }

        [Test]
        public void TestReapply7()
        {
            var list = new List<SchedulingTest>();

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReapplyRules(),
                             Message = "Reapply Rules 1",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReapplyRules(),
                             Message = "Reapply Rules 2",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReindex(),
                             Message = "Reindex",
                             CanStart = true
                         });

            DoTest(list);
        }

        [Test]
        public void Test8()
        {
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
            var msg2 = new DicomMessage();
            SetupMR(msg2.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg1),
                             Message = "Study Send msg1",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg1),
                             Message = "Study Process msg1",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg1),
                             Message = "Study Delete msg1",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg2),
                             Message = "Study Send msg2",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg2),
                             Message = "Study Process msg2",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg2),
                             Message = "Study Delete msg2",
                             CanStart = false
                         });

            DoTest(list);
        }
    }
}
#endif