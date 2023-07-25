# DiaoDiaoAudio
基于SCPSLAudio插件的播放音频方法类

音频文件：使用OGG格式，频率48000HZ，单声道
默认位置：\plugins\对应端口\CustomSounds
插件会自动创建文件夹在此位置

具有以下方法可直接调用：

单人音频播放：
player.PlayAudio(_AudioFile, Loop, Volume);

_AudioFile：音频文件
Loop：循环，默认不开启
Volume：音量，默认80

距离范围音频播放：
player.PlayAudio(_AudioFile, distance, Loop, Volume);

_AudioFile：音频文件
distance：播放范围
Loop：循环，默认不开启
Volume：音量，默认80

指定玩家播放：
AudioHelper.Play(_AudioFile, To, Loop, Volume);

_AudioFile：音频文件
To：指定玩家的ID泛型，为空则是全体
Loop：循环，默认不开启
Volume：音量，默认80

插件包含配置：
EnableAudio：是否开启音频，默认开启

特性：
插件会自动创建假人用于播放，使用的是修改后的SCPSLAudio插件，增加了播放完成IsFinish参数（请使用https://github.com/DiaoDiaoCN/SCPSLAudioApi中的dll）
插件会判定已有的播放列表中未播放的播放器进行播放，如果都在播放中则会创建新的播放器
