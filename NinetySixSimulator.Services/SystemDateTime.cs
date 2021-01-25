using System;

namespace NinetySixSimulator.Services
{
    internal class SystemDateTime : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
