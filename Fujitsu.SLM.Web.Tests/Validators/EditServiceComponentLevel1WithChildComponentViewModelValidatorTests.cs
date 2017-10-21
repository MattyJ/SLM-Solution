using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class EditServiceComponentLevel1WithChildComponentViewModelValidatorTests
    {
        private EditServiceComponentLevel1WithChildComponentViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new EditServiceComponentLevel1WithChildComponentViewModelValidator();
        }

    }
}
