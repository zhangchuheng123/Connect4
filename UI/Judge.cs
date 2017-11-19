/********************************************************
*	Judge : 棋局判定辅助函数                            *
*	张永锋                                              *
*	zhangyf07@gmail.com                                 *
*	2010.8                                              *
*********************************************************/

using System.Windows.Forms;

namespace UI
{
    public class Judge
    {
        private static bool userWin(int x, int y)
        {
            //横向检测
            int i, j;
            int count = 0;
            for (i = y; i >= 0; i--)
                if (!(Data.board[x * Data.N + i] == 1))
                    break;
            count += (y - i);
            for (i = y; i < Data.N; i++)
                if (!(Data.board[x * Data.N + i] == 1))
                    break;
            count += (i - y - 1);
            if (count >= 4) return true;

            //纵向检测
            count = 0;
            for (i = x; i < Data.M; i++)
                if (!(Data.board[i * Data.N + y] == 1))
                    break;
            count += (i - x);
            if (count >= 4) return true;

            //左下-右上
            count = 0;
            for (i = x, j = y; i < Data.M && j >= 0; i++, j--)
                if (!(Data.board[i * Data.N + j] == 1))
                    break;
            count += (y - j);
            for (i = x, j = y; i >= 0 && j < Data.N; i--, j++)
                if (!(Data.board[i * Data.N + j] == 1))
                    break;
            count += (j - y - 1);
            if (count >= 4) return true;

            //左上-右下
            count = 0;
            for (i = x, j = y; i >= 0 && j >= 0; i--, j--)
                if (!(Data.board[i * Data.N + j] == 1))
                    break;
            count += (y - j);
            for (i = x, j = y; i < Data.M && j < Data.N; i++, j++)
                if (!(Data.board[i * Data.N + j] == 1))
                    break;
            count += (j - y - 1);
            if (count >= 4) return true;

            return false;
            
        }

        private static bool comWin(int x, int y)
        {
            //横向检测
            int i, j;
            int count = 0;
            for (i = y; i >= 0; i--)
                if (!(Data.board[x * Data.N + i] == 2))
                    break;
            count += (y - i);
            for (i = y; i < Data.N; i++)
                if (!(Data.board[x * Data.N + i] == 2))
                    break;
            count += (i - y - 1);
            if (count >= 4) return true;

            //纵向检测
            count = 0;
            for (i = x; i < Data.M; i++)
                if (!(Data.board[i * Data.N + y] == 2))
                    break;
            count += (i - x);
            if (count >= 4) return true;

            //左下-右上
            count = 0;
            for (i = x, j = y; i < Data.M && j >= 0; i++, j--)
                if (!(Data.board[i * Data.N + j] == 2))
                    break;
            count += (y - j);
            for (i = x, j = y; i >= 0 && j < Data.N; i--, j++)
                if (!(Data.board[i * Data.N + j] == 2))
                    break;
            count += (j - y - 1);
            if (count >= 4) return true;

            //左上-右下
            count = 0;
            for (i = x, j = y; i >= 0 && j >= 0; i--, j--)
                if (!(Data.board[i * Data.N + j] == 2))
                    break;
            count += (y - j);
            for (i = x, j = y; i < Data.M && j < Data.N; i++, j++)
                if (!(Data.board[i * Data.N + j] == 2))
                    break;
            count += (j - y - 1);
            if (count >= 4) return true;

            return false;
        }

        private static bool isTieUM()
        {
            bool tie = true;
            for (int i = 0; i < Data.N; i++)
            {
                if (Data.top[i] > 0)
                {
                    tie = false;
                    break;
                }
            }
            return tie;
        }

        private static int determinResultUM(bool isUser)
        {
            int ans = -1;

            //平局结束,ans = 0
            //用户赢,ans = 1
            //计算机赢, ans = 2
            //游戏未结束, ans = -1

            if (isUser)//上一次是用户落子，要检测是否为用户赢
            {
                if (userWin(Data.lastX, Data.lastY))
                {
                    ans = 1;
                }
                else if (isTieUM())
                {
                    ans = 0;
                }
                else
                {
                    ans = -1;
                }

            }
            else//电脑落子，检测是否为电脑赢
            {
                if (comWin(Data.lastX, Data.lastY))
                {
                    ans = 2;
                }
                else if (isTieUM())
                {
                    ans = 0;
                }
                else
                {
                    ans = -1;
                }
            }

            return ans;
        }


        //对User-Machine对抗的情况进行Judge
        //isUser: true-上一次是用户落子 false-上一次是计算机落子
        //return:   true-游戏已经结束,包括三种情况平局结束0、用户赢1、计算机赢2
        //          false-游戏尚未结束-1
        public static bool judgeUM(bool isUser)
        {
            int ans = determinResultUM(isUser);

            if (ans == 0)
            {
                Data.mainFrame.gameOver();
                MessageBox.Show("We tied");
                return true;
            }
            else if (ans == 1)
            {
                Data.mainFrame.gameOver();
                MessageBox.Show("You have won!");
                return true;
            }
            else if (ans == 2)
            {
                Data.mainFrame.gameOver();
                MessageBox.Show("You have lost!");
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool AWin(int x, int y)
        {
            //横向检测
            int i, j;
            int count = 0;
            for (i = y; i >= 0; i--)
                if (!(Data.boardA[x * Data.N + i] == 2))
                    break;
            count += (y - i);
            for (i = y; i < Data.N; i++)
                if (!(Data.boardA[x * Data.N + i] == 2))
                    break;
            count += (i - y - 1);
            if (count >= 4) return true;

            //纵向检测
            count = 0;
            for (i = x; i < Data.M; i++)
                if (!(Data.boardA[i * Data.N + y] == 2))
                    break;
            count += (i - x);
            if (count >= 4) return true;

            //左下-右上
            count = 0;
            for (i = x, j = y; i < Data.M && j >= 0; i++, j--)
                if (!(Data.boardA[i * Data.N + j] == 2))
                    break;
            count += (y - j);
            for (i = x, j = y; i >= 0 && j < Data.N; i--, j++)
                if (!(Data.boardA[i * Data.N + j] == 2))
                    break;
            count += (j - y - 1);
            if (count >= 4) return true;

            //左上-右下
            count = 0;
            for (i = x, j = y; i >= 0 && j >= 0; i--, j--)
                if (!(Data.boardA[i * Data.N + j] == 2))
                    break;
            count += (y - j);
            for (i = x, j = y; i < Data.M && j < Data.N; i++, j++)
                if (!(Data.boardA[i * Data.N + j] == 2))
                    break;
            count += (j - y - 1);
            if (count >= 4) return true;

            return false;
        }

        private static bool BWin(int x, int y)
        {
            //横向检测
            int i, j;
            int count = 0;
            for (i = y; i >= 0; i--)
                if (!(Data.boardB[x * Data.N + i] == 2))
                    break;
            count += (y - i);
            for (i = y; i < Data.N; i++)
                if (!(Data.boardB[x * Data.N + i] == 2))
                    break;
            count += (i - y - 1);
            if (count >= 4) return true;

            //纵向检测
            count = 0;
            for (i = x; i < Data.M; i++)
                if (!(Data.boardB[i * Data.N + y] == 2))
                    break;
            count += (i - x);
            if (count >= 4) return true;

            //左下-右上
            count = 0;
            for (i = x, j = y; i < Data.M && j >= 0; i++, j--)
                if (!(Data.boardB[i * Data.N + j] == 2))
                    break;
            count += (y - j);
            for (i = x, j = y; i >= 0 && j < Data.N; i--, j++)
                if (!(Data.boardB[i * Data.N + j] == 2))
                    break;
            count += (j - y - 1);
            if (count >= 4) return true;

            //左上-右下
            count = 0;
            for (i = x, j = y; i >= 0 && j >= 0; i--, j--)
                if (!(Data.boardB[i * Data.N + j] == 2))
                    break;
            count += (y - j);
            for (i = x, j = y; i < Data.M && j < Data.N; i++, j++)
                if (!(Data.boardB[i * Data.N + j] == 2))
                    break;
            count += (j - y - 1);
            if (count >= 4) return true;

            return false;
        }

        private static bool isTieMM()
        {
            bool tie = true;
            for (int i = 0; i < Data.N; i++)
            {
                if (Data.top[i] > 0)
                {
                    tie = false;
                    break;
                }
            }
            return tie;
        }

        private static int determinResultMM(bool isA)
        {
            int ans = -1;

            //平局结束,ans = 0
            //A赢,ans = 1
            //B赢, ans = 2
            //游戏未结束, ans = -1

            if (isA)//上一次是A落子
            {
                if (AWin(Data.lastX, Data.lastY))
                {
                    ans = 1;
                }
                else if (isTieMM())
                {
                    ans = 0;
                }
                else
                {
                    ans = -1;
                }
            }
            else
            {
                if (BWin(Data.lastX, Data.lastY))
                {
                    ans = 2;
                }
                else if (isTieMM())
                {
                    ans = 0;
                }
                else
                {
                    ans = -1;
                }
            }

            return ans;
        }

        public static bool judgeMM(bool isA)
        {
            int ans = determinResultMM(isA);

            if (ans == 0)
            {
                Data.mainFrame.gameOver();
                MessageBox.Show("A and B tied");
                return true;
            }
            else if (ans == 1)
            {
                Data.mainFrame.gameOver();
                MessageBox.Show("A have won!");
                return true;
            }
            else if (ans == 2)
            {
                Data.mainFrame.gameOver();
                MessageBox.Show("B have won!");
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}