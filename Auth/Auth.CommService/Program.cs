using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace Auth.CommService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            UnhandledExceptionDlg();
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new CommService() 
            };
            ServiceBase.Run(ServicesToRun);
        }

        static void UnhandledExceptionDlg()
        {
            // Add the event handler for handling UI thread exceptions to the event:
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);

            // Set the unhandled exception mode to force all Windows Forms errors to go through our handler:
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Add the event handler for handling non-UI thread exceptions to the event:
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionFunction);
        }

        static void Application_ThreadException(object obj, System.Threading.ThreadExceptionEventArgs e)
        {
            Exit(e.Exception);
        }

        static void UnhandledExceptionFunction(Object sender, UnhandledExceptionEventArgs args)
        {
            Exit((Exception)args.ExceptionObject);
        }

        static void Exit(Exception ex)
        {
            //记录异常日志
            ExceptionLog.MLog(ex, "服务异常退出！");

            try
            {
                //关闭进程
                //Process.GetCurrentProcess().Kill();
            }
            catch { }
        }


    }
}
