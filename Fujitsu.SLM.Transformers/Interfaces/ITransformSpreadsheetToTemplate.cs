using Fujitsu.SLM.Model;
using System.Collections.Generic;

namespace Fujitsu.SLM.Transformers.Interfaces
{
    public interface ITransformSpreadsheetToTemplate
    {
        Template Transform(Template template, List<TemplateRow> templateRows);
    }
}