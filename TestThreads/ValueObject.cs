using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestThreads
{
    public class ValueObject
    {
        int numb;
        string name;
        bool yesorno;

        public ValueObject(int numb, string name, bool yesorno)
        {
            this.numb = numb;
            this.name = name;
            this.yesorno = yesorno;
        }

        public ValueObject()
        {
            numb = -1;
            name = "noname";
            yesorno = false;
        }

        public int getNumb()
        {
            return numb;
        }

        public void setNumb(int numb)
        {
            this.numb = numb;
        }

        public string getName()
        {
            return name;
        }

        public void setName(string name)
        {
            this.name = name;
        }

        public bool getYesorno()
        {
            return yesorno;
        }

        public void setYesorno(bool yesorno)
        {
            this.yesorno = yesorno;
        }
    }
}
