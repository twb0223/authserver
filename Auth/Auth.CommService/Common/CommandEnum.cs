using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.CommService
{
    #region 操作指令枚举
    /// <summary>操作指令枚举</summary>
    public enum CommandEnum
    {
        /// <summary>升级</summary>
        UpGrade,
        /// <summary>音量控制</summary>
        VolumeControl,
        /// <summary>音量查询</summary>
        VolumeQuery,
        /// <summary>设置分辨率</summary>
        SetScreen,
        /// <summary>获得分辨率</summary>
        GetScreen,
        /// <summary>开机</summary>
        TurnOn,
        /// <summary>关机</summary>
        TurnDown,
        /// <summary>重启</summary>
        Reboot,
        /// <summary>休眠</summary>
        PalyerSleep,
        /// <summary>获取下载进度</summary>
        DloadPercent,
        /// <summary>获取节目列表</summary>
        PlaylistMsg,
        /// <summary>屏幕同步</summary>
        SynScreen,
        /// <summary>切屏</summary>
        ChgScreen,
        /// <summary>暂停播放</summary>
        PausePlay,
        /// <summary>停止播放</summary>
        StopPlay,
        /// <summary>恢复播放</summary>
        OnPlay,

        /// <summary>节目发布</summary>
        Publish,
        /// <summary>点播</summary>
        RequestPlay,
        /// <summary>直播</summary>
        LivePlay,
        /// <summary>插播</summary>
        InterPlay,

        /// <summary>心跳</summary>
        Heartbeat,

        /// <summary>注册</summary>
        Register,

        /// <summary>发声</summary>
        Speak,
        /// <summary>开关机</summary>
        Shutdown,
        /// <summary>滚动字幕</summary>
        Scroll,
        /// <summary>时间同步</summary>
        SynchronizationTime,
        /// <summary>获得系统信息</summary>
        SystemInfo,

        /// <summary>插播</summary>
        InterCut,

        /// <summary>日志</summary>
        UploadLog,

        //回复指令
        /// <summary>处理成功</summary>
        ACK_OK,
        /// <summary>处理失败</summary>
        ACK_FAILED,
        /// <summary>已接收，正在处理。
        /// 适用于长时间处理的指令，如收到下载指令时返回已接收，下载成功后回复成功</summary>
        ACK_RECEIVE,
        /// <summary>其他情形（对回复报文进行扩展用）</summary>
        ACK_OTHER,
        /// <summary>
        /// 播控平台发送自定义命令到终端
        /// </summary>
        ShellExe,
        /// <summary>
        /// 休眠
        /// </summary>
        Sleep,
        /// <summary>
        /// 唤醒
        /// </summary>
        Wakeup,

        /// <summary>更新表格数据</summary>
        UpdateTable,
        /// <summary>执行JS函数</summary>
        ExecuteJS,
        /// <summary>终端截屏</summary>
        ShotScreen

    }
    #endregion
    /// <summary>enum类型转换</summary>
    public class EnumConvert
    {
        /// <summary>将string转为enum</summary>
        public static T Parse<T>(string value) where T : struct
        {
            T t = default(T);
            if (!Enum.TryParse<T>(value, true, out t))
            {
                t = default(T);
            }
            return t;
        }
    }
}
