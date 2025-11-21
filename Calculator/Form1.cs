using System.Data;
using System.Linq.Expressions;

namespace Calculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // exprestion
        string[] numbers = new string[50]; // Array to store numbers/operators/brackets
        string expression = ""; // Store current number 
        int index = 0; // Index pointer for array

        // Adds digit to the display and the expression
        private void AddDigit(string digit)
        {
            textBox1.Text += digit;
            expression += digit;
        }

        // Check if last index in the array is a operator or not
        private bool IsIsLastCharOperator()
        {
            if (textBox1.Text.Length == 0) return false;

            char last = textBox1.Text[textBox1.Text.Length - 1];
            return "+-*/".Contains(last);
        }

        // Add operator to the array
        private void AddOperator(string op)
        {
            // Check if operator is duplicated or not
            if (IsIsLastCharOperator() || expression.Length == 0) return;

            textBox1.Text += op;
            numbers[index++] = expression;
            numbers[index++] = op;
            expression = "";
        }

        // Numbers click event
        private void btn_0_Click(object sender, EventArgs e) => AddDigit("0");
        private void btn_1_Click(object sender, EventArgs e) => AddDigit("1");
        private void btn_2_Click(object sender, EventArgs e) => AddDigit("2");
        private void btn_3_Click(object sender, EventArgs e) => AddDigit("3");
        private void btn_4_Click(object sender, EventArgs e) => AddDigit("4");
        private void btn_5_Click(object sender, EventArgs e) => AddDigit("5");
        private void btn_6_Click(object sender, EventArgs e) => AddDigit("6");
        private void btn_7_Click(object sender, EventArgs e) => AddDigit("7");
        private void btn_8_Click(object sender, EventArgs e) => AddDigit("8");
        private void btn_9_Click(object sender, EventArgs e) => AddDigit("9");

        private void btn_dot_Click(object sender, EventArgs e)
        {
            // Prevent multiple dots in same number
            if (expression.Contains(".")) return;

            // Make sure there is a number before the dot
            if (expression == "")
            {
                textBox1.Text += "0.";
                expression += "0.";
            }
            else
            {
                textBox1.Text += ".";
                expression += ".";
            }
        }

        private void btn_neg_Click(object sender, EventArgs e)
        {
            // Make sure negative not in the middle of a number or the end of a number
            if (expression == "")
            {
                textBox1.Text += "-";
                expression += "-";
            }
        }

        private void btn_openBracket_Click(object sender, EventArgs e)
        {
            textBox1.Text += "(";
            numbers[index++] = "(";
        }

        private void btn_closeBracket_Click(object sender, EventArgs e)
        {
            if (expression != "")
            {
                numbers[index++] = expression;
                numbers[index++] = ")";
                textBox1.Text += ")";
                expression = "";
            }
        }

        // Operators click event
        private void btn_div_Click(object sender, EventArgs e) => AddOperator("/");
        private void btn_multi_Click(object sender, EventArgs e) => AddOperator("*");
        private void btn_sub_Click(object sender, EventArgs e) => AddOperator("-");
        private void btn_sum_Click(object sender, EventArgs e) => AddOperator("+");

        private void btn_clear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            expression = "";
            Array.Clear(numbers);
            index = 0;
        }

        private void btn_del_Click(object sender, EventArgs e)
        {
            // Check if there is something to delete
            if (textBox1.Text.Length == 0) return;

            char last = textBox1.Text[textBox1.Text.Length - 1];

            // Delete digit or dot
            if (char.IsDigit(last) || last == '.')
            {
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);

                if (expression.Length > 0)
                {
                    expression = expression.Remove(expression.Length - 1);
                }

                return;
            }

            // Delete operator
            if ("+-*/".Contains(last))
            {
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);

                numbers[--index] = null;

                expression = numbers[--index];
                numbers[index] = null;

                return;
            }

            // Delete brackets
            if (last == '(' || last == ')')
            {
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);

                numbers[--index] = null;
            }
        }

        private void btn_equal_Click(object sender, EventArgs e)
        {
            numbers[index++] = expression;

            if (textBox1.Text.Length == 0) return;

            // Start looking for brackets to calculate it first
            bool bracketFound = true;

            while (bracketFound)
            {
                bracketFound = false;
                int close = -1;

                // Find the closing bracket
                for (int i = 0; i < index; i++)
                {
                    if (numbers[i] == ")")
                    {
                        close = i;
                        bracketFound = true;
                        break;
                    }
                }

                if (!bracketFound) break;

                // Looking for opening bracket
                int open = close;

                while (open >= 0 && numbers[open] != "(") open--;

                int length = close - open - 1;

                string[] temp = new string[length];
                int tempIndex = 0;

                // Add numbers and operators of bracket inside a new array
                for (int i = open + 1; i < close; i++)
                    temp[tempIndex++] = numbers[i];

                // Do math for the new array
                for (int i = 0; i < tempIndex; i++)
                {
                    if (temp[i] == "*" || temp[i] == "/")
                    {
                        double left = Convert.ToDouble(temp[i - 1]);
                        double right = Convert.ToDouble(temp[i + 1]);
                        double result = temp[i] == "*" ? left * right : left / right;

                        temp[i - 1] = result.ToString();

                        for (int j = i; j < tempIndex - 2; j++)
                            temp[j] = temp[j + 2];

                        tempIndex -= 2;
                        i--;
                    }
                }

                double resultInside = Convert.ToDouble(temp[0]);

                for (int i = 1; i < tempIndex; i += 2)
                {
                    string op = temp[i];
                    double num = Convert.ToDouble(temp[i + 1]);

                    if (op == "+") resultInside += num;
                    if (op == "-") resultInside -= num;
                }

                // Replace open bracket with the final value of calculation
                numbers[open] = resultInside.ToString();

                // Shift the array to and remove used numbers and operators
                int removeCount = close - open + 1;

                for (int i = open + 1; i < index - removeCount; i++)
                    numbers[i] = numbers[i + removeCount];

                index -= removeCount;

                for (int i = index; i < numbers.Length; i++) numbers[i] = null;
            }

            // Look for * or / to do it second
            for (int i = 0; i < index; i++)
            {
                if (numbers[i] == "*" || numbers[i] == "/")
                {
                    double left = Convert.ToDouble(numbers[i - 1]);
                    double right = Convert.ToDouble(numbers[i + 1]);

                    double temp = numbers[i] == "*" ? left * right : left / right;
                    numbers[i - 1] = temp.ToString();

                    // Shift array and remove used numbers and operators
                    for (int j = i; j < index - 2; j++)
                    {
                        numbers[j] = numbers[j + 2];
                    }

                    index -= 2;
                    i -= 1;
                }
            }

            // Look for + or - to do it last
            for (int i = 0; i < index; i++)
            {
                if (numbers[i] == "+" || numbers[i] == "-")
                {
                    double left = Convert.ToDouble(numbers[i - 1]);
                    double right = Convert.ToDouble(numbers[i + 1]);

                    double temp = numbers[i] == "+" ? left + right : left - right;
                    numbers[i - 1] = temp.ToString();

                    // Shift array and remove used numbers and operators
                    for (int j = i; j < index - 2; j++)
                    {
                        numbers[j] = numbers[j + 2];
                    }

                    index -= 2;
                    i -= 1;
                }
            }

            // Show the final
            textBox1.Text = numbers[0];

            // Reset every thing and store the last final value to the array to start again
            double finalValue = Convert.ToDouble(numbers[0]);
            Array.Clear(numbers);
            numbers[0] = finalValue.ToString();
            expression = numbers[0];
            index = 0;
        }
    }
}