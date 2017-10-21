using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    public class ResolverServiceTests
    {
        private Mock<IRepository<Resolver>> _mockResolverRepository;
        private Mock<IRepository<OperationalProcessTypeRefData>> _mockOperationalProcessTypeRefDataRepository;
        private Mock<IRepository<OperationalProcessType>> _mockOperationalProcessTypeRepository;
        private Mock<IRepository<ServiceComponent>> _mockServiceComponentRepository;
        private Mock<IRepository<ServiceDesk>> _mockServiceDeskRepository;
        private Mock<IUserIdentity> _mockUserIdentity;
        private Mock<IUnitOfWork> _mockUnitOfWork;

        private List<Resolver> _resolvers;
        private List<OperationalProcessTypeRefData> _operationalProcessTypeRefDatas;
        private List<OperationalProcessType> _operationalProcessTypes;
        private List<ServiceComponent> _serviceComponents;

        private const int ResolverIdExists1 = 63;
        private const int ResolverIdExists2 = 87;
        private const int ResolverIdExists3 = 37;

        private const int CustomerId = 32;

        private IResolverService _target;

        [TestInitialize]
        public void Initialise()
        {
            #region Data

            _operationalProcessTypeRefDatas = new List<OperationalProcessTypeRefData>
            {
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(op =>
                {
                    op.OperationalProcessTypeName = "Operational Process A";
                    op.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(op =>
                {
                    op.OperationalProcessTypeName = "Operational Process B";
                    op.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(op =>
                {
                    op.OperationalProcessTypeName = "Operational Process C";
                    op.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(op =>
                {
                    op.OperationalProcessTypeName = "Operational Process D";
                    op.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(op =>
                {
                    op.OperationalProcessTypeName = "Operational Process E";
                    op.Visible = true;
                }),
            };

            _resolvers = new List<Resolver>
            {
                UnitTestHelper.GenerateRandomData<Resolver>(x => // 0
                {
                    x.Id = ResolverIdExists1;
                    SetResolverGroupCustomer(x, CustomerId);
                    x.ResolverGroupType = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(rg =>
                    {
                        rg.ResolverGroupTypeName = "Resolver Group A";
                    });
                    SetResolverGroupOpProcs(x, new[] {_operationalProcessTypeRefDatas[2], _operationalProcessTypeRefDatas[4]});
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x => // 1
                {
                    SetResolverGroupCustomer(x, x.Id + 1);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x => // 2
                {
                    x.Id = ResolverIdExists2;
                    SetResolverGroupCustomer(x, CustomerId);
                    x.ResolverGroupType = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(rg =>
                    {
                        rg.ResolverGroupTypeName = "Resolver Group B";
                    });
                    SetResolverGroupOpProcs(x, new[] {_operationalProcessTypeRefDatas[1], _operationalProcessTypeRefDatas[3]});
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x => // 3
                {
                    x.Id = ResolverIdExists3;
                    x.ResolverGroupType = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>(rg =>
                    {
                        rg.ResolverGroupTypeName = "Resolver Group C";
                    });
                    SetResolverGroupCustomer(x, CustomerId);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x => // 4
                {
                    x.ResolverGroupType = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>();
                    SetResolverGroupCustomer(x, x.Id + 1);
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x => // 5
                {
                    x.ResolverGroupType = UnitTestHelper.GenerateRandomData<ResolverGroupTypeRefData>();
                    SetResolverGroupCustomer(x, x.Id + 1);
                })
            };

            _operationalProcessTypes = new List<OperationalProcessType>
            {
                UnitTestHelper.GenerateRandomData<OperationalProcessType>(x =>
                {
                    x.Resolver = _resolvers[0];
                    x.OperationalProcessTypeRefData = _operationalProcessTypeRefDatas[2];
                    x.OperationalProcessTypeRefDataId = x.OperationalProcessTypeRefData.Id;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessType>(x =>
                {
                    x.Resolver = _resolvers[0];
                    x.OperationalProcessTypeRefData = _operationalProcessTypeRefDatas[4];
                    x.OperationalProcessTypeRefDataId = x.OperationalProcessTypeRefData.Id;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessType>(x =>
                {
                    x.Resolver = _resolvers[2];
                    x.OperationalProcessTypeRefData = _operationalProcessTypeRefDatas[1];
                    x.OperationalProcessTypeRefDataId = x.OperationalProcessTypeRefData.Id;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessType>(x =>
                {
                    x.Resolver = _resolvers[2];
                    x.OperationalProcessTypeRefData = _operationalProcessTypeRefDatas[3];
                    x.OperationalProcessTypeRefDataId = x.OperationalProcessTypeRefData.Id;
                })
            };

            _serviceComponents = new List<ServiceComponent>
            {
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
                {
                    x.Resolver = _resolvers[0];
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
                {
                    x.Resolver = _resolvers[2];
                }),
                UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
                {
                    x.Resolver = _resolvers[3];
                }),
            };

            #endregion

            _mockResolverRepository = MockRepositoryHelper.Create(_resolvers,
                (entity, id) => entity.Id == (int)id,
                (p1, p2) => p1.Id == p2.Id);

            _mockOperationalProcessTypeRefDataRepository = MockRepositoryHelper.Create(_operationalProcessTypeRefDatas);

            _mockOperationalProcessTypeRepository = MockRepositoryHelper.Create(_operationalProcessTypes);

            _mockServiceComponentRepository = MockRepositoryHelper.Create(_serviceComponents);

            _mockServiceDeskRepository = new Mock<IRepository<ServiceDesk>>();

            _mockUserIdentity = new Mock<IUserIdentity>();
            _mockUserIdentity.Setup(s => s.Name).Returns("test@uk.fujitsu.com");

            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _target = new ResolverService(_mockResolverRepository.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockOperationalProcessTypeRefDataRepository.Object,
                _mockServiceComponentRepository.Object,
                _mockServiceDeskRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverService_Ctor_ResolverRepositoryIsNull_ThrowsArgumentNullException()
        {
            new ResolverService(null,
                _mockOperationalProcessTypeRepository.Object,
                _mockOperationalProcessTypeRefDataRepository.Object,
                _mockServiceComponentRepository.Object,
                _mockServiceDeskRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverService_Ctor_OperationalProcessTypeIsNull_ThrowsArgumentNullException()
        {
            new ResolverService(_mockResolverRepository.Object,
                null,
                _mockOperationalProcessTypeRefDataRepository.Object,
                _mockServiceComponentRepository.Object,
                _mockServiceDeskRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverService_Ctor_OperationalProcessTypeRefDataRepositoryIsNull_ThrowsArgumentNullException()
        {
            new ResolverService(_mockResolverRepository.Object,
                _mockOperationalProcessTypeRepository.Object,
                null,
                _mockServiceComponentRepository.Object,
                _mockServiceDeskRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverService_Ctor_ServiceComponentRespositoryIsNull_ThrowsArgumentNullException()
        {
            new ResolverService(_mockResolverRepository.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockOperationalProcessTypeRefDataRepository.Object,
                null,
                _mockServiceDeskRepository.Object,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverService_Ctor_ServiceDeskRepositoryIsNull_ThrowsArgumentNullException()
        {
            new ResolverService(_mockResolverRepository.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockOperationalProcessTypeRefDataRepository.Object,
                _mockServiceComponentRepository.Object,
                null,
                _mockUserIdentity.Object,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverService_Ctor_UserIdentityIsNull_ThrowsArgumentNullException()
        {
            new ResolverService(_mockResolverRepository.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockOperationalProcessTypeRefDataRepository.Object,
                _mockServiceComponentRepository.Object,
                _mockServiceDeskRepository.Object,
                null,
                _mockUnitOfWork.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ResolverService_Ctor_UnitOfWorkIsNull_ThrowsArgumentNullException()
        {
            new ResolverService(_mockResolverRepository.Object,
                _mockOperationalProcessTypeRepository.Object,
                _mockOperationalProcessTypeRefDataRepository.Object,
                _mockServiceComponentRepository.Object,
                _mockServiceDeskRepository.Object,
                _mockUserIdentity.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void ResolverService_All_ReturnsData_CountCorrect()
        {
            var expected = _resolvers
                .OrderBy(o => o.Id)
                .Select(s => s.Id)
                .ToList();
            var result = _target
                .All()
                .OrderBy(o => o.Id)
                .Select(s => s.Id)
                .ToList();
            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void ResolverService_GetById_ReturnsData_CorrectEntity()
        {
            var expected = _resolvers.Single(x => x.Id == ResolverIdExists1);
            var result = _target.GetById(ResolverIdExists1);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ResolverService_Create_AddedToRepository_CorrectEntity()
        {
            var create = UnitTestHelper.GenerateRandomData<Resolver>();
            _target.Create(create);
            Assert.IsTrue(_resolvers.Any(x => x.Id == create.Id));
        }

        [TestMethod]
        public void ResolverService_Create_AddedToRepository_UnitOfWorkCalled()
        {
            var create = UnitTestHelper.GenerateRandomData<Resolver>();
            _target.Create(create);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ResolverService_Move_UnitOfWorkCalled()
        {
            _mockServiceDeskRepository.Setup(s => s.GetById(It.IsAny<int>())).Returns(new ServiceDesk());
            _mockResolverRepository.Setup(s => s.GetById(It.IsAny<int>())).Returns(new Resolver());
            _target.Move(1, 1);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ResolverService_Update_DataSet_ResolverGroupNameSet()
        {
            var update = UnitTestHelper.GenerateRandomData<Resolver>(x =>
            {
                x.Id = ResolverIdExists1;
            });
            _target.Update(update);
            var result = _resolvers.Single(x => x.Id == ResolverIdExists1);
        }

        [TestMethod]
        public void ResolverService_Update_DataSet_UnitOfWorkCalled()
        {
            var update = UnitTestHelper.GenerateRandomData<Resolver>(x =>
            {
                x.Id = ResolverIdExists1;
            });
            _target.Update(update);
            var result = _resolvers.Single(x => x.Id == ResolverIdExists1);
            _mockUnitOfWork.Verify(v => v.Save(), Times.Once);
        }

        [TestMethod]
        public void ResolverService_GetDotMatrix_ResolverGroups_ListAddedToResultsForEachResolverGroup()
        {
            var result = _target.GetDotMatrix(CustomerId, false);
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod]
        public void ResolverService_GetDotMatrix_ResolverGroupId_DotMatrixListItemAdded()
        {
            var result = _target.GetDotMatrix(CustomerId, false);
            Assert.AreEqual(3, result.Count(x => x.Any(y => y.Name == DotMatrixNames.ResolverId)));
        }

        [TestMethod]
        public void ResolverService_GetDotMatrix_ResolverGroupId_DotMatrixListItemValueSet()
        {
            var expected = new List<int> { ResolverIdExists1, ResolverIdExists2, ResolverIdExists3 };
            var result = _target.GetDotMatrix(CustomerId, false);
            var compare = result
                .SelectMany(x => x.Where(y => y.Name == DotMatrixNames.ResolverId)
                    .Select(s => (int)s.Value))
                .ToList();

            Assert.IsTrue(expected.Contains(compare[0]));
            Assert.IsTrue(expected.Contains(compare[1]));
            Assert.IsTrue(expected.Contains(compare[2]));
        }

        [TestMethod]
        public void ResolverService_GetDotMatrix_ResolverGroupName_DotMatrixListItemAdded()
        {
            var result = _target.GetDotMatrix(CustomerId, false);
            Assert.AreEqual(3, result.Count(x => x.Any(y => y.Name == DotMatrixNames.ResolverName)));
        }

        [TestMethod]
        public void ResolverService_GetDotMatrix_ComponentName_DotMatrixListItemAdded()
        {
            var result = _target.GetDotMatrix(CustomerId, false);
            Assert.AreEqual(3, result.Count(x => x.Any(y => y.Name == DotMatrixNames.ComponentName)));
        }

        [TestMethod]
        public void ResolverService_GetDotMatrix_OpProcColumns_DotMatrixListItemAdded()
        {
            var result = _target.GetDotMatrix(CustomerId, false);
            Assert.AreEqual(8, result[0].Count);
            Assert.AreEqual(8, result[1].Count);
        }

        [TestMethod]
        public void ResolverService_GetDotMatrix_OpProcColumns_NamePrefixed()
        {
            var result = _target.GetDotMatrix(CustomerId, false);
            Assert.AreEqual(5, result[0].Count(x => x.Name.StartsWith(DotMatrixNames.OpIdPrefix)));
            Assert.AreEqual(5, result[1].Count(x => x.Name.StartsWith(DotMatrixNames.OpIdPrefix)));
        }

        [TestMethod]
        public void ResolverService_GetDotMatrix_OpProcColumns_ValueSetToTrueForMatchedOpProcs1()
        {
            var expected = _resolvers
                .Single(x => x.Id == ResolverIdExists1)
                .OperationalProcessTypes
                .Select(y => y.OperationalProcessTypeRefData.OperationalProcessTypeName)
                .OrderBy(o => o)
                .ToList();

            var result = _target.GetDotMatrix(CustomerId, false);

            var compare = result[0]
                .Where(x => x.Value is bool && (bool)x.Value)
                .Select(s => s.DisplayName)
                .OrderBy(o => o)
                .ToList();

            Assert.IsTrue(expected.SequenceEqual(compare));
        }

        [TestMethod]
        public void ResolverService_GetDotMatrix_OpProcColumns_ValueSetToTrueForMatchedOpProcs2()
        {
            var expected = _resolvers
                .Single(x => x.Id == ResolverIdExists2)
                .OperationalProcessTypes
                .Select(y => y.OperationalProcessTypeRefData.OperationalProcessTypeName)
                .OrderBy(o => o)
                .ToList();

            var result = _target.GetDotMatrix(CustomerId, false);

            var compare = result[1]
                .Where(x => x.Value is bool && (bool)x.Value)
                .Select(s => s.DisplayName)
                .OrderBy(o => o)
                .ToList();

            Assert.IsTrue(expected.SequenceEqual(compare));
        }

        [TestMethod]
        public void ResolverService_GetDotMatrix_OpProcColumns_ValueAllSetToFalseForOpProcs3()
        {
            var result = _target.GetDotMatrix(CustomerId, false);
            Assert.IsFalse(result[2].Any(x => x.Value is bool && (bool)x.Value));
        }

        [TestMethod]
        public void ResolverService_GetByCustomer_CustomerIdSupplied_ReturnsOnlyThoseComponentsForCustomer()
        {
            var results = _target
                .GetByCustomer(CustomerId)
                .ToList();
            Assert.IsFalse(results.Any(x => x.ServiceDesk.CustomerId != CustomerId));
        }

        #region Helpers

        private void SetResolverGroupCustomer(Resolver rg, int customerId)
        {
            rg.ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(d =>
            {
                d.CustomerId = customerId;
            });
        }

        private void SetResolverGroupOpProcs(Resolver rg, IEnumerable<OperationalProcessTypeRefData> opProcs)
        {
            rg.OperationalProcessTypes = new List<OperationalProcessType>();
            opProcs.ForEach(f => rg.OperationalProcessTypes.Add(new OperationalProcessType
            {
                OperationalProcessTypeRefData = f,
                OperationalProcessTypeRefDataId = f.Id,
                Resolver = rg
            }));
        }

        #endregion
    }
}