using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Extensions;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using System.Collections.Generic;

namespace Fujitsu.SLM.Web.Validators
{
    public class UploadCustomerFileViewModelValidator : AbstractValidator<UploadCustomerFileViewModel>
    {
        public UploadCustomerFileViewModelValidator()
        {
            var validContentTypes = new List<string>
            {
                "application/pdf",
                "application/vnd.ms-excel",
                "application/msexcel",
                "application/x-msexcel",
                "application/x-ms-excel",
                "application/x-excel",
                "application/x-dos_ms_excel",
                "application/xls",
                "application/x-xls",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "application/octet-stream",
                "application/x-zip-compressed" // Visio in IE11
            };

            RuleFor(x => x.CustomerFile.ContentType).Cascade(CascadeMode.StopOnFirstFailure)
                .Contains(validContentTypes).When(w => w.CustomerFile != null).WithMessage(WebResources.InvalidContentType);

            RuleFor(x => x.CustomerFile.ContentLength).Cascade(CascadeMode.StopOnFirstFailure)
                .NotEqual(0).When(w => w.CustomerFile != null).WithMessage(WebResources.NoFileContent);

            RuleFor(x => x.CustomerFile).Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(WebResources.NoFileSelected);

            RuleFor(x => x.EditLevel)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty();

            RuleFor(x => x.Notes)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.StandardNotes);
        }
    }
}