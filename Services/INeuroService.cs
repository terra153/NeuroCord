using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuroCord.Services
{
    public interface INeuroService
    {
        public void ResetContext();
        public Task<string> AskNeuro(string content, string author);
    }
}