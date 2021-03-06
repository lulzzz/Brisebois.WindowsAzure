﻿namespace Brisebois.WindowsAzure.Database
{
    public class RetryParams
    {
        private static readonly RetryParams DefaultRetryParams = new RetryParams(10, 20, 8000, 20);
        private readonly int deltaBackOffInMilliseconds;
        private readonly int maxBackOffDelayInMilliseconds;
        private readonly int maxRetries;
        private readonly int minBackOffDelayInMilliseconds;

        public RetryParams(int maxRetries,
            int minBackOffDelayInMilliseconds,
            int maxBackOffDelayInMilliseconds,
            int deltaBackOffInMilliseconds)
        {
            this.maxRetries = maxRetries;
            this.minBackOffDelayInMilliseconds = minBackOffDelayInMilliseconds;
            this.maxBackOffDelayInMilliseconds = maxBackOffDelayInMilliseconds;
            this.deltaBackOffInMilliseconds = deltaBackOffInMilliseconds;
        }

        public static RetryParams Default
        {
            get { return DefaultRetryParams; }
        }

        public int MaxRetries
        {
            get { return maxRetries; }
        }

        public int MinBackOffDelayInMilliseconds
        {
            get { return minBackOffDelayInMilliseconds; }
        }

        public int MaxBackOffDelayInMilliseconds
        {
            get { return maxBackOffDelayInMilliseconds; }
        }

        public int DeltaBackOffInMilliseconds
        {
            get { return deltaBackOffInMilliseconds; }
        }
    }
}