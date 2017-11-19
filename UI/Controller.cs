using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UI
{
    class Controller
    {
        public static void ManualBackwordClicked(object sender, EventArgs e)
        {
            //MessageBox.Show("点击悔棋");
            if (Data.history.Count == 1)//这种情况肯定是机器先手且棋盘上只有一个子的情形
            {
                Step temp = Data.history.Pop();
                int x = temp.x;
                int y = temp.y;
                //Data.mainFrame中需要改的是图标
                Data.mainFrame.buttons[x][y].clearImage();
                Data.mainFrame.updataBoard();
                //Data中需要改回的是lastX lastY board top
                Data.board[x * Data.N + y] = 0;
                Data.top[y] = x + 1;
                Data.lastX = -1;
                Data.lastY = -1;
                
                //继续
                Data.mainFrame.computerGo();
            }
            else//这种情况肯定是计算机刚刚落子 或 人落子且游戏结束
            {
                if (Data.history.Peek().isUser())//如果上一步是人落子，只退一步
                {
                    Step temp = Data.history.Pop();
                    int x = temp.x;
                    int y = temp.y;
                    Data.mainFrame.buttons[x][y].clearImage();
                    Data.board[x * Data.N + y] = 0;
                    Data.top[y] = x + 1;
                    if (Data.history.Count != 0)
                    {
                        Data.lastX = Data.history.Peek().x;
                        Data.lastY = Data.history.Peek().y;
                    }
                    else
                    {
                        Data.lastX = -1;
                        Data.lastY = -1;
                    }
                }

                else//否则，一次性恢复两步
                {
                    Step temp = Data.history.Pop();
                    int x = temp.x;
                    int y = temp.y;
                    Data.mainFrame.buttons[x][y].clearImage();
                    Data.board[x * Data.N + y] = 0;
                    Data.top[y] = x + 1;

                    temp = Data.history.Pop();
                    x = temp.x;
                    y = temp.y;
                    Data.mainFrame.buttons[x][y].clearImage();
                    Data.board[x * Data.N + y] = 0;
                    Data.top[y] = x + 1;
                    if (Data.history.Count != 0)
                    {
                        Data.lastX = Data.history.Peek().x;
                        Data.lastY = Data.history.Peek().y;
                    }
                    else
                    {
                        Data.lastX = -1;
                        Data.lastY = -1;
                    }
                }

                //如果游戏已经结束,恢复棋盘可点
                if (Data.over)
                {
                    Data.mainFrame.gameRecover();
                }

                Data.mainFrame.updataBoard();
            }

            Data.mainFrame.updateCtrlPanel();
        }

        public static void AutoPauseClicked(object sender, EventArgs e)
        {
            Data.mainFrame.gameSuspend();
            Data.paused = true;
            Data.mainFrame.updateCtrlPanel();
        }

        public static void AutoContinueClicked(object sender, EventArgs e)
        {
            Data.paused = false;
            Data.mainFrame.updateCtrlPanel();
            Data.mainFrame.gameContinue();
        }

        public static void AutoBackwardClicked(object sender, EventArgs e)
        {
            //根据updateCtrlPanel中的逻辑，只有backward栈中有内容时该按钮才能点
            if (!Data.mainFrame.buttons[0][0].Enabled)
            {
                Data.mainFrame.gameRecover();
            }

            Data.foreward.Push(Data.backward.Pop());
            Data.mainFrame.buttons[Data.foreward.Peek().x][Data.foreward.Peek().y].clearImage();
            Data.mainFrame.updataBoard();
            Data.mainFrame.updateCtrlPanel();
        }

        public static void AutoForewardClicked(object sender, EventArgs e)
        {
            //同上
            Data.backward.Push(Data.foreward.Pop());
            Data.mainFrame.buttons[Data.backward.Peek().x][Data.backward.Peek().y].setImage(Data.backward.Peek().isA());
            Data.mainFrame.updataBoard();
            Data.mainFrame.updateCtrlPanel();
        }
    }
}
