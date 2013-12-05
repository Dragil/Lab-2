using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace L2
{
    public partial class Form1 : Form
    {
        private List<string> standart_operators = new List<string>(new string[] { "(", ")", "+", "-", "*", "/" });
        private List<string> operators;

        public Form1()
        {
            InitializeComponent();
        }

        public string[] ConvertToPostfixNotation(string input)
        {
            List<string> outputSeparated = new List<string>();
            Stack<string> stack = new Stack<string>();
            List<string> inputSeparated = Separate(input);
            string c;
            for (int i = 0; i < inputSeparated.Count; i++)
            {
                c = inputSeparated[i];
                if (operators.Contains(c))
                {
                    if (stack.Count > 0 && !c.Equals("("))
                    {
                        if (c.Equals(")"))
                        {
                            string s = stack.Pop();
                            while (s != "(")
                            {
                                outputSeparated.Add(s);
                                s = stack.Pop();
                            }
                        }
                        else if (c.Equals("-") && inputSeparated[i - 1].Equals("("))
                        {
                            stack.Push("--");
                        }
                        else if (GetPriority(c) >= GetPriority(stack.Peek()))
                            stack.Push(c);
                        else
                        {
                            while (stack.Count > 0 && GetPriority(c) < GetPriority(stack.Peek()))
                                outputSeparated.Add(stack.Pop());
                            stack.Push(c);
                        }
                    }
                    else
                    {
                        if (c.Equals("-") && (i == 0 || inputSeparated[i - 1].Equals("=")))
                        {
                            stack.Push("--");
                        }
                        else
                        {
                            stack.Push(c);
                        }
                    }
                }
                else if (!c.Equals(" "))
                {
                    outputSeparated.Add(c);
                    if (stack.Count > 0 && stack.Peek().Equals("-") && inputSeparated.Count > (i + 1) && inputSeparated[i + 1].Equals("-"))
                    {
                        outputSeparated.Add(stack.Pop());
                    }
                }
                else
                {
                    inputSeparated.RemoveAt(i);
                    i--;
                }

            }
            if (stack.Count > 0)
                foreach (string d in stack)
                    outputSeparated.Add(d);

            return outputSeparated.ToArray();
        }

        private byte GetPriority(string s)
        {
            switch (s)
            {
                case "(":
                    return 0;
                case ")":
                    return 1;
                case "+":
                case "-":
                    return 2;
                case "/":
                    return 3;
                case "*":
                    return 4;
                default:
                    return 5;
            }
        }

        private List<string> Separate(string input)
        {
            List<string> output = new List<string>();
            int pos = 0;
            while (pos < input.Length)
            {
                string s = string.Empty + input[pos];
                if (!standart_operators.Contains(input[pos].ToString()))
                {
                    if (Char.IsDigit(input[pos]))
                        for (int i = pos + 1; i < input.Length &&
                            (Char.IsDigit(input[i]) || input[i] == ',' || input[i] == '.'); i++)
                            s += input[i];
                    else if (Char.IsLetter(input[pos]))
                        for (int i = pos + 1; i < input.Length &&
                            (Char.IsLetter(input[i]) || Char.IsDigit(input[i])); i++)
                            s += input[i];
                }
                output.Add(s);
                pos += s.Length;
            }
            return output;
        }

        private string[] TurnItToAssembler(string[] input)
        {
            string g;
            string l;
            List<string> output = new List<string>();
            List<string> inputList = input.ToList();
            int i = 0;
            operators.Add("--");
            while (i < inputList.Count)
            {
                if (operators.Contains(inputList[i]))
                {
                    
                       g=inputList[i - 2];
                       l=inputList[i - 1];
                    switch (inputList[i])
                    {   
                        case "+":
                            if (!g.Equals("ax"))
                            {
                                if (g.Equals("pop"))
                                {
                                    if (l.Equals("ax"))
                                    {
                                        output.Add("pop bx");
                                        output.Add("add ax,bx");
                                    }
                                    else
                                    {
                                        if (idDemical(l))
                                        {
                                            output.Add("pop ax");
                                            output.Add("add ax," + l);
                                        }
                                        else
                                        {
                                            output.Add("pop ax");
                                            if (!l.Equals("bx"))
                                            {
                                                output.Add("mov bx," + l);
                                            }
                                            output.Add("add ax,bx");
                                        }
                                    }
                                }
                                else if (l.Equals("ax"))
                                {
                                    if (idDemical(g))
                                    {   
                                        output.Add("add ax," + g);
                                    }
                                    else
                                    {
                                        if (!g.Equals("bx"))
                                        {
                                            output.Add("mov bx," + g);
                                        }
                                        output.Add("add ax,bx");
                                    }
                                }
                                else
                                {
                                    output.Add("mov ax," + g);
                                    if (idDemical(l))
                                    {
                                        output.Add("add ax," + l);
                                    }
                                    else
                                    {
                                        if (!l.Equals("bx"))
                                        {
                                            output.Add("mov bx," + l);
                                        }
                                        output.Add("add ax,bx");
                                    }
                                }
                            }
                            else
                            {
                                if (idDemical(l))
                                {
                                    output.Add("add ax," + l);
                                }
                                else
                                {
                                    if (!l.Equals("bx"))
                                    {
                                        output.Add("mov bx," + l);
                                    }
                                    output.Add("add ax,bx");
                                }
                            }
                            if ((i + 2) < inputList.Count && !standart_operators.Contains(l) && !standart_operators.Contains(g))
                            {
                                output.Add("push ax");
                                inputList[i] = "pop";
                            }
                            else
                            {
                                inputList[i] = "ax";
                            }
                            inputList.RemoveAt(i - 1);
                            inputList.RemoveAt(i - 2);
                            i = -1;
                            break;
                        case "-":
                            if (!g.Equals("ax"))
                            {
                                if (g.Equals("pop"))
                                {
                                    if (idDemical(l))
                                    {
                                        output.Add("pop ax");
                                        output.Add("sub ax," + inputList[i - 1]);
                                    }
                                    else
                                    {
                                        if (!inputList[i - 1].Equals("bx"))
                                        {
                                            output.Add("mov bx," + inputList[i - 1]);
                                        }
                                        output.Add("pop ax");
                                        output.Add("sub ax,bx");
                                    }
                                }
                                else if (inputList[i - 1].Equals("ax"))
                                {
                                    output.Add("mov bx," + inputList[i - 1]);
                                    output.Add("mov ax," + inputList[i - 2]);
                                }
                                else
                                {
                                    output.Add("mov ax," + inputList[i - 2]);
                                    if (idDemical(l))
                                    {
                                        output.Add("sub ax," + inputList[i - 1]);
                                    }
                                    else
                                    {
                                        if (!inputList[i - 1].Equals("bx"))
                                        {
                                            output.Add("mov bx," + inputList[i - 1]);
                                        }
                                        output.Add("sub ax,bx");
                                    }
                                }
                            }
                            else
                            {
                                if (idDemical(l))
                                {
                                    output.Add("sub ax," + inputList[i - 1]);
                                }
                                else
                                {
                                    if (!inputList[i - 1].Equals("bx"))
                                    {
                                        output.Add("mov bx," + inputList[i - 1]);
                                    }
                                    output.Add("sub ax,bx");
                                }
                            }
                            if ((i + 2) < inputList.Count && !standart_operators.Contains(inputList[i + 1]) && !standart_operators.Contains(inputList[i + 2]))
                            {
                                output.Add("push ax");
                                inputList[i] = "pop";
                            }
                            else
                            {
                                inputList[i] = "ax";
                            }
                            inputList.RemoveAt(i - 1);
                            inputList.RemoveAt(i - 2);
                            i = -1;
                            break;
                        case "--":
                            if (idDemical(l))
                            {
                                l = "-" + l;
                                inputList.RemoveAt(i);
                            }
                                else
                            {
                                if (!l.Equals("ax"))
                                {
                                    output.Add("mov ax," + l);
                                }
                                output.Add("neg ax");
                                if (inputList.Count > (i + 2) && standart_operators.Contains(g) && !standart_operators.Contains(l))
                                {
                                    inputList[i] = "ax";
                                }
                                else if (inputList.Count > (i + 1) && standart_operators.Contains(inputList[i + 1]))
                                {
                                    output.Add("xchg bx,ax");
                                    inputList[i] = "bx";
                                }
                                else
                                {
                                    output.Add("push ax");
                                    inputList[i] = "pop";
                                }
                                inputList.RemoveAt(i - 1);
                            }
                            i = -1;
                            break;
                        case "*":
                            if (!g.Equals("ax"))
                            {
                                if (g.Equals("pop"))
                                {
                                    if (l.Equals("ax"))
                                    {
                                        output.Add("pop bx");
                                        output.Add("mul ax,bx");
                                    }
                                    else
                                    {
                                        if (idDemical(l))
                                        {
                                            output.Add("pop ax");
                                            output.Add("mul ax," + l);
                                        }
                                        else
                                        {
                                            output.Add("pop ax");
                                            if (!l.Equals("bx"))
                                            {
                                                output.Add("mov bx," + l);
                                            }
                                            output.Add("mul ax,bx");
                                        }
                                    }
                                }
                                else if (l.Equals("ax"))
                                {
                                    if (idDemical(g))
                                    {
                                        output.Add("mul ax," + g);
                                    }
                                    else
                                    {
                                        if (!inputList[i - 2].Equals("bx"))
                                        {
                                            output.Add("mov bx," + g);
                                        }
                                        output.Add("mul ax,bx");
                                    }
                                }
                                else
                                {
                                    output.Add("mov ax," + g);
                                    if (idDemical(l))
                                    {
                                        output.Add("mul ax," + l);
                                    }
                                    else
                                    {
                                        if (!l.Equals("bx"))
                                        {
                                            output.Add("mov bx," + l);
                                        }
                                        output.Add("mul ax,bx");
                                    }
                                }
                            }
                            else
                            {
                                if (idDemical(l))
                                {
                                    output.Add("mul ax," + l);
                                }
                                else
                                {
                                    if (!l.Equals("bx"))
                                    {
                                        output.Add("mov bx," + l);
                                    }
                                    output.Add("mul ax,bx");
                                }
                            }
                            if ((i + 2) < inputList.Count && !standart_operators.Contains(l) && !standart_operators.Contains(g))
                            {
                                output.Add("push ax");
                                inputList[i] = "pop";
                            }
                            else
                            {
                                inputList[i] = "ax";
                            }
                            inputList.RemoveAt(i - 1);
                            inputList.RemoveAt(i - 2);
                            i = -1;
                            break;
                        case "/":
                            if (!g.Equals("ax"))
                            {
                                if (g.Equals("pop"))
                                {
                                    if (idDemical(l))
                                    {
                                        output.Add("pop ax");
                                        output.Add("div ax," + l);
                                    }
                                    else                                    {
                                        if (!l.Equals("bx"))
                                        {
                                            output.Add("mov bx," + l);
                                        }
                                        output.Add("pop ax");
                                        output.Add("div ax,bx");
                                    }
                                }
                                else if (l.Equals("ax"))
                                {
                                    output.Add("mov bx," + l);
                                    output.Add("mov ax," + g);
                                    output.Add("div ax,bx");
                                }
                                else
                                {
                                    output.Add("mov ax," + g);
                                    if (idDemical(l))
                                    {
                                        output.Add("div ax," + l);
                                    }
                                    else
                                    {
                                        if (!l.Equals("bx"))
                                        {
                                            output.Add("mov bx," + l);
                                        }
                                        output.Add("div ax,bx");
                                    }
                                }
                            }
                            else
                            {
                                if (idDemical(l))
                                {
                                    
                                    output.Add("div ax," + l);
                                }
                                else
                                {
                                    if (!l.Equals("bx"))
                                    {
                                        output.Add("mov bx," + l);
                                    }
                                    output.Add("div ax,bx");
                                }
                            }
                            if ((i + 2) < inputList.Count && !operators.Contains(l) && !operators.Contains(g))
                            {
                                output.Add("push ax");
                                inputList[i] = "pop";
                            }
                            else
                            {
                                inputList[i] = "ax";
                            }
                            inputList.RemoveAt(i - 1);
                            inputList.RemoveAt(i - 2);
                            i = -1;
                            break;
                    }
                }
                i = i + 1;
            }
            if (output.Count > 0 && output[output.Count - 1].Equals("push ax"))
            {
                output.RemoveAt(output.Count - 1);
                inputList[2] = "ax";
            }
            if (inputList.Count > 2 && inputList[1].Equals("="))
            {
                output.Add("mov " + inputList[0] + ", " + inputList[2]);
            }
            return output.ToArray();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            string input = textBox1.Text;
            operators = new List<string>(standart_operators);
            string[] output = ConvertToPostfixNotation(input);
            output = TurnItToAssembler(output);
            for (int i = 0; i < output.Length; i++)
            {
                listBox1.Items.Add(output[i]);
            }
        }

        private bool idDemical(String decimalString)
        {
            try
            {
                Convert.ToDecimal(decimalString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
