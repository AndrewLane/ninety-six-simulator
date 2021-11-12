namespace NinetySixSimulator.Tests;

public class TrackIndividualGameStateTests
{
    [Fact]
    public void TestInitializeGameState()
    {
        var gameStateTracker = new TrackIndividualGameState();
        var gameState = gameStateTracker.InitializeGameState();

        PlayerGameStateInitializationAssertions(gameState.FirstPlayerState);
        PlayerGameStateInitializationAssertions(gameState.SecondPlayerState);

        Assert.True(gameState.TimedOut == false);
        Assert.True(gameState.TimeElapsed == Constants.GamePlayParameters.TimeToShuffle + Constants.GamePlayParameters.TimeToDeal);

        Assert.True(gameState.MaxDuration >= TimeSpan.FromMinutes(Constants.GamePlayParameters.MinGameLengthInMinutes));
        Assert.True(gameState.MaxDuration <= TimeSpan.FromMinutes(Constants.GamePlayParameters.MaxGameLengthInMinutes));

        //make sure all cards are accounted for
        var allCards = gameState.FirstPlayerState.PlayPile.Cards.Concat(gameState.SecondPlayerState.PlayPile.Cards);

        foreach (var suit in new List<string> { Constants.Suits.Clubs, Constants.Suits.Diamonds, Constants.Suits.Hearts, Constants.Suits.Spades })
        {
            foreach (var rank in new List<string> { "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A" })
            {
                Assert.True(allCards.Where(item => item.Rank == rank && item.Suit == suit).Count() == 1);
            }
        }
    }

    [Fact]
    public void TestTimeOut()
    {
        var gameStateTracker = new TrackIndividualGameState();
        var gameState = gameStateTracker.InitializeGameState();

        gameState.Tick(TimeSpan.FromMinutes(Constants.GamePlayParameters.MaxGameLengthInMinutes));

        Assert.True(gameState.TimedOut == true);
    }

    private static void PlayerGameStateInitializationAssertions(PlayerGameState playerGameState)
    {
        Assert.True(playerGameState.CannotContinueBecauseCantPlayEnoughCardsForWar == false);
        Assert.True(playerGameState.GatherPile.Cards.Count == 0);
        Assert.True(playerGameState.PlayedCards.Cards.Count == 0);
        Assert.True(playerGameState.PlayPile.Cards.Count == 26);
    }
}
