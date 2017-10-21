using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(EditServiceComponentLevel1WithResolverViewModelValidator))]
    public class EditServiceComponentLevel1WithResolverViewModel : EditServiceComponentWithResolverViewModel
    {
        public EditServiceComponentNameViewModel ComponentName { get; set; }
        public EditServiceComponentDiagramOrderViewModel DiagramOrder { get; set; }
        public ServiceActivityViewModel ServiceActivities { get; set; }
    }
}