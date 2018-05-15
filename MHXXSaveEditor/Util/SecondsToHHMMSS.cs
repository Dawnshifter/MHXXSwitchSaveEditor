using System;

namespace MHXXSaveEditor.Util
{
    class SecondsToHHMMSS
    {
        public TimeSpan GetTime(int secs)
        {
            TimeSpan time = TimeSpan.FromSeconds(secs);

            return time;
        }

    }
}
