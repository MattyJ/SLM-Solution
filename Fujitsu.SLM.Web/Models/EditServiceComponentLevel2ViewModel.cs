using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(EditServiceComponentLevel2ViewModelValidator))]
    public class EditServiceComponentLevel2ViewModel : EditServiceComponentWithResolverViewModel
    {
        public EditServiceComponentNameLevelViewModel ComponentNameLevel { get; set; }
        public ServiceActivityViewModel ServiceActivities { get; set; }
        public bool CanEdit { get; set; }
    }
}