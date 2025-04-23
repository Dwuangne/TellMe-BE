using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TellMe.Service.Errors
{
    public class Error
    {
        public int StatusCode { get; set; }
        public List<ErrorDetail>? Message { get; set; }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false 
            });
        }
    }
}
