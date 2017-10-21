using FluentValidation;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class EditServiceComponentLevel1WithChildComponentViewModelValidator : AbstractValidator<EditServiceComponentLevel1WithChildComponentViewModel>
    {
        public EditServiceComponentLevel1WithChildComponentViewModelValidator()
        {
            // NotEmpty for ComponentName is picked up from validator on EditServiceComponentNameViewModel

        }
    }
}