#include <iostream>
#include <ctime>				// 使用clock函数计时
#include "Point.h"
#include "Strategy.h"
#include "Judge.h"
#include <conio.h>				// 方便调试打印
#include <atlstr.h>


#define MAX_TIME 2.9
#define MAX_M 12
#define MAX_N 12


using namespace std;

// 调试程序
const bool DEBUG = false;

// 棋盘格点
const int GRID_EMPTY = 0;
const int GRID_YOU = 1;
const int GRID_ME = 2;
const int GRID_NO = 3;

// 最大节点数
const int MAX_NODES = 2000000;

// 定义玩家编号
const int PLAYER_YOU = 1;
const int PLAYER_ME = 2;
const int PLAYER_ADD = 3;

// 定义棋局结束判断的返回代码
const int CODE_TIE = 0;
const int CODE_WIN = 1;
const int CODE_NOT_END = 2;

// 最优子节点公式中的常数, 常数越大越倾向于发现, 越小越倾向于利用
const float FORMULA_C = 1;

// 返回值惩罚
const float PUNISH_QUICK_DEAD = 5;

// 棋盘大小, 定义为全局变量, 避免各种函数中相互传递
int GRID_M = 0;
int GRID_N = 0;

// 先定义一个临时的棋盘,避免每次新开一个棋盘浪费时间
int tmp_top[MAX_N];
int** tmp_board = new int*[MAX_M];

// 定义搜索树结构, 一次性把可能用到的treenode空间全开
int num_node_used = 0;
struct searchTreeNode
{
	searchTreeNode *child[MAX_M];
	searchTreeNode *father;
	int num_visit;
	int num_win;
};
searchTreeNode tree[MAX_NODES];

// 成员函数
searchTreeNode* newNode(searchTreeNode *father);
int isTerminate(int *top, int** board, int x, int y, int current_player);
int isTerminate(int** board, int x, int y, int current_player);
void backUp(searchTreeNode *node, int gain);
int defaultPolicy(int *top, int** board, int last_x, int last_y, int current_player);
searchTreeNode* treePolicy(searchTreeNode *root, int *top, int** board, int &last_x, int &last_y, int &current_player);
int bestChild(searchTreeNode *root, int *feasible, int feasible_count);
searchTreeNode* expand(searchTreeNode *node, int &chosen_child, int *feasible, int feasible_count);
int bestSolution(searchTreeNode* root);
void print_board_top(int *top, int** board);
void print_feasible(int *feasible, int feasible_count);

/*
	策略函数接口,该函数被对抗平台调用,每次传入当前状态,要求输出你的落子点,该落子点必须是一个符合游戏规则的落子点,不然对抗平台会直接认为你的程序有误
	
	input:
		为了防止对对抗平台维护的数据造成更改，所有传入的参数均为const属性
		M, N : 棋盘大小 M - 行数 N - 列数 均从0开始计， 左上角为坐标原点，行用x标记，列用y标记
		top : 当前棋盘每一列列顶的实际位置. e.g. 第i列为空,则_top[i] == M, 第i列已满,则_top[i] == 0
		_board : 棋盘的一维数组表示, 为了方便使用，在该函数刚开始处，我们已经将其转化为了二维数组board
				你只需直接使用board即可，左上角为坐标原点，数组从[0][0]开始计(不是[1][1])
				board[x][y]表示第x行、第y列的点(从0开始计)
				board[x][y] == 0/1/2 分别对应(x,y)处 无落子/有用户的子/有程序的子,不可落子点处的值也为0
		lastX, lastY : 对方上一次落子的位置, 你可能不需要该参数，也可能需要的不仅仅是对方一步的
				落子位置，这时你可以在自己的程序中记录对方连续多步的落子位置，这完全取决于你自己的策略
		noX, noY : 棋盘上的不可落子点(注:其实这里给出的top已经替你处理了不可落子点，也就是说如果某一步
				所落的子的上面恰是不可落子点，那么UI工程中的代码就已经将该列的top值又进行了一次减一操作，
				所以在你的代码中也可以根本不使用noX和noY这两个参数，完全认为top数组就是当前每列的顶部即可,
				当然如果你想使用lastX,lastY参数，有可能就要同时考虑noX和noY了)
		以上参数实际上包含了当前状态(M N _top _board)以及历史信息(lastX lastY),你要做的就是在这些信息下给出尽可能明智的落子点
	output:
		你的落子点Point
*/
extern "C" __declspec(dllexport) Point* getPoint(const int M, const int N, const int* top, const int* _board,
	const int lastX, const int lastY, const int noX, const int noY) {

	// 记录开始时刻
	double tic = (double)clock() / CLOCKS_PER_SEC;
	// 记录中间的时刻
	double toc = 0;
	// 记录每次探索的节点
	searchTreeNode *node;
	// 记录探索节点的落子的位置
	int last_x = 0;
	int last_y = 0;
	// 记录探索节点中上一步的对手
	int player = 0;
	// 记录每一次探索的结果
	int gain = 0;
	// 记录棋盘大小
	GRID_M = M;
	GRID_N = N;
	// 初始化node计数
	num_node_used = 0;

	// for debug
	if (DEBUG) AllocConsole();

	/*
		建立当前棋盘
		board中可能的状态为EMPTY, YOU, ME, NO
	*/
	// 返回的落子点
	int x = -1, y = -1;
	// 新建一个可以棋盘变量, 新建一个临时棋盘变量
	int** board = new int*[M];
	for (int i = 0; i < M; i++) {
		board[i] = new int[N];
		tmp_board[i] = new int[MAX_N];
		for (int j = 0; j < N; j++) {
			tmp_board[i][j] = 0;
			board[i][j] = _board[i * N + j];
			if ((i == noX) && (j == noY))
				board[i][j] = GRID_NO;
		}
	}

	/*
		使用信心上界树算法(UCT)
	*/
	searchTreeNode *root = newNode(NULL);
	// 构建主循环
	int count = 0;
	while (1) {
		count++;
		// 复制棋盘
		for (int i = 0; i < GRID_N; i++) {
			tmp_top[i] = top[i];
			for (int j = 0; j < GRID_M; j++) {
				tmp_board[j][i] = board[j][i];
			}
		}
		// 调试打印
		if ((DEBUG) && (count == 1)) print_board_top(tmp_top, tmp_board);
		// 每次找出目前最好的节点进行扩展, 节点的评价兼顾使用较好的分支和探索次数较小的分支
		node = treePolicy(root, tmp_top, tmp_board, last_x, last_y, player);
		gain = defaultPolicy(tmp_top, tmp_board, last_x, last_y, player);
		backUp(node, gain);
		// 计时,如果时间快到了就不搜索了
		toc = (double)clock() / CLOCKS_PER_SEC;
		if ((toc - tic > MAX_TIME) || (num_node_used > MAX_NODES - 10))
			break;
	}
	y = bestSolution(root);
	x = top[y] - 1;

	if (DEBUG) _cprintf("node exploited = %d  path searched = %d \n", num_node_used, count);
	
	clearArray(M, N, board);
	return new Point(x, y);
}

/*
getPoint函数返回的Point指针是在本dll模块中声明的，为避免产生堆错误，应在外部调用本dll中的
函数来释放空间，而不应该在外部直接delete
*/
extern "C" __declspec(dllexport) void clearPoint(Point* p) {
	delete p;
	return;
}

/*
	打印棋局,调试专用
*/
void print_board_top(int *top, int** board) {
	_cprintf("top = [");
	for (int i = 0; i < GRID_N; i++) {
		_cprintf("%d, ", top[i]);
	}
	_cprintf("] \n board = \n");
	for (int j = 0; j < GRID_M; j++) {
		_cprintf("[");
		for (int i = 0; i < GRID_N; i++) {
			_cprintf("%d, ", board[j][i]);
		}
		_cprintf("]\n");
	}
}

/*
	打印可行解,调试专用
*/
void print_feasible(int *feasible, int feasible_count) {
	_cprintf("Feasible list = [");
	for (int i = 0; i < feasible_count; i++) {
		_cprintf("%d ", feasible[i]);
	}
	_cprintf("]\n");
}

/*
	清除top和board数组
*/
void clearArray(int M, int N, int** board) {
	for(int i = 0; i < M; i++){
		delete[] board[i];
	}
	delete[] board;
}

/*
	找出最后的落子决定
*/
int bestSolution(searchTreeNode* root) {
	float max_val = -FLT_MAX;
	float val;
	int ind = -1;
	if (DEBUG) _cprintf("find sol \n");
	for (int i = 0; i < GRID_N; i++) {
		if (root->child[i]) {
			val = (float)root->child[i]->num_win / (float)root->child[i]->num_visit;
			if (DEBUG)  _cprintf(" [%d, %d, %d, %.3f] \n", i, root->child[i]->num_visit, root->child[i]->num_win, val);
			if (val > max_val) {
				max_val = val;
				ind = i;
			}
		}
	}
	return ind;
}

/*
	初始化搜索节点
*/
searchTreeNode* newNode(searchTreeNode *father) {
	searchTreeNode *node = &tree[++num_node_used];
	node->num_visit = 0;
	node->num_win = 0;
	node->father = father;
	for (int i = 0; i < GRID_N; i++) {
		node->child[i] = NULL;
	}
	return node;
}

/* 
	IsTerminate Function
	给定一个棋局状态, 上一步落子情况, 落子的选手
	判断这个棋局是否结束,返回的值为CODE_WIN, CODE_TIE, CODE_NOT_END
*/
int isTerminate(int *top, int** board, int x, int y, int current_player) {
	if ((current_player == PLAYER_ME) && (machineWin(x, y, GRID_M, GRID_N, board))) {
		return CODE_WIN;
	}
	else if ((current_player == PLAYER_YOU) && (userWin(x, y, GRID_M, GRID_N, board))) {
		return CODE_WIN;
	}
	else {
		for (int i = 0; i < GRID_N; i++) {
			if (top[i] != 0) {
				return CODE_NOT_END;
			}
		}
		return CODE_TIE;
	}
}

/*
	IsTerminate Function 重载
	给定一个棋局状态, 上一步落子情况, 落子的选手
	判断该落子是否能够取胜,返回的值为CODE_WIN, CODE_NOT_END
	用于treePolicy中的判断,由于这里面不需要对于平局进行判断,因此 重载了这个函数用于对于是否获胜的判断
*/
int isTerminate(int** board, int x, int y, int current_player) {
	if ((current_player == PLAYER_ME) && (machineWin(x, y, GRID_M, GRID_N, board))) {
		return CODE_WIN;
	}
	else if ((current_player == PLAYER_YOU) && (userWin(x, y, GRID_M, GRID_N, board))) {
		return CODE_WIN;
	}
	else {
		return CODE_NOT_END;
	}
}

/*
	BackUp Function in UCT
	这里把得到的节点的收益反向更新到各个父节点中,并且把相应的棋盘状态依次恢复到初始状态
*/
void backUp(searchTreeNode *node, int gain) {
	while (node) {
		// 改变相关的统计量
		node->num_visit++;
		node->num_win += gain;
		gain = (gain > 1) ? (1 - gain) : ( (gain < -1) ? (-1 - gain) : (-gain) );
		node = node->father;
	}
}

/*
	DefaultPolicy Function in UCT
	按照随机等概率在行动集上找出行动步骤然后进行行动,达到最后的节点之后返回相应的收益
	这里将会传入棋盘的状态,最后返回的时候棋盘状态和传入的时候一致
*/
int defaultPolicy(int *top, int** board, int last_x, int last_y, int current_player) {
	// _cprintf("enter defaultPolicy\n");
	// _cprintf("defaultPolicy:: enter with (%d, %d) with player %d \n", last_x, last_y, current_player);

	const int init_player = current_player;
	int status = 0;
	int feasible[MAX_N];
	int feasible_count;
	int ind;
	
	int count = 0;
	while (1) {
		count++;
		if (count > 150) { _cprintf("default policy inf loop\n"); }
		status = isTerminate(top, board, last_x, last_y, current_player);
		switch (status)
		{
			case CODE_WIN: {
				if (init_player == current_player) {
					// 如果这个节点上立马获胜,更多奖励
					return (count == 1) ? PUNISH_QUICK_DEAD : 1;
				}
				else {
					// 如果这个节点上立马死亡,更多惩罚
					return (count == 1) ? -PUNISH_QUICK_DEAD : -1;
				}
			}
			case CODE_TIE: {
				return 0;
			}
			case CODE_NOT_END: {
				// 查找所有可行的点
				feasible_count = 0;
				for (int i = 0; i < GRID_N; i++) {
					// 上面有空位置能够着子,并且要么上一个为空,要么上一个为禁着点但是禁着点上方还有空位置
					if ((top[i] > 0) && ((board[top[i] - 1][i] == GRID_EMPTY) || (top[i] > 1))) {
						feasible[feasible_count] = i;
						feasible_count++;
					}
				}

				// 随机找出一个可行的解
				ind = feasible[rand() % feasible_count];
				// 更换落子方
				current_player = PLAYER_ADD - current_player;
				// 落子
				top[ind]--;
				// 如果此点禁着,就落子其上方的一个位置
				if (board[top[ind]][ind] == GRID_NO) {
					top[ind]--;
				}
				// 更新棋盘布局
				board[top[ind]][ind] = current_player;

				// 更新落子点
				last_x = top[ind];
				last_y = ind;
			}
			default:
				break;
		}
	}
}

/*
	TreePolicy Function in UCT
	找出目前探索过的最前沿,给出探索的根节点指针,返回探索的最前沿节点指针
	与此同时top与board也与该指针的状态相对应,为了后续判断方便,还返回了上一步的落子坐标
	返回的current_player为上一步下棋的一方(形成这样的棋局最后一步下棋的一方)
*/
searchTreeNode* treePolicy(searchTreeNode *root, int *top, int** board, int &last_x, int &last_y, int &current_player) {
	// 每一步被挑出来的最亟待探索的节点编号
	int chosen_child = 0;
	current_player = PLAYER_ME;
	int feasible[MAX_N];
	int feasible_count;
	// 模拟落子点x坐标
	int check_x;
	// 最开始的棋局肯定是没有终止的
	int count = 0;
	// 记录是否遇到了一步必胜的情形
	bool win_flag = false;
	while (1) {
		count++;
		if (count > 150) { _cprintf("tree policy inf loop\n"); }
		// 查找所有可行的点
		feasible_count = 0;
		for (int i = 0; i < GRID_N; i++) {
			// 上面有空位置能够着子,并且要么上一个为空,要么上一个为禁着点但是禁着点上方还有空位置
			if ((top[i] > 0) && ((board[top[i] - 1][i] == GRID_EMPTY) || (top[i] > 1))) {
				feasible[feasible_count] = i;
				feasible_count++;
				// 如果该用户在某个位置上面落子之后就直接获胜,可行点集就只有这一个点
				check_x = (board[top[i] - 1][i] == GRID_EMPTY) ? (top[i] - 1) : (top[i] - 2);
				board[check_x][i] = current_player;
				if (isTerminate(board, check_x, i, current_player) == CODE_WIN) {
					feasible[0] = i;
					feasible_count = 1;
					board[check_x][i] = GRID_EMPTY;
					win_flag = true;
					break;
				}
				board[check_x][i] = GRID_EMPTY;
			}
		}

		if (feasible_count > 0) {
			// 在可行点集中找出没有被探索过的
			searchTreeNode *node = expand(root, chosen_child, feasible, feasible_count);
			// 如果返回NULL,同时chosen_child为-1,则表示每个可行节点都被探索过了,因此返回其最亟待探索的节点
			if (!node) {
				// 只要有可行解,那么一定能返回一个子节点
				chosen_child = bestChild(root, feasible, feasible_count);
			}
			// 扩展这个节点
			top[chosen_child]--;
			// 如果此点禁着,就落子其上方的一个位置
			if (board[top[chosen_child]][chosen_child] == GRID_NO) {
				top[chosen_child]--;
			}
			// 落子
			last_x = top[chosen_child];
			last_y = chosen_child;
			board[top[chosen_child]][chosen_child] = current_player;

			if (node) {
				return node;
			}
			else {
				// 接着往下面探索
				root = root->child[chosen_child];
			}
		}
		else {
			// 没有进一步的可扩展节点了,返回这个节点
			return root;
		}

		if (win_flag) {
			return root;
		}

		// 更换下一步落子方
		current_player = PLAYER_ADD - current_player;
	}
}

/*
	BestChild Function in UCT
	找出给定节点上最亟待探索的子节点,需要注意的是,当运行这个函数的时候给定的root节点的各个子节点肯定是都被探索过的
*/
int bestChild(searchTreeNode *root, int *feasible, int feasible_count) {
	float logN = (float)log(root->num_visit);
	float max_val = -FLT_MAX;
	int max_ind = -1;
	float val = 0;
	int j = 0;
	for (int i = 0; i < feasible_count; i++) {
		j = feasible[i];
		val = ((float)root->child[j]->num_win / (float)root->child[j]->num_visit) \
			+ (FORMULA_C * sqrt(2 * logN / (float)root->child[j]->num_visit));
		if (val > max_val) {
			max_val = val;
			max_ind = j;
		}
	}
	return max_ind;
}


/*
	Expand Function in UCT
	扩展某个节点上面还没有选择过的行动, 新建这个节点并且返回此子节点的指针
	同时为了方便也返回究竟选择子节点的编号
*/
searchTreeNode* expand(searchTreeNode *node, int &chosen_child, int *feasible, int feasible_count) {
	for (int i = 0; i < feasible_count; i++) {
		if (!(node->child[feasible[i]])) {
			chosen_child = feasible[i];
			node->child[chosen_child] = newNode(node);
			return node->child[chosen_child];
		}
	}
	chosen_child = -1;
	return NULL;
}
