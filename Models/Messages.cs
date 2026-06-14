using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuroCord.Models
{
    public class Messages
    {
        public string Ping { get; set; } = "";
        public string ResetContext { get; set; } = "";
        public string ApiError { get; set; } = "";
        public string Hello { get; set; } = "";
        public string Typing { get; set; } = "";
    }
}