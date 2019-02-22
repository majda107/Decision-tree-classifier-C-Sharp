using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionTreeClassifierV2.Dataset;
using System.Threading.Tasks;

namespace DecisionTreeClassifierV2.DecisionTreeClassifier
{
    class DecisionTree
    {
        public Node start_node { get; private set; }
        public DecisionTree(Node start_node)
        {
            this.start_node = start_node;
        }

        private Node ClassifyData(Datarow row, Node node)
        {
            if (node is Leaf) return node;

            if ((node as DecisionNode).question.Match(row)) return (ClassifyData(row, (node as DecisionNode).true_branch));
            else return (ClassifyData(row, (node as DecisionNode).false_branch));
        }

        public Leaf Classify(Datarow row)
        {
            return (Leaf)ClassifyData(row, this.start_node);
        }

        static public void PrintTree(Node startNode, string spacing)
        {
            if (startNode is Leaf)
            {
                Console.WriteLine((startNode as Leaf).ToString());
                return;
            }

            Console.WriteLine(spacing + (startNode as DecisionNode).ToString());
            Console.WriteLine(spacing + "--> true");
            PrintTree((startNode as DecisionNode).true_branch, spacing + "  ");

            Console.WriteLine(spacing + "--> false");
            PrintTree((startNode as DecisionNode).false_branch, spacing + "  ");
        }

        static public Node BuildTree(Datarow[] data)
        {
            Tuple<double, Question> best_question_tuple = FindBestQuestion(data);
            if (best_question_tuple.Item1 == 0) return new Leaf(data);

            Tuple<Datarow[], Datarow[]> truefalse_rows = Partition(data, best_question_tuple.Item2);

            var true_branch = BuildTree(truefalse_rows.Item1);
            var false_branch = BuildTree(truefalse_rows.Item2);

            return new DecisionNode(best_question_tuple.Item2, true_branch, false_branch);
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
