using System.Threading;

namespace de.fearvel.net.DataTypes
{
    /// <summary>
    /// Function for a Sleep-timer in a different Thread
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class TimeDelay
    {
        /// <summary>
        /// Locked flag
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// time to sleep
        /// </summary>
        public int SleepTime { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sleepTime"></param>
        public TimeDelay(int sleepTime)
        {
            Locked = true;
            SleepTime = sleepTime;
            Thread thread = new Thread(SleepAndUnlock);
            thread.Start();
        }

        /// <summary>
        /// Threaded Call
        /// </summary>
        public void SleepAndUnlock()
        {
            Thread.Sleep(SleepTime);
            Locked = false;
        }
    }
}