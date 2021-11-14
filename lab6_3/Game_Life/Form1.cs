using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_Life
{
    public partial class Form1 : Form
    {
        int n = 20;
        int[,] now;
        int[,] after;
        DateTime dt1, dt2;
        bool cancel = false;
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public Form1()
        {
            InitializeComponent();
            dataGridView1.CellClick += dataGridView1_CellClick;
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            timer1.Tick += timer1_Tick;
            now = new int[n, n];
            after = new int[n, n];
            CancellationToken token = cancelTokenSource.Token;

            dataGridView1.ColumnCount = n;
            dataGridView1.RowCount = n;
            for (int i = 0; i < n; i++)
            {
                dataGridView1.Columns[i].Width = 20;
                dataGridView1.Rows[i].Height = 20;
                for (int j = 0; j < n; j++)
                {
                    now[i, j] = 0;
                    after[i, j] = 0;
                }
            }
        }
        //Действия при нажатии на ячейки DataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor != Color.LightSkyBlue)
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.LightSkyBlue;
                now[e.RowIndex, e.ColumnIndex] = 1;
            }
            else
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                now[e.RowIndex, e.ColumnIndex] = 0;
            }
            dataGridView1.ClearSelection();
        }
        //Присвоение массиву After значений клеток на следующем шаге
        void ParallelArrayAfter()
        {
            int left, right, up, down;
            Parallel.For(0, n, (i) =>
            {
                if (cancel == true)
                    cancelTokenSource.Cancel();
                else
                    for (int j = 0; j < n; j++)
                    {
                        if (i != 0)
                            left = i - 1;
                        else
                            left = n - 1;

                        if (i != n - 1)
                            right = i + 1;
                        else
                            right = 0;

                        if (j != 0)
                            up = j - 1;
                        else
                            up = n - 1;
                        if (j != n - 1)
                            down = j + 1;
                        else
                            down = 0;
                        int neighbors = 0;
                        neighbors += now[left, j];
                        neighbors += now[left, down];
                        neighbors += now[left, up];
                        neighbors += now[right, j];
                        neighbors += now[right, down];
                        neighbors += now[right, up];
                        neighbors += now[i, down];
                        neighbors += now[i, up];
                        if (neighbors == 3 && now[i, j] == 0)
                            after[i, j] = 1;
                        else if ((neighbors < 2 || neighbors > 3) && now[i, j] == 1)
                            after[i, j] = 0;
                        else
                            after[i, j] = now[i, j];
                    }
            });
        }
        //Копирование значений массива After в массив Now 
        void ParallelCopyToNow()
        {
            Parallel.For(0, n, (i) =>
            {
                if (cancel == true)
                    cancelTokenSource.Cancel();
                else
                    for (int j = 0; j < n; j++)
                    now[i, j] = after[i, j];
            });
        }
        //Закрашивание клеток
        void ParallelShowGrid()
        {
            Parallel.For(0, n, (i) =>
            {
                if (cancel == true)
                    cancelTokenSource.Cancel();
                else
                    for (int j = 0; j < n; j++)
                    {
                    if (now[i, j] == 1)
                        dataGridView1[j, i].Style.BackColor = Color.LightSkyBlue;
                    else
                        dataGridView1[j, i].Style.BackColor = Color.White;

                    }
            });
            dataGridView1.Refresh();
        }
        //Нажатие кнопки Старт
        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            dt1 = DateTime.Now;
        }
        //Нажатие кнопки Стоп
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            dt2 = DateTime.Now;
            textBox1.Text = ((dt2 - dt1).TotalMilliseconds).ToString();
        }
        //Нажатие кнопки Завершить
        private void button3_Click(object sender, EventArgs e)
        {
            cancel = true;
            dt2 = DateTime.Now;
            textBox1.Text = ((dt2 - dt1).TotalMilliseconds).ToString();
        }
        //Действия timer1
        private void timer1_Tick(object sender, EventArgs e)
        {
            ParallelArrayAfter();
            ParallelCopyToNow();
            ParallelShowGrid();
        }
        //Код при закрытии формы
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}