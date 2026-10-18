using Spectre.Console;

namespace Blackjack;

public class Game
{
    public enum GameState
    {
        Play,
        Stand,
        GameOver,
        Quit
    }

    public enum Turn
    {
        Player,
        Dealer
    }
    
    public static GameState CurrentGameState;
    public static Turn CurrentTurn;

    public void ResetGame(Player player, Dealer dealer)
    {
        dealer.Reset();
        player.Reset();
        
        InitializeNewRound(player, dealer);
    }
    
    private void InitializeNewRound(Player player, Dealer dealer)
    {
        CurrentGameState = GameState.Play;
        CurrentTurn = Turn.Player;

        dealer.IsStanding = false;
        player.IsStanding = false;

        dealer.Stats.IsDoubleDownActive = false;
        player.Stats.IsDoubleDownActive = false;
        
        dealer.Hand.Reset();
        player.Hand.Reset();
        
        // Deal a starting hand to dealer and player.
        for (int i = 0; i < Settings.StartingCardCount; i++)
        {
            dealer.Hand.AddRandomCard();
            player.Hand.AddRandomCard();
        }
    }

    private void ProcessGameState(ref string gameMessageMarkupText, Player player, Dealer dealer)
    {
        switch (CurrentGameState)
        {
            case GameState.Play:
                switch (CurrentTurn)
                {
                    case Turn.Player:
                        gameMessageMarkupText = player.ProcessTurn(this, dealer);
                        break;
                    
                    case Turn.Dealer:
                        dealer.ProcessTurn();
                        break;
                }
                break;
            
            case GameState.Stand:
                var menuChoice = Menu.PromptAfterRoundChoice();
                switch (menuChoice)
                {
                    case Menu.AfterRoundChoice.Continue:
                        InitializeNewRound(player, dealer);
                        break;
                }
                break;
        }
    }
    
    public void Run()
    {
        var dealer = new Dealer();
        var player = new Player();
        var gameMessageMarkupText = "";
        
        InitializeNewRound(player, dealer);
        
        while (CurrentGameState != GameState.Quit)
        {
            AnsiConsole.Clear();
            
            // Display game logo and instructions.
            Display.DisplayGameLogoPanel();
            AnsiConsole.WriteLine("");
            
            // Display dealer panel.
            var revealDealerCards = player.IsStanding && dealer.IsStanding;
            dealer.DisplayPanel(revealDealerCards);
            
            // Display player panel.
            AnsiConsole.WriteLine("");
            player.DisplayPanel();
            
            if (gameMessageMarkupText.Length > 0)
            {
                // Show message (for example when the user has too many cards).
                AnsiConsole.MarkupLine(gameMessageMarkupText);
                gameMessageMarkupText = "";
            }

            if (player.IsStanding && dealer.IsStanding)
            {
                WinCondition.CheckRoundWinCondition(player, dealer);
            }

            if (CurrentGameState != GameState.GameOver)
            {
                ProcessGameState(ref gameMessageMarkupText, player, dealer);
            }
            
            WinCondition.CheckGameWinCondition(this, player, dealer);
        }
    }
}