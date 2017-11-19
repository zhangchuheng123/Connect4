/********************************************************
*	Strategy : 引入策略的接口类                         *
*	张永锋                                              *
*	zhangyf07@gmail.com                                 *
*	2010.8                                              *
*********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms;

namespace UI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int x;
        public int y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public unsafe delegate Point* GET_POINT(int M, int N, int* _top, int* _board, int lastX, int lastY, int noX, int noY);
    public unsafe delegate void CLEAR_POINT(Point* p);

    public class Strategy
    {
        /*
        [DllImport(@"./StrategyFile/StrategyFile.dll")]
        public static extern unsafe Point* getPoint(int M, int N, int* _top, int* _board, int lastX, int lastY, int noX, int noY);
        [DllImport(@"./StrategyFile/StrategyFile.dll")]
        public static extern unsafe void clearPoint(Point* p);
        */

        private int hDLL;
        public GET_POINT getPoint;
        public CLEAR_POINT clearPoint;

        /*
         * 如果是载入dll出错,则返回0;如果载入dll成功但寻找函数入口失败,则返回1,如果全部成功，则返回2
         */
        public int init(string strategyFile, string who)
        {
            hDLL = DLLWrapper.LoadLibrary(strategyFile);
            if (hDLL == 0)
            {
                MessageBox.Show("Failed to load dll" + who + " :\n" + strategyFile + "\nIs it a dll or is it in use now?");
                return 0;
            }
            getPoint = (GET_POINT)DLLWrapper.GetFunctionAddress(hDLL, "getPoint", typeof(GET_POINT));
            clearPoint = (CLEAR_POINT)DLLWrapper.GetFunctionAddress(hDLL, "clearPoint", typeof(CLEAR_POINT));
            if (getPoint == null || clearPoint == null)
            {
                MessageBox.Show("Failed to find desired functions in dll" + who + " :\n" + strategyFile + "\nHave you chosen a right dll?");
                return 1;
            }
            return 2;
        }

        public void release()
        {
            DLLWrapper.FreeLibrary(hDLL);
            return;
        }
    }

    /*
    public class StrategyA
    {
        [DllImport(@"./StrategyFile/StrategyFileA.dll")]
        public static extern unsafe Point* getPoint(int M, int N, int* _top, int* _board, int lastX, int lastY, int noX, int noY);
        [DllImport(@"./StrategyFile/StrategyFileA.dll")]
        public static extern unsafe void clearPoint(Point* p);
    }

    public class StrategyB
    {
        [DllImport(@"./StrategyFile/StrategyFileB.dll")]
        public static extern unsafe Point* getPoint(int M, int N, int* _top, int* _board, int lastX, int lastY, int noX, int noY);
        [DllImport(@"./StrategyFile/StrategyFileB.dll")]
        public static extern unsafe void clearPoint(Point* p);
    }
    */
}
