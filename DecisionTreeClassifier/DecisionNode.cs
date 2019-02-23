using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionTreeClassifierV2.Dataset;
using System.Threading.Tasks;

namespace DecisionTreeClassifierV2.DecisionTreeClassifier
{
    [Serializable()]
    class DecisionNode : Node, ICloneable
    {
        public Question question { get; private set; }
        public Node true_branch { get; set; }
        public Node false_branch { get; set; }

        public Datarow[] true_branch_data { get; private set; }
        public Datarow[] false_branch_data { get; private set; }
        public DecisionNode(Question q, Node true_branch, Node false_branch)
        {
            this.question = q;
            this.true_branch = true_branch;
            this.false_branch = false_branch;
        }

        public DecisionNode(Question q, Datarow[] true_branch_data, Datarow[] false_branch_data)
        {
            this.question = q;
            this.true_branch_data = true_branch_data;
            this.false_branch_data = false_branch_data;
        }

        public override string ToString()
        {
            return question.ToString();
        }

        public object Clone()
        {
            return (object)new DecisionNode(this.question, this.true_branch, this.false_branch);
        }
    }
}
