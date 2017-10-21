using System;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Model.Tests
{
    [TestClass]
    public class LevelViewModelTests
    {
        public class TestViewModel : LevelViewModel
        {

        }

        [TestMethod]
        public void LevelViewModel_SetLevelZero_ReturnsTrueIsLevelZero()
        {
            var model = new TestViewModel()
            {
                EditLevel = NavigationLevelNames.LevelZero,
            };

            Assert.IsTrue(model.IsLevelZero);
        }

        [TestMethod]
        public void LevelViewModel_SetLevelOne_ReturnsTrueIsLeveOne()
        {
            var model = new TestViewModel()
            {
                EditLevel = NavigationLevelNames.LevelOne,
            };

            Assert.IsTrue(model.IsLevelOne);
        }

        [TestMethod]
        public void LevelViewModel_SetLevelTwo_ReturnsTrueIsLevelTwo()
        {
            var model = new TestViewModel()
            {
                EditLevel = NavigationLevelNames.LevelTwo,
            };

            Assert.IsTrue(model.IsLevelTwo);
        }
    }
}
