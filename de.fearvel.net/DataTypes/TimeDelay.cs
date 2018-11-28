using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace de.fearvel.net.DataTypes
{
    public class TimeDelay
    {
        public bool Locked { get; private set; }
        public int SleepTime { get; private set; }

        public TimeDelay(int sleepTime)
        {
            Locked = true;
            SleepTime = sleepTime;
            Thread thread = new Thread(SleepAndUnlock);
            thread.Start();
        }
        public void SleepAndUnlock()
        {
            Thread.Sleep(SleepTime);
            Locked = false;
        }
    }    
}
