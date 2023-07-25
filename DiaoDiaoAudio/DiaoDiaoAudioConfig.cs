using PlayerRoles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginApiAudio
{
    [Obfuscation(Exclude = true)]
    public sealed class DiaoDiaoAudioConfig
    {
        /// <summary>
        /// 是否开启音频
        /// </summary>
        public bool EnableAudio { get; set; } = true;
    }
}
