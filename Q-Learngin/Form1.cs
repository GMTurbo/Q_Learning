using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Q_Learngin
{
    public partial class Form1 : Form
    {
        TextBoxStreamWriter box = null;

        public Form1()
        {
            InitializeComponent();
            box = new TextBoxStreamWriter(textBox1);
            Console.SetOut(box);

        }
        bool run = false;
        /// <summary>
        /// initiliaze all data to -1
        /// </summary>
        /// <param name="data"></param>
        void InitializeArray(ref double[,] data, int val)
        {
            for (int i = 0; i < data.GetLength(0); i++)
                for (int j = 0; j < data.GetLength(1); j++)
                    data[i, j] = val;
        }

        void RunQLearning()
        {
            //http://people.revoledu.com/kardi/tutorial/ReinforcementLearning/Q-Learning-Example.htm
            //http://mnemstudio.org/path-finding-q-learning-tutorial.htm

            // Connection  Matrix
            double[,] R = new double[6, 6]{

                        {-1, -1, -1, -1, 0, -1},
                        {-1, -1, -1, 0, -1, 100},
                        {-1, -1, -1, 0, -1, -1},
                        {-1, 0, 0, -1, 0, -1},
                        {0, -1, -1, 0, -1, 100},
                        {-1, 0, -1, -1, 0, 100}
            };

            int GoalState = 5;

            double[,] Q = new double[6, 6];

            InitializeArray(ref Q, 0); // initialize to 0

            double gamma = 0.8;

            RunAlgo(R, Q, gamma, GoalState);
        }

        private void RunAlgo(double[,] R, double[,] Q, double gamma, int GoalState)
        {
            #region Comments

            /*
             * Our virtual agent will learn through experience, without a teacher (this is called unsupervised learning).  
             * The agent will explore from state to state until it reaches the goal. We'll call each exploration an episode.  
             * Each episode consists of the agent moving from the initial state to the goal state.  
             * Each time the agent arrives at the goal state, the program goes to the next episode.
             * 
             * The Q-Learning algorithm goes as follows:

                    1. Set the gamma parameter, and environment rewards in matrix R.

                    2. Initialize matrix Q to zero.

                    3. For each episode:

                        Select a random initial state.

                    Do While the goal state hasn't been reached.

                    Select one among all possible actions for the current state.
                    Using this possible action, consider going to the next state.
                    Get maximum Q value for this next state based on all possible actions.
                    Compute: Q(state, action) = R(state, action) + Gamma * Max[Q(next state, all actions)]
                    Set the next state as the current state.
                    End Do

                    End For
             */

            #endregion

            started = true;

            bool Exploration = radioButton1.Checked;

            Console.WriteLine("\n\n{0}", Exploration ? "Exploration Mode" : "Exploitation Mode");
            int initialState = 1;
            bool first = true;
            int RandomState = 0;
            List<int> initialStates = new List<int>();
            List<int> RandomStates = new List<int>();

            for (int i = 0; i < 50; i++)
            {
                while (!run)
                {
                    System.Threading.Thread.Sleep(100);
                    Application.DoEvents();
                }
                

                if (first)
                {
                    initialState = 1;
                    first = false;
                }
                else
                    initialState = GetRandomIntWithin(6);


                RandomState = GetRandomIntWithin(6);
                RandomStates.Clear();
                int count = 0;
                while (count < 20)
                {
                    //UpdateFrame(initialState, ref Q);
                    count++;
                    if (RandomStates.Count == 6)
                        break;
                    if (Exploration)
                        RandomState = RandomlyGetNextPath(ref R, RandomState, ref RandomStates);
                    else
                        RandomState = RandomlyGetBestPath(ref R, RandomState, ref RandomStates);
                    //if (RandomStates.Contains(RandomState))
                    //    continue;

                    Q[initialState, RandomState] = R[initialState, RandomState] == -1 ? 0 : R[initialState, RandomState] + gamma * GetRowMax(ref Q, RandomState);

                    RandomStates.Add(RandomState);

                    Console.WriteLine(String.Format("Move From {0} to {1}", initialState + 1, RandomState + 1));
                    UpdateFrame(initialState, RandomState, ref Q);
                    initialState = RandomState;

                }

                Console.WriteLine();
                Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[0, 0], Q[0, 1], Q[0, 2], Q[0, 3], Q[0, 4], Q[0, 5]));
                Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[1, 0], Q[1, 1], Q[1, 2], Q[1, 3], Q[1, 4], Q[1, 5]));
                Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[2, 0], Q[2, 1], Q[2, 2], Q[2, 3], Q[2, 4], Q[2, 5]));
                Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[3, 0], Q[3, 1], Q[3, 2], Q[3, 3], Q[3, 4], Q[3, 5]));
                Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[4, 0], Q[4, 1], Q[4, 2], Q[4, 3], Q[4, 4], Q[4, 5]));
                Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[5, 0], Q[5, 1], Q[5, 2], Q[5, 3], Q[5, 4], Q[5, 5]));
                Console.WriteLine();

                UpdateFrame(ref Q);
            }

            NormalizeMat(ref Q);

            Console.WriteLine("Final State/Action Table");
            Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[0, 0], Q[0, 1], Q[0, 2], Q[0, 3], Q[0, 4], Q[0, 5]));
            Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[1, 0], Q[1, 1], Q[1, 2], Q[1, 3], Q[1, 4], Q[1, 5]));
            Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[2, 0], Q[2, 1], Q[2, 2], Q[2, 3], Q[2, 4], Q[2, 5]));
            Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[3, 0], Q[3, 1], Q[3, 2], Q[3, 3], Q[3, 4], Q[3, 5]));
            Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[4, 0], Q[4, 1], Q[4, 2], Q[4, 3], Q[4, 4], Q[4, 5]));
            Console.WriteLine(String.Format("{0}, {1}, {2}, {3}, {4}, {5}", Q[5, 0], Q[5, 1], Q[5, 2], Q[5, 3], Q[5, 4], Q[5, 5]));
            Console.WriteLine();

            UpdateFrame(ref Q);

            started = false;

            // Console.WriteLine(GetFinalPath());
        }

        private void UpdateFrame(int oldstate, int newstate, ref double[,] Q)
        {
            //oval1.FillColor = Color.Gray;
            //oval2.FillColor = Color.Gray;
            //oval3.FillColor = Color.Gray;
            //oval4.FillColor = Color.Gray;
            //oval5.FillColor = Color.Gray;
            //oval6.FillColor = Color.Gray;

            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0:
                        oval1.FillColor = Color.Gray;
                        break;
                    case 1:
                        oval2.FillColor = Color.Gray;
                        break;
                    case 2:
                        oval3.FillColor = Color.Gray;
                        break;
                    case 3:
                        oval4.FillColor = Color.Gray;
                        break;
                    case 4:
                        oval5.FillColor = Color.Gray;
                        break;
                    case 5:
                        oval6.FillColor = Color.Gray;
                        break;
                    default:
                        break;
                }
            }

            switch (oldstate)
            {
                case 0:
                    oval1.FillColor = Color.Yellow;
                    oval1.Refresh();
                    break;
                case 1:
                    oval2.FillColor = Color.Yellow;
                    oval2.Refresh();
                    break;
                case 2:
                    oval3.FillColor = Color.Yellow;
                    oval3.Refresh();
                    break;
                case 3:
                    oval4.FillColor = Color.Yellow;
                    oval4.Refresh();
                    break;
                case 4:
                    oval5.FillColor = Color.Yellow;
                    oval5.Refresh();
                    break;
                case 5:
                    oval6.FillColor = Color.Yellow;
                    oval6.Refresh();
                    break;
                default:
                    break;
            }

            switch (newstate)
            {
                case 0:
                    oval1.FillColor = Color.Green;
                    oval1.Refresh();
                    break;
                case 1:
                    oval2.FillColor = Color.Green;
                    oval2.Refresh();
                    break;
                case 2:
                    oval3.FillColor = Color.Green;
                    oval3.Refresh();
                    break;
                case 3:
                    oval4.FillColor = Color.Green;
                    oval4.Refresh();
                    break;
                case 4:
                    oval5.FillColor = Color.Green;
                    oval5.Refresh();
                    break;
                case 5:
                    oval6.FillColor = Color.Green;
                    oval6.Refresh();
                    break;
                default:
                    break;
            }

            line151.BorderColor = Color.Black; //0 -4
            line152.BorderColor = Color.Black;
            line241.BorderColor = Color.Black; // 1 - 3
            line242.BorderColor = Color.Black;
            line261.BorderColor = Color.Black;// 1 - 5
            line262.BorderColor = Color.Black;
            line341.BorderColor = Color.Black;// 2 - 3
            line342.BorderColor = Color.Black;
            line42.BorderColor = Color.Black; // 3 - 1
            line422.BorderColor = Color.Black;
            line431.BorderColor = Color.Black; //3 - 2
            line432.BorderColor = Color.Black;
            line451.BorderColor = Color.Black;// 3 - 4
            line452.BorderColor = Color.Black;
            line511.BorderColor = Color.Black;// 4 - 0
            line512.BorderColor = Color.Black;
            line541.BorderColor = Color.Black;// 4 - 3
            line542.BorderColor = Color.Black;
            line561.BorderColor = Color.Black;// 4 - 5
            line562.BorderColor = Color.Black;
            line621.BorderColor = Color.Black;// 5 - 1
            line622.BorderColor = Color.Black;
            line651.BorderColor = Color.Black;// 5 - 4
            line652.BorderColor = Color.Black;
            line661.BorderColor = Color.Black;// 5- 5
            line662.BorderColor = Color.Black;
            line663.BorderColor = Color.Black;
            line664.BorderColor = Color.Black;
            line665.BorderColor = Color.Black;

            for (int i = 0; i < 6; i++)
                updateLabel(i, GetIndexOfHighestInRow(ref Q, i));

            Application.DoEvents();
            System.Threading.Thread.Sleep(100*timeSlider.Value);
            
        }

        private void updateLabel(int start, int end)
        {
            
            //line151.BorderColor = Color.Black; //0 -4
            //line152.BorderColor = Color.Black;
            //line241.BorderColor = Color.Black; // 1 - 3
            //line242.BorderColor = Color.Black;
            //line261.BorderColor = Color.Black;// 1 - 5
            //line262.BorderColor = Color.Black;
            //line341.BorderColor = Color.Black;// 2 - 3
            //line342.BorderColor = Color.Black;
            //line42.BorderColor = Color.Black; // 3 - 1
            //line422.BorderColor = Color.Black;
            //line431.BorderColor = Color.Black; //3 - 2
            //line432.BorderColor = Color.Black;
            //line451.BorderColor = Color.Black;// 3 - 4
            //line452.BorderColor = Color.Black;
            //line511.BorderColor = Color.Black;// 4 - 0
            //line512.BorderColor = Color.Black;
            //line541.BorderColor = Color.Black;// 4 - 3
            //line542.BorderColor = Color.Black;
            //line561.BorderColor = Color.Black;// 4 - 5
            //line562.BorderColor = Color.Black;
            //line621.BorderColor = Color.Black;// 5 - 1
            //line622.BorderColor = Color.Black;
            //line651.BorderColor = Color.Black;// 5 - 4
            //line652.BorderColor = Color.Black;
            //line661.BorderColor = Color.Black;// 5- 5
            //line662.BorderColor = Color.Black;
            //line663.BorderColor = Color.Black;
            //line664.BorderColor = Color.Black;
            //line665.BorderColor = Color.Black;
             
            switch (start)
            {
                case 0:
                    switch (end)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            line151.BorderColor = Color.Red;
                            line152.BorderColor = Color.Red;
                            break;
                        case 5:
                            break;
                    }
                    break;
                case 1:
                    switch (end)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            line241.BorderColor = Color.Red;
                            line242.BorderColor = Color.Red;
                            break;
                        case 4:
                            break;
                        case 5:
                            line261.BorderColor = Color.Red;
                            line262.BorderColor = Color.Red;
                            break;
                    }
                    break;
                case 2:
                    switch (end)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            line341.BorderColor = Color.Red;
                            line342.BorderColor = Color.Red;
                            break;
                        case 4:
                            break;
                        case 5:
                            break;
                    }
                    break;
                case 3:
                    switch (end)
                    {
                        case 0:
                            break;
                        case 1:
                            line42.BorderColor = Color.Red;
                            line422.BorderColor = Color.Red;
                            break;
                        case 2:
                            line431.BorderColor = Color.Red;
                            line432.BorderColor = Color.Red;
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        case 5:
                            break;
                    }
                    break;
                case 4:
                    switch (end)
                    {
                        case 0:
                            line511.BorderColor = Color.Red;
                            line512.BorderColor = Color.Red;
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            line541.BorderColor = Color.Red;
                            line542.BorderColor = Color.Red;
                            break;
                        case 4:
                            break;
                        case 5:
                            line561.BorderColor = Color.Red;
                            line562.BorderColor = Color.Red;
                            break;
                    }
                    break;
                case 5:
                    switch (end)
                    {
                        case 0:
                            break;
                        case 1:
                            line621.BorderColor = Color.Red;
                            line622.BorderColor = Color.Red;
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            line651.BorderColor = Color.Red;
                            line652.BorderColor = Color.Red;
                            break;
                        case 5:
                            line661.BorderColor = Color.Red;// 5- 5
                            line662.BorderColor = Color.Red;
                            line663.BorderColor = Color.Red;
                            line664.BorderColor = Color.Red;
                            line665.BorderColor = Color.Red;
                            break;
                    }
                    break;
            }
        }
        int GetIndexOfHighestInRow(ref double[,] Q, int row)
        {
            List<double> vals = new List<double>();

            for (int i = 0; i < Q.GetLength(1); i++)
                vals.Add(Q[row, i]);

            return vals.IndexOf(vals.Max());
        }
        private void UpdateFrame(ref double[,] Q)
        {
            double[,] tmp = new double[Q.GetLength(0), Q.GetLength(1)];
            for (int i = 0; i < Q.GetLength(0); i++)
                for (int j = 0; j < Q.GetLength(1); j++)
                    tmp[i, j] = Q[i, j];

            NormalizeMat(ref tmp);

            l15.Text = tmp[0, 4].ToString("#0.0");
            l24.Text = tmp[1, 3].ToString("#0.0");
            l26.Text = tmp[1, 5].ToString("#0.0");
            l34.Text = tmp[2, 3].ToString("#0.0");
            l42.Text = tmp[3, 1].ToString("#0.0");
            l43.Text = tmp[3, 2].ToString("#0.0");
            l45.Text = tmp[3, 4].ToString("#0.0");
            l51.Text = tmp[4, 0].ToString("#0.0");
            l54.Text = tmp[4, 3].ToString("#0.0");
            l56.Text = tmp[4, 5].ToString("#0.0");
            l62.Text = tmp[5, 1].ToString("#0.0");
            l65.Text = tmp[5, 4].ToString("#0.0");
            l66.Text = tmp[5, 5].ToString("#0.0");

            Application.DoEvents();

        }

        private int RandomlyGetBestPath(ref double[,] R, int rowVal, ref List<int> used)
        {
            List<double> vals = new List<double>();

            for (int i = 0; i < R.GetLength(1); i++)
                vals.Add(R[rowVal, i]);

            return vals.IndexOf(vals.Max());
        }

        int RandomlyGetNextPath(ref double[,] R, int rowVal, ref List<int> used)
        {
            int lookup = GetRandomIntWithin(R.GetLength(0));

            while (R[rowVal,lookup] == -1)
                lookup = GetRandomIntWithin(R.GetLength(0));


            return lookup;

        }

        private void NormalizeMat(ref double[,] Q)
        {
            double max = double.MinValue;

            for (int i = 0; i < Q.GetLength(0); i++)
            {
                for (int j = 0; j < Q.GetLength(1); j++)
                {
                    if (max < Q[i, j])
                        max = Q[i, j];
                }
            }
            if (max == 0)
                return;
            for (int i = 0; i < Q.GetLength(0); i++)
                for (int j = 0; j < Q.GetLength(1); j++)
                    Q[i, j] = (int)((Q[i, j] / max) * 100.0);


        }

        private double GetRowMax(ref double[,] Q, int rowVal)
        {
            List<double> vals = new List<double>();

            for (int i = 0; i < Q.GetLength(1); i++)
                vals.Add(Q[rowVal, i]);

            return vals.Max();
        }

        int GetRandomIntWithin(int top)
        {
            Random rand = new Random();
            double percentage = rand.NextDouble();
            return (int)Math.Floor(((double)top * percentage));
            //double per = 
        }
        bool started = false;
        private void button1_Click(object sender, EventArgs e)
        {
            run = !run;
            button1.Text = run ? "Run" : "Pause";
            if (run && !started)
                RunQLearning();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            started = false;
            run = false;
            button1.Text = run ? "Run" : "Pause";
            if (run && !started)
                RunQLearning();
        }
    }

    public class TextBoxStreamWriter : TextWriter
    {
        TextBox _output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            _output.AppendText(value.ToString());

        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
