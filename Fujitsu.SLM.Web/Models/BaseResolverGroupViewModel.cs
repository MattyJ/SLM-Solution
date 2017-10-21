using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(BaseResolverGroupViewModelValidator))]
    public abstract class BaseResolverGroupViewModel : InsertedUpdatedViewModel
    {
        [Display(Name = "Resolver Group")]
        public int? ResolverGroupTypeId { get; set; }
    }
}