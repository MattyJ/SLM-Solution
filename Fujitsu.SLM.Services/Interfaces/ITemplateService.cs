using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using System.Collections.Generic;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface ITemplateService : IService<Template>
    {
        IEnumerable<TemplateListItem> AllTemplates();
        IEnumerable<TemplateDomainListItem> AllTemplateDomains(string templateType);
        //void TransformSelectedTemplateDomains(int serviceDeskId, IReadOnlyCollection<TemplateDomainListItem> selectedDomains);
        IEnumerable<TemplateRowListItem> GetTemplateRows(int id);
    }
}