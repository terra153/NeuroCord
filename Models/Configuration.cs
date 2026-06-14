using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuroCord.Models
{
    public class Configuration
    {
        public Connection Connection { get; set; } = new();
        public Messages Messages { get; set; } = new();
        public Neuro Neuro { get; set; } = new();
        public Settings Settings { get; set; } = new();
    }
}