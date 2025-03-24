using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Utility
{
    public interface IResponse
    {
        string Message { get; set; }
        string Status { get; set; }
        bool Succeeded { get; set; }
    }

    public interface IResponse<out T> : IResponse
    {
        T Data { get; }
    }
}
