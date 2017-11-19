/********************************************************
*	MainFrame : 主棋盘界面                              *
*	张永锋                                              *
*	zhangyf07@gmail.com                                 *
*	2010.8                                              *
*********************************************************/

using System.Threading;
using System.Windows.Forms;

namespace UI
{
    partial class MainFrame
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
            
            //added
            if (thread != null)//表明是在运行过程中或运行结束后
            {
                thread.Abort();
                //此时才需要释放dll,因为如果thread==null,表明在之前载入dll的时候出错,在那里调用dispose的时候,早已根据具体实际情况释放了dll(加入真的载入了的话)
                if (Data.competeMode)
                {
                    Data.strategy.release();
                }
                else
                {
                    Data.strategyA.release();
                    Data.strategyB.release();
                }
            }
            Data.rootDialog.Show();
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.BottomPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // BottomPanel
            // 
            this.BottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BottomPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomPanel.Name = "BottomPanel";
            this.BottomPanel.Size = new System.Drawing.Size(263, 200);
            this.BottomPanel.TabIndex = 0;
            // 
            // MainFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(263, 200);
            this.Controls.Add(this.BottomPanel);
            this.Name = "MainFrame";
            this.Text = "Connect4";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel BottomPanel;

        //Button Panel
        private TableLayoutPanel Board;
        private int buttonSize = 50;
        public MyButton[][] buttons;
        private Thread thread = null;

        //Panel Split
        private Label spliter;
        private int spliter_Width = 2;

        //Control Panel
        private Panel ctrlPanel;
        private int ctrlPanel_Width = 50;

        //Buttons
        private int button_Width = 50;
        private Button manualBackward;
        private Button autoPause;
        private Button autoContinue;
        private Button autoBackward;
        private Button autoForeward;

        //For scyncrolization
        private System.Object thisLock = new System.Object();
    }
}
