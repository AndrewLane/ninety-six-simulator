using System;

namespace NinetySixSimulator.Services.Models;

public class Card
{
    public string Rank { get; set; }
    public string Suit { get; set; }

    public override string ToString()
    {
        return $"{Rank}{Suit}";
    }

    public override bool Equals(object obj)
    {
        // If parameter is null, return false.
        if (obj is null)
        {
            return false;
        }

        // Optimization for a common success case.
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        // If run-time types are not exactly the same, return false.
        if (GetType() != obj.GetType())
        {
            return false;
        }

        return (obj as Card).RankToNumber() == RankToNumber();
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public static bool operator <(Card first, Card second)
    {
        return first.RankToNumber() < second.RankToNumber();
    }

    public static bool operator >(Card first, Card second)
    {
        return first.RankToNumber() > second.RankToNumber();
    }

    public static bool operator ==(Card first, Card second)
    {
        return first.Equals(second);
    }

    public static bool operator !=(Card first, Card second)
    {
        return !first.Equals(second);
    }


    private int RankToNumber()
    {
        return Rank switch
        {
            "A" => 14,
            "K" => 13,
            "Q" => 12,
            "J" => 11,
            "T" => 10,
            _ => Convert.ToInt32(Rank),
        };
    }
}
