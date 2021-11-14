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

namespace lab6_2
{
    public partial class Form1 : Form
    {
        int n = 20;
        int steps = 1;
        int seconds = 0;
        bool cancel = false;
        int[,] now1, now2;
        int[,] after1, after2;
        Thread thread1, thread2, thread3;
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        public Form1()
        {
            InitializeComponent();
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView2.CellClick += dataGridView2_CellClick;
            button1.Click += button1_Click;
            now1 = new int[n, n];
            now2 = new int[n, n];
            after1 = new int[n, n];
            after2 = new int[n, n];
            thread1 = new Thread(new ThreadStart(Posled));
            thread2 = new Thread(new ThreadStart(Paral));
            thread3 = new Thread(new ThreadStart(Stop));
            CancellationToken token = cancelTokenSource.Token;

            dataGridView1.ColumnCount = n;
            dataGridView1.RowCount = n;
            dataGridView2.ColumnCount = n;
            dataGridView2.RowCount = n;
            for (int i = 0; i < n; i++)
            {
                dataGridView1.Columns[i].Width = 20;
                dataGridView1.Rows[i].Height = 20;
                dataGridView2.Columns[i].Width = 20;
                dataGridView2.Rows[i].Height = 20;
                for (int j = 0; j < n; j++)
                {
                    now1[i, j] = 0;
                    after1[i, j] = 0;
                    now2[i, j] = 0;
                    after2[i, j] = 0;
                }
            }
        }
        //Действия при нажатии на ячейки DataGridView
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor != Color.LightSkyBlue)
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.LightSkyBlue;
                now1[e.RowIndex, e.ColumnIndex] = 1;
            }
            else
            {
                dataGridView1[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                now1[e.RowIndex, e.ColumnIndex] = 0;
            }
            dataGridView1.ClearSelection();
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2[e.ColumnIndex, e.RowIndex].Style.BackColor != Color.LightSkyBlue)
            {
                dataGridView2[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.LightSkyBlue;
                now2[e.RowIndex, e.ColumnIndex] = 1;
            }
            else
            {
                dataGridView2[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                now2[e.RowIndex, e.ColumnIndex] = 0;
            }
            dataGridView2.ClearSelection();
        }
        //Присвоение массиву After значений клеток на следующем шаге
        void ArrayAfter()
        {

            int left, right, up, down;
            for (int i = 0; i < n; i++)
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
                    neighbors += now1[left, j];
                    neighbors += now1[left, down];
                    neighbors += now1[left, up];
                    neighbors += now1[right, j];
                    neighbors += now1[right, down];
                    neighbors += now1[right, up];
                    neighbors += now1[i, down];
                    neighbors += now1[i, up];
                    if (neighbors == 3 && now1[i, j] == 0)
                        after1[i, j] = 1;
                    else if ((neighbors < 2 || neighbors > 3) && now1[i, j] == 1)
                        after1[i, j] = 0;
                    else
                        after1[i, j] = now1[i, j];
                }
        }
        //Копирование значений массива After в массив Now 
        void CopyToNow()
        {
            for (int i = 0; i < n; i++)
                if (cancel == true)
                    cancelTokenSource.Cancel();
            else
            for (int j = 0; j < n; j++)
                    now1[i, j] = after1[i, j];
        }
        //Закрашивание клеток
        void ShowGrid()
        {
            for (int i = 0; i < n; i++)
                if (cancel == true)
                    cancelTokenSource.Cancel();
            else
            for (int j = 0; j < n; j++)
                {
                    if (now1[i, j] == 1)
                        dataGridView1[j, i].Style.BackColor = Color.LightSkyBlue;
                    else
                        dataGridView1[j, i].Style.BackColor = Color.White;

                }
            dataGridView1.Invoke((MethodInvoker)delegate ()
            {
                dataGridView1.Refresh();
            });
        }
        //Те же методы только при распараллеливании циклов
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
                        neighbors += now2[left, j];
                        neighbors += now2[left, down];
                        neighbors += now2[left, up];
                        neighbors += now2[right, j];
                        neighbors += now2[right, down];
                        neighbors += now2[right, up];
                        neighbors += now2[i, down];
                        neighbors += now2[i, up];
                        if (neighbors == 3 && now2[i, j] == 0)
                            after2[i, j] = 1;
                        else if ((neighbors < 2 || neighbors > 3) && now2[i, j] == 1)
                            after2[i, j] = 0;
                        else
                            after2[i, j] = now2[i, j];
                    }
            });
        }
        void ParallelCopyToNow()
        {
            Parallel.For(0, n, (i) =>
            {
                if (cancel == true)
                    cancelTokenSource.Cancel();
                else
                    for (int j = 0; j < n; j++)
                        now2[i, j] = after2[i, j];
            });
        }
        void ParallelShowGrid()
        {
            Parallel.For(0, n, (i) =>
            {
                if (cancel == true)
                    cancelTokenSource.Cancel();
                else
                    for (int j = 0; j < n; j++)
                    {
                        if (now2[i, j] == 1)
                            dataGridView2[j, i].Style.BackColor = Color.LightSkyBlue;
                        else
                            dataGridView2[j, i].Style.BackColor = Color.White;

                    }
            });
            dataGridView2.Invoke((MethodInvoker)delegate ()
            {
                dataGridView2.Refresh();
            });
        }
        //Последовательный алгоритм
        private void Posled()
        {
            DateTime dt1 = DateTime.Now;
            int k = 0;
            while (k < steps)
            {
                ArrayAfter();
                CopyToNow();
                ShowGrid();
                k++;
            }
            DateTime dt2 = DateTime.Now;
            textBox1.Invoke((MethodInvoker)delegate ()
            {
                textBox1.Text = ((dt2 - dt1).TotalMilliseconds).ToString();
            });
        }
        //Параллельный алгоритм
        private void Paral()
        {
            DateTime dt1 = DateTime.Now;
            int k = 0;
            while (k < steps)
            {
                 ParallelArrayAfter();
                 ParallelCopyToNow();
                 ParallelShowGrid();
                 k++;
            }
            DateTime dt2 = DateTime.Now;
            textBox2.Invoke((MethodInvoker)delegate ()
            {
                textBox2.Text = ((dt2 - dt1).TotalMilliseconds).ToString();
            });
        }
        //Остановка
        private void Stop()
        {
            Thread.Sleep(seconds);
            cancel = true;
        }
        //Нажатие кнопки Пуск
        private void button1_Click(object sender, EventArgs e)
        {
            steps = Convert.ToInt32(textBox4.Text);
            seconds = Convert.ToInt32(textBox3.Text)*1000;
            try
            {
                thread1.Start();
                thread2.Start();
                if(seconds!=0)
                    thread3.Start();
            }
            catch (Exception ex) { }
        }
        //Код при закрытии формы
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}
