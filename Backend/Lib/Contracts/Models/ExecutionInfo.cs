using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Contracts.Models
{
    public class ExecutionInfo
    {
        public string? ClassName { get; set; }
        public string? MethodName { get; set; }
        public string? Activity { get; set; }
    }
}
