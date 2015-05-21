using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.Collections;
using System.ServiceProcess;

namespace WSManager
{
    #region 服务操作
    /// <summary>
    /// 服务操作
    /// </summary>
    public class ServiceManager
    {
        /// <summary>
        /// 检查服务是否存在
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static bool ISWindowsServiceInstalled(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController service in services)
            {
                if (service.ServiceName.ToUpper() == serviceName.ToUpper()) return true;
            }
            return false;
        }


        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="name"></param>
        public static bool InstallService(string filepath, string name)
        {
            try
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(name);
                if (!ISWindowsServiceInstalled(name))
                {
                    AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller();
                    myAssemblyInstaller.UseNewContext = true;
                    myAssemblyInstaller.Path = filepath;
                    IDictionary stateSaver = new Hashtable();
                    myAssemblyInstaller.Install(stateSaver);
                    myAssemblyInstaller.Commit(stateSaver);
                    myAssemblyInstaller.Dispose();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 卸载服务
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="name"></param>
        public static bool UnInstallService(string filepath, string name)
        {
            try
            {
                if (ISWindowsServiceInstalled(name))
                {
                    //UnInstall Service  
                    AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller();
                    myAssemblyInstaller.UseNewContext = true;
                    myAssemblyInstaller.Path = filepath;
                    myAssemblyInstaller.Uninstall(null);
                    myAssemblyInstaller.Dispose();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
    #endregion

}
