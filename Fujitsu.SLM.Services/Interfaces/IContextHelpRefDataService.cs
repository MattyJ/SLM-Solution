using System.Collections.Generic;
using Fujitsu.SLM.Model;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IContextHelpRefDataService : IService<ContextHelpRefData>
    {
        ContextHelpRefData GetByHelpKey(string helpKey);
    }
}