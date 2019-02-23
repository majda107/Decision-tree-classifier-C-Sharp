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
            // SOME BASIC EXAMPLES OF USING THIS LIBRARY

            // begin of data loading

            Datarow[] testdata = Datarow.GetDatarowsFromCSV(@"C:\Users\Marián Trpkoš\source\repos\DecisionTreeClassifier\DecisionTreeClassifierV2\testdata.csv");

            Datarow[] training = Datarow.GetDatarowsFromStringArray(new string[]
            {
                "Green;3;Apple",
                "Yellow;3;Apple",
                "Red;1;Grape",
                "Red;1;Grape",
                "Yellow;3;Lemon",
            });

            // end of data loading

            foreach (object val in DecisionTree.FindUniqueValues(testdata)) Console.WriteLine(val.ToString()); // finding unique values in last column

            Console.WriteLine("----------------");

            foreach (Tuple<object, int> tuple in DecisionTree.GetClassCounts(testdata)) Console.WriteLine($"{tuple.Item1.ToString()}: {tuple.Item2}"); // getting counts of unique values

            Console.WriteLine("----------------");

            Tuple<Datarow[], Datarow[]> truefalse_rows = DecisionTree.Partition(testdata, new Question(1, testdata[1].values[1])); // partition data with one basic question - Is <something> orange?
            Console.WriteLine("TRUE: ");
            foreach (Datarow row in truefalse_rows.Item1) Console.WriteLine(row.ToString()); // printing all true rows
            Console.WriteLine("FALSE: ");
            foreach (Datarow row in truefalse_rows.Item2) Console.WriteLine(row.ToString()); // printing all false rows

            Console.WriteLine("----------------");

            Datarow[] lots_of_mixing = Datarow.GetDatarowsFromStringArray(new string[] { "Apple", "Orange", "Grape", "Grapefruit", "Blueberry" });
            Console.WriteLine(DecisionTree.Gini(lots_of_mixing)); // just a basic test of gini - value which tells how many different kinds of things there are (uncertainty)

            Console.WriteLine("----------------");

            double uncertainty = DecisionTree.Gini(training); // getting uncertainty for training dataset
            Console.WriteLine(uncertainty); // pritning this uncertainty
            Tuple<Datarow[], Datarow[]> truefalse_training = DecisionTree.Partition(training, new Question(0, "Red")); // splitting data with question Is <something> red?
            Console.WriteLine(DecisionTree.InfoGain(truefalse_training.Item1, truefalse_training.Item2, uncertainty)); // getting info (numeric value) how efficient this question is (higher = better)

            Console.WriteLine("----------------");

            var bestdata = DecisionTree.FindBestQuestion(training); // finding best question to ask for training data
            Console.WriteLine(bestdata.Item1); // printing gain
            Console.WriteLine(bestdata.Item2.ToString()); // printing question 

            Console.WriteLine("----------------");

            DecisionTree trainingDataTree = new DecisionTree(training); // creating decision tree classifier with training dataset
            trainingDataTree.BuildTree(); // building tree

            trainingDataTree.PrintTree(); // tree visualized

            Console.WriteLine(); // spacing lel

            // classifying data
            Console.WriteLine((trainingDataTree.Classify(new Datarow("Yellow;5")).ToString())); // should output: can be both apple and lemon
            Console.WriteLine((trainingDataTree.Classify(new Datarow("Red;2")).ToString())); // should output: can be only grape
            Console.WriteLine((trainingDataTree.Classify(new Datarow("Green;10")).ToString())); // should ouput: can be only apple

            Console.ReadKey();
        }
    }
}
