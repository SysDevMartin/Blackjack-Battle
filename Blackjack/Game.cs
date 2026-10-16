using Spectre.Console;

namespace Blackjack;

public class Game
{
    public struct Settings
    {
        public const int GamePanelWidth = 75;
        public const int TargetCardSum = 21;
        public const int StartingCardCount = 2;
        public const int MaxCardCount = 10;
        public const int MinCardValue = 1;
        public const int MaxCardValue = 10;
        public const int StartingMaxHealth = 1000;
        public const int StartingDamageMultiplier = 1;
    }
    
    public enum GameState
    {
        Play,
        Stand,
        Quit
    }

    public enum Turn
    {
        Player,
        Dealer
    }
    
    public static GameState CurrentGameState;
    public static Turn CurrentTurn;
    
    private void InitializeNewRound(Player player, Dealer dealer)
    {
        CurrentGameState = GameState.Play;
        CurrentTurn = Turn.Player;

        dealer.IsStanding = false;
        player.IsStanding = false;
        
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
                var menuChoice = Menu.PromptGameChoice();
                switch (menuChoice)
                {
                    case Menu.MenuChoice.Continue:
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
            Display.DisplayGameLogoPanel();
            AnsiConsole.WriteLine("");
            
            var revealDealerCards = player.IsStanding && dealer.IsStanding;
            dealer.DisplayPanel(revealDealerCards);
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
                WinCondition.CheckGameWinCondition(player, dealer);
            }

            ProcessGameState(ref gameMessageMarkupText, player, dealer);
        }
    }
}