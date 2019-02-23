using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace DecisionTreeClassifierV2.Dataset
{
    [Serializable()]
    class Datarow
    {
        public object[] values { get; private set; }
        public int Count { get
            {
                return this.values.Length;
            } }
        public Datarow(string line)
        {
            string[] lineSplit = line.Split(';');
            values = new object[lineSplit.Length];
            for(int i = 0; i < values.Length; i++)
            {
                int intVal;
                double doubleVal;
                if(double.TryParse(lineSplit[i], out doubleVal))
                {
                    values[i] = (object)doubleVal;
                }
                else if(int.TryParse(lineSplit[i], out intVal))
                {
                    values[i] = (object)intVal;
                }
                else
                {
                    values[i] = (object)lineSplit[i];
                }
            }
        }


        public object this[int column]
        {
            get
            {
                if (column < Count - 1) return this.values[column];
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (column < Count - 1) this.values[column] = value;
                else throw new IndexOutOfRangeException();
            }
        }

        public static Datarow[] GetDatarowsFromCSV(string path)
        {
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                Datarow[] datarows = new Datarow[lines.Length];
                for (int i = 0; i < lines.Length; i++) datarows[i] = new Datarow(lines[i]);

                return datarows;
            }
            else throw new FileNotFoundException();
        }

        public static Datarow[] GetDatarowsFromStringArray(string[] input_data)
        {
            Datarow[] rows = new Datarow[input_data.Length];
            for(int i = 0; i < input_data.Length; i++)
            {
                rows[i] = new Datarow(input_data[i]);
            }
            return rows;
        }

        public override string ToString()
        {
            string str = "";
            for(int i = 0; i < this.values.Length; i++)
            {
                str += values[i].ToString() + " ";
            }
            return str;
        }
    }
}
