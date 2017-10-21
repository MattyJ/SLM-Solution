using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Fujitsu.SLM.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        public static List<string> GetModelStateMesssages(this ModelStateDictionary modelStateDictionary)
        {
            var result = modelStateDictionary.Values
                .SelectMany(m => m.Errors)
                .Select(s => s.ErrorMessage)
                .ToList();
            return result;
        }
    }
}