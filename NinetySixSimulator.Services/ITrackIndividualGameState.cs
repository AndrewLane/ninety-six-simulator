﻿namespace NinetySixSimulator.Services;

public interface ITrackIndividualGameState
{
    TimeSpan MaxDuration { get; }
    PlayerGameState FirstPlayerState { get; set; }
    PlayerGameState SecondPlayerState { get; set; }
    bool TimedOut { get; }
    TimeSpan TimeElapsed { get; }

    ITrackIndividualGameState InitializeGameState();
    void Tick(TimeSpan time);

    ISingleGameStats Stats { get; set; }
}
