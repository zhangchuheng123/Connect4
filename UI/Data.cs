/********************************************************
*	Data : 对一些全局变量的封装                         *
*	张永锋                                              *
*	zhangyf07@gmail.com                                 *
*	2010.8                                              *
*********************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Drawing;

namespace UI
{
    public class Data{
        public static bool competeMode = true;//true-人机对抗 false-机器对抗
        public static CompeteMode rootDialog;
        public static MainFrame mainFrame;

        public static string strategyFile = null;//人机对抗时的机器策略文件
        public static Strategy strategy = new Strategy();
        public static bool manFirst = true;//true-人先手

        public static string strategyFileA = null;
        public static Strategy strategyA = new Strategy();
        public static string strategyFileB = null;
        public static Strategy strategyB = new Strategy();
        public static bool aFirst = true;//true-A先手
        public static double deltaT = 1.0;

        public const int minSize = 9;
        public const int maxSize = 13;
	    public static int M;	//棋盘大小
        public static int N;
	    public static int[] top;	//当前每一列的列顶，在MainFrame.InitBoard函数中初始化
	    public static int[] board;	//搜索过程中模拟出来的当前棋局	0空位置 1有用户的棋 2有电脑的棋
        public static int[] boardA;
        public static int[] boardB;

        public static int noX = 0;
        public static int noY = 0;

        public static int lastX = -1;	//计算机或用户上一次落子的地方
        public static int lastY = -1;

        public static long maxMiliSecs = 5000;//每步允许的最长时间(ms)

        /*
        public static string comImageFile = "./res/Ninja.png";
        public static string userImageFile = "./res/Tongue.png";
        public static string xImageFile = "./res/X.png";
        */

        public static System.Reflection.Assembly asm = System.Reflection.Assembly.GetEntryAssembly();
        public static ResourceManager rm = new ResourceManager("UI.picture", asm);
        public static Image userImage = (Image)rm.GetObject("userImage");
        public static Image comImage = (Image)rm.GetObject("comImage");
        public static Image xImage = (Image)rm.GetObject("xImage");
        public static Icon icon = (Icon)rm.GetObject("icon");

        public static bool over = false;

        //人机对抗下的悔棋机制只需要一个stack
        public static Stack<Step> history;
        //机器对抗下没有悔棋的概念，只能后退和前进，慢慢查看落子的情况，以便于同学们调试，此时需要两个栈
        public static Stack<Step> backward;
        public static Stack<Step> foreward;

        public static bool paused = false;
    }
}