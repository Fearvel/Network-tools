using System.Threading;

namespace de.fearvel.net.DataTypes
{
    /// <summary>
    /// Function for a Sleep-timer in a different Thread
    /// After a set amount of time the Locked bool will be set false.
    /// <copyright>Andreas Schreiner 2019</copyright>
    /// </summary>
    public class TimeDelay
    {
        /// <summary>
        /// Locked flag
        /// will be false if the set time passes
        /// </summary>
        public bool Locked { get; private set; }

        /// <summary>
        /// time to sleep
        /// until this time has passed Locked will be true
        /// </summary>
        public int SleepTime { get; private set; }

        /// <summary>
        /// Constructor
        /// sets the SleepTime property
        /// starts the thread that will flip the Locked
        /// bool after the sleepTime has passed
        /// </summary>
        /// <param name="sleepTime">milliseconds to sleep</param>
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