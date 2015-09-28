﻿using System;
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

                string sayWhat;
        string info = "nevermind..";

        public ThreadSleeper(string sayWhat)
        {
            this.sayWhat = sayWhat;
        }

        public void ShareSomeInfo(string info)
        {
            this.info = info;
        }

        public void DoSomeWork()
        {
            Thread current = Thread.CurrentThread;
            System.Diagnostics.Debug.WriteLine(current.Name + " is running! " + sayWhat + ". Hear me out: "+info);
        }
    }
}
