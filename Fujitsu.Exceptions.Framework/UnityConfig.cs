using System;
using System.Data.Entity.Validation;
using Fujitsu.Exceptions.Framework.ExceptionFormatters;
using Fujitsu.Exceptions.Framework.Interfaces;
using Microsoft.Practices.Unity;

namespace Fujitsu.Exceptions.Framework
{
    public class UnityConfig
    {
        public static void RegisterTypes(IUnityContainer container, Func<LifetimeManager> manager)
        {
            container.RegisterType<IExceptionFormatter, DbEntityValidationExceptionFormatter>(typeof(DbEntityValidationException).Name, manager());
            container.RegisterType<IExceptionFormatter, DefaultExceptionFormatter>(manager());
        }
    }
}
