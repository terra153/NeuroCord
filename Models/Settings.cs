using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NeuroCord.Models
{
    public class Settings
    {
        public ulong DefaultChannelId { get; set; }
        public bool FullServerMode { get; set; } = false;
        public bool IgnoreOtherBots { get; set; } = true;
        public bool AnswerAllMessages { get; set; } = false;
        public ulong[] IgnoreUserIds { get; set; } = [];
        public string[] IgnoreMessages { get; set; } = [];
    }
}