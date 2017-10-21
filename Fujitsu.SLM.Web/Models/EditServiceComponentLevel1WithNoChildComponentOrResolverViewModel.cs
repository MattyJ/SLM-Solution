using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(EditServiceComponentLevel1WithNoChildComponentOrResolverViewModelValidator))]
    public class EditServiceComponentLevel1WithNoChildComponentOrResolverViewModel : EditServiceComponentWithResolverViewModel
    {
        public EditServiceComponentNameViewModel ComponentName { get; set; }
        public EditServiceComponentDiagramOrderViewModel DiagramOrder { get; set; }
        public ServiceActivityViewModel ServiceActivities { get; set; }
        public EditServiceComponentChildViewModel ChildComponent { get; set; }
        public int InputMode { get; set; }
    }
}