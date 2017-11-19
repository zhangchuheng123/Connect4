/********************************************************
*	MyButton : 自定义按钮类                             *
*	张永锋                                              *
*	zhangyf07@gmail.com                                 *
*	2010.8                                              *
*********************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace UI
{
    public class MyButton : Button
    {
        public int x;
        public int y;

        public MyButton(int x, int y)
        {
            //注：此处是否应当加上父类构造函数
            this.x = x;
            this.y = y;
            if (Data.competeMode)//如果是人机对抗才为每一个按键添加事件函数
            {
                this.Click += new EventHandler(ButtonClicked);//添加事件处理函数
            }
        }

        public void setImage(bool isUser)
        {
            try
            {
                if (isUser)
                {
                    try
                    {
                        //this.Image = Image.FromFile(Data.userImageFile);
                        this.Image = Data.userImage;
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        MessageBox.Show("Can not find image file ./res/Tongue.png");
                        Application.Exit();
                    }
                }
                else
                {
                    try
                    {
                        //this.Image = Image.FromFile(Data.comImageFile);
                        this.Image = Data.comImage;
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        MessageBox.Show("Can not find image file ./res/Ninja.png");
                        Application.Exit();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Image File Not Found\n{0}", e.ToString());
            }

            this.ImageAlign = ContentAlignment.MiddleCenter;
            this.FlatStyle = FlatStyle.Flat;
            
            /*
            if (isUser)
            {
                this.Text = "○";
            }
            else
            {
                this.Text = "×";
            }
            */
            
        }

        public void clearImage()
        {
            this.Image = null;
        }
        
        private void ButtonClicked(object sender, EventArgs e)
        {
            MyButton btn = (MyButton)sender;
            if (Data.top[btn.y] <= 0)
            {
                MessageBox.Show("This column has been full, chose another column");
                return;
            }
            else
            {
                Data.lastX = Data.top[btn.y] - 1;
                Data.lastY = btn.y;
                Data.board[Data.lastX * Data.N + Data.lastY] = 1;
                Data.top[Data.lastY]--;
                //对不可落子点进行处理
                if (Data.lastX == Data.noX + 1 && Data.lastY == Data.noY)
                {
                    Data.top[y]--;
                }
                Data.mainFrame.buttons[Data.lastX][Data.lastY].setImage(true);
                Data.mainFrame.updataBoard();

                //判断胜负
                /*
                bool over = Judge.judgeUM(true);
                if(!over)
                {
                    Data.mainFrame.computerGo();
                }
                */
                Data.over = Judge.judgeUM(true);

                //保存历史，要在判断胜负的后面，因为ctrlPanel的修改与游戏是否结束有关
                Data.history.Push(new Step(true, Data.lastX, Data.lastY));
                Data.mainFrame.updateCtrlPanel();

                if (!Data.over)
                {
                    Data.mainFrame.computerGo();
                }
            }
        }
    }
}