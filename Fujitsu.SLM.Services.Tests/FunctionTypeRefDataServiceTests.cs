using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Fujitsu.SLM.Services.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class FunctionTypeRefDataServiceTests
    {

        private Mock<IRepository<FunctionTypeRefData>> _mockFunctionRefDataRepository;
        private Mock<IRepository<ServiceFunction>> _mockServiceFunctionRepository;
        private Mock<IRepository<ServiceDomain>> _mockServiceDomainRepository;
        private Mock<IUnitOfWork> _mockUnitOfWork;
        private Mock<IParameterService> _mockParameterService;

        private IFunctionTypeRefDataService _functionTypeRefDataService;
        private List<FunctionTypeRefData> _functions;
        private List<ServiceDomain> _serviceDomains;
        private List<ServiceFunction> _serviceFunctions;

        private const string UserNameOne = "matthew.jordan@uk.fujitsu.com";
        private const string UserNameTwo = "patrick.williams@uk.fujitsu.com";
        private const string UserNameThree = "joanne.jordan@uk.fujitsu.com";

        private const int CustomerId = 298;

        [TestInitialize]
        public void TestInitilize()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();

            _functions = new List<FunctionTypeRefData>
            {
                new FunctionTypeRefData{Id=1,FunctionName= "System Management Infrastructure", Visible = true, SortOrder = 5},
                new FunctionTypeRefData{Id=2,FunctionName= "Desktop Virtualisation", Visible= true, SortOrder = 5},
                new FunctionTypeRefData{Id=3,FunctionName= "Secure Remote Access", Visible = true, SortOrder = 5},
                new FunctionTypeRefData{Id=4,FunctionName= "Computing Management", Visible=true, SortOrder = 5},
                new FunctionTypeRefData{Id=5,FunctionName= "Computing Management 3663", Visible=false, SortOrder = 5},
                new FunctionTypeRefData{Id=6,FunctionName= "Computing Management Robeco", Visible=false, SortOrder = 5},
            };

            _serviceDomains = new List<ServiceDomain>
            {
                new ServiceDomain
                {
                    ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(desk =>
                    {
                        desk.CustomerId = CustomerId;
                    }),
                    ServiceFunctions = new List<ServiceFunction>
                    {
                        UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                        {
                            z.FunctionType = _functions.First(x => x.Id == 1);
                            z.FunctionTypeId = 1;
                            z.ServiceDomain = new ServiceDomain
                            {
                                Id = 1,
                                ServiceDesk = new ServiceDesk
                                {
                                    Id =1,
                                    CustomerId = 1,
                                    Customer = UnitTestHelper.GenerateRandomData<Customer>(c =>
                                    {
                                        c.Id = 1;
                                        c.AssignedArchitect = UserNameTwo;
                                        c.Contributors = new List<Contributor>();
                                    })
                                }
                            };
                        }),
                        UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                        {
                            z.FunctionType = _functions.First(x => x.Id == 2);
                            z.FunctionTypeId = 2;
                            z.ServiceDomain = new ServiceDomain
                            {
                                Id = 2,
                                ServiceDesk = new ServiceDesk
                                {
                                    Id =2,
                                    CustomerId = 2,
                                    Customer = UnitTestHelper.GenerateRandomData<Customer>(c =>
                                    {
                                        c.Id = 2;
                                        c.AssignedArchitect = UserNameTwo;
                                        c.Contributors = new List<Contributor>();
                                    })
                                }
                            };
                        }),
                        UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                        {
                            z.FunctionType = _functions.First(x => x.Id == 5);
                            z.FunctionTypeId = 5;
                            z.ServiceDomain = new ServiceDomain
                            {
                                Id = 3,
                                ServiceDesk = new ServiceDesk
                                {
                                    Id =2,
                                    CustomerId = 2,
                                    Customer = UnitTestHelper.GenerateRandomData<Customer>(c =>
                                    {
                                        c.Id = 2;
                                        c.AssignedArchitect = UserNameOne;
                                        c.Contributors = new List<Contributor>
                                        {
                                            new Contributor
                                            {
                                                Id = 1,
                                                CustomerId = 3,
                                                EmailAddress = UserNameThree
                                            }
                                        };
                                    })
                                }
                            };
                        })
                    }
                },
                new ServiceDomain
                {
                    ServiceDesk = UnitTestHelper.GenerateRandomData<ServiceDesk>(desk =>
                    {
                        desk.Customer = UnitTestHelper.GenerateRandomData<Customer>();
                        desk.CustomerId = desk.Customer.Id;
                    }),
                    ServiceFunctions = new List<ServiceFunction>
                    {
                        UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                        {
                            z.FunctionType = _functions.First(x => x.Id == 3);
                            z.FunctionTypeId = 3;
                        }),
                        UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                        {
                            z.FunctionType = _functions.First(x => x.Id == 2);
                            z.FunctionTypeId = 2;
                        }),
                        UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                        {
                            z.FunctionType = _functions.First(x => x.Id == 6);
                            z.FunctionTypeId = 6;
                        })
                    }
                }
            };

            _mockFunctionRefDataRepository = MockRepositoryHelper.Create(_functions);

            _serviceFunctions = new List<ServiceFunction>
            {
                    UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                    {
                        z.FunctionType = _functions.First(x => x.Id == 3);
                        z.FunctionTypeId = 3;
                    }),
                    UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                    {
                        z.FunctionType = _functions.First(x => x.Id == 2);
                        z.FunctionTypeId = 2;
                    }),
                    UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                    {
                        z.FunctionType = _functions.First(x => x.Id == 6);
                        z.FunctionTypeId = 6;
                    }),
                    UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                    {
                        z.FunctionType = _functions.First(x => x.Id == 2);
                        z.FunctionTypeId = 2;
                    }),
                    UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                    {
                        z.FunctionType = _functions.First(x => x.Id == 6);
                        z.FunctionTypeId = 6;
                    }),
                    UnitTestHelper.GenerateRandomData<ServiceFunction>(z =>
                    {
                        z.FunctionType = _functions.First(x => x.Id == 5);
                        z.FunctionTypeId = 5;
                        z.ServiceDomain = new ServiceDomain
                            {
                                Id = 3,
                                ServiceDesk = new ServiceDesk
                                {
                                    Id =2,
                                    CustomerId = 2,
                                    Customer = UnitTestHelper.GenerateRandomData<Customer>(c =>
                                    {
                                        c.Id = 2;
                                        c.AssignedArchitect = UserNameOne;
                                        c.Contributors = new List<Contributor>
                                        {
                                            new Contributor
                                            {
                                                Id = 1,
                                                CustomerId = 3,
                                                EmailAddress = UserNameThree
                                            }
                                        };
                                    })
                                }
                            };
                    })
            };

            _mockServiceFunctionRepository = MockRepositoryHelper.Create(_serviceFunctions);
            _mockServiceDomainRepository = MockRepositoryHelper.Create(_serviceDomains);
            _mockParameterService = new Mock<IParameterService>();

            _functionTypeRefDataService = new FunctionTypeRefDataService(
                _mockFunctionRefDataRepository.Object,
                _mockServiceFunctionRepository.Object,
                _mockServiceDomainRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FunctionTypeRefDataService_Constructor_NoFunctionRefDataRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new FunctionTypeRefDataService(
                null,
                _mockServiceFunctionRepository.Object,
                _mockServiceDomainRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FunctionTypeRefDataService_Constructor_NoServiceFunctionRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new FunctionTypeRefDataService(
                _mockFunctionRefDataRepository.Object,
                null,
                _mockServiceDomainRepository.Object,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FunctionTypeRefDataService_Constructor_NoServiceDomainRepository()
        {
            #region Arrange

            #endregion

            #region Act

            new FunctionTypeRefDataService(
                _mockFunctionRefDataRepository.Object,
                _mockServiceFunctionRepository.Object,
                null,
                _mockParameterService.Object,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FunctionTypeRefDataService_Constructor_NoParameterService()
        {
            #region Arrange

            #endregion

            #region Act

            new FunctionTypeRefDataService(
                _mockFunctionRefDataRepository.Object,
                _mockServiceFunctionRepository.Object,
                _mockServiceDomainRepository.Object,
                null,
                _mockUnitOfWork.Object);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FunctionTypeRefDataService_Constructor_NoUnitOfWork()
        {
            #region Arrange

            #endregion

            #region Act

            new FunctionTypeRefDataService(
                _mockFunctionRefDataRepository.Object,
                _mockServiceFunctionRepository.Object,
                _mockServiceDomainRepository.Object,
                 _mockParameterService.Object,
                null);

            #endregion

            #region Assert

            #endregion
        }

        #endregion


        [TestMethod]
        public void FunctionTypeRefDataService_Create_CallSaveChanges()
        {
            #region Arrange

            var function = new FunctionTypeRefData()
            {
                Id = 5,
                FunctionName = "MJJ Management",
                SortOrder = 5
            };

            #endregion

            #region Act

            var response = _functionTypeRefDataService.Create(function);

            #endregion

            #region Assert

            _mockFunctionRefDataRepository.Verify(x => x.Insert(It.IsAny<FunctionTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            Assert.IsNotNull(response);
            Assert.AreEqual(5, response);

            #endregion
        }

        [TestMethod]
        public void FunctionTypeRefDataService_Update_CallSaveChanges()
        {
            #region Arrange

            var function = new FunctionTypeRefData()
            {
                Id = 4,
                FunctionName = "MJJ Management",
            };

            #endregion

            #region Act

            _functionTypeRefDataService.Update(function);

            #endregion

            #region Assert

            _mockFunctionRefDataRepository.Verify(x => x.Update(It.IsAny<FunctionTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void FunctionTypeRefDataService_Delete_CallSaveChanges()
        {
            #region Arrange

            var function = new FunctionTypeRefData()
            {
                Id = 4,
            };

            #endregion

            #region Act

            _functionTypeRefDataService.Delete(function);

            #endregion

            #region Assert

            _mockFunctionRefDataRepository.Verify(x => x.Delete(It.IsAny<FunctionTypeRefData>()), Times.Once());
            _mockUnitOfWork.Verify(x => x.Save(), Times.Exactly(1));

            #endregion
        }

        [TestMethod]
        public void FunctionTypeRefDataService_GetAll_CallsRepositoryAll()
        {
            #region Arrange

            #endregion

            #region Act

            _functionTypeRefDataService.All();

            #endregion

            #region Assert

            _mockFunctionRefDataRepository.Verify(x => x.All(), Times.Once);

            #endregion
        }

        [TestMethod]
        public void FunctionTypeRefDataService_GetById_CallsRepositoryGetById()
        {
            #region Arrange

            #endregion

            #region Act

            _functionTypeRefDataService.GetById(1);

            #endregion

            #region Assert

            _mockFunctionRefDataRepository.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void FunctionTypeRefDataService_IsFunctionTypeReferenced_CallsRepositoryFind()
        {
            #region Arrange

            #endregion

            #region Act

            _functionTypeRefDataService.GetNumberOfFunctionTypeReferences(1);

            #endregion

            #region Assert

            _mockServiceFunctionRepository.Verify(x => x.Find(It.IsAny<Expression<Func<ServiceFunction, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void FunctionTypeRefDataService_GetAllAndNotVisibleForCustomer_ResultsMerged_AllVisibleIncluded()
        {
            var resultIds = _functionTypeRefDataService
                .GetAllAndNotVisibleForCustomer(CustomerId)
                .Select(s => s.Id)
                .ToList();

            var expectedIds = _functions
                .Where(x => x.Visible)
                .Select(s => s.Id)
                .ToList();

            Assert.IsTrue(!expectedIds.Except(resultIds).Any());
        }

        [TestMethod]
        public void FunctionTypeRefDataService_GetAllAndNotVisibleForCustomer_ResultsMerged_NonVisibleIncludedForCustomer()
        {
            var resultIds = _functionTypeRefDataService
                .GetAllAndNotVisibleForCustomer(CustomerId)
                .Select(s => s.Id)
                .ToList();

            Assert.AreEqual(5, resultIds.Count);
        }

        [TestMethod]
        public void FunctionTypeRefDataService_GetFunctionTypeRefDataWithUsageStats_ReturnsCorrectType()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _functionTypeRefDataService.GetFunctionTypeRefData(true, null);

            #endregion

            #region Assert

            _mockFunctionRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsInstanceOfType(result, typeof(List<FunctionTypeRefDataListItem>));

            #endregion
        }

        [TestMethod]
        public void FunctionTypeRefDataService_GetFunctionTypeRefDataWithUsageStats_ReturnsCorrectUsageStats()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _functionTypeRefDataService.GetFunctionTypeRefData(true, null).ToList();

            #endregion

            #region Assert

            _mockFunctionRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.AreEqual(0, result.First(x => x.Id == 1).UsageCount);
            Assert.AreEqual(2, result.First(x => x.Id == 2).UsageCount);
            Assert.AreEqual(1, result.First(x => x.Id == 3).UsageCount);
            Assert.AreEqual(2, result.First(x => x.Id == 6).UsageCount);

            #endregion
        }
        [TestMethod]
        public void FunctionTypeRefDataService_GetFunctionTypeRefDataAsAdmin_AdministratorCanEditAndDeleteAllDomainTypes()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _functionTypeRefDataService.GetFunctionTypeRefData(true, null).ToList();

            #endregion

            #region Assert

            _mockFunctionRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsFalse(result.Any(x => x.CanEdit == false || x.CanDelete == false));

            #endregion
        }

        [TestMethod]
        public void FunctionTypeRefDataService_GetFunctionTypeRefDataAsAdmin_NotAdminAssignedArchitectCanEditFunctionType()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _functionTypeRefDataService.GetFunctionTypeRefData(false, UserNameOne).ToList();

            #endregion

            #region Assert

            _mockFunctionRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsTrue(result.Any(x => x.Id == 5 && x.CanEdit && x.CanDelete == false));

            #endregion
        }

        [TestMethod]
        public void FunctionTypeRefDataService_GetFunctionTypeRefDataAsAdmin_NotAdminUnknownAssignedArchitectCannotEditOrDeleteAny()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _functionTypeRefDataService.GetFunctionTypeRefData(false, "unknownArchitect@uk.fujitsu.com").ToList();

            #endregion

            #region Assert

            _mockFunctionRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsFalse(result.Any(x => x.CanEdit || x.CanDelete));

            #endregion
        }

        [TestMethod]
        public void FunctionTypeRefDataService_GetFunctionTypeRefDataAsAdmin_NotAdminContributorCanEditFunctionType()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _functionTypeRefDataService.GetFunctionTypeRefData(false, UserNameThree).ToList();

            #endregion

            #region Assert

            _mockFunctionRefDataRepository.Verify(x => x.All(), Times.Once);
            Assert.IsTrue(result.Any(x => x.Id == 5 && x.CanEdit && x.CanDelete == false));

            #endregion
        }

    }
}
