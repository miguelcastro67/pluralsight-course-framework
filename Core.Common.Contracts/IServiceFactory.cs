using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Contracts
{
    public interface IServiceFactory
    {
        T CreateClient<T>() where T : IServiceContract;
    }
}