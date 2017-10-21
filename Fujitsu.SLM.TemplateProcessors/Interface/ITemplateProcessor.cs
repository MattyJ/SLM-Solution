using Fujitsu.SLM.Services.Entities;
using System;
using System.Collections.Generic;

namespace Fujitsu.SLM.TemplateProcessors.Interface
{
    public interface ITemplateProcessor : IDisposable
    {
        void Execute(int serviceDeskId, IReadOnlyCollection<TemplateDomainListItem> selectedDomains);
        void Save();
        void Rollback();
    }
}
