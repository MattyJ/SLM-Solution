using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using System.Collections.Generic;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IInputTypeRefDataService : IService<InputTypeRefData>
    {
        bool IsInputTypeReferenced(int id);
        IEnumerable<InputTypeRefDataListItem> GetInputTypeRefDataWithUsageStats();
    }
}
