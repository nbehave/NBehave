using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NBehave.Narrator.Framework.EventListeners.Xml
{
    public class Timer
    {
        public Timer()
        {
            StartTime = DateTime.Now;
            TimeTaken = -1;
        }

        public void Stop()
        {
            TimeSpan timeTaken = DateTime.Now.Subtract(StartTime);
            TimeTaken = timeTaken.TotalSeconds;
        }

        double _timeTaken = -1;
        public double TimeTaken
        {
            get
            {
                if (_timeTaken < 0)
                    Stop();
                return _timeTaken;
            }
            private set
            {
                _timeTaken = value;
            }
        }
       
        public DateTime StartTime { get; private set; }
    }
}
