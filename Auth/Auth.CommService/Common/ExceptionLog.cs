using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace Auth.CommService
{
    /// <summary>
    /// 异常记录类
    /// </summary>
    public class ExceptionLog
    {
        /// <summary>日志名称</summary>
        private static string LogName = "";
        /// <summary>
        /// 设置日志名称
        /// </summary>
        /// <param name="logName"></param>
        public static void SetLogname(string logName)
        {
            LogName = logName;
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="Message">日志内容</param>
        public static void MLog(string message)
        {
            Action<StreamWriter> action = (sw) =>
            {
                sw.WriteLine();
                sw.WriteLine("异常日志");
                sw.WriteLine("发生时间：{0}", DateTime.Now.ToString());
                sw.WriteLine("异常消息：{0}", message);
                sw.WriteLine();
            };
            MLog(action);
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void MLog(string message, params string[] args)
        {
            MLog(string.Format(message, args));
        }

        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="exception">异常类</param>
        public static void MLog(Exception exception)
        {
            Action<StreamWriter> action = (sw) =>
            {
                sw.WriteLine();
                sw.WriteLine("异常日志");
                sw.WriteLine("异常名称：{0}", exception.GetType().Name);
                sw.WriteLine("发生时间：{0}", DateTime.Now.ToString());
                sw.WriteLine("异常消息：{0}", exception.Message);
                sw.WriteLine("堆栈信息：{0}", exception.StackTrace);
                sw.WriteLine("对象名称：{0}", exception.Source);
                sw.WriteLine();
            };
            MLog(action);
        }
        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="exception">异常类</param>
        /// <param name="Message">记录异常日志</param>
        public static void MLog(Exception exception, string Message)
        {
            Action<StreamWriter> action = (sw) =>
            {
                sw.WriteLine("异常日志");
                sw.WriteLine("异常名称：{0}", exception.GetType().Name);
                sw.WriteLine("发生时间：{0}", DateTime.Now.ToString());
                sw.WriteLine("异常消息：{0}", exception.Message);
                sw.WriteLine("堆栈信息：{0}", exception.StackTrace);
                sw.WriteLine("对象名称：{0}", exception.Source);
                sw.WriteLine("自定义内容：{0}", Message);
                sw.WriteLine();
            };
            MLog(action);
        }


        /// <summary>
        /// 记录异常日志
        /// </summary>
        /// <param name="exception">异常类</param>
        /// <param name="message">日志内容</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象</param>
        public static void MLog(Exception exception, string message, params string[] args)
        {
            MLog(exception, string.Format(message, args));
        }


        /// <summary>LOCK用的变量</summary>
        private static object symObj = new object();
        private static void MLog(Action<StreamWriter> action)
        {
            try
            {
                string strpath = Application.StartupPath + "\\LOG\\Exception";
                Directory.CreateDirectory(strpath);
                string Pathfile = strpath + "\\" + LogName + DateTime.Now.ToString("yyyyMMdd") + ".log";
                lock (symObj)
                {
                    FileStream fsStream = new FileStream(Pathfile, FileMode.Append, FileAccess.Write, FileShare.Write);
                    using (StreamWriter sw = new StreamWriter(fsStream, Encoding.UTF8))
                    {
                        action(sw);
                    }
                }
            }
            catch { }
        }

    }
}
