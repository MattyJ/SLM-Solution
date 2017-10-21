using System;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.ModelHelpers;

namespace Fujitsu.SLM.Extensions
{
    public static class ServiceComponentExtensions
    {
        internal static Func<IServiceComponentHelper> ServiceComponentHelperFactory = () => new ServiceComponentHelper();

        public static IServiceComponentHelper ServiceComponentHelper(this ServiceComponent serviceComponent)
        {
            return ServiceComponentHelperFactory();
        }
    }
}