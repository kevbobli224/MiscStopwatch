using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyStopwatch
{
    class Stopwatch
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public int Id { get; set; }
        public Stopwatch(int id, int hours, int minutes, int seconds)
        {
            Hours = hours;
            Minutes = minutes;
            Seconds = seconds;
            Id = id;
        }
        public Stopwatch()
        {
            Hours = 0;
            Minutes = 0;
            Seconds = 0;
            Id = 0;
        }
        public override string ToString()
        {
            return "Timer: " + Id.ToString();
        }
        public void Increment()
        {
            Seconds++;
            if (Seconds == 60)
            {
                Minutes++;
                Seconds = 0;
                if (Minutes == 60)
                {
                    Hours++;
                    Minutes = 0;
                }
            }
        }
    }
}
