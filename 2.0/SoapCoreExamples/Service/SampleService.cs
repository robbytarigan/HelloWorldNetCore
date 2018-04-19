using System;
using System.Linq;

namespace Service
{
    public class SampleService : IPingService
    {
        public string Ping(string msg)
        {
            return string.Join(string.Empty, msg.Reverse());
        }
    }
}