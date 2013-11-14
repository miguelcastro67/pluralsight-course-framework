using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Common.Contracts
{
    public interface IBusinessEngineFactory
    {
        T GetBusinessEngine<T>() where T : IBusinessEngine;
    }
}
