using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DecisionTreeClassifierV2.Dataset;
using System.Threading.Tasks;

namespace DecisionTreeClassifierV2.DecisionTreeClassifier
{
    class DecisionNode : Node
    {
        public Question question { get; private set; }
        public Node true_branch { get; private set; }
        public Node false_branch { get; private set; }
        public DecisionNode(Question q, Node true_branch, Node false_branch)
        {
            this.question = q;
            this.true_branch = true_branch;
            this.false_branch = false_branch;
        }

        public override string ToString()
        {
            return question.ToString();
        }
    }
}
