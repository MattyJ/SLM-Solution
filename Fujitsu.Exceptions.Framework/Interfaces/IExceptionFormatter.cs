using System;

namespace Fujitsu.Exceptions.Framework.Interfaces
{
    public interface IExceptionFormatter
    {
        string ToString(Exception exception);
    }
}