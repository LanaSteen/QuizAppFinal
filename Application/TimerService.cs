using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class TimerService
    {
        private DateTime _startTime;
        private TimeSpan _timeLimit;
        private DateTime _endTime;

        public TimerService(TimeSpan timeLimit)
        {
            _timeLimit = timeLimit;
        }

        
        public void StartTimer()
        {
            _startTime = DateTime.Now;
            _endTime = _startTime.Add(_timeLimit);
        }

        public bool IsTimeUp()
        {
            return DateTime.Now >= _endTime;
        }

        public void DisplayTimeLeft()
        {
            var timeRemaining = _endTime - DateTime.Now; 

            if (timeRemaining < TimeSpan.Zero)
            {
                timeRemaining = TimeSpan.Zero;
            }
          
            Console.WriteLine($"Time left: {timeRemaining.Minutes:D2}:{timeRemaining.Seconds:D2}");
        }
    }
}
