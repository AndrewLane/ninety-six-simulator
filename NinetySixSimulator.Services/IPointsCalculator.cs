using System.Collections.Generic;
using NinetySixSimulator.Services.Models;

namespace NinetySixSimulator.Services
{
    public interface IPointsCalculator
    {
        int GetPoints(List<Card> cards);
    }
}
