using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace ShuDu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region 定义变量
        List<int>[,] sd = new List<int>[9, 9];  // 用来存储原始数据
        string _temp;                           // 错误输入值后，用来还原原来的值
        int _sumcount = 0;
        bool _getResult = false;
        #endregion

        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalculate_Click(object sender, EventArgs e)
        {
            Init();
            AddColour();
            do Cal(sd);
            while (!CheckEnd(sd)); // 直到count数不再改变
            if (HasCount0(sd))
            {
                MessageBox.Show(@"该数独木有解~~");
            }
            else
            {
                Jisuan(sd, 0, 0);
                if (!_getResult)
                {
                    MessageBox.Show(@"该数独木有解~~");
                }
            }
        }

        /// <summary>
        /// 使所有值都添加1到9
        /// </summary>
        private void Bind()
        {
            for (var r = 0; r < 9; r++)
            {
                for (var c = 0; c < 9; c++)
                {
                    sd[r, c] = new List<int>();
                    for (var v = 1; v < 10; v++)
                        sd[r, c].Add(v);
                }
            }
        }

        /// <summary>
        /// 给数独赋初值
        /// </summary>
        private void Init()
        {
            for (var r = 0; r < dataGridView1.Rows.Count; r++)
            {
                for (var c = 0; c < 9; c++)
                {
                    if (dataGridView1.Rows[r].Cells[c].Value != null)
                    {
                        sd[r, c].Clear();
                        sd[r, c].Add(int.Parse(dataGridView1.Rows[r].Cells[c].Value.ToString()));
                    }
                }
            }
        }


        /// <summary>
        /// 计算
        /// </summary>
        /// <param name="l"></param>
        private void Cal(List<int>[,] l)
        {
            for (var r = 0; r < 9; r++)
            {
                for (var c = 0; c < 9; c++)
                {
                    //if (L[r, c].Count != 1)//等于1时不需要进行排除//异常状况下无法进行
                    CalRow(l, r, c);
                }
            }
        }

        /// <summary>
        /// 简单的一轮推算
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="c"></param>
        private void CalRow(List<int>[,] l, int r, int c)
        {
            #region 排除行和列
            for (var i = 0; i < 9; i++)
            {
                if (l[r, i].Count == 1)//排除行重复
                {
                    if (c != i)//不对自己本身排除
                        if (l[r, c].Contains(l[r, i][0]))
                            l[r, c].Remove(l[r, i][0]);
                }
                if (l[i, c].Count == 1)//排除列重复
                {
                    if (r != i)
                        if (l[r, c].Contains(l[i, c][0]))
                            l[r, c].Remove(l[i, c][0]);
                }
            }
            #endregion

            #region 排除九宫格
            int startRow = 0;
            int endRow = 3;

            if (r >= 0 && r < 3)
            {
                startRow = 0;
                endRow = 3;
            }
            else if (r > 2 && r < 6)
            {
                startRow = 3;
                endRow = 6;
            }
            else if (r > 5 && r < 9)
            {
                startRow = 6;
                endRow = 9;
            }

            for (var i = startRow; i < endRow; i++)
            {
                CalColum(l, r, c, i);
            }
            #endregion
        }

        /// <summary>
        /// 确定九宫中的列的位置
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <param name="i"></param>
        private void CalColum(List<int>[,] l, int r, int c, int i)
        {
            int startColumn = 0;
            int endColumn = 0;
            if (c >= 0 && c < 3)
            {
                startColumn = 0;
                endColumn = 3;
            }
            else if (c > 2 && c < 6)
            {
                startColumn = 3;
                endColumn = 6;
            }
            else if (c > 5 && c < 9)
            {
                startColumn = 6;
                endColumn = 9;
            }
            for (int j = startColumn; j < endColumn; j++)
            {
                CalCell(l, r, c, i, j);
            }
        }

        /// <summary>
        /// 九宫排除
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <param name="i"></param>
        /// <param name="j"></param>
        private void CalCell(List<int>[,] l, int r, int c, int i, int j)
        {
            if (l[i, j].Count == 1)
                if (i != r && j != c)
                {
                    l[r, c].Contains(l[i, j][0]);
                    l[r, c].Remove(l[i, j][0]);
                }
        }

        /// <summary>
        /// 推算结果是否结束 待确定
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private bool CheckEnd(List<int>[,] l)
        {
            // bool IsEnd = true;
            int counts = 0;
            for (var r = 0; r < 9; r++)
            {
                for (var c = 0; c < 9; c++)
                {
                    counts += l[r, c].Count;
                }
            }

            if (counts != _sumcount)
            {
                _sumcount = counts;
                return false;
            }
            _sumcount = 0;
            return true;
        }

        /// <summary>
        /// 将结果输出到页面上
        /// </summary>
        /// <param name="l"></param>
        private void Printout(List<int>[,] l)
        {
            for (var r = 0; r < 9; r++)
            {
                for (var c = 0; c < 9; c++)
                {
                    textBox1.AppendText(l[r, c][0].ToString());
                    dataGridView1.Rows[r].Cells[c].Value = (l[r, c][0]);
                }
                textBox1.AppendText("\r\n");
            }
        }

        /// <summary>
        /// 将计算步骤写入文件，暂时不用了，可删
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <param name="i"></param>
        private void FileWrite(int r, int c, int i)
        {
            string path = "c://1.txt";
            FileStream fs = new FileStream(path, FileMode.Append);
            Byte[] wByte = Encoding.Default.GetBytes("行：" + r.ToString() + "列：" + c.ToString() + "序号：" + i.ToString() + "\r\n");
            fs.Write(wByte, 0, wByte.Length);
            fs.Close();
        }

        /// <summary>
        /// 将第一个list拷贝到第二个list中
        /// </summary>
        /// <param name="originList"></param>
        /// <param name="objectList"></param>
        private void CopyList(List<int>[,] originList, List<int>[,] objectList)
        {
            for (var r = 0; r < 9; r++)
            {
                for (var c = 0; c < 9; c++)
                {
                    objectList[r, c] = new List<int>(originList[r, c].ToArray<int>());
                }
            }
        }

        /// <summary>
        /// 递归计算
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <param name="c"></param>
        private void Jisuan(List<int>[,] l, int r, int c)
        {
            List<int>[,] tempList = new List<int>[9, 9];
            CopyList(l, tempList); // 这里重写copy方法的话更易于理解一些

            if (tempList[r, c].Count != 1)
            {
                List<int>[,] temptempList = new List<int>[9, 9];
                CopyList(tempList, temptempList);
                for (var i = 0; i < tempList[r, c].Count && !_getResult; i++)
                {
                    temptempList[r, c].Clear();
                    temptempList[r, c].Add(tempList[r, c][i]);

                    List<int>[,] temptemptempList = new List<int>[9, 9];
                    CopyList(temptempList, temptemptempList);
                    do Cal(temptemptempList);
                    while (!CheckEnd(temptemptempList));  // 直到count数不再改变
                    if (!HasCount0(temptemptempList))     // 不含有count为0的情况
                    {
                        // 获得nextr和nextc
                        int nextr = 0; int nextc = 0;
                        GetNextRc(r, c, ref nextr, ref nextc);

                        if (nextr == 9 && nextc == 0) // 到了最后一个节点
                        {
                            Printout(temptemptempList);
                            _getResult = true;
                            return;
                        }
                        else
                            Jisuan(temptemptempList, nextr, nextc);
                    }
                }
            }
            else//tempList[r, c].Count == 1的情况
            {
                // 获得nextr和nextc
                int nextr = 0; int nextc = 0;
                GetNextRc(r, c, ref nextr, ref nextc);

                if (nextr == 9 && nextc == 0) // 到了最后一个节点
                {
                    Printout(tempList);
                    _getResult = true;
                    return;
                }
                else
                    Jisuan(tempList, nextr, nextc);
            }
        }

        /// <summary>
        /// 获得递增的r，c的值
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        /// <param name="nextr"></param>
        /// <param name="nextc"></param>
        private void GetNextRc(int r, int c, ref int nextr, ref int nextc)
        {
            // if (nextr <= 0) throw new ArgumentOutOfRangeException(nameof(nextr));
            // if (nextc <= 0) throw new ArgumentOutOfRangeException(nameof(nextc));

            if (c == 8)
            {
                nextc = 0; nextr = r + 1;
            }
            else
            {
                nextc = c + 1;
                nextr = r;
            }
        }

        /// <summary>
        /// 判断count是否为0
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        private bool HasCount0(List<int>[,] l)
        {
            for (var r = 0; r < 9; r++)
            {
                for (var c = 0; c < 9; c++)
                {
                    if (l[r, c].Count == 0)
                        return true;
                }
            }
            return false;
        }

        #region 事件
        /// <summary>
        /// 加载程序后初始化数独表格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            for (var i = 0; i < 9; i++)
            {
                dataGridView1.Rows.Add();
            }
            Bind();
        }

        /// <summary>
        /// 接下来两个事件保证输入值合法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Regex reg = new Regex("[1-9]");
            DataGridView dg = (DataGridView)sender;
            if (dg.CurrentCell.Value != null)
            {
                if (!reg.IsMatch(dg.CurrentCell.Value.ToString()))
                {
                    MessageBox.Show(@"请输入1~9");
                    dg.CurrentCell.Value = _temp;
                }
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            DataGridView dg = (DataGridView)sender;
            if (dg.CurrentCell.Value != null)
            {
                _temp = dg.CurrentCell.Value.ToString();
            }
            else
            {
                _temp = "";
            }
        }

        /// <summary>
        /// 填充最难数独
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInit_Click(object sender, EventArgs e)
        {
            Clear();

            #region 初始化各值
            sd[0, 0].Clear();
            sd[1, 2].Clear();
            sd[1, 3].Clear();
            sd[2, 1].Clear();
            sd[2, 4].Clear();
            sd[2, 6].Clear();
            sd[3, 1].Clear();
            sd[3, 5].Clear();
            sd[4, 4].Clear();
            sd[4, 5].Clear();
            sd[4, 6].Clear();
            sd[5, 3].Clear();
            sd[5, 7].Clear();
            sd[6, 2].Clear();
            sd[6, 7].Clear();
            sd[6, 8].Clear();
            sd[7, 2].Clear();
            sd[7, 3].Clear();
            sd[7, 7].Clear();
            sd[8, 1].Clear();
            sd[8, 6].Clear();

            sd[0, 0].Add(8);
            sd[1, 2].Add(3);
            sd[1, 3].Add(6);
            sd[2, 1].Add(7);
            sd[2, 4].Add(9);
            sd[2, 6].Add(2);
            sd[3, 1].Add(5);
            sd[3, 5].Add(7);
            sd[4, 4].Add(4);
            sd[4, 5].Add(5);
            sd[4, 6].Add(7);
            sd[5, 3].Add(1);
            sd[5, 7].Add(3);
            sd[6, 2].Add(1);
            sd[6, 7].Add(6);
            sd[6, 8].Add(8);
            sd[7, 2].Add(8);
            sd[7, 3].Add(5);
            sd[7, 7].Add(1);
            sd[8, 1].Add(9);
            sd[8, 6].Add(4);
            #endregion

            #region 填充颜色
            AddColour();
            #endregion

        }

        /// <summary>
        /// 清空表中的值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            dataGridView1.Rows.Clear();
            for (var i = 0; i < 9; i++)
            {
                dataGridView1.Rows.Add();
            }
            _getResult = false;
            for (var r = 0; r < 9; r++)
            {
                for (var c = 0; c < 9; c++)
                {
                    sd[r, c].Clear();
                }
            }
            Bind();
        }
        #endregion

        /// <summary>
        /// 清空表的方法
        /// </summary>
        private void Clear()
        {
            textBox1.Clear();
            dataGridView1.Rows.Clear();
            for (var i = 0; i < 9; i++)
            {
                dataGridView1.Rows.Add();
            }
            _getResult = false;
            for (var r = 0; r < 9; r++)
            {
                for (var c = 0; c < 9; c++)
                {
                    sd[r, c].Clear();
                }
            }
            Bind();
        }

        /// <summary>
        /// 为表中已填充的数字添加颜色
        /// </summary>
        private void AddColour()
        {
            for (var r = 0; r < 9; r++)
            {
                for (var c = 0; c < 9; c++)
                {
                    if (sd[r, c].Count == 1)
                    {
                        dataGridView1.Rows[r].Cells[c].Value = (sd[r, c][0]);
                        var style = new DataGridViewCellStyle {BackColor = Color.YellowGreen};
                        dataGridView1.Rows[r].Cells[c].Style = style;
                    }
                }
            }
        }


    }
}
