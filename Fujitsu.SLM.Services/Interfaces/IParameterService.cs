using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Model;
using System.Collections.Generic;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IParameterService
    {
        IEnumerable<Parameter> All();
        int Create(Parameter entity);
        void Update(Parameter entity);
        void Delete(Parameter entity);
        Parameter Find(string parameterName);
        T GetParameterByName<T>(string parameterName);
        T GetParameterByNameAndCache<T>(string parameterName);
        T GetParameterByNameOrCreate<T>(string parameterName, T defaultValue, ParameterType parameterType);
        void SaveParameter<T>(string parameterName, T value, ParameterType parameterType);
    }
}