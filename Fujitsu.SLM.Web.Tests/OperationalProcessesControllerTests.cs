using CloneExtensions;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Controllers;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using KellermanSoftware.CompareNetObjects;
using Kendo.Mvc.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    public class OperationalProcessesControllerTests
    {
        private Mock<IOperationalProcessTypeRefDataService> _mockOperationalProcessTypeRefDataService;
        private Mock<IResolverService> _mockResolverService;
        private Mock<IContextManager> _mockContextManager;
        private Mock<IUserManager> _mockUserManager;
        private Mock<IResponseManager> _mockResponseManager;
        private Mock<IAppUserContext> _mockAppUserContext;

        private List<OperationalProcessTypeRefData> _operationalProcessTypeRefDatas;
        private List<Resolver> _resolvers;
        private AppContext _appContext;
        private const int CustomerId = 15;
        private const int ResolverId1 = 324;
        private const int ResolverId2 = 344;
        private const string UserName = "matthew.jordan@uk.fujitsu.com";
        private IEnumerable<Resolver> _resolversUpdated;

        private OperationalProcessesController _target;

        [TestInitialize]
        public void Initialise()
        {
            #region Data

            _operationalProcessTypeRefDatas = new List<OperationalProcessTypeRefData>
            {
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                    x.Visible = true;
                }),
                UnitTestHelper.GenerateRandomData<OperationalProcessTypeRefData>(x =>
                {
                    x.Visible = true;
                }),
            };

            _resolvers = new List<Resolver>
            {
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                     x.ServiceComponent = UnitTestHelper.GenerateRandomData<ServiceComponent>();
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.Id = ResolverId1;
                    x.ServiceComponent = UnitTestHelper.GenerateRandomData<ServiceComponent>();
                    x.ServiceComponent.Id = ResolverId1;
                    x.OperationalProcessTypes = new List<OperationalProcessType>
                    {
                        new OperationalProcessType
                        {
                            OperationalProcessTypeRefData = _operationalProcessTypeRefDatas[2]
                        },
                        new OperationalProcessType
                        {
                            OperationalProcessTypeRefData = _operationalProcessTypeRefDatas[4]
                        }
                    };
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                     x.ServiceComponent = UnitTestHelper.GenerateRandomData<ServiceComponent>();
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                    x.Id = ResolverId2;
                    x.ServiceComponent = UnitTestHelper.GenerateRandomData<ServiceComponent>();
                    x.ServiceComponent.Id = ResolverId2;
                }),
                UnitTestHelper.GenerateRandomData<Resolver>(x =>
                {
                     x.ServiceComponent = UnitTestHelper.GenerateRandomData<ServiceComponent>();
                }),
            };

            _appContext = new AppContext
            {
                CurrentCustomer = new CurrentCustomerViewModel
                {
                    Id = CustomerId
                }
            };

            #endregion

            _mockOperationalProcessTypeRefDataService = new Mock<IOperationalProcessTypeRefDataService>();
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.All())
                .Returns(_operationalProcessTypeRefDatas.AsQueryable());
            _mockOperationalProcessTypeRefDataService
                .Setup(s => s.GetAllAndNotVisibleForCustomer(It.IsAny<int>()))
                .Returns(_operationalProcessTypeRefDatas.AsQueryable());
            _mockOperationalProcessTypeRefDataService
               .Setup(s => s.PurgeOrphans());

            _mockResolverService = new Mock<IResolverService>();
            _mockResolverService.Setup(s => s.GetDotMatrix(It.IsAny<int>(), It.IsAny<bool>()))
                .Returns(new List<List<DotMatrixListItem>>());
            _mockResolverService.Setup(s => s.GetByCustomer(It.IsAny<int>()))
                .Returns(_resolvers.AsQueryable());
            _mockResolverService.Setup(s => s.Update(It.IsAny<IEnumerable<Resolver>>()))
                .Callback<IEnumerable<Resolver>>(x => _resolversUpdated = x);

            _mockResponseManager = new Mock<IResponseManager>();

            _mockUserManager = new Mock<IUserManager>();
            _mockUserManager.Setup(s => s.Name).Returns(UserName);

            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.ResponseManager).Returns(_mockResponseManager.Object);
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            Bootstrapper.SetupAutoMapper();

            _target = new OperationalProcessesController(_mockOperationalProcessTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        #region Ctor

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationalProcessesController_Ctor_NoOperationalProcessTypeRefDataService_ThrowsException()
        {
            new OperationalProcessesController(null,
                _mockResolverService.Object,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationalProcessesController_Ctor_NoResolverGroupService_ThrowsException()
        {
            new OperationalProcessesController(_mockOperationalProcessTypeRefDataService.Object,
                null,
                _mockContextManager.Object,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationalProcessesController_Ctor_NoContextManager_ThrowsException()
        {
            new OperationalProcessesController(_mockOperationalProcessTypeRefDataService.Object,
                _mockResolverService.Object,
                null,
                _mockAppUserContext.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void OperationalProcessesController_Ctor_NoAppUserContext_ThrowsException()
        {
            new OperationalProcessesController(_mockOperationalProcessTypeRefDataService.Object,
                _mockResolverService.Object,
                _mockContextManager.Object,
                null);
        }

        #endregion

        [TestMethod]
        public void OperationalProcessesController_Index_LevelPassed_LevelIsAppendedOntoModelName()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            Assert.AreEqual("DotMatrix" + NavigationLevelNames.LevelTwo, result.ViewName);
        }

        [TestMethod]
        public void OperationalProcessesController_Index_LevelPassed_LevelSet()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.AreEqual(NavigationLevelNames.LevelTwo, model.EditLevel);
        }

        [TestMethod]
        public void OperationalProcessesController_Index_ResolverGroupIdColumn_AddedToModel()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsTrue(model.Columns.Any(x => x.Name == DotMatrixNames.ResolverId));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_ResolverGroupIdColumn_VisibleSetToFalse()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsTrue(model.Columns.Any(x => x.Name == DotMatrixNames.ResolverId && x.Visible == false));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_ResolverGroupIdColumn_TypeSetToInt()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsTrue(model.Columns.Any(x => x.Name == DotMatrixNames.ResolverId && x.Type == typeof(int)));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_ResolverGroupNameColumn_AddedToModel()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsTrue(model.Columns.Any(x => x.Name == DotMatrixNames.ResolverName));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_ResolverGroupNameColumn_VisibleSetToTrue()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsTrue(model.Columns.Any(x => x.Name == DotMatrixNames.ResolverName && x.Visible));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_ResolverGroupNameColumn_EditableSetToFalse()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsTrue(model.Columns.Any(x => x.Name == DotMatrixNames.ResolverName && !x.Editable));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_ResolverGroupNameColumn_TypeSetToString()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsTrue(model.Columns.Any(x => x.Name == DotMatrixNames.ResolverName && x.Type == typeof(string)));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_ServiceComponentColumn_AddedToModel()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsTrue(model.Columns.Any(x => x.Name == DotMatrixNames.ComponentName));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_ServiceComponentColumn_VisibleSetToTrue()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsTrue(model.Columns.Any(x => x.Name == DotMatrixNames.ComponentName && x.Visible));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_ServiceComponentColumn_EditableSetToFalse()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsTrue(model.Columns.Any(x => x.Name == DotMatrixNames.ComponentName && !x.Editable));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_ServiceComponentColumn_TypeSetToString()
        {
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsTrue(model.Columns.Any(x => x.Name == DotMatrixNames.ComponentName && x.Type == typeof(string)));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_OpProcColumns_AllAddedToModel()
        {
            var expected = _operationalProcessTypeRefDatas
                .Select(s => string.Concat(DotMatrixNames.OpIdPrefix, s.Id))
                .ToList();

            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.AreEqual(5, model.Columns.Count(x => expected.Contains(x.Name)));
        }

        [TestMethod]
        public void OperationalProcessesController_Index_OpProcColumns_TypeIsBool()
        {
            var expected = _operationalProcessTypeRefDatas
                .Select(s => string.Concat(DotMatrixNames.OpIdPrefix, s.Id))
                .ToList();
            var result = _target.Index(NavigationLevelNames.LevelTwo) as ViewResult;
            var model = result.Model as ViewProcessDotMatrixViewModel;
            Assert.IsFalse(model.Columns.Any(x => expected.Contains(x.Name) && x.Type != typeof(bool)));
        }

        [TestMethod]
        public void OperationalProcessesController_ReadAjaxDotMatrixGrid_Exception_AppendsErrorMessageToHeader()
        {
            SetGetDotMatrixException();
            _target.ReadAjaxDotMatrixGrid(new DataSourceRequest());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void OperationalProcessesController_ReadAjaxDotMatrixGrid_Exception_SetsStatusCodeTo500()
        {
            SetGetDotMatrixException();
            _target.ReadAjaxDotMatrixGrid(new DataSourceRequest());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        [TestMethod]
        public void OperationalProcessesController_ReadAjaxDotMatrixGrid_ContextNull_NoDataReturned()
        {
            SetCustomerContextNull();
            _target.ReadAjaxDotMatrixGrid(new DataSourceRequest());
            _mockResolverService.Verify(v => v.GetDotMatrix(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void OperationalProcessesController_ReadAjaxDotMatrixGrid_ContextZero_NoDataReturned()
        {
            SetCustomerContextZero();
            _target.ReadAjaxDotMatrixGrid(new DataSourceRequest());
            _mockResolverService.Verify(v => v.GetDotMatrix(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void OperationalProcessesController_ReadAjaxDotMatrixGrid_CustomerSelected_DotMatrixServiceCalled()
        {
            _target.ReadAjaxDotMatrixGrid(new DataSourceRequest());
            _mockResolverService.Verify(v => v.GetDotMatrix(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ContextNull_NoDataReturned()
        {
            SetCustomerContextNull();
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());
            _mockOperationalProcessTypeRefDataService.Verify(v => v.All(), Times.Never);
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ContextZero_NoDataReturned()
        {
            SetCustomerContextZero();
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());
            _mockOperationalProcessTypeRefDataService.Verify(v => v.All(), Times.Never);
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ModelValid_OpProcRefDataRetrieved()
        {
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());
            _mockOperationalProcessTypeRefDataService.Verify(v => v.All(), Times.Once);
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ResolverNotFound_ErrorAddedToModelState()
        {
            var model = GetUpdateAjaxDotMatrixGridData();
            model[0][DotMatrixNames.ResolverId] = "29282";
            var result = _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), model) as JsonResult;
            var dataSourceResult = result.Data as DataSourceResult;
            var errors = dataSourceResult.Errors as Dictionary<string, Dictionary<string, object>>;
            Assert.IsTrue(errors.ContainsKey(ModelStateErrorNames.ResolverGroupCannotBeFound));
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ModelValid_TwoEntitiesAreUpdated()
        {
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());
            Assert.AreEqual(2, _resolversUpdated.Count());
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ModelValid_Entity1HasExpectedOpProcsAdded()
        {
            var expected = new[]
            {
                _operationalProcessTypeRefDatas[0].Id,
                _operationalProcessTypeRefDatas[2].Id,
                _operationalProcessTypeRefDatas[3].Id
            };
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());

            var result = _resolversUpdated
                .Where(x => x.ServiceComponent.Id == ResolverId1)
                .SelectMany(x => x.OperationalProcessTypes.Select(y => y.OperationalProcessTypeRefData.Id))
                .ToArray();

            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ModelValid_Entity2HasExpectedOpProcsAdded()
        {
            var expected = new[]
            {
                _operationalProcessTypeRefDatas[0].Id
            };
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());
            var result = _resolversUpdated
                .Where(x => x.ServiceComponent.Id == ResolverId2)
                .SelectMany(x => x.OperationalProcessTypes.Select(y => y.OperationalProcessTypeRefData.Id))
                .ToArray();
            Assert.IsTrue(expected.SequenceEqual(result));
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ModelValid_OnlyOpProcsUpdated()
        {
            var inits = new Dictionary<Type, Func<object, object>>
            {
                {typeof (ICollection<OperationalProcessType>), (s) => null}
            };

            var original = _resolvers
                .Single(x => x.ServiceComponent.Id == ResolverId1)
                .GetClone(inits);

            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());

            var result = _resolversUpdated
                .Single(x => x.ServiceComponent.Id == ResolverId1);

            var compare = new CompareLogic(new ComparisonConfig
            {
                MaxDifferences = 100,
                MembersToIgnore = new List<string>
                {
                    "OperationalProcessTypes",
                    "UpdatedDate",
                    "UpdatedBy"
                }
            });

            var same = compare.Compare(original, result);
            Assert.IsTrue(same.AreEqual);
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ModelValid_UpdatedBySet()
        {
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());
            Assert.IsFalse(_resolversUpdated.Any(x => x.UpdatedBy != UserName));
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ModelValid_UpdatedDateSet()
        {
            var now = DateTime.Now;
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());
            Assert.IsFalse(_resolversUpdated.Any(x => x.UpdatedDate.Year != now.Year));
            Assert.IsFalse(_resolversUpdated.Any(x => x.UpdatedDate.Month != now.Month));
            Assert.IsFalse(_resolversUpdated.Any(x => x.UpdatedDate.Day != now.Day));
            Assert.IsFalse(_resolversUpdated.Any(x => x.UpdatedDate.Hour != now.Hour));
            Assert.IsFalse(_resolversUpdated.Any(x => x.UpdatedDate.Minute != now.Minute));
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ModelValid_BatchUpdatedCalled()
        {
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());
            _mockResolverService.Verify(v => v.Update(It.IsAny<IEnumerable<Resolver>>()), Times.Once);
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_ModelValid_DotMatrixDataRefreshed()
        {
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());
            _mockResolverService.Verify(v => v.GetDotMatrix(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_Exception_AppendsErrorMessageToHeader()
        {
            SetGetByCustomerException();
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());
            _mockResponseManager.Verify(v => v.AppendHeader(ModelStateErrorNames.ErrorMessage, It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void OperationalProcessesController_UpdateAjaxDotMatrixGrid_Exception_SetsStatusCodeTo500()
        {
            SetGetByCustomerException();
            _target.UpdateAjaxDotMatrixGrid(new DataSourceRequest(), GetUpdateAjaxDotMatrixGridData());
            _mockResponseManager.VerifySet(v => v.StatusCode = 500, Times.Once);
        }

        #region Role Checks

        [TestMethod]
        public void OperationalProcessesController_Index_CheckRole_RoleIsViewer()
        {
            Assert.AreEqual(UserRoles.Viewer, _target.GetMethodAttributeValue("Index", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceComponentController_ReadAjaxDotMatrixGrid_CheckRole_RoleIsViewer()
        {
            Assert.AreEqual(UserRoles.Viewer, _target.GetMethodAttributeValue("ReadAjaxDotMatrixGrid", (AuthorizeAttribute att) => att.Roles));
        }

        [TestMethod]
        public void ServiceComponentController_UpdateAjaxDotMatrixGrid_CheckRole_RoleIsArchitect()
        {
            Assert.AreEqual(UserRoles.Architect, _target.GetMethodAttributeValue("UpdateAjaxDotMatrixGrid", (AuthorizeAttribute att) => att.Roles));
        }

        #endregion

        #region Helpers

        private void SetCustomerContextNull()
        {
            _mockAppUserContext.Setup(s => s.Current)
                .Returns(new AppContext { CurrentCustomer = null });
        }

        private void SetCustomerContextZero()
        {
            _mockAppUserContext.Setup(s => s.Current)
                .Returns(new AppContext { CurrentCustomer = new CurrentCustomerViewModel { Id = 0 } });
        }

        private void SetGetDotMatrixException()
        {
            _mockResolverService
                .Setup(x => x.GetDotMatrix(It.IsAny<int>(), It.IsAny<bool>()))
                .Throws(new Exception("Oh no!!"));
        }

        private void SetGetByCustomerException()
        {
            _mockResolverService
                .Setup(x => x.GetByCustomer(It.IsAny<int>()))
                .Throws(new Exception("Oh no!!"));
        }

        private IList<Dictionary<string, string>> GetUpdateAjaxDotMatrixGridData()
        {
            var d = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                {
                    {DotMatrixNames.ResolverId, ResolverId1.ToString()},
                    {DotMatrixNames.ResolverName, "XXX"},
                    {DotMatrixNames.ComponentName, "AAA"},
                    {string.Concat(DotMatrixNames.OpIdPrefix, _operationalProcessTypeRefDatas[0].Id.ToString()), "true"},
                    {string.Concat(DotMatrixNames.OpIdPrefix, _operationalProcessTypeRefDatas[1].Id.ToString()), "false"},
                    {string.Concat(DotMatrixNames.OpIdPrefix, _operationalProcessTypeRefDatas[2].Id.ToString()), "true"},
                    {string.Concat(DotMatrixNames.OpIdPrefix, _operationalProcessTypeRefDatas[3].Id.ToString()), "true"},
                },
                new Dictionary<string, string>
                {
                    {DotMatrixNames.ResolverId, ResolverId2.ToString()},
                    {DotMatrixNames.ResolverName, "YYY"},
                    {DotMatrixNames.ComponentName, "BBB"},
                    {string.Concat(DotMatrixNames.OpIdPrefix, _operationalProcessTypeRefDatas[0].Id.ToString()), "true"},
                    {string.Concat(DotMatrixNames.OpIdPrefix, _operationalProcessTypeRefDatas[1].Id.ToString()), "false"},
                    {string.Concat(DotMatrixNames.OpIdPrefix, _operationalProcessTypeRefDatas[2].Id.ToString()), "false"},
                    {string.Concat(DotMatrixNames.OpIdPrefix, _operationalProcessTypeRefDatas[3].Id.ToString()), "false"},
                }
            };

            return d;
        }

        #endregion
    }
}
