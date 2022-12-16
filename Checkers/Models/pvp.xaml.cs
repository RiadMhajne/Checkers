using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Funcs.Funcs;

namespace Checkers.Models
{
    /// <summary>
    /// English draughts
    /// Interaction logic for pvp.xaml
    /// </summary>
    public partial class pvp : Window
    {
       
        private Button[,] Board;
        private Boolean white_turn = true;
        private Boolean p_lock = false;
        private Boolean ate = false;
        private string Bplayer = "Black";
        private string Wplayer = "White";
        private int Bpice = 12;
        private int Wpice = 12;
        private List<string> playedMoves = new List<string>();
        private Button prevb;

        public pvp(Double l ,Double t,Double w,Double h,WindowState windowState)
        {
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

        private void Init()
        {

            Board = new Button[n,n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Board[i,j] = new Button();
                    Board[i, j].Background = (i+j)%2==0 ? Brushes.Black : Brushes.White;
                    
                    Grid.SetColumn(Board[i, j], j);
                    Grid.SetRow(Board[i, j], i);
                    
                    if(i<(n /2)-1 && (i+j)%2==0)
                    {

                        Board[i, j].Background = black_p;
                    }else if (i > (n / 2)  && (i + j) % 2 == 0)
                    {

                        Board[i, j].Background = white_p;
                    }
                    Board[i,j].Name = "A_"+i+"_" + j;
                    Board[i, j].Click += (sender, e) =>
                    {
                        
                        Button b = sender as Button;
                        
                        if (b.Background != Brushes.Red)
                        {
                            unmarkall(ref Board);
                        }
                        if(CanMove(ref b, ref Board, white_turn,p_lock,prevb))
                        {
                            marklgl(ref b, ref Board, white_turn, p_lock, prevb);
                            prevb = b;
                        }
                        else if (b.Background == Brushes.Red)
                        {
                            string cbrd = BoardToString(ref Board);
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
                                }else if(px < cx && py > cy)
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
                                playedMoves.Clear();
                                if(Board[x, y].Background == white_k || Board[x,y].Background == white_p)
                                {
                                    Wpice--;
                                    Wcount.Text = "" + Wpice;
                                }
                                else
                                {
                                    Bpice--;
                                    Bcount.Text = "" + Bpice;
                                }
                                Board[x, y].Background = Brushes.Black;
                                ate = true;
                                
                            }
                            if (prevb != null)
                            {
                                b.Background = prevb.Background;
                                prevb.Background = Brushes.Black;
                            }
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
                            string nbrd = BoardToString(ref Board);
                            for (int i = 0; i < n; i++)
                            {
                                for (int j = 0; j < n; j++)
                                {

                                    if (cbrd[(i * 8) + j] != nbrd[(i * 8) + j])
                                    {
                                        Board[i, j].BorderBrush = Brushes.Red;
                                        Board[i, j].BorderThickness = new Thickness(3);
                                    }
                                    else
                                    {
                                        Board[i, j].BorderBrush = Brushes.Black;
                                        Board[i, j].BorderThickness = new Thickness(0);

                                    }
                                }
                            }
                            string lastMove = BoardToString(ref Board);
                            playedMoves.Add(lastMove);
                            if (repetition(playedMoves, lastMove))
                            {
                                pop.Visibility = Visibility.Visible;
                                hide.Visibility = Visibility.Visible;
                                hide.Opacity = 0.50;
                                final.Text = "The game ended in a draw\ndue to repetition";
                            }
                        }
                        else
                        {
                            unmarkall(ref Board);
                        }
                        
                        if (Bpice == 0 || (!PlayerCanMove(ref Board, white_turn, p_lock, prevb) && !white_turn))
                        {
                            pop.Visibility = Visibility.Visible;
                            hide.Visibility = Visibility.Visible;
                            final.Text =  Wplayer + " won \nblack have no moves";
                        }
                        else if (Wpice == 0 || (!PlayerCanMove(ref Board, white_turn, p_lock, prevb) && white_turn))
                        {

                            pop.Visibility = Visibility.Visible;
                            hide.Visibility = Visibility.Visible;
                            final.Text = Bplayer + " won\nwhite have no moves";
                        }
                        else if (Bpice == 1 && Wpice == 1)
                        {
                            pop.Visibility = Visibility.Visible;
                            hide.Visibility = Visibility.Visible;
                            final.Text = "The game ended in a draw\ndou to insufficient material";
                        }
                        whosturn.Text = white_turn ? "White's turn" : "Black's turn";

                    };
                    Board[i, j].BorderThickness = new Thickness(0);
                    board.Children.Add(Board[i, j]);
                    
                }

            }
        }

        private void resb_Click(object sender, RoutedEventArgs e)
        {
            pop.Visibility = Visibility.Visible;
            hide.Visibility = Visibility.Visible;
            final.Text = Wplayer + " won \nblack resigned";
        }

        private void res_Click(object sender, RoutedEventArgs e)
        {
            pop.Visibility = Visibility.Visible;
            hide.Visibility = Visibility.Visible;
            final.Text = Bplayer + " won \nwhite resigned";
        }

        private void draw_Click(object sender, RoutedEventArgs e)
        {
            pop.Visibility=Visibility.Visible;
            hide.Visibility = Visibility.Visible;
            final.Text = "The game ended in a draw";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double l = this.Left;
            double t = this.Top;
            double w = this.Width;
            double h = this.Height;
            var newForm = new Start(l,t,w,h, this.WindowState); //create your new form.
            newForm.Show(); //show the new form.
            this.Close(); //
        }
    }
}
