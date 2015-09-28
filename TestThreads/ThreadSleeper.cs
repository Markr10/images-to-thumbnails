using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TestThreads
{
    class ThreadSleeper
    {
        private int sleepTime;
        private static Random random = new Random();

        public int MySleepThread()
        {
            sleepTime = random.Next(1000);
            return sleepTime;
        }
        
        public void mySleepMethod()
        {
            int sleepytime = MySleepThread();
            Thread current = Thread.CurrentThread;
            System.Diagnostics.Debug.WriteLine(current.Name + " sleeping for " + sleepytime);
            Thread.Sleep(sleepytime);
            System.Diagnostics.Debug.WriteLine(current.Name + " awake!");
        }

        string whatWork;
        string info = "nevermind..";

        int numb;
        string name;
        bool yesorno;

        public ThreadSleeper(ValueObject object1)
        {
            numb = object1.getNumb();
            name = object1.getName();
            yesorno = object1.getYesorno();
        }

        public void ShareSomeInfo(string info)
        {
            this.info = info;
        }

        public void DoSomeWork()
        {
            Thread current = Thread.CurrentThread;
            System.Diagnostics.Debug.WriteLine(current.Name + " is running! My work: "+ numb+"/"+name+"/"+yesorno);
        }
    }
}
