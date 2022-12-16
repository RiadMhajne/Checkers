using Minimax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static Funcs.Funcs;

namespace Checkers.Models
{
    /// <summary>
    /// English draughts
    /// Interaction logic for pvp.xaml
    /// </summary>
    public partial class Computer : Window
    {
        private Button[,] Board;
        private Boolean white_turn = true;
        private Boolean p_lock = false;
        private Boolean ate = false;
        private string Bplayer =  "Copmuter";
        private string Wplayer =  "White";
        private int Bpice = 12;
        private int Wpice = 12;
        MiniMax m ;
        private List<string> playedMoves = new List<string>();
        private Button prevb;

        public Computer(double l,double t , double w,double h, WindowState windowState ,int depth = 2)
        {
            m = new MiniMax(depth);
            InitializeComponent();
            Init();
            this.Loaded += new RoutedEventHandler(
      delegate (object sender, RoutedEventArgs args)
      {
          Left = l;
          Top = t;
          Width = w;
          Height = h;
          WindowState = windowState;
      });
        }

        void BuildBoard(string st)
        {
            Bpice = 0;
            Wpice = 0;
            if (st.Length<8*8)
            {

            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (st[(i * n) + j] == 'w')
                        {
                            Board[i, j].Background = white_p;
                            Wpice++;
                        }
                        else if (st[(i * n) + j] == 'W')
                        {
                            Board[i, j].Background = white_k;
                            Wpice++;
                        }
                        else if (st[(i * n) + j] == 'b')
                        {
                            Board[i, j].Background = black_p;
                            Bpice++;
                        }
                        else if (st[(i * n) + j] == 'B')
                        {
                            Board[i, j].Background = black_k;
                            Bpice++;
                        }
                        else if (st[(i * n) + j] == 'e')
                        {
                            Board[i, j].Background = Brushes.White;
                        }
                        else if (st[(i * n) + j] == 'E')
                        {
                            Board[i, j].Background = Brushes.Black;
                        }
                    }
                }
                Bcount.Text = "" + Bpice;
                Wcount.Text = "" + Wpice;
                if (Bpice == 0 || (!PlayerCanMove( ref Board, white_turn, p_lock, prevb) && !white_turn))
                {


                    pop.Visibility = Visibility.Visible;
                    hide.Visibility = Visibility.Visible;
                    hide.Opacity = 0.50;
                    final.Text = "White won !!!\nBlack have no moves";
                }
                else if (Wpice == 0 || (!PlayerCanMove( ref Board, white_turn, p_lock, prevb) && white_turn))
                {

                    pop.Visibility = Visibility.Visible;
                    hide.Visibility = Visibility.Visible;
                    hide.Opacity = 0.50;
                    final.Text = "Black won !!!\nWhite have no moves";
                }
                else if (Bpice == 1 && Wpice == 1)
                {

                    pop.Visibility = Visibility.Visible;
                    hide.Visibility = Visibility.Visible;
                    hide.Opacity = 0.50;
                    final.Text = "The game ended in a draw\ninsufficient material";
                }
            }
        }

        private void Init()
        {
            Board = new Button[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Board[i, j] = new Button();
                    Board[i, j].Background = (i + j) % 2 == 0 ? Brushes.Black : Brushes.White;

                    Grid.SetColumn(Board[i, j], j);
                    Grid.SetRow(Board[i, j], i);

                    if (i < (n / 2) - 1 && (i + j) % 2 == 0)
                    {

                        Board[i, j].Background = black_p;
                    }
                    else if (i > (n / 2) && (i + j) % 2 == 0)
                    {

                        Board[i, j].Background = white_p;
                    }
                    Board[i, j].Name = "A_" + i + "_" + j;
                    
                    Board[i, j].Click += async (sender, e) =>
                    {
                        
                        Button b = sender as Button;
                        
                       
                        if (b.Background != Brushes.Red)
                        {
                            unmarkall( ref Board);
                        }
                        if (CanMove(ref b, ref Board, white_turn, p_lock, prevb) && white_turn)
                        {
                            marklgl(ref b, ref Board, white_turn, p_lock, prevb);
                            prevb = b;
                        }
                        else if (b.Background == Brushes.Red && white_turn)
                        {
                           
                            b.Background = Brushes.Black;
                            int cx = Convert.ToInt32(b.Name.Split('_')[1]);
                            int cy = Convert.ToInt32(b.Name.Split('_')[2]);
                            if (piceCanSkip(ref prevb, ref Board, white_turn, p_lock, prevb))
                            {

                                int px = Convert.ToInt32(prevb.Name.Split('_')[1]);
                                int py = Convert.ToInt32(prevb.Name.Split('_')[2]);
                                int x, y;
                                if (px > cx && py > cy)
                                {
                                    x = cx + 1;
                                    y = cy + 1;
                                }
                                else if (px < cx && py > cy)
                                {
                                    x = cx - 1;
                                    y = cy + 1;
                                }
                                else if (px < cx && py < cy)
                                {
                                    x = cx - 1;
                                    y = cy - 1;
                                }
                                else
                                {
                                    x = cx + 1;
                                    y = cy - 1;
                                }
                                Bpice--;
                                Bcount.Text = "" + Bpice;
                                Board[x, y].Background = Brushes.Black;
                                ate = true;
                                playedMoves.Clear();
                            }
                            
                            b.Background = prevb.Background;
                            prevb.Background = Brushes.Black;
                            if (cx == 7 && !white_turn)
                            {
                                b.Background = black_k;
                            }
                            if (cx == 0 && white_turn)
                            {
                                b.Background = white_k;
                            }
                            unmarkall(ref Board);
                            p_lock = false;
                            if (piceCanSkip(ref b, ref Board, white_turn, p_lock, prevb) && ate)
                            {
                                p_lock = true;
                                white_turn = white_turn == false;
                            }
                            else
                            {
                                p_lock = false;
                            }
                            white_turn = white_turn == false;
                            prevb = b;
                            ate = false;
                            
                        }
                        else if(!white_turn)
                        {
                            string brd = BoardToString(ref Board);
                            string nbrd = m.GetNextMove(brd);
                            BuildBoard(nbrd);
                            for (int i = 0; i < brd.Length; i++)
                            {
                                decimal ii = (i / 8);
                                int x = (int)Math.Floor(ii);
                                int y = i % 8;
                                if (brd[i] != nbrd[i])
                                {

                                    Board[x, y].BorderThickness = new Thickness(3);
                                    Board[x, y].BorderBrush = Brushes.Red;
                                }
                                else
                                {
                                    Board[x, y].BorderThickness = new Thickness(0);
                                    Board[x, y].BorderBrush = Brushes.Black;
                                }
                            }
                            if (nbrd[nbrd.Length - 1] == 'f')
                            {
                                white_turn = !white_turn;
                            }
                            int tempb = Bpice;
                            int tempw = Wpice;
                            Wpice = 0;
                            Bpice = 0;
                            foreach(char c in nbrd)
                            {
                                if(c=='w' || c == 'W')
                                {
                                    Wpice++;
                                }
                                else if(c == 'b' || c == 'B')
                                {
                                    Bpice++;
                                }
                            }
                            if(Wpice!=tempw || tempb != Bpice)
                            {
                                playedMoves.Clear();
                            }


                        }
                        else
                        {
                            unmarkall(ref Board);
                        }
                        if (Bpice == 0 || (!PlayerCanMove( ref Board, white_turn, p_lock, prevb) && !white_turn))
                        {
                            pop.Visibility = Visibility.Visible;
                            hide.Visibility = Visibility.Visible;
                            final.Text =  Wplayer + " won \nblack have no moves";
                        }
                        else if (Wpice == 0 || (!PlayerCanMove( ref Board, white_turn, p_lock, prevb) && white_turn))
                        {
                            pop.Visibility = Visibility.Visible;
                            hide.Visibility = Visibility.Visible;
                            final.Text = Bplayer + " won \nwhite have no moves";
                        }
                        else if (Bpice == 1 && Wpice == 1)
                        {
                            pop.Visibility = Visibility.Visible;
                            hide.Visibility = Visibility.Visible;
                            final.Text = "The game ended in a draw\n dou to insufficient material";
                        }
                        else
                        {
                            string lastMove = BoardToString(ref Board);
                            if (repetition(playedMoves, lastMove))
                            {
                                pop.Visibility = Visibility.Visible;
                                hide.Visibility = Visibility.Visible;
                                hide.Opacity = 0.50;
                                final.Text = "The game ended in a draw\ndue to repetition";
                            }
                        }
                        whosturn.Text = white_turn ? "White's turn" : "Black's turn";
                        Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ContextIdle, null);
                    };
                    Board[i, j].Click += (s, e) =>
                    {
                        if (!white_turn && Bpice >0 && Wpice >0)
                        {
                            Board[0, 1].RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                        }
                    };
                    Board[i, j].BorderThickness = new Thickness(0);
                    board.Children.Add(Board[i, j]);
                }
            }
        }

        private void res_Click(object sender, RoutedEventArgs e)
        {
            pop.Visibility = Visibility.Visible;
            hide.Visibility = Visibility.Visible;
            final.Text =Bplayer + " won";

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double l = this.Left;
            double t = this.Top;
            double w = this.Width;
            double h = this.Height;
            var newForm = new Start(l,t,w,h,this.WindowState); //create your new form.
            newForm.Show(); //show the new form.
            this.Close(); //
        }
    }
}
