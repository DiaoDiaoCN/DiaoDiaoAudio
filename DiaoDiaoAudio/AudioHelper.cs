using PluginAPI.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SCPSLAudioApi.AudioCore;
using System.IO;
using PluginAPI.Helpers;
using Mirror;
using CommandSystem;
using System.Xml.Linq;
using UnityEngine;

namespace PluginApiAudio
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    ///音频
    public class SCP_Audio : ParentCommand
    {
        public SCP_Audio() => LoadGeneratedCommands();

        public override string Command { get; } = "audio";

        public override string[] Aliases { get; } = new string[] { "audio" };

        public override string Description { get; } = "播放阴乐";

        public override void LoadGeneratedCommands() { }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            AudioHelper.Play(arguments.At(0));
            response = "听";
            return true;
        }
    }
    /// <summary>
    /// 音乐播放方法
    /// 使用音频核心方案来源：https://github.com/CedModV2/SCPSLAudioApi
    /// </summary>
    public static class AudioHelper
    {
        /// <summary>
        /// 播放文件夹
        /// </summary>
        public static readonly string AudioPath = Path.Combine($"{Paths.Plugins + "\\" + Server.Port}", "CustomSounds");

        /// <summary>
        /// 注册
        /// </summary>
        internal static void Register()
        {
            SCPSLAudioApi.Startup.SetupDependencies();
            //创建文件夹
            if (!Directory.Exists(AudioPath))
                Directory.CreateDirectory(AudioPath);
        }

        /// <summary>
        /// 播放音效
        /// 仅本人收听
        /// </summary>
        /// <param name="AudioPath">文件路径</param>
        /// <param name="hub">播放人</param>
        /// <param name="To">目标，不设置就是全员收听</param>
        public static AudioPlayerBase PlayAudio(this Player player, string _AudioFile, bool Loop = false, int Volume = 80)
        {
            //播放
            return Play(_AudioFile, new List<int>() { player.PlayerId }, Loop, Volume);
        }

        /// <summary>
        /// 播放音效
        /// 附近玩家听到
        /// </summary>
        /// <param name="AudioPath">文件路径</param>
        /// <param name="hub">播放人</param>
        /// <param name="To">目标，不设置就是全员收听</param>
        public static AudioPlayerBase PlayAudio(this Player player, string _AudioFile, float distance, bool Loop = false, int Volume = 80)
        {
            var playerids = new List<int>();
            foreach (var p in Player.GetPlayers().Where(p => UnityEngine.Vector3.Distance(p.Position, player.Position) <= distance))
            {
                playerids.Add(p.PlayerId);
            }
            //播放
            return Play(_AudioFile, playerids, Loop, Volume);
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="AudioPath">文件路径</param>
        /// <param name="hub">播放人</param>
        /// <param name="To">目标，不设置就是全员收听</param>
        public static AudioPlayerBase Play(string _AudioFile, List<int> To = null, bool Loop = false, int Volume = 80)
        {
            //收听人
            if (To == null)
                To = Player.GetPlayers().Select(p => p.PlayerId).ToList();
            //播放
            return _Play(_AudioFile, To, Loop, Volume);
        }

        private static AudioPlayerBase _Play(string _AudioFile, List<int> To, bool Loop, int Volume = 80)
        {
            if (DiaoDiaoAudio.Singleton.DiaoDiaoConfig.EnableAudio)
            {
                var AudioFile = Path.Combine(AudioPath, _AudioFile + ".ogg");
                if (!File.Exists(AudioFile))
                {
                    Log.Error("音频文件不存在：" + AudioFile);
                    return null;
                }

                //播放
                AudioPlayerBase audtioPlayer = null;
                if (AudioRob.GetNoPlayRob(out var _ad))
                {
                    audtioPlayer = _ad;//已有的播放器
                }
                else
                {
                    //创建假人
                    var hubPlayer = AudioRob.RobPlayerSimple();
                    //播放器
                    audtioPlayer = AudioPlayerBase.Get(hubPlayer);
                }
                audtioPlayer.Enqueue(AudioFile, -1);//播放文件
                if (To != null)
                    audtioPlayer.BroadcastTo.AddRange(To);//收听方，不设置就是全员收听
                audtioPlayer.Volume = Volume;//音量
                audtioPlayer.Loop = Loop;//循环
                audtioPlayer.Play(0);
                return audtioPlayer;
            }
            else
            {
                Log.Error("插件未启用");
            }
            return null;
        }
    }

    /// <summary>
    /// 作者：调调
    /// 说明：
    /// 假人
    /// </summary>
    public static class AudioRob
    {
        private static int id = 0;

        /// <summary>
        /// 是否假人
        /// </summary>
        public static bool IsRob(this ReferenceHub player)
        {
            return AudioPlayerBase.AudioPlayers.ContainsKey(player);
        }

        /// <summary>
        /// 获取没有播放的假人
        /// </summary>
        public static bool GetNoPlayRob(out AudioPlayerBase _ad)
        {
            if (AudioPlayerBase.AudioPlayers.Count(p => p.Value.IsFinish ) > 0)
            {
                _ad = AudioPlayerBase.AudioPlayers.First(p => p.Value.IsFinish).Value;
                return true;
            }
            _ad = null;
            return false;
        }

        /// <summary>
        /// 简单创建假人，用于播放音频
        /// </summary>
        public static ReferenceHub RobPlayerSimple()
        {
            if (DiaoDiaoAudio.Singleton.DiaoDiaoConfig.EnableAudio)
            {
                var newPlayer = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
                var fakeConnection = new FakeConnection(id);
                var hubPlayer = newPlayer.GetComponent<ReferenceHub>();
                NetworkServer.AddPlayerForConnection(fakeConnection, newPlayer);
                return hubPlayer;
            }
            else
            {
                Log.Error("插件未启用");
            }
            return null;
        }

        public class FakeConnection : NetworkConnectionToClient
        {
            public FakeConnection(int connectionId) : base(connectionId)
            {

            }

            public override string address
            {
                get
                {
                    return "localhost";
                }
            }

            public override void Send(ArraySegment<byte> segment, int channelId = 0)
            {
            }
            public override void Disconnect()
            {
            }
        }
    }
}
