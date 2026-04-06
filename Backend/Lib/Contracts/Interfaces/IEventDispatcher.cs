using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.Contracts.Models;

namespace Lib.Contracts.Interfaces
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(List<ThreadEvent> events);
    }
}
