using System;
using Fujitsu.SLM.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Model
{
    [TestClass]
    public class LevelViewModelTests
    {
        [TestMethod]
        public void LevelViewModel_Get_AddNullLevel_SetsLevelEmptyString()
        {
            var levelViewModel = new WrapperLevelViewModel
            {
                EditLevel = null
            };

            Assert.AreEqual(string.Empty, levelViewModel.EditLevel);
        }
    }

    internal class WrapperLevelViewModel : LevelViewModel
    {
    }
}
