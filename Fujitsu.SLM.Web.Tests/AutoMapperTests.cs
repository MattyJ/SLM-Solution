using AutoMapper;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.UnitTesting;
using Fujitsu.SLM.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class AutoMapperTests
    {
        [TestInitialize]
        public void Initialize()
        {
            Bootstrapper.SetupAutoMapper();
        }

        [TestMethod]
        public void MapperConfigTest()
        {
            Mapper.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void MapperConfig_Explicit_ListDotMatrix_ConvertsToDictionary()
        {
            var src = new List<DotMatrixListItem>
            {
                new DotMatrixListItem {DisplayName = "X1", Name = "X2", Value = 10},
                new DotMatrixListItem {DisplayName = "Y1", Name = "Y2", Value = true}
            };

            var res = Mapper.Map<Dictionary<string, object>>(src);

            Assert.AreEqual(res["X2"], 10);
            Assert.AreEqual(res["Y2"], true);
        }

        [TestMethod]
        public void MapperConfig_Explicit_ListOfListDotMatrix_ConvertsToListOfDictionary()
        {
            var src = new List<List<DotMatrixListItem>>
            {
                new List<DotMatrixListItem>
                {
                    new DotMatrixListItem {DisplayName = "X1", Name = "X2", Value = 10},
                    new DotMatrixListItem {DisplayName = "Y1", Name = "Y2", Value = true}
                },
                new List<DotMatrixListItem>
                {
                    new DotMatrixListItem {DisplayName = "A1", Name = "A2", Value = 15},
                    new DotMatrixListItem {DisplayName = "B1", Name = "B2", Value = false}
                }
            };

            var res = Mapper.Map<List<Dictionary<string, object>>>(src);

            Assert.AreEqual(res[0]["X2"], 10);
            Assert.AreEqual(res[0]["Y2"], true);
            Assert.AreEqual(res[1]["A2"], 15);
            Assert.AreEqual(res[1]["B2"], false);
        }

        [TestMethod]
        public void MapperConfig_Explicit_EditServiceComponentLevel2ViewModel_NoResolverCreatedCanEditFalse()
        {
            var sc = UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
            {
                x.Resolver = new Resolver
                {
                    ServiceDeliveryOrganisationType = new ServiceDeliveryOrganisationTypeRefData(),
                    ServiceDeliveryUnitType = new ServiceDeliveryUnitTypeRefData(),
                    ResolverGroupType = new ResolverGroupTypeRefData(),

                };
            });


            var result = Mapper.Map<ServiceComponent, EditServiceComponentLevel2ViewModel>(sc);
            Assert.IsFalse(result.CanEdit);
        }

        [TestMethod]
        public void MapperConfig_Explicit_EditServiceComponentLevel2ViewModelHasResolver_CanEditTrue()
        {
            var sc = UnitTestHelper.GenerateRandomData<ServiceComponent>(x =>
            {
                x.Resolver = new Resolver
                {
                    Id = 1,
                    ServiceDeliveryOrganisationType = new ServiceDeliveryOrganisationTypeRefData(),
                    ServiceDeliveryUnitType = new ServiceDeliveryUnitTypeRefData(),
                    ResolverGroupType = new ResolverGroupTypeRefData(),

                };
            });
            var result = Mapper.Map<ServiceComponent, EditServiceComponentLevel2ViewModel>(sc);
            Assert.IsTrue(result.CanEdit);
        }
    }
}