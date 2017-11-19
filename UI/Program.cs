/********************************************************
*	Programe : 程序入口                                 *
*	张永锋                                              *
*	zhangyf07@gmail.com                                 *
*	2010.8                                              *
*********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            CompeteMode rootDialog = new CompeteMode();
            Data.rootDialog = rootDialog;//将根对话框记录下来以便下一轮对抗时进行参数配置
            Control.CheckForIllegalCrossThreadCalls = false;
            Application.Run(rootDialog);
        }
    }
}
