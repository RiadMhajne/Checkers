using Checkers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static Funcs.Funcs;


namespace Minimax
{
    public class MiniMax
    {

       
        private Boolean white_turn = false;
        private Boolean p_lock = false;
        private int Bpice = 12;
        private int Wpice = 12;
        private Button prevb;
        Button[,] p = new Button[8, 8];
        NTree t { get; set; }
        int maxDepth;

        public static string SpliceText(string text, int lineLength)
        {
            return Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
        }

        string BoardToString()
        {
            string s = "";
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (p[i, j].Background == white_k)
                    {
                        s += "W";
                    }
                    else if (p[i, j].Background == white_p)
                    {
                        s += "w";
                    }
                    else if (p[i, j].Background == black_p)
                    {
                        s += "b";
                    }
                    else if (p[i, j].Background == black_k)
                    {
                        s += "B";
                    }
                    else if (p[i, j].Background == Brushes.White)
                    {
                        s += "e";
                    }
                    else if (p[i, j].Background == Brushes.Black)
                    {
                        s += "E";
                    }
                    else if (p[i, j].Background == Brushes.Red)
                    {
                        s += "r";
                    }
                }
            }
            return s;
        } //different from the other BoardToString function

        void BuildBoard(string st)
        {
            Bpice = 0;
            Wpice = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (st[(i * n) + j] == 'w')
                    {
                        p[i, j].Background = white_p;
                        Wpice++;
                    }
                    else if (st[(i * n) + j] == 'W')
                    {
                        p[i, j].Background = white_k;
                        Wpice++;
                    }
                    else if (st[(i * n) + j] == 'b')
                    {
                        p[i, j].Background = black_p;
                        Bpice++;
                    }
                    else if (st[(i * n) + j] == 'B')
                    {
                        p[i, j].Background = black_k;
                        Bpice++;
                    }
                    else if (st[(i * n) + j] == 'e')
                    {
                        p[i, j].Background = Brushes.White;
                    }
                    else if (st[(i * n) + j] == 'E')
                    {
                        p[i, j].Background = Brushes.Black;
                    }
                    else if (st[(i * n) + j] == 'r')
                    {
                        p[i, j].Background = Brushes.Red;
                    }
                    p[i, j].Name = "A_" + i + "_" + j;
                }
            }

        }//different from the other BuildBoard function

        public MiniMax(int maxDepth)
        {
            t = null;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    p[i, j] = new Button();

                }

            }
            this.maxDepth = maxDepth;
        }

        public string GetNextMove(string b)
        {
            white_turn = false;
            t = new NTree(b);
            BuildBoard(b);
            TreeBulid(t, b, p, 0, true);
            int ii = 1;
            int maxval = -100;
            string maxstr = b;
            NTree maxtree = t.GetChild(ii);
            var cil = t.GetChild(ii);
            while (cil != null)
            {
                if (cil.value > maxval)
                {
                    maxval = cil.value;
                    maxstr = cil.data;
                    maxtree = cil;
                }


                cil = t.GetChild(++ii);

            }
            List<string> possipleMoves = new List<string>();
            ii = 1;
            cil = t.GetChild(ii);
            while (cil != null)
            {
                if (cil.value == maxval)
                {
                    possipleMoves.Add(cil.data);
                }


                cil = t.GetChild(++ii);

            }
            Random r = new Random();
            int rInt = r.Next(0, possipleMoves.Count);

            return possipleMoves[rInt];
        }

        private void TreeBulid(NTree root, string b, Button[,] p, int cuurd = 0, bool max = true, bool ate = false, bool p_lock = false)
        {
            if(!b.Contains("b") && !b.Contains("B"))
            {
                root.value = -12;
                return;
            }
            if (!b.Contains("w") && !b.Contains("W"))
            {
                root.value = 12;
                return;
            }
            if (cuurd == maxDepth)
            {
                
                int wp = 0, bp = 0;
                foreach (char c in root.data)
                {
                    switch (c)
                    {
                        case 'w':
                            wp++;
                            break;
                        case 'W':
                            wp += 4;
                            break;
                        case 'b':
                            bp++;
                            break;
                        case 'B':
                            bp += 4;
                            break;

                    }
                }
                
                root.value = bp - wp;
                return;
            }

            BuildBoard(b);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {

                    if (CanMove(ref p[i, j], ref p, white_turn, p_lock, prevb))
                    {
                        marklgl(ref p[i, j], ref p, white_turn, p_lock, prevb);

                        b = BoardToString();
                        string bb = b;
                        for (int ii = 0; ii < n; ii++)
                        {
                            for (int jj = 0; jj < n; jj++)
                            {
                                int x = ii;
                                int y = jj;
                                var temp = p[i, j].Background;
                                string move;

                                if (p[ii, jj].Background == Brushes.Red)
                                {

                                    unmarkall(ref p);
                                    if (piceCanSkip(ref p[i, j],ref p,white_turn,p_lock,prevb) == true)
                                    {

                                        x = ii > i ? ii - 1 : ii + 1;
                                        y = jj > j ? jj - 1 : jj + 1;
                                        ate = true;
                                    }
                                    p[ii, jj].Background = p[i, j].Background;
                                    p[i, j].Background = Brushes.Black;
                                    if (ii == 7 && p[ii, jj].Background == black_p)
                                    {
                                        p[ii, jj].Background = black_k;
                                    }
                                    else if (ii == 0 && p[ii, jj].Background == white_p)
                                    {
                                        p[ii, jj].Background = white_k;
                                    }
                                    if (ate && x != ii && y != jj)
                                    {
                                        temp = p[x, y].Background;
                                        p[x, y].Background = Brushes.Black;
                                        p_lock = piceCanSkip(ref p[ii, jj], ref p, white_turn, p_lock, prevb) ? true : false;
                                    }
                                    move = BoardToString();
                                    move += p_lock ? 't' : 'f';

                                    root.AddChild(move);
                                    p[i, j].Background = temp;
                                    p[ii, jj].Background = Brushes.Black;
                                    b = BoardToString();
                                    if (ate && x != ii && y != jj)
                                    {
                                        p[x, y].Background = temp;
                                        if (p_lock)
                                        {
                                            prevb = p[ii, jj];
                                            maxDepth++;
                                            NTree newroot = root.GetChild(1);
                                            TreeBulid(newroot, newroot.data, p, cuurd + 1, max, false, p_lock);
                                            maxDepth--;
                                            p_lock = false;
                                        }
                                        else
                                        {
                                            white_turn = !white_turn;
                                            NTree newroot = root.GetChild(1);
                                            TreeBulid(newroot, newroot.data, p, cuurd + 1, !max, false, false);
                                            white_turn = !white_turn;
                                        }
                                    }
                                    else
                                    {
                                        white_turn = !white_turn;
                                        NTree newroot = root.GetChild(1);
                                        TreeBulid(newroot, newroot.data, p, cuurd + 1, !max, false, false);
                                        white_turn = !white_turn;
                                    }
                                }
                                b = bb;
                                BuildBoard(b);
                            }
                        }
                        BuildBoard(b);
                    }
                }
            }
            
                int iii = 1;
                int val = max ? -100 : 100;
                var cc = root.GetChild(iii);
                while (cc != null)
                {
                    if (max)
                    {
                        val = val > cc.value ? val : cc.value;
                    }

                    else
                    {
                        val = val > cc.value ? cc.value : val;
                    }
                    
                    cc = root.GetChild(++iii);
                }
                root.value = val;
            
        }

    }
}
