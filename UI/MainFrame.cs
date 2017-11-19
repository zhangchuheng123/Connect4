/********************************************************
*	MainFrame : 主棋盘界面                              *
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
using System.Diagnostics;

namespace UI
{
    public partial class MainFrame : Form
    {
        public MainFrame()
        {
            InitializeComponent();
            InitBoard(Data.M, Data.N);
        }

        private void InitBoard(int M, int N)
        {
            //初始化每一个按钮
            buttons = new MyButton[M][];
            for (int i = 0; i < M; i++)
            {
                buttons[i] = new MyButton[N];
                for (int j = 0; j < N; j++)
                {
                    buttons[i][j] = new MyButton(i, j);
                    buttons[i][j].Dock = DockStyle.Fill;
                    buttons[i][j].Margin = new Padding(0);
                }
            }

            //生成一个随机的不可落子点,存放在Data.noX 和 Data.noY里面
            Random rand = new Random();
            Data.noX = rand.Next(0, M);
            Data.noY = rand.Next(0, N);
            //Data.noX = M - 2;
            //Data.noY = 2;

            try
            {
                //buttons[Data.noX][Data.noY].Image = Image.FromFile(Data.xImageFile);
                buttons[Data.noX][Data.noY].Image = Data.xImage;
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("Can not find image file ./res/X.png");
                Application.Exit();
            }

            //初始化Data.top和Data.board
            if (Data.competeMode)//人机对抗，只使用top和board
            {
                Data.top = new int[N];
                for (int i = 0; i < N; i++)
                {
                    Data.top[i] = M;
                }
                
                //要对随机不可落子点进行处理
                if (Data.noX == M - 1)
                {
                    Data.top[Data.noY] = M - 1;
                }

                Data.board = new int[M * N];
                for (int i = 0; i < M * N; i++)
                {
                    Data.board[i] = 0;
                }
            }
            else//机器对抗，需要top boardA boardB
            {
                Data.top = new int[N];
                for (int i = 0; i < N; i++)
                {
                    Data.top[i] = M;
                }

                //要对随机不可落子点进行处理
                if (Data.noX == M - 1)
                {
                    Data.top[Data.noY] = M - 1;
                }

                Data.boardA = new int[M * N];
                Data.boardB = new int[M * N];
                for (int i = 0; i < M * N; i++)
                {
                    Data.boardA[i] = 0;
                    Data.boardB[i] = 0;
                }
            }

            //初始化棋盘Board
            Board = new TableLayoutPanel();
            Board.RowCount = M;
            Board.ColumnCount = N;
            Board.Dock = DockStyle.Left;
            for (int i = 0; i < Board.RowCount; i++)
            {
                Board.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, buttonSize));
            }
            for (int i = 0; i < Board.ColumnCount; i++)
            {
                Board.ColumnStyles.Add(
                    new ColumnStyle(SizeType.Absolute, buttonSize));
            }
            Board.Padding = new Padding(0);
            //添加按钮
            for (int i = 0; i < M; i++)
            {
                Board.Controls.AddRange(buttons[i]);
            }
            //设置位置和大小
            Board.Size = new Size(buttonSize * N, buttonSize * N);
            Board.Location = new System.Drawing.Point(0, 0);


            //初始化控制面板ctrlPanel
            ctrlPanel = new Panel();
            ctrlPanel.Dock = DockStyle.Right;
            //设置位置和大小
            ctrlPanel.Size = new Size(ctrlPanel_Width, buttonSize * M);
            ctrlPanel.Location = new System.Drawing.Point(buttonSize * N + spliter_Width, 0);
            //加入按钮
            if (Data.competeMode)//人机对抗
            {
                manualBackward = new Button();
                manualBackward.Text = "悔棋";
                manualBackward.Width = button_Width;
                manualBackward.Click += new EventHandler(Controller.ManualBackwordClicked);
                if (Data.manFirst)
                {
                    manualBackward.Enabled = false;
                }
                else
                {
                    manualBackward.Enabled = true;
                }

                manualBackward.Location = new System.Drawing.Point(0,0);//相对于ctrlPanel的位置
                ctrlPanel.Controls.Add(manualBackward);

                Data.history = new Stack<Step>();
            }
            else
            {
                autoPause = new Button();
                autoPause.Text = "暂停";
                autoPause.Width = button_Width;
                autoPause.Enabled = true;
                autoPause.Click += new EventHandler(Controller.AutoPauseClicked);

                autoContinue = new Button();
                autoContinue.Text = "继续";
                autoContinue.Width = button_Width;
                autoContinue.Enabled = false;
                autoContinue.Click += new EventHandler(Controller.AutoContinueClicked);

                autoBackward = new Button();
                autoBackward.Text = "后退";
                autoBackward.Width = button_Width;
                autoBackward.Enabled = false;
                autoBackward.Click += new EventHandler(Controller.AutoBackwardClicked);

                autoForeward = new Button();
                autoForeward.Text = "前进";
                autoForeward.Width = button_Width;
                autoForeward.Enabled = false;
                autoForeward.Click += new EventHandler(Controller.AutoForewardClicked);

                autoPause.Location = new System.Drawing.Point(0, 0);
                autoContinue.Location = new System.Drawing.Point(0, autoPause.Location.Y + autoPause.Height);
                autoBackward.Location = new System.Drawing.Point(0, autoContinue.Location.Y + 2 * autoContinue.Height);
                autoForeward.Location = new System.Drawing.Point(0, autoBackward.Location.Y + autoBackward.Height);
                ctrlPanel.Controls.Add(autoPause);
                ctrlPanel.Controls.Add(autoContinue);
                ctrlPanel.Controls.Add(autoBackward);
                ctrlPanel.Controls.Add(autoForeward);

                Data.backward = new Stack<Step>();
                Data.foreward = new Stack<Step>();
            }

            
            //初始化分割线
            spliter = new Label();
            spliter.BorderStyle = BorderStyle.FixedSingle;
            spliter.Size = new Size(spliter_Width, buttonSize * M);
            spliter.Location = new System.Drawing.Point(buttonSize * N, 0);


            //重设窗口大小
            //this.ClientSize = new System.Drawing.Size(buttonSize * N, buttonSize * M);
            this.ClientSize = new System.Drawing.Size(buttonSize * N + spliter_Width + ctrlPanel_Width, buttonSize * M);

            //将Borad和spliter和controlPanel添加到BottomPanel中
            BottomPanel.Controls.Add(Board);
            BottomPanel.Controls.Add(spliter);
            BottomPanel.Controls.Add(ctrlPanel);
        }

        //即时显示图标
        public void updataBoard()
        {
            Board.Update();
            return;
        }

        public void updateCtrlPanel()
        {
            if (Data.competeMode)//人机对抗,此时ctrlPanel中只有一个manualBackward
            {
                if(Data.history.Count == 0)
                {
                    manualBackward.Enabled = false;
                }
                else if (Data.history.Peek().isUser())
                {
                    if (Data.over)//如果人落子且游戏结束，是可以悔棋的，方便同学们调试
                    {
                        manualBackward.Enabled = true;
                    }
                    else
                    {
                        manualBackward.Enabled = false;
                    }
                }
                else
                {
                    manualBackward.Enabled = true;
                }

                ctrlPanel.Update();
            }
            else
            {
                if (Data.paused || Data.over)
                {
                    if (Data.over)
                    {
                        autoPause.Enabled = false;
                        autoContinue.Enabled = false;
                    }
                    else
                    {
                        autoPause.Enabled = false;
                        autoContinue.Enabled = true;
                    }

                    if (Data.backward.Count == 0)
                    {
                        autoBackward.Enabled = false;
                    }
                    else
                    {
                        autoBackward.Enabled = true;
                    }

                    if (Data.foreward.Count == 0)
                    {
                        autoForeward.Enabled = false;
                    }
                    else
                    {
                        autoForeward.Enabled = true;
                    }
                }
                else
                {
                    autoPause.Enabled = true;
                    autoContinue.Enabled = false;
                    autoBackward.Enabled = false;
                    autoForeward.Enabled = false;
                }

                ctrlPanel.Update();
            }
        }

        //初始化策略函数
        public bool setStrategy()
        {
            if (Data.competeMode)//人机对抗
            {
                int res = Data.strategy.init(Data.strategyFile, "");
                if (res == 0)//载入dll出错
                {
                    this.Dispose(true);
                    return false;
                }
                else if (res == 1)//载入dll成功，但是未发现函数入口
                {
                    Data.strategy.release();
                    this.Dispose(true);
                    return false;
                }
                return true;
            }
            else
            {
                int resA = Data.strategyA.init(Data.strategyFileA, "A");
                if (resA == 0)
                {
                    this.Dispose(true);
                    return false;
                }
                else if (resA == 1)
                {
                    Data.strategyA.release();
                    this.Dispose(true);
                    return false;
                }
                int resB = Data.strategyB.init(Data.strategyFileB, "B");
                if (resB == 0)
                {
                    this.Dispose(true);
                    return false;
                }
                else if (resB == 1)
                {
                    Data.strategyB.release();
                    this.Dispose(true);
                    return false;
                }
                return true;
            }
        }

        //游戏结束时，将所有的按钮设为不可点
        public void gameOver()
        {
            for (int i = 0; i < Data.M; i++)
            {
                for (int j = 0; j < Data.N; j++)
                {
                    buttons[i][j].Enabled = false;
                }
            }
            return;
        }

        //在悔棋的情况下，将棋盘重新设为可点
        public void gameRecover()
        {
            for (int i = 0; i < Data.M; i++)
            {
                for (int j = 0; j < Data.N; j++)
                {
                    buttons[i][j].Enabled = true;
                }
            }
            return;
        }

        public void gameSuspend()
        {
            lock (thisLock)
            {
                thread.Abort();
            }
            return;
        }

        public void gameContinue()
        {
            while (Data.foreward.Count != 0)//恢复棋子
            {
                Data.backward.Push(Data.foreward.Pop());
                if (Data.backward.Peek().isA())
                {
                    buttons[Data.backward.Peek().x][Data.backward.Peek().y].setImage(true);
                }
                else
                {
                    buttons[Data.backward.Peek().x][Data.backward.Peek().y].setImage(false);
                }
                updataBoard();
            }

            //设定恰当的走步
            Data.aFirst = !(Data.backward.Peek().isA());
            //继续
            Go();
        }

        private bool isLegal(int x, int y, bool competeMode, bool isA)
        {
            if (competeMode)//人机对抗
            {
                if (x < 0 || x >= Data.M || y < 0 || y >= Data.N)
                {
                    MessageBox.Show("Your strategy has made an illegal step:\nYour output point (x = " + x + ", y = " + y + ") is not in the range of chess board.\n(i.e. x < 0 || x >= M || y < 0 || y >=N)");
                    return false;
                }
                if (Data.top[y] != x + 1)
                {
                    MessageBox.Show("Your strategy has made an illegal step:\nYour output point (x = " + x + ", y = " + y + ") is not on the top of the column.\n(i.e. top[y] != x + 1)");
                    return false;
                }
                if (x == Data.noX && y == Data.noY)
                {
                    MessageBox.Show("Your strategy has made an illegal step:\nYour output point (x = " + x + ", y = " + y + ") is the button on which is not allowed to put a chess.");
                }
                return true;
            }
            else
            {
                if (x < 0 || x >= Data.M || y < 0 || y >= Data.N)
                {
                    if (isA)
                    {
                        MessageBox.Show("Strategy A has made an illegal step:\nThe output point (x = " + x + ", y = " + y + ") is not in the range of chess board.\n(i.e. x < 0 || x >= M || y < 0 || y >=N)");
                    }
                    else
                    {
                        MessageBox.Show("Strategy B has made an illegal step:\nThe output point (x = " + x + ", y = " + y + ") is not in the range of chess board.\n(i.e. x < 0 || x >= M || y < 0 || y >=N)");
                    }
                    return false;
                }
                if (Data.top[y] != x + 1)
                {
                    if (isA)
                    {
                        MessageBox.Show("Strategy A has made an illegal step:\nThe output point (x = " + x + ", y = " + y + ") is not on the top of the column.\n(i.e. top[y] != x + 1)");
                    }
                    else
                    {
                        MessageBox.Show("Strategy B has made an illegal step:\nThe output point (x = " + x + ", y = " + y + ") is not on the top of the column.\n(i.e. top[y] != x + 1)");
                    }
                    return false;
                }
                if (x == Data.noX && y == Data.noY)
                {
                    if (isA)
                    {
                        MessageBox.Show("Strategy A has made an illegal step:\nThe output point (x = " + x + ", y = " + y + ") is the button on which is not allowed to put a chess.");
                    }
                    else
                    {
                        MessageBox.Show("Strategy B has made an illegal step:\nThe output point (x = " + x + ", y = " + y + ") is the button on which is not allowed to put a chess.");
                    }
                    return false;
                }
                return true;
            }
        }

        //人机对抗中计算机在某一个点上落子
        public void computerGo()
        {
            int x, y;
            unsafe
            {
                fixed (int* _top = Data.top, _board = Data.board)
                {
                    try
                    {
                        //---
                        Stopwatch s = new Stopwatch();
                        Stopwatch e = Stopwatch.StartNew();
                        s.Start();
                        //---

                        Point* p = Data.strategy.getPoint(Data.M, Data.N, _top, _board, Data.lastX, Data.lastY, Data.noX, Data.noY);

                        //---
                        e.Stop();
                        long milisecs = s.ElapsedMilliseconds;
                        if (milisecs > Data.maxMiliSecs)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Warning : \nYour last step used ");
                            sb.Append(milisecs.ToString());
                            sb.Append(" ms,\nwhich is more than the limited ");
                            sb.Append(Data.maxMiliSecs.ToString());
                            sb.Append(" ms.");
                            MessageBox.Show(sb.ToString());
                        }
                        //---

                        x = p->x;
                        y = p->y;
                        Data.strategy.clearPoint(p);
                    }
                    catch
                    {
                        MessageBox.Show("A bug occurred in your strategy file!\nYou have lost.");
                        gameOver();
                        thread.Abort();
                        return;
                    }
                }
            }

            if (!isLegal(x, y, true, true))
            {
                gameOver();
                thread.Abort();
                return;
            }
            
            buttons[x][y].setImage(false);
            updataBoard();
            Data.lastX = x;
            Data.lastY = y;
            Data.board[x * Data.N + y] = 2;
            Data.top[y]--;
            //对不可落子点进行处理
            if (x == Data.noX + 1 && y == Data.noY)
            {
                Data.top[y]--;
            }
            //将该步落子放到栈中
            Data.history.Push(new Step(false, x, y));
            //更新控制面板
            updateCtrlPanel();
            //判断胜负
            //Judge.judgeUM(false);
            Data.over = Judge.judgeUM(false);
        }

        //机器对抗中A落子
        //return - true : 游戏已经结束 false - 游戏尚未结束
        private bool AGo()
        {
            int x, y;
            unsafe
            {
                fixed (int* _top = Data.top, _board = Data.boardA)
                {
                    try
                    {
                        //---
                        Stopwatch s = new Stopwatch();
                        Stopwatch e = Stopwatch.StartNew();
                        s.Start();
                        //---

                        Point* p = Data.strategyA.getPoint(Data.M, Data.N, _top, _board, Data.lastX, Data.lastY, Data.noX, Data.noY);

                        //---
                        e.Stop();
                        long milisecs = s.ElapsedMilliseconds;
                        if (milisecs > Data.maxMiliSecs)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Warning : \nA's last step used ");
                            sb.Append(milisecs.ToString());
                            sb.Append(" ms,\nwhich is more than the limited ");
                            sb.Append(Data.maxMiliSecs.ToString());
                            sb.Append(" ms.");
                            MessageBox.Show(sb.ToString());
                        }
                        //---

                        x = p->x;
                        y = p->y;
                        Data.strategyA.clearPoint(p);
                    }
                    catch
                    {
                        MessageBox.Show("A bug occurred in strategy file A!\nA has lost, B has won!");
                        gameOver();
                        thread.Abort();
                        return true;
                    }
                }
            }

            if (!isLegal(x, y, false, true))
            {
                gameOver();
                thread.Abort();
                return true;
            }

            buttons[x][y].setImage(true);
            updataBoard();
            Data.lastX = x;
            Data.lastY = y;
            Data.boardA[x * Data.N + y] = 2;
            Data.boardB[x * Data.N + y] = 1;
            Data.top[y]--;
            //对不可落子点进行处理
            if (x == Data.noX + 1 && y == Data.noY)
            {
                Data.top[y]--;
            }
            //将该步放到堆栈中
            Data.backward.Push(new Step(true, x, y));

            bool over = Judge.judgeMM(true);
            return over;
        }

        //机器对抗中B落子
        //return - 同上
        private bool BGo()
        {
            int x, y;
            unsafe
            {
                fixed (int* _top = Data.top, _board = Data.boardB)
                {
                    try
                    {
                        //---
                        Stopwatch s = new Stopwatch();
                        Stopwatch e = Stopwatch.StartNew();
                        s.Start();
                        //---

                        Point* p = Data.strategyB.getPoint(Data.M, Data.N, _top, _board, Data.lastX, Data.lastY, Data.noX, Data.noY);

                        //---
                        e.Stop();
                        long milisecs = s.ElapsedMilliseconds;
                        if (milisecs > Data.maxMiliSecs)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("Warning : \nB's last step used ");
                            sb.Append(milisecs.ToString());
                            sb.Append(" ms,\nwhich is more than the limited ");
                            sb.Append(Data.maxMiliSecs.ToString());
                            sb.Append(" ms.");
                            MessageBox.Show(sb.ToString());
                        }
                        //---

                        x = p->x;
                        y = p->y;
                        Data.strategyB.clearPoint(p);
                    }
                    catch
                    {
                        MessageBox.Show("A bug occurred in strategy file B!\nB has lost, A has won!");
                        gameOver();
                        thread.Abort();
                        return true;
                    }
                }
            }
            if (!isLegal(x, y, false, false))
            {
                gameOver();
                thread.Abort();
                return true;
            }

            buttons[x][y].setImage(false);
            updataBoard();
            Data.lastX = x;
            Data.lastY = y;
            Data.boardA[x * Data.N + y] = 1;
            Data.boardB[x * Data.N + y] = 2;
            Data.top[y]--;
            //对不可落子点进行处理
            if (x == Data.noX + 1 && y == Data.noY)
            {
                Data.top[y]--;
            }
            //将该步放到堆栈中
            Data.backward.Push(new Step(false, x, y));

            bool over = Judge.judgeMM(false);
            return over;
        }

        //开始进行对抗的入口函数
        public void Go()
        {
            if (Data.competeMode)//人机对抗
            {
                //manComputerGo();
                thread = new Thread(new ThreadStart(manComputerGo));
                thread.Start();
            }
            else//机器对抗
            {
                //computerComputerGo();
                thread = new Thread(new ThreadStart(computerComputerGo));
                thread.Start();
            }
        }

        private void manComputerGo()
        {
            if (Data.manFirst)//人先手
            {
                //什么都不做，等待人点击按钮之后由事件处理函数引发机器下棋
            }
            else//机器先手
            {
                //此处需要首先调用策略让计算机进行第一次落子，之后就交给事件处理函数去做了
                computerGo();
            }
        }

        private void computerComputerGo()
        {
            if (Data.aFirst)
            {
                lock (thisLock)
                {
                    Data.over = AGo();
                    if (Data.over)
                    {
                        updateCtrlPanel();
                    }
                }
                //waite
                System.Threading.Thread.Sleep(Convert.ToInt32(Data.deltaT * 1000));
            }

            //现在变成了B先走
            //bool over = false;
            Data.over = false;
            bool aGo = false;
            //while (!over)
            while(!Data.over)
            {
                lock (thisLock)
                {
                    if (aGo)
                    {
                        Data.over = AGo();
                        aGo = false;
                    }
                    else
                    {
                        Data.over = BGo();
                        aGo = true;
                    }

                    if (Data.over)
                    {
                        updateCtrlPanel();
                    }
                }
                //waite
                System.Threading.Thread.Sleep(Convert.ToInt32(Data.deltaT * 1000));
            }
        }
    }
}
