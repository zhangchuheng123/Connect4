/********************************************************
*	CompeteMode : 对抗模式及参数设定对话框              *
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

namespace UI
{
    public partial class CompeteMode : Form
    {
        public CompeteMode()
        {
            InitializeComponent();
            setComboBox();
        }

        private void setComboBox()
        {
            String[] size = new String[]{
                "9", "10", "11", "12"
            };
            height.Items.AddRange(size);
            width.Items.AddRange(size);

            String[] t = new String[]{
                "0.0", "0.5", "1.0", "1.5", "2.0", "2.5", "3.0", "3.5", "4.0", "4.5", "5.0", "6.0", "7.0", "8.0"
            };
            deltaT.Items.AddRange(t);
        }

        private void copyFile(string sourceFile, int which)//which:0-strategyFile 1-A 2-B
        {
            string fileName;
            if (which == 0)
            {
                fileName = @"StrategyFile.dll";
            }
            else if (which == 1)
            {
                fileName = @"StrategyFileA.dll";
            }
            else
            {
                fileName = @"StrategyFileB.dll";
            }
            string targetPath = @"./StrategyFile/";
            string destFile = System.IO.Path.Combine(targetPath, fileName);
            if (!System.IO.Directory.Exists(targetPath))
            {
                System.IO.Directory.CreateDirectory(targetPath);
            }
            System.IO.File.Copy(sourceFile, destFile, true);

            return;
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            groupBox2.Enabled = true;
            groupBox3.Enabled = false;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Enabled = false;
            groupBox3.Enabled = true;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            groupBox5.Enabled = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            groupBox5.Enabled = true;
        }

        private void Done_Click(object sender, EventArgs e)
        {
            if(radioButton1.Checked && Data.strategyFile == null)
            {
                MessageBox.Show("Please choose the strategy file");
                return;
            }
            if (radioButton2.Checked && Data.strategyFileA == null)
            {
                MessageBox.Show("Please choose the strategy file A");
                return;
            }
            if (radioButton2.Checked && Data.strategyFileB == null)
            {
                MessageBox.Show("Please choose the strategy file B");
                return;
            }
            //加入设定Data相关变量的代码
            if (radioButton1.Checked)//人机对抗
            {
                Data.competeMode = true;

                if (radioButton5.Checked)
                {
                    Data.manFirst = true;
                }
                else
                {
                    Data.manFirst = false;
                }
                //将选中的人机对抗策略文件拷贝到../StrategyFlie目录下
                //copyFile(Data.strategyFile, 0);
            }
            else if (radioButton2.Checked)
            {
                Data.competeMode = false;

                if (radioButton7.Checked)
                {
                    Data.aFirst = true;
                }
                else
                {
                    Data.aFirst = false;
                }
                Data.deltaT = Convert.ToDouble(deltaT.Text);

                //拷贝文件
                //copyFile(Data.strategyFileA, 1);
                //copyFile(Data.strategyFileB, 2);
            }
            if (radioButton3.Checked)//Auto Size
            {
                Random rand = new Random();
                Data.M = rand.Next(Data.minSize, Data.maxSize);
                Data.N = rand.Next(Data.minSize, Data.maxSize);
            }
            else if (radioButton4.Checked)
            {
                Data.M = Convert.ToInt32(height.Text);
                Data.N = Convert.ToInt32(width.Text);
            }

            //当前对话框关闭，打开棋盘对话框
            MainFrame mainFrame = new MainFrame();
            Data.mainFrame = mainFrame;
            if (mainFrame.setStrategy())
            {
                mainFrame.Show();
                this.Hide();
                //开始进行对抗
                mainFrame.Go();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.InitialDirectory = ".";//记住上次选择文件的路径，下次再打开时仍然打开这个路径
            DialogResult res = diag.ShowDialog();
            if (res == DialogResult.OK)
            {
                Data.strategyFile = diag.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.InitialDirectory = ".";
            DialogResult res = diag.ShowDialog();
            if (res == DialogResult.OK)
            {
                Data.strategyFileA = diag.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.InitialDirectory = ".";
            DialogResult res = diag.ShowDialog();
            if (res == DialogResult.OK)
            {
                Data.strategyFileB = diag.FileName;
            }
        }
    }
}
