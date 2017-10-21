using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(EditResolverViewModelValidator))]
    public class EditResolverViewModel : EditServiceComponentLevel1WithResolverViewModel
    {
        public OperationalProcessTypesViewModel OperationalProcesses { get; set; }
    }
}