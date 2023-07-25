using HarmonyLib;
using MEC;
using Microsoft.Win32;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;
using SCPSLAudioApi.AudioCore;
using System.Collections.Generic;
using System.Linq;
using static RoundSummary;

namespace PluginApiAudio
{
    public class DiaoDiaoAudio
    {
        public static DiaoDiaoAudio Singleton { get; private set; }

        [PluginEntryPoint("音频插件", "1.0.0.", "音频", "调调")]
        void LoadPlugin()
        {
            Singleton = this;
            AudioHelper.Register();
            EventManager.RegisterEvents(this);
        }

        [PluginConfig]
        public DiaoDiaoAudioConfig DiaoDiaoConfig;
    }
}
