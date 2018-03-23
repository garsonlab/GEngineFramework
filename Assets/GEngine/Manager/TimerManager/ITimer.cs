/*
 * GEngine Framework for Unity By Garson(https://github.com/garsonlab)
 * -------------------------------------------------------------------
 * FileName: ITimer
 * Date    : 2018/01/05
 * Version : v1.0
 * Describe: 
 */

namespace GEngine.Managers
{
    public interface ITimer
    {
        /// <summary>
        /// 延时调用
        /// </summary>
        /// <param name="delay">延迟时间</param>
        /// <param name="handler">回调函数</param>
        /// <param name="parm">回调参数</param>
        /// <returns></returns>
        TimerData DelayCall(float delay, TimerHandler handler, object parm = null);
        /// <summary>
        /// 间隔调用
        /// </summary>
        /// <param name="times">调用次数，-1表示无尽循环调用</param>
        /// <param name="interval">调用间隔</param>
        /// <param name="handler">回调函数</param>
        /// <param name="parm">回调参数</param>
        /// <returns></returns>
        TimerData IntervalCall(int times, float interval, TimerHandler handler, object parm = null);
        /// <summary>
        /// 延时间隔回调
        /// </summary>
        /// <param name="times">调用次数，-1表示无尽循环调用</param>
        /// <param name="delay">延迟时间</param>
        /// <param name="interval">调用间隔</param>
        /// <param name="handler">回调函数</param>
        /// <param name="parm">回调参数</param>
        /// <returns></returns>
        TimerData IntervalDelayCall(int times, float delay, float interval, TimerHandler handler, object parm = null);
        /// <summary>
        /// 重复调用
        /// </summary>
        /// <param name="times">调用次数，-1表示无尽循环调用</param>
        /// <param name="delay">延迟时间</param>
        /// <param name="interval">调用间隔</param>
        /// <param name="ignoreTimeScale">是否忽略时间缩放</param>
        /// <param name="handler">回调函数</param>
        /// <param name="parm">回调参数</param>
        /// <returns></returns>
        TimerData RepeatedCall(int times, float delay, float interval, bool ignoreTimeScale, TimerHandler handler, object parm = null);
        /// <summary>
        /// 设置暂停
        /// </summary>
        /// <param name="timerId">回调id</param>
        /// <param name="pause">是否暂停</param>
        void SetPause(uint timerId, bool pause);
        /// <summary>
        /// 取消回调
        /// </summary>
        /// <param name="timerId">回调id</param>
        void CancelCallback(uint timerId);
        /// <summary>
        /// 获取计时器数据
        /// </summary>
        /// <param name="timerId">回调id</param>
        /// <returns></returns>
        TimerData GetTimerData(uint timerId);
    }
}