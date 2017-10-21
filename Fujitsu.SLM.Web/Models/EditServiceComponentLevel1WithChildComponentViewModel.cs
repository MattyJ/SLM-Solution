using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(EditServiceComponentLevel1WithChildComponentViewModelValidator))]
    public class EditServiceComponentLevel1WithChildComponentViewModel : EditServiceComponentWithResolverViewModel
    {
        public EditServiceComponentNameViewModel ComponentName { get; set; }

        public EditServiceComponentDiagramOrderViewModel DiagramOrder { get; set; }
        public ServiceActivityViewModel ServiceActivities { get; set; }
        public ServiceComponentViewModel ServiceComponent { get; set; }

    }
}