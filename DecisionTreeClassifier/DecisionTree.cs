using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;
using DecisionTreeClassifierV2.Dataset;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace DecisionTreeClassifierV2.DecisionTreeClassifier
{
    [Serializable()]
    class DecisionTree
    {
        public Datarow[] data { get; private set; }
        public Node start_node { get; private set; }
        public Stack<Node> nodes_to_build { get; private set; }
        public DecisionTree(Datarow[] data)
        {
            this.data = data;
            this.nodes_to_build = new Stack<Node>();
        }

        public void SaveToDat(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();

            try
            {
                formatter.Serialize(fs, this);
            }
            catch (SerializationException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public Leaf Classify(Datarow row)
        {
            Node thisNode;
            if (start_node is DecisionNode) thisNode = (Node)(start_node as DecisionNode).Clone();
            else thisNode = (Node)(start_node as Leaf).Clone();

            while (!(thisNode is Leaf))
            {
                if ((thisNode as DecisionNode).question.Match(row))
                {
                    thisNode = (thisNode as DecisionNode).true_branch;
                }
                else
                {
                    thisNode = (thisNode as DecisionNode).false_branch;
                }
            }
            return (Leaf)thisNode;
        }

        public void PrintTree()
        {
            string spacing = "";
            Stack<Tuple<Node, bool, string>> nodes_to_print = new Stack<Tuple<Node, bool, string>>();
            nodes_to_print.Push(new Tuple<Node, bool, string>(this.start_node, true, spacing));
            while(nodes_to_print.Count > 0)
            {
                Tuple<Node, bool, string>  tuple = nodes_to_print.Pop();
                PrintNode(tuple.Item1, tuple.Item3, tuple.Item2);
                if(tuple.Item1 is DecisionNode)
                {
                    spacing += "  ";
                    nodes_to_print.Push(new Tuple<Node, bool, string>((tuple.Item1 as DecisionNode).false_branch, false, spacing));
                    nodes_to_print.Push(new Tuple<Node, bool, string>((tuple.Item1 as DecisionNode).true_branch, true, spacing));
                }
            }
        }

        public void ProcessStack()
        {
            Node node = this.nodes_to_build.Pop();
            if (node is DecisionNode)
            {
                DecisionNode decisionNode = (DecisionNode)node;
                decisionNode.true_branch = BuildNode(decisionNode.true_branch_data);
                decisionNode.false_branch = BuildNode(decisionNode.false_branch_data);
                this.nodes_to_build.Push(decisionNode.true_branch);
                this.nodes_to_build.Push(decisionNode.false_branch);
            }
        }

       
        public void BuildTree()
        {
            this.start_node = BuildNode(this.data);
            nodes_to_build.Push(this.start_node);
            while(nodes_to_build.Count > 0)
            {
                ProcessStack();
            }
        }


        // static data

        static public DecisionTree ReadFromDat(string path)
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                return (DecisionTree)formatter.Deserialize(fs);
            }
            else throw new FileNotFoundException();
        }
        private static void PrintNode(Node node, string spacing, bool status)
        {
            Console.Write(spacing);
            Console.ForegroundColor = (status) ? ConsoleColor.Green : ConsoleColor.Red;
            Console.Write("--> " + status.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
            if (node is DecisionNode)
            {
                Console.Write(" " + (node as DecisionNode).ToString());
                Console.Write("\n");
            }
            else
            {
                Console.Write(" " + (node as Leaf).ToString());
                Console.Write("\n");
            }
        }

        static public Node BuildNode(Datarow[] data)
        {
            Tuple<double, Question> best_question_tuple = FindBestQuestion(data);
            if (best_question_tuple.Item1 == 0) return new Leaf(data);

            Tuple<Datarow[], Datarow[]> truefalse_rows = Partition(data, best_question_tuple.Item2);

            return new DecisionNode(best_question_tuple.Item2, truefalse_rows.Item1, truefalse_rows.Item2);
        }

        static public double Gini(Datarow[] rows)
        {
            Tuple<object, int>[] counts = GetClassCounts(rows);
            double impurity = 1.0;

            foreach(Tuple<object, int> count in counts)
            {
                double prop = (double)count.Item2 / (double)rows.Length;
                impurity -= (prop * prop);
            }

            return impurity;
        }

        public static Tuple<double, Question> FindBestQuestion(Datarow[] rows)
        {
            double bestGain = 0;
            Question bestQuestion = null;
            double uncertainty = Gini(rows);
            
            for(int i = 0; i < rows[0].Count - 1; i++)
            {
                for (int j = 0; j < rows.Length; j++)
                {
                    Question q = new Question(i, rows[j].values[i]);

                    Tuple<Datarow[], Datarow[]> truefalse_rows = Partition(rows, q);

                    if (truefalse_rows.Item1.Length == 0 || truefalse_rows.Item2.Length == 0) continue;

                    double gain = InfoGain(truefalse_rows.Item1, truefalse_rows.Item2, uncertainty);

                    if (gain > bestGain)
                    {
                        bestGain = gain;
                        bestQuestion = q;
                    }
                }
            }

            return new Tuple<double, Question>(bestGain, bestQuestion);
        }

        static public double InfoGain(Datarow[] left, Datarow[] right, double current_uncertainty)
        {
            double p = (double)left.Length / ((double)left.Length + (double)right.Length);
            return current_uncertainty - p * Gini(left) - (1 - p) * Gini(right);
        }

        static public object[] FindUniqueValues(Datarow[] rows)
        {
            List<object> uniqueValues = new List<object>();
            for(int i = 0; i < rows.Length; i++)
            {
                object dat = rows[i].values[rows[i].Count - 1];
                if (!uniqueValues.Contains(dat)) uniqueValues.Add(dat);
            }
            return uniqueValues.ToArray();
        }

        static public Tuple<Datarow[], Datarow[]> Partition(Datarow[] rows, Question q)
        {
            List<Datarow> true_rows = new List<Datarow>();
            List<Datarow> false_rows = new List<Datarow>();

            foreach (Datarow row in rows)
            {
                if (q.Match(row)) true_rows.Add(row);
                else false_rows.Add(row);
            }

            return new Tuple<Datarow[], Datarow[]>(true_rows.ToArray(), false_rows.ToArray());
        }

        static public Tuple<object, int>[] GetClassCounts(Datarow[] rows)
        {
            List<object> classes = new List<object>(FindUniqueValues(rows));
            int[] counts = new int[classes.Count];
            for (int i = 0; i < counts.Length; i++) counts[i] = 0;

            for(int i = 0; i < rows.Length; i++)
            {
                object dat = rows[i].values[rows[i].Count - 1];
                if (classes.Contains(dat)) counts[classes.IndexOf(dat)] += 1;
            }

            Tuple<object, int>[] tuples = new Tuple<object, int>[classes.Count];
            for (int i = 0; i < tuples.Length; i++) tuples[i] = new Tuple<object, int>(classes[i], counts[i]);

            return tuples;
        }
    }
}
