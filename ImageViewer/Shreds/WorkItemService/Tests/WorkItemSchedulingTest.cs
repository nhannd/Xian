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
using ClearCanvas.ImageViewer.Shreds.WorkItemService.Import;
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
        public WorkItemPriorityEnum Priority { get; set; }
    }

    [TestFixture]
    public class WorkItemSchedulingTest : AbstractTest
    {

        private IWorkItemProcessor InsertImportFiles(WorkItemPriorityEnum priority, WorkItemStatusEnum status)
        {
            var rq = new WorkItemInsertRequest
            {
                Request = new ImportFilesRequest()
                {
                    Priority = priority,
                    BadFileBehaviour = BadFileBehaviourEnum.Delete,
                    FileImportBehaviour = FileImportBehaviourEnum.Save,
                    FilePaths = new List<string>(),
                }
            };
            var rsp = WorkItemService.Instance.Insert(rq);

            var updateRequest = new WorkItemUpdateRequest
            {
                Status = status,
                Identifier = rsp.Item.Identifier
            };

            WorkItemService.Instance.Update(updateRequest);

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();

                var d = new ImportItemProcessor();
                d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
                return d;
            }
        }

        private IWorkItemProcessor InsertStudyDelete(DicomMessageBase msg, WorkItemPriorityEnum priority, WorkItemStatusEnum status)
        {
            var rq = new WorkItemInsertRequest
                         {
                             Request = new DeleteStudyRequest
                                           {
                                               Patient = new WorkItemPatient(msg.DataSet),
                                               Study = new WorkItemStudy(msg.DataSet),
                                               Priority = priority
                                           }
                         };
            var rsp = WorkItemService.Instance.Insert(rq);

            var updateRequest = new WorkItemUpdateRequest
            {
                Status = status,
                Identifier = rsp.Item.Identifier
            };

            WorkItemService.Instance.Update(updateRequest);

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();

                var d = new DeleteStudyItemProcessor();
                d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
                return d;
            }
        }

        private IWorkItemProcessor InsertStudyProcess(DicomMessageBase msg, WorkItemPriorityEnum priority, WorkItemStatusEnum status)
        {
            var rq = new WorkItemInsertRequest
                         {
                             Request = new DicomReceiveRequest
                                           {
                                               Patient = new WorkItemPatient(msg.DataSet),
                                               Study = new WorkItemStudy(msg.DataSet),
                                               SourceServerName = "TEST",
                                               Priority = priority
                                           }
                         };
            var rsp = WorkItemService.Instance.Insert(rq);

            var updateRequest = new WorkItemUpdateRequest
            {
                Status = status,
                Identifier = rsp.Item.Identifier
            };

            WorkItemService.Instance.Update(updateRequest);

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                var d = new StudyProcessProcessor();
                d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
                return d;
            }
        }

       
        private IWorkItemProcessor InsertSendStudy(DicomMessageBase msg, WorkItemPriorityEnum priority, WorkItemStatusEnum status)
        {
            var rq = new WorkItemInsertRequest
                         {
                             Request = new DicomSendStudyRequest
                                           {
                                               Patient = new WorkItemPatient(msg.DataSet),
                                               Study = new WorkItemStudy(msg.DataSet),
                                               DestinationServerName = "Dest AE",
                                               Priority = priority
                                           }
                         };
            var rsp = WorkItemService.Instance.Insert(rq);

            var updateRequest = new WorkItemUpdateRequest
                                    {
                                        Status = status,
                                        Identifier = rsp.Item.Identifier
                                    };

            WorkItemService.Instance.Update(updateRequest);

            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                var d = new DicomSendItemProcessor();
                d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
                return d;
            }
        }


        private IWorkItemProcessor InsertReindex(WorkItemPriorityEnum priority, WorkItemStatusEnum status)
        {
            var rq = new WorkItemInsertRequest
                         {
                             Request = new ReindexRequest
                                           {                                               
                                               Priority = priority
                                           }
                         };
            var rsp = WorkItemService.Instance.Insert(rq);

            var updateRequest = new WorkItemUpdateRequest
            {
                Status = status,
                Identifier = rsp.Item.Identifier
            };

            WorkItemService.Instance.Update(updateRequest);
            
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                var d = new ReindexItemProcessor();
                d.Initialize(new WorkItemStatusProxy(broker.GetWorkItem(rsp.Item.Identifier)));
                return d;
            }
        }

        private IWorkItemProcessor InsertReapplyRules(WorkItemPriorityEnum priority, WorkItemStatusEnum status)
        {
            var rq = new WorkItemInsertRequest
                         {
                             Request = new ReapplyRulesRequest
                                           {
                                               ApplyDeleteActions = true,
                                               ApplyRouteActions = true,
                                               Priority = priority
                                           }
                         };
            var rsp = WorkItemService.Instance.Insert(rq);

            var updateRequest = new WorkItemUpdateRequest
            {
                Status = status,
                Identifier = rsp.Item.Identifier
            };

            WorkItemService.Instance.Update(updateRequest);
            
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

        private void DeleteAllWorkItems()
        {
            using (var context = new DataAccessContext(DataAccessContext.WorkItemMutex))
            {
                var broker = context.GetWorkItemBroker();
                
                foreach (var test in broker.GetWorkItemsForProcessing(1000))
                {
                    broker.Delete(test);
                }

                foreach (var test in broker.GetWorkItems(null, WorkItemStatusEnum.InProgress, null))
                {
                    broker.Delete(test);
                }


                context.Commit();
            }
        }

        private void DoTest(IList<SchedulingTest> list)
        {
            foreach (var test in list)
            {
                if (test.Processor.Proxy.Item.Status == WorkItemStatusEnum.InProgress)
                    continue;

                string reason;
                if (test.CanStart)
                    Assert.IsTrue(WorkItemQuery.UnitTestCanStart(test.Processor.Proxy.Item, out reason), "Scheduling test failed for: " + test.Message);
                else
                    Assert.IsFalse(WorkItemQuery.UnitTestCanStart(test.Processor.Proxy.Item, out reason), "Scheduling test failed for: " + test.Message);
            }

            DeleteWorkItems(list);
        }

        [Test]
        public void Test01()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Delete",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Process",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send",
                             CanStart = false
                         });

            DoTest(list);
        }

        [Test]
        public void Test02()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Process",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Delete",
                             CanStart = false
                         });

            DoTest(list);
        }

        [Test]
        public void Test03()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send 1",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send 2",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send 3",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Process",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send 4",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Delete",
                             CanStart = false
                         });


            DoTest(list);
        }

        [Test]
        public void Test04()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Process 1",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Process 2",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send",
                             CanStart = false
                         });


            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Delete",
                             CanStart = false
                         });

            DoTest(list);
        }

        [Test]
        public void Test05Reindex()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);


            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Process",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReindex(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Reindex",
                             CanStart = false
                         });

            DoTest(list);
        }

        [Test]
        public void Test06Reapply()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg = new DicomMessage();
            SetupMR(msg.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Process",
                             CanStart = true
                         });

            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Delete",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReapplyRules(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Reapply Rules",
                             CanStart = true
                         });

            DoTest(list);
        }

        [Test]
        public void Test07Reapply()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReapplyRules(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Reapply Rules 1",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReapplyRules(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Reapply Rules 2",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReindex(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Reindex",
                             CanStart = false
                         });

            DoTest(list);
        }

        [Test]
        public void Test08()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
            var msg2 = new DicomMessage();
            SetupMR(msg2.DataSet);

            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send msg1",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Process msg1",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Delete msg1",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send msg2",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Process msg2",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyDelete(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Delete msg2",
                             CanStart = false
                         });

            DoTest(list);
        }

        [Test]
        public void Test09Priorities()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
            var msg2 = new DicomMessage();
            SetupMR(msg2.DataSet);

            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Send msg1",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Process msg1",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
                Message = "Study Delete msg1",
                CanStart = true
            });

            DoTest(list);
        }


        [Test]
        public void Test10Priorities()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
            var msg2 = new DicomMessage();
            SetupMR(msg2.DataSet);

            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Send msg1",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Process msg1",
                CanStart = true // Updates don't wait for reads
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                Message = "Study Send 2 msg1",
                CanStart = true
            });


            DoTest(list);
        }

        [Test]
        public void Test11Priorities()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
            var msg2 = new DicomMessage();
            SetupMR(msg2.DataSet);

            list.Add(new SchedulingTest
            {
                Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Process msg1",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                Message = "Study Send msg1",
                CanStart = true
            });


            DoTest(list);
        }

        [Test]
        public void Test12Priorities()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
            var msg2 = new DicomMessage();
            SetupMR(msg2.DataSet);

            // Stat Read + Stat Update scheduled later, Noraml priority must wait

            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Send msg1",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Process msg1",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                Message = "Study Send 2 msg1",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                Message = "Study Delete msg1 Stat",
                CanStart = false
            });

            DoTest(list);
        }

        [Test]
        public void Test13ReindexStat()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();

            list.Add(new SchedulingTest
            {
                Processor = InsertReapplyRules(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Reapply Rules 1",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertReapplyRules(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Reapply Rules 2",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertReindex(WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                Message = "Reindex",
                CanStart = true
            });

            DoTest(list);
        }


        [Test]
        public void Test14ReindexStat2()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();

            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
          
            list.Add(new SchedulingTest
            {
                Processor = InsertReapplyRules(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Reapply Rules 2",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Send msg1",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertReindex(WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                Message = "Reindex",
                CanStart = true
            });

            DoTest(list);
        }

        [Test]
        public void Test15Priorities()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
            var msg2 = new DicomMessage();
            SetupMR(msg2.DataSet);

            list.Add(new SchedulingTest
            {
                Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Delete msg1",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                Message = "Study Send msg1",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
                Message = "Study Send msg1",
                CanStart = true
            });

            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study send msg2 normal",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyDelete(msg2, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
                Message = "Study Delete msg2",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                Message = "Study Send msg2",
                CanStart = true
            });

            DoTest(list);
        }

        [Test]
        public void Test16InProgress()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
            var msg2 = new DicomMessage();
            SetupMR(msg2.DataSet);


            // Lower Priority later scheduled item in progress, delete should wait
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
                Message = "Study Delete msg1",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
                Message = "Study Send msg1 In Progress",                
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
                Message = "Study Send msg1",
                CanStart = false
            });

            // In Progress reads
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
                Message = "Study send msg2 normal in progress",
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Send 2 msg2 Normal",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                Message = "Study Send 2 msg2 Stat",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyProcess(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Process msg2",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyDelete(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Delete msg2",
                CanStart = false
            });

            DoTest(list);
        }


        [Test]
        public void Test17InProgress()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
            var msg2 = new DicomMessage();
            SetupMR(msg2.DataSet);


            // Lower Priority later scheduled item in progress, delete should wait
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyDelete(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
                Message = "Study Delete msg1",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
                Message = "Study Send msg1 In Progress",
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
                Message = "Study Send msg1",
                CanStart = false
            });

            // In Progress reads
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
                Message = "Study send msg2 normal in progress",
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Send 2 msg2 Normal",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertSendStudy(msg2, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                Message = "Study Send 2 msg2 Stat",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyProcess(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Process msg2",
                CanStart = true
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertStudyDelete(msg2, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                Message = "Study Delete msg2",
                CanStart = false
            });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
            {
                Processor = InsertImportFiles(WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                Message = "Import Files",
                CanStart = true
            });


            DoTest(list);
        }

        [Test]
        public void Test18InProgress()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
            var msg2 = new DicomMessage();
            SetupMR(msg2.DataSet);


            // Non-Exclusive In Progress doesn't block
            list.Add(new SchedulingTest
                         {
                             Processor = InsertImportFiles(WorkItemPriorityEnum.High, WorkItemStatusEnum.InProgress),
                             Message = "Import Files",
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
                             Message = "Study Process msg1",
                             CanStart = true
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor =
                                 InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.Pending),
                             Message = "Study Send msg1",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor =
                                 InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                             Message = "Study Send 2 msg1",
                             CanStart = true
                         });

            DoTest(list);
        }

        [Test]
        public void Test19InProgress()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);

            // Reindex waits for lower priority item
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReindex(WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
                             Message = "Reindex",
                             CanStart = false,
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor =
                                 InsertSendStudy(msg1, WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
                             Message = "Study Send msg1",
                         });

            DoTest(list);
        }

        [Test]
        public void Test20InProgress()
        {
            DeleteAllWorkItems();
            var list = new List<SchedulingTest>();
            var msg1 = new DicomMessage();
            SetupMR(msg1.DataSet);
            var msg2 = new DicomMessage();
            SetupMR(msg2.DataSet);


            // Reindex in progress, all others wait, including those with higher priorities
            list.Add(new SchedulingTest
                         {
                             Processor = InsertReindex(WorkItemPriorityEnum.Normal, WorkItemStatusEnum.InProgress),
                             Message = "Reindex",
                             CanStart = false,
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor =
                                 InsertSendStudy(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                             Message = "Study Send msg1",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertStudyProcess(msg1, WorkItemPriorityEnum.Stat, WorkItemStatusEnum.Pending),
                             Message = "Study Process msg1",
                             CanStart = false
                         });

            Thread.Sleep(2);
            list.Add(new SchedulingTest
                         {
                             Processor = InsertImportFiles(WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
                             Message = "Import Files",
                             CanStart = false
                         });

            list.Add(new SchedulingTest
                         {
                             Processor = InsertReapplyRules(WorkItemPriorityEnum.High, WorkItemStatusEnum.Pending),
                             Message = "Reapply Rules",
                             CanStart = false
                         });
            DoTest(list);
        }
    }
}
#endif