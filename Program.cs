using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionTreeClassifierV2.Dataset;
using DecisionTreeClassifierV2.DecisionTreeClassifier;

namespace DecisionTreeClassifierV2
{
    class Program
    {
        static void Main(string[] args)
        {
            Datarow[] testdata = Datarow.GetDatarowsFromCSV(@"C:\Users\Marián Trpkoš\source\repos\DecisionTreeClassifier\DecisionTreeClassifierV2\testdata.csv");

            foreach(object val in DecisionTree.FindUniqueValues(testdata)) Console.WriteLine(val.ToString());

            Console.WriteLine("----------------");

            foreach (Tuple<object, int> tuple in DecisionTree.GetClassCounts(testdata)) Console.WriteLine($"{tuple.Item1.ToString()}: {tuple.Item2}");

            Console.WriteLine("----------------");

            Tuple<Datarow[], Datarow[]> truefalse_rows = DecisionTree.Partition(testdata, new Question(1, testdata[1].values[1]));
            Console.WriteLine("TRUE: ");
            foreach (Datarow row in truefalse_rows.Item1) Console.WriteLine(row.ToString());
            Console.WriteLine("FALSE: ");
            foreach (Datarow row in truefalse_rows.Item2) Console.WriteLine(row.ToString());

            Console.WriteLine("----------------");

            Datarow[] lots_of_mixing = Datarow.GetDatarowsFromStringArray(new string[] { "Apple", "Orange", "Grape", "Grapefruit", "Blueberry" });
            Console.WriteLine(DecisionTree.Gini(lots_of_mixing));

            Console.WriteLine("----------------");

            Datarow[] training = Datarow.GetDatarowsFromStringArray(new string[]
            {
                "Green;3;Apple",
                "Yellow;3;Apple",
                "Red;1;Grape",
                "Red;1;Grape",
                "Yellow;3;Lemon",
            });

            double uncertainty = DecisionTree.Gini(training);
            Console.WriteLine(uncertainty);
            Tuple<Datarow[], Datarow[]> truefalse_training = DecisionTree.Partition(training, new Question(0, "Red"));
            Console.WriteLine(DecisionTree.InfoGain(truefalse_training.Item1, truefalse_training.Item2, uncertainty));

            Console.WriteLine("----------------");

            var bestdata = DecisionTree.FindBestQuestion(training);
            Console.WriteLine(bestdata.Item1);
            Console.WriteLine(bestdata.Item2.ToString());

            Console.WriteLine("----------------");

            DecisionTree trainingDataTree = new DecisionTree(DecisionTree.BuildTree(training));
            DecisionTree.PrintTree(trainingDataTree.start_node, "");

            Console.WriteLine();

            Console.WriteLine((trainingDataTree.Classify(new Datarow("Yellow;5")).ToString()));
            Console.WriteLine((trainingDataTree.Classify(new Datarow("Red;2")).ToString()));
            Console.WriteLine((trainingDataTree.Classify(new Datarow("Green;10")).ToString()));

            Console.ReadKey();
        }
    }
}
