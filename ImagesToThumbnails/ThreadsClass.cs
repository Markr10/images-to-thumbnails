using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ImagesToThumbnails
{
    class ThreadsClass
    {
        string sayWhat;
        string info = "nevermind..";

        public ThreadsClass()
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
