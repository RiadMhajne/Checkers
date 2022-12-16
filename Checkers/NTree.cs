using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Minimax
{
    public class NTree//simple tree to use in MiniMax algorithm
    {
        public int value { get; set; }
        public string data { get; set; }
        LinkedList<NTree> children;

        public NTree(string data)
        {
            this.data = data;
            children = new LinkedList<NTree>();

        }

        public static string SpliceText(string text, int lineLength)
        {
            return Regex.Replace(text, "(.{" + lineLength + "})", "$1" + Environment.NewLine);
        }

        public void AddChild(string data)
        {
            children.AddFirst(new NTree(data));
        }

        public NTree GetChild(int i)
        {
            foreach (NTree n in children)
                if (--i == 0)
                    return n;
            return null;
        }

        public int GetChildrenNum()
        {
            return children.Count;
        }
    }
}
