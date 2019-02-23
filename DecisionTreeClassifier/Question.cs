using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeClassifierV2.DecisionTreeClassifier
{
    [Serializable()]
    class Question
    {
        private int column;
        public object value { get; private set; }
        public Question(int column, object value)
        {
            this.column = column;
            this.value = value;
        }

        private bool IsNumeric(object obj)
        {
            return (obj is double || obj is int);
        }

        public bool Match(Dataset.Datarow row)
        {
            object rowValue = row[column];
            if(IsNumeric(rowValue) && IsNumeric(value))
            {
                return (double)rowValue >= (double)value;
            }
            else
            {
                return (string)rowValue == (string)value;
            }
        }

        public override string ToString()
        {
            string message = "";
            if(IsNumeric(value))
            {
                message = $"Is col.{column} >= {(double)value}?";
            }
            else
            {
                message = $"Is col.{column} == {(string)value}";
            }
            return message;
        }
    }
}
