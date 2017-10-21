using Fujitsu.SLM.Model;
using System.Collections.Generic;

namespace Fujitsu.SLM.Transformers.Interfaces
{
    public interface ITransformTemplateToDesign
    {
        void Transform(int serviceDeskId, List<TemplateRow> templateRows);
    }
}
