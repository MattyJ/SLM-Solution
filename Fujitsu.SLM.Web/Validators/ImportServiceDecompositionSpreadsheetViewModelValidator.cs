using System.Collections.Generic;
using FluentValidation;
using Fujitsu.SLM.Web.Extensions;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class ImportServiceDecompositionSpreadsheetViewModelValidator : AbstractValidator<ImportServiceDecompositionSpreadsheetViewModel>
    {
        public ImportServiceDecompositionSpreadsheetViewModelValidator()
        {
            var validContentTypes = new List<string>
            {
                "application/vnd.ms-excel",
                "application/msexcel",
                "application/x-msexcel",
                "application/x-ms-excel",
                "application/x-excel",
                "application/x-dos_ms_excel",
                "application/xls",
                "application/x-xls",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };

            RuleFor(x => x.SpreadsheetFile).Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage(WebResources.NoFileSelected);

            RuleFor(x => x.SpreadsheetFile.ContentLength)
                .NotEqual(0).When(w => w.SpreadsheetFile != null).WithMessage(WebResources.NoFileContent);

            RuleFor(x => x.SpreadsheetFile.ContentType)
                .Contains(validContentTypes).When(w => w.SpreadsheetFile != null).WithMessage(WebResources.InvalidContentType);
        }

    }
}