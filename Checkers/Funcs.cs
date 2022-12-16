using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel;
using System.Net.Sockets;
using Checkers.Models;
using System.Windows;

namespace Funcs
{
    
    public class Funcs
    {


        public static  int n = 8;
        public static  string UriString = "..\\..\\..\\Images\\";
        public static ImageBrush black_p = new(new BitmapImage(new Uri(UriString + "bp.png", UriKind.Relative)));
        public static ImageBrush black_k = new(new BitmapImage(new Uri(UriString + "bk.png", UriKind.Relative)));
        public static ImageBrush white_p = new(new BitmapImage(new Uri(UriString + "wp.png", UriKind.Relative)));
        public static ImageBrush white_k = new(new BitmapImage(new Uri(UriString + "wk.png", UriKind.Relative)));
        


        public static string BoardToString(ref Button[,] p)
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
                    else if (p[i, j].Background == Brushes.Black || p[i, j].Background == Brushes.Red)
                    {
                        s += "E";
                    }
                }
            }
            return s;
        }

        public static Boolean PlayerCanSkip(ref Button[,] p,bool white_turn,bool p_lock = false,Button prevb=null)
        {

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (piceCanSkip(ref p[i, j], ref p, white_turn, p_lock, prevb))
                    {
                        return true;
                    }


                }
            }
            return false;

        }

        public static Boolean piceCanSkip(ref Button b, ref Button[,] p, bool white_turn, bool p_lock = false, Button prevb = null  )
        {
            if (b == null)
            {
                return false;
            }
            int x = Convert.ToInt32(b.Name.Split('_')[1]);
            int y = Convert.ToInt32(b.Name.Split('_')[2]);
            if (p_lock)
            {
                return b == prevb;
            }
            if (b.Background == white_p && white_turn)
            {

                if (x - 2 >= 0 && y - 2 >= 0)
                {
                    if ((p[x - 1, y - 1].Background == black_k || p[x - 1, y - 1].Background == black_p) && (p[x - 2, y - 2].Background == Brushes.Black || p[x - 2, y - 2].Background == Brushes.Red))
                    {
                        return true;
                    }
                }
                if (x - 2 >= 0 && y + 2 < 8)
                {
                    if ((p[x - 1, y + 1].Background == black_k || p[x - 1, y + 1].Background == black_p) && (p[x - 2, y + 2].Background == Brushes.Black|| p[x - 2, y + 2].Background == Brushes.Red))
                    {
                        return true;
                    }
                }
            }
            else if (b.Background == black_p && !white_turn)
            {

                if (x + 2 < 8 && y - 2 >= 0)
                {

                    if ((p[x + 1, y - 1].Background == white_p || p[x + 1, y - 1].Background == white_k) && (p[x + 2, y - 2].Background == Brushes.Black|| p[x + 2, y - 2].Background == Brushes.Red))
                    {

                        return true;
                    }
                }
                if (x + 2 < 8 && y + 2 < 8)
                {
                    if ((p[x + 1, y + 1].Background == white_p || p[x + 1, y + 1].Background == white_k) && (p[x + 2, y + 2].Background == Brushes.Black || p[x + 2, y + 2].Background == Brushes.Red))
                    {
                        return true;
                    }
                }
            }
            else if (b.Background == black_k && !white_turn)
            {
                for (int i = x + 1, j = y + 1; i < 7 && j < 7; i++)
                {

                    if ((p[i, j].Background == white_p || p[i, j].Background == white_k) && (p[i + 1, j + 1].Background == Brushes.Black || p[i + 1, j + 1].Background == Brushes.Red))
                    {
                        return true;
                    }
                    else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                    {
                        break;
                    }
                    j++;
                }
                for (int i = x + 1, j = y - 1; i < 7 && j > 0; i++)
                {

                    if ((p[i, j].Background == white_p || p[i, j].Background == white_k) && (p[i + 1, j - 1].Background == Brushes.Black || p[i + 1, j - 1].Background == Brushes.Red))
                    {
                        return true;
                    }
                    else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                    {
                        break;
                    }
                    j--;
                }
                for (int i = x - 1, j = y + 1; i > 0 && j < 7; i--)
                {

                    if ((p[i, j].Background == white_p || p[i, j].Background == white_k) && (p[i - 1, j + 1].Background == Brushes.Black || p[i - 1, j + 1].Background == Brushes.Red))
                    {
                        return true;
                    }
                    else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                    {
                        break;
                    }
                    j++;
                }
                for (int i = x - 1, j = y - 1; i > 0 && j > 0; i--)
                {

                    if ((p[i, j].Background == white_p || p[i, j].Background == white_k) && (p[i - 1, j - 1].Background == Brushes.Black || p[i - 1, j - 1].Background == Brushes.Red))
                    {
                        return true;
                    }
                    else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                    {
                        break;
                    }
                    j--;
                }
            }
            else if (b.Background == white_k && white_turn)
            {
                for (int i = x + 1, j = y + 1; i < 7 && j < 7; i++)
                {

                    if ((p[i, j].Background == black_p || p[i, j].Background == black_k) && (p[i + 1, j + 1].Background == Brushes.Black || p[i + 1, j + 1].Background == Brushes.Red))
                    {
                        return true;
                    }
                    else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                    {
                        break;
                    }
                    j++;
                }
                for (int i = x + 1, j = y - 1; i < 7 && j > 0; i++)
                {

                    if ((p[i, j].Background == black_p || p[i, j].Background == black_k) && (p[i + 1, j - 1].Background == Brushes.Black || p[i + 1, j - 1].Background == Brushes.Red))
                    {
                        return true;
                    }
                    else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                    {
                        break;
                    }
                    j--;
                }
                for (int i = x - 1, j = y + 1; i > 0 && j < 7; i--)
                {

                    if ((p[i, j].Background == black_p || p[i, j].Background == black_k) && (p[i - 1, j + 1].Background == Brushes.Black || p[i - 1, j + 1].Background == Brushes.Red))
                    {
                        return true;
                    }
                    else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                    {
                        break;
                    }
                    j++;
                }
                for (int i = x - 1, j = y - 1; i > 0 && j > 0; i--)
                {

                    if ((p[i, j].Background == black_p || p[i, j].Background == black_k) && (p[i - 1, j - 1].Background == Brushes.Black || p[i - 1, j - 1].Background == Brushes.Red))
                    {
                        return true;
                    }
                    else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                    {
                        break;
                    }
                    j--;
                }
            }

            return false;
        }

        public static Boolean CanMove(ref Button b, ref Button[,] p,bool white_turn,bool p_lock =false, Button prevb = null)
        {

            if (b.Background == Brushes.Black || b.Background == Brushes.White || b.Background == Brushes.Red)
            {
                
                return false;
            }
            
           
            if (b.Background == white_k || b.Background == white_p)
            {
                if (!white_turn)
                {
                    
                    return false;
                }
            }
            else
            {
                if (white_turn)
                {

                    return false;
                }
            }
            if (piceCanSkip(ref b, ref p, white_turn,p_lock,prevb))
            {
                
                return true;
            }
            else if (PlayerCanSkip(ref p, white_turn, p_lock, prevb) == true)
            {
                return false;
            }
            else
            {

                int cx = Convert.ToInt32(b.Name.Split('_')[1]);
                int cy = Convert.ToInt32(b.Name.Split('_')[2]);

                Boolean res = false;
                if (b.Background == white_p || b.Background == black_k || b.Background == white_k)
                {
                    res = res || (cx > 0 && cy > 0 && p[cx - 1, cy - 1].Background == Brushes.Black) || (cx > 0 && cy < 7 && p[cx - 1, cy + 1].Background == Brushes.Black) || (cx > 0 && cy < 7 && p[cx - 1, cy + 1].Background == Brushes.Red || (cx > 0 && cy > 0 && p[cx - 1, cy - 1].Background == Brushes.Red));
                }
                if (b.Background == black_p || b.Background == black_k || b.Background == white_k)
                {
                    res = res || (cx < 7 && cy > 0 && p[cx + 1, cy - 1].Background == Brushes.Black) || (cx < 7 && cy < 7 && p[cx + 1, cy + 1].Background == Brushes.Black) || (cx < 7 && cy > 0 && p[cx + 1, cy - 1].Background == Brushes.Red) || (cx < 7 && cy < 7 && p[cx + 1, cy + 1].Background == Brushes.Red);
                }
                
                return res;
            }
        }

        public static void marklgl(ref Button b, ref Button[,] p,bool white_turn, bool p_lock = false, Button prevb = null)
        {
            unmarkall(ref p);
            if (piceCanSkip(ref b, ref p, white_turn,p_lock,prevb))
            {
                int x = Convert.ToInt32(b.Name.Split('_')[1]);
                int y = Convert.ToInt32(b.Name.Split('_')[2]);
                if (b.Background == white_p && white_turn)
                {

                    if (x - 2 >= 0 && y - 2 >= 0)
                    {
                        if ((p[x - 1, y - 1].Background == black_k || p[x - 1, y - 1].Background == black_p) && p[x - 2, y - 2].Background == Brushes.Black)
                        {
                            p[x - 2, y - 2].Background = Brushes.Red;
                        }
                    }
                    if (x - 2 >= 0 && y + 2 < 8)
                    {
                        if ((p[x - 1, y + 1].Background == black_k || p[x - 1, y + 1].Background == black_p) && p[x - 2, y + 2].Background == Brushes.Black)
                        {
                            p[x - 2, y + 2].Background = Brushes.Red;
                        }
                    }
                }
                else if (b.Background == black_p && !white_turn)
                {

                    if (x + 2 < 8 && y - 2 >= 0)
                    {
                        if ((p[x + 1, y - 1].Background == white_p || p[x + 1, y - 1].Background == white_k) && p[x + 2, y - 2].Background == Brushes.Black)
                        {
                            p[x + 2, y - 2].Background = Brushes.Red;
                        }
                    }
                    if (x + 2 < 8 && y + 2 < 8)
                    {
                        if ((p[x + 1, y + 1].Background == white_p || p[x + 1, y + 1].Background == white_k) && p[x + 2, y + 2].Background == Brushes.Black)
                        {
                            p[x + 2, y + 2].Background = Brushes.Red;
                        }
                    }
                }
                else if (b.Background == black_k && !white_turn)
                {
                    for (int i = x + 1, j = y + 1; i < 7 && j < 7; i++)
                    {

                        if ((p[i, j].Background == white_p || p[i, j].Background == white_k) && p[i + 1, j + 1].Background == Brushes.Black)
                        {
                            p[i + 1, j + 1].Background = Brushes.Red;
                            break;
                        }
                        else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                        {
                            break;
                        }
                        j++;
                    }
                    for (int i = x + 1, j = y - 1; i < 7 && j > 0; i++)
                    {


                        if ((p[i, j].Background == white_p || p[i, j].Background == white_k) && p[i + 1, j - 1].Background == Brushes.Black)
                        {
                            p[i + 1, j - 1].Background = Brushes.Red;
                            break;
                        }
                        else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                        {
                            break;
                        }
                        j--;
                    }
                    for (int i = x - 1, j = y + 1; i > 0 && j < 7; i--)
                    {
                        if ((p[i, j].Background == white_p || p[i, j].Background == white_k) && p[i - 1, j + 1].Background == Brushes.Black)
                        {
                            p[i - 1, j + 1].Background = Brushes.Red;
                            break;
                        }
                        else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                        {
                            break;
                        }
                        j++;
                    }
                    for (int i = x - 1, j = y - 1; i > 0 && j > 0; i--)
                    {

                        if ((p[i, j].Background == white_p || p[i, j].Background == white_k) && p[i - 1, j - 1].Background == Brushes.Black)
                        {
                            p[i - 1, j - 1].Background = Brushes.Red;
                            break;
                        }
                        else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                        {
                            break;
                        }
                        j--;
                    }
                }
                else if (b.Background == white_k && white_turn)
                {
                    for (int i = x + 1, j = y + 1; i < 7 && j < 7; i++)
                    {

                        if ((p[i, j].Background == black_p || p[i, j].Background == black_k) && p[i + 1, j + 1].Background == Brushes.Black)
                        {
                            p[i + 1, j + 1].Background = Brushes.Red;
                            break;
                        }
                        else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                        {
                            break;
                        }
                        j++;
                    }
                    for (int i = x + 1, j = y - 1; i < 7 && j > 0; i++)
                    {

                        if ((p[i, j].Background == black_p || p[i, j].Background == black_k) && p[i + 1, j - 1].Background == Brushes.Black)
                        {
                            p[i + 1, j - 1].Background = Brushes.Red;
                            break;
                        }
                        else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                        {
                            break;
                        }
                        j--;
                    }
                    for (int i = x - 1, j = y + 1; i > 0 && j < 7; i--)
                    {

                        if ((p[i, j].Background == black_p || p[i, j].Background == black_k) && p[i - 1, j + 1].Background == Brushes.Black)
                        {
                            p[i - 1, j + 1].Background = Brushes.Red;
                            break;
                        }
                        else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                        {
                            break;
                        }
                        j++;
                    }
                    for (int i = x - 1, j = y - 1; i > 0 && j > 0; i--)
                    {

                        if ((p[i, j].Background == black_p || p[i, j].Background == black_k) && p[i - 1, j - 1].Background == Brushes.Black)
                        {
                            p[i - 1, j - 1].Background = Brushes.Red;
                            break;
                        }
                        else if (p[i, j].Background == white_p || p[i, j].Background == white_k || p[i, j].Background == black_k || p[i, j].Background == black_p)
                        {
                            break;
                        }
                        j--;
                    }
                }
            }
            else
            {
                int x = Convert.ToInt32(b.Name.Split('_')[1]);
                int y = Convert.ToInt32(b.Name.Split('_')[2]);
                if (b.Background == white_p && white_turn)
                {

                    if (x - 1 >= 0 && y - 1 >= 0 && p[x - 1, y - 1].Background == Brushes.Black)
                    {

                        p[x - 1, y - 1].Background = Brushes.Red;

                    }
                    if (x - 1 >= 0 && y + 1 < 8 && p[x - 1, y + 1].Background == Brushes.Black)
                    {

                        p[x - 1, y + 1].Background = Brushes.Red;

                    }
                }
                else if (b.Background == black_p && !white_turn)
                {

                    if (x + 1 < 8 && y - 1 >= 0 && p[x + 1, y - 1].Background == Brushes.Black)
                    {

                        p[x + 1, y - 1].Background = Brushes.Red;

                    }
                    if (x + 1 < 8 && y + 1 < 8 && p[x + 1, y + 1].Background == Brushes.Black)
                    {

                        p[x + 1, y + 1].Background = Brushes.Red;

                    }
                }
                else if (b.Background == black_k || b.Background == white_k)
                {
                    for (int i = x + 1, j = y + 1; i < 8 && j < 8; i++)
                    {

                        if (p[i, j].Background != Brushes.Black)
                        {
                            break;
                        }
                        p[i, j].Background = Brushes.Red;
                        j++;
                    }
                    for (int i = x + 1, j = y - 1; i < 8 && j >= 0; i++)
                    {

                        if (p[i, j].Background != Brushes.Black)
                        {
                            break;
                        }
                        p[i, j].Background = Brushes.Red;
                        j--;
                    }
                    for (int i = x - 1, j = y + 1; i >= 0 && j < 8; i--)
                    {

                        if (p[i, j].Background != Brushes.Black)
                        {
                            break;
                        }
                        p[i, j].Background = Brushes.Red;
                        j++;
                    }
                    for (int i = x - 1, j = y - 1; i >= 0 && j >= 0; i--)
                    {

                        if (p[i, j].Background != Brushes.Black)
                        {
                            break;
                        }
                        p[i, j].Background = Brushes.Red;
                        j--;
                    }
                }

            }
        }

        public static void unmarkall(ref Button[,] p)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (p[i, j].Background == Brushes.Red)
                    {
                        p[i, j].Background = Brushes.Black;
                    }
                }
            }
        }

        public static Boolean PlayerCanMove(ref Button[,] p,bool white_turn, bool p_lock = false, Button prevb = null)
        {
            foreach (var b in p)
            {
                int x = Convert.ToInt32(b.Name.Split('_')[1]);
                int y = Convert.ToInt32(b.Name.Split('_')[2]);
                if (CanMove(ref p[x,y], ref p, white_turn,p_lock,prevb))
                {
                    return true;
                }
            }
            return false;
        }

        public static Boolean repetition(List<string> l,string lastMove)
        {
            int count = 0;
            foreach (string s in l)
            {
                if(s == lastMove)
                {
                    count++;
                }
            }
            return count >= 3;
        }


    }
}