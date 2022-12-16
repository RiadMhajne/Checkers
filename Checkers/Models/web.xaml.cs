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
using System.Net.Sockets;
using System.ComponentModel;
using System.Net;
using System.IO;
using static Funcs.Funcs;
using System.Runtime.CompilerServices;
using System.Collections;

namespace Checkers.Models
{
    public partial class web : Window
    {
        private Socket s;
        private BackgroundWorker mr = new BackgroundWorker();
        private TcpListener server = null;
        private TcpClient clint;
        private bool myturn;
        private string ip;
        private Button[,] Board;
        private Boolean white_turn = true;
        private Boolean p_lock = false;
        private Boolean ate = false;
        private string Bplayer = "Black";
        private string Wplayer = "White";
        private int Bpice = 12;
        private int Wpice = 12;
        private Button prevb;
        private bool ishost;
        private List<string> playedMoves =new List<string>();
        public web(bool turn , Double l, Double t, Double w, Double h, WindowState windowState, string ip = null)
        {
            ishost = turn;
            myturn = turn;
            this.ip = ip;
            InitializeComponent();
            Title += ishost ? " host" : " clint";
            mr.DoWork += Mr_DoWork;
           
            if (ishost)
            {
                server = new TcpListener(System.Net.IPAddress.Any, 5734);
                server.Start();
                s = server.AcceptSocket();
            }
            else
            {
                try
                {
                    clint = new TcpClient(ip, 5734);
                    s = clint.Client;
                    mr.RunWorkerAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Close();
                }
                white_turn = false;
            }
            this.Loaded += new RoutedEventHandler(
            delegate (object sender, RoutedEventArgs args)
            {
                Left = l;
                Top = t;
                Width = w;
                Height = h;
                WindowState = windowState;
                
            });
            Init();
            Closed += new EventHandler((se, e) =>
            {
                try
                {
                    res_Click(null, null);
                }
                catch { }
            });


        }

        bool SocketConnected()
        {
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }

        private string ReceiveMove()
        {
            var buffer = new List<char>();


            var currByte = new Byte[(n * n) +1];
            var byteCounter = s.Receive(currByte, (n * n) +1, SocketFlags.None);

            for (int i = 0; i < (n * n)+1; i++)
            {
                buffer.Add(Convert.ToChar(currByte[i]));
            }
            return new string(buffer.ToArray());
        }
       
        void web_Closing(object sender, CancelEventArgs e)
        {
            mr.WorkerSupportsCancellation = true;
            mr.CancelAsync();
            if (server != null)
            {
                server.Stop();
            }

            // If data is dirty, notify user and ask for a response

        }
        
        byte[] StrToByte(string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            return bytes;
        }
        
        string ByteToStr(byte[] b)
        {
            string str = Encoding.Default.GetString(b);
            return str;
        }
        
        void BuildBoard(string st)
        {
            int tempb = Bpice;
            int tempw = Wpice;
            Bpice = 0;
            Wpice = 0;
            if (st.Contains("Draw"))
            {

            }
            else if (st.Contains("res"))
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
                            Board[i,j].Background = white_p;
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
                if(tempb != Bpice || tempw != Wpice)
                {
                    playedMoves.Clear();
                }
            }
        }
        
        private void Mr_DoWork(object? sender, DoWorkEventArgs e)
        {
            string st = " ";
            this.Dispatcher.Invoke(() =>
            {
                hide.Visibility = Visibility.Visible;
            });
            do
            {
                st = ReceiveMove();
                if (st.Contains("res"))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        hide.Visibility = Visibility.Visible;
                        hide.Opacity = 0.50;
                        pop.Visibility = Visibility.Visible;
                        s.Close();
                        if (ishost)
                        {
                            server.Stop();

                        }
                        else
                        {
                            clint.Close();
                        }
                        final.Text = "You won!!! \n opponent resigned";

                    });
                }
                else if (st.Contains("draw"))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        hide.Visibility = Visibility.Visible;
                        hide.Opacity = 0.50;
                        drawpop.Visibility = Visibility.Visible;
                        accbut.Visibility = Visibility.Visible;
                        decbut.Visibility = Visibility.Visible;
                        dfinal.Text = "Your opponent offered you a draw";
                    });
                }
                else if (st.Contains("acce"))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        drawpop.Visibility = Visibility.Collapsed;
                        pop.Visibility = Visibility.Visible;
                        final.Text = "The game ended with a draw";
                        s.Close();
                        if (ishost)
                        {
                            server.Stop();
                        }
                        else
                        {
                            clint.Close();
                        }
                    });
                }
                else if (st.Contains("decl"))
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        accbut.Visibility = Visibility.Visible;
                        decbut.Visibility = Visibility.Visible;
                        drawpop.Visibility = Visibility.Collapsed;
                        hide.Visibility = Visibility.Collapsed;
                        hide.Opacity = 0.0;
                    });
                }
                else {
                    this.Dispatcher.Invoke(() =>
                    {
                        string cbrd = BoardToString(ref Board);
                        BuildBoard(st);
                        playedMoves.Add(st);
                        for(int i = 0; i < cbrd.Length; i++)
                        {
                            decimal ii = (i / 8);
                            int x = (int)Math.Floor(ii);
                            int y = i % 8;
                            if (cbrd[i] != st[i] )
                            {
                                
                                Board[x,y].BorderThickness = new Thickness(3);
                                Board[x,y].BorderBrush = Brushes.Red;
                            }
                            else
                            {
                                Board[x, y].BorderThickness = new Thickness(0);
                                Board[x, y].BorderBrush = Brushes.Black;
                            }
                        }
                        if (st[st.Length - 1] != 'c' && pop.Visibility == Visibility.Collapsed)
                        {
                            hide.Visibility = Visibility.Collapsed;
                            whosturn.Text = ishost ? "White's turn" : "Black's turn";

                        }
                        if (repetition(playedMoves, st))
                        {
                            pop.Visibility = Visibility.Visible;
                            hide.Visibility = Visibility.Visible;
                            hide.Opacity = 0.50;
                            final.Text = "The game ended in a draw\ndue to repetition";
                        }


                    });
                }
            } while (st[st.Length - 1] == 'c');
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
                    Board[i, j].Click += (sender, e) =>
                    {
                        
                        Button b = sender as Button;
                        if (b.Background != Brushes.Red)
                        {
                            unmarkall(ref Board);
                        }
                        if (CanMove(ref b, ref Board, white_turn, p_lock, prevb))
                        {
                            marklgl(ref b,  ref Board, white_turn, p_lock, prevb);
                            prevb = b;
                        }
                        else if (b.Background == Brushes.Red)
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
                                playedMoves.Clear();
                                if (Board[x, y].Background == white_k || Board[x, y].Background == white_p)
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
                            for(int i = 0; i < n; i++)
                            {
                                for(int j = 0; j < n; j++)
                                {
                                    Board[i, j].BorderBrush = Brushes.Black;
                                    Board[i, j].BorderThickness = new Thickness(0);
                                }
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
                            }
                            else
                            {
                                p_lock = false;
                                if (Bpice == 0 || (!PlayerCanMove(ref Board, white_turn, p_lock, prevb) && !white_turn))
                                {
                                    try
                                    {
                                        s.Send(StrToByte(BoardToString(ref Board)));
                                    }
                                    catch { }
                                    pop.Visibility = Visibility.Visible;
                                    hide.Visibility = Visibility.Visible;
                                    hide.Opacity = 0.50;
                                    final.Text = "White won\nblack have no moves";
                                }
                                else if (Wpice == 0 || (!PlayerCanMove(ref Board, white_turn, p_lock, prevb) && white_turn))
                                {
                                    try
                                    {
                                        s.Send(StrToByte(BoardToString(ref Board)));
                                    }
                                    catch { }
                                    pop.Visibility = Visibility.Visible;
                                    hide.Visibility = Visibility.Visible;
                                    hide.Opacity = 0.50;
                                    final.Text = "Black won\nwhite have no moves";
                                }
                                else if (Bpice == 1 && Wpice == 1)
                                {
                                    try
                                    {
                                        s.Send(StrToByte(BoardToString(ref Board)));
                                    }
                                    catch { }
                                    pop.Visibility = Visibility.Visible;
                                    hide.Visibility = Visibility.Visible;
                                    hide.Opacity = 0.50;
                                    final.Text = "The game ended in a draw\ndue to insufficient material";
                                }
                            }



                            prevb = b;
                            ate = false;
                           
                           
                            if (!p_lock)
                            {
                                try
                                {
                                    whosturn.Text = ishost ? "Black's turn" : "White's turn";
                                    s.Send(StrToByte(BoardToString(ref Board)));
                                    while (mr.IsBusy) ;
                                    mr.RunWorkerAsync();
                                }
                                catch
                                {
                                    hide.Visibility = Visibility.Visible;
                                    hide.Opacity = 0.50;
                                    pop.Visibility = Visibility.Visible;
                                    s.Close();
                                    if (ishost)
                                    {
                                        server.Stop();

                                    }
                                    else
                                    {
                                        clint.Close();
                                    }
                                    final.Text = "You won!!! \n opponent resigned";
                                }
                            }
                            else
                            {
                                try
                                {
                                   
                                    s.Send(StrToByte((BoardToString(ref Board) + 'c')));
                                }
                                catch
                                {
                                    
                                    hide.Visibility = Visibility.Visible;
                                    hide.Opacity = 0.50;
                                    pop.Visibility = Visibility.Visible;
                                    s.Close();
                                    if (ishost)
                                    {
                                        server.Stop();

                                    }
                                    else
                                    {
                                        clint.Close();
                                    }
                                    final.Text = "You won!!! \n opponent resigned";
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

                        

                    };
                    Board[i, j].BorderThickness = new Thickness(0);
                    board.Children.Add(Board[i, j]);
                }
            }
        }
        
        private void res_Click(object sender, RoutedEventArgs e)
        {
            string st = "res" + new string(' ', (n * n) - 3 + 1);
            try
            {
                s.Send(StrToByte(st));
                s.Close();
                if (ishost)
                {
                    final.Text = Bplayer + " won";
                    server.Stop();
                }
                else
                {
                    final.Text = Wplayer + " won";
                    clint.Close();
                }
                hide.Visibility = Visibility.Visible;
                hide.Opacity = 0.50;
                pop.Visibility = Visibility.Visible;
            }
            catch
            {
                hide.Visibility = Visibility.Visible;
                hide.Opacity = 0.50;
                pop.Visibility = Visibility.Visible;
                s.Close();
                if (ishost)
                {
                    server.Stop();

                }
                else
                {
                    clint.Close();
                }
                final.Text = "You won!!! \n opponent resigned";
            }
            
        }
        
        private void draw_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string st = "draw" + new string(' ', (n * n) - 3);
                s.Send(StrToByte(st));
                hide.Visibility = Visibility.Visible;
                hide.Opacity = 0.50;
                drawpop.Visibility = Visibility.Visible;
                accbut.Visibility = Visibility.Collapsed;
                decbut.Visibility = Visibility.Collapsed;
                dfinal.Text = "Waiting for your opponent...";
                mr.RunWorkerAsync();
            }
            catch
            {
                hide.Visibility = Visibility.Visible;
                hide.Opacity = 0.50;
                pop.Visibility = Visibility.Visible;
                s.Close();
                if (ishost)
                {
                    server.Stop();

                }
                else
                {
                    clint.Close();
                }
                final.Text = "You won!!! \n opponent resigned";
            }

        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double l = this.Left;
            double t = this.Top;
            double w = this.Width;
            double h = this.Height;
            var newForm = new Start(l, t, w, h, this.WindowState); //create your new form.
            newForm.Show(); //show the new form.
            this.Close(); //
        }
        
        void acc_Click(object sender , RoutedEventArgs r)
        {
            try
            {
                string st = "acce" + new string(' ', (n * n) - 3);
                s.Send(StrToByte(st));
                drawpop.Visibility = Visibility.Collapsed;
                pop.Visibility = Visibility.Visible;
                final.Text = "The game ended with a draw";
                s.Close();
                if (ishost)
                {
                    server.Stop();
                }
                else
                {
                    clint.Close();
                }
            }
            catch
            {
                hide.Visibility = Visibility.Visible;
                hide.Opacity = 0.50;
                pop.Visibility = Visibility.Visible;
                s.Close();
                if (ishost)
                {
                    server.Stop();

                }
                else
                {
                    clint.Close();
                }
                final.Text = "You won!!! \n opponent resigned";
            }
        }
        
        void dec_Click(object sender, RoutedEventArgs r)
        {
            try
            {
                string st = "decl" + new string(' ', (n * n) - 3);
                s.Send(StrToByte(st));
                accbut.Visibility = Visibility.Visible;
                decbut.Visibility = Visibility.Visible;
                drawpop.Visibility = Visibility.Collapsed;
                hide.Opacity = 0.0;
                mr.RunWorkerAsync();
            }
            catch
            {
                hide.Visibility = Visibility.Visible;
                hide.Opacity = 0.50;
                pop.Visibility = Visibility.Visible;
                s.Close();
                if (ishost)
                {
                    server.Stop();

                }
                else
                {
                    clint.Close();
                }
                final.Text = "You won!!! \n opponent resigned";
            }
        }
    }
}
