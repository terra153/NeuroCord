using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NeuroCord.Models
{
    public class Settings
    {
        [JsonProperty("settings:defaultChannelId")]
        public ulong DefaultChannelId;
        [JsonProperty("settings:fullServerMode")]
        public bool FullServerMode = false;
        [JsonProperty("settings:ignoreOtherBots")]
        public bool IgnoreOtherBots = true;
        [JsonProperty("settings:ignoreUserIds")]
        public ulong[] IgnoreUserIds = [];
    }
}