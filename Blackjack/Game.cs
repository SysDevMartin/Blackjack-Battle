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

    public void End()
    {
        AnsiConsole.WriteLine("");
        if (AnsiConsole.Confirm("[bold]Quit the game?[/] [gray50]All progress will be lost.[/]"))
        {
            CurrentGameState = GameState.Quit;
        }
    }

    private void CheckWinCondition(Player player, Dealer dealer)
    {
        CurrentGameState = GameState.Stand;
        var playerCardSum = player.Hand.GetEffectiveCardSum();
        var dealerCardSum = dealer.Hand.GetEffectiveCardSum();
        var playerDealerSumDifference = playerCardSum - dealerCardSum;
        var isPlayerOverTargetSum = playerCardSum > Settings.TargetCardSum;
        var isDealerOverTargetSum = dealerCardSum > Settings.TargetCardSum;
        var playerWins = false;
        var dealerWins = false;
        
        if (isPlayerOverTargetSum && isDealerOverTargetSum)
        {
            if (playerCardSum < dealerCardSum)
            {
                playerWins = true;
            }
            else
            {
                dealerWins = true;
            }
        }
        else if (isDealerOverTargetSum && !isPlayerOverTargetSum)
        {
            playerWins = true;
        }
        else if (isPlayerOverTargetSum && !isDealerOverTargetSum)
        {
            dealerWins = true;
        }
        else if (playerDealerSumDifference > 0)
        {
            playerWins = true;
        }
        else
        {
            dealerWins = true;
        }
        
        AnsiConsole.WriteLine("");
        
        if (playerWins)
        {
            AnsiConsole.MarkupLine("[bold][green]You win the round![/][/]");
        }
        else if (dealerWins)
        {
            AnsiConsole.MarkupLine("[bold][red]Dealer wins the round![/][/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[bold][yellow]Draw![/][/]");
        }
        
        AnsiConsole.MarkupLine($"[darkgoldenrod]Your sum is [bold]{playerCardSum}[/] and dealer's is [bold]{dealerCardSum}[/][/].");
        AnsiConsole.WriteLine("");
        
        if (playerWins)
        {
            var playerDamage = player.Stats.GetDamage(playerCardSum);
            var isDead = dealer.Stats.TakeDamage(playerDamage);
            
            var previousDamageMultiplier = player.Stats.DamageMultiplier;
            AnsiConsole.MarkupLine($"[green]Dealer takes [bold]{playerDamage}[/] damage and has [bold]{dealer.Stats.CurrentHealth}[/] health left.[/]");
            
            player.Stats.IncreaseDamageMultiplier();
            AnsiConsole.MarkupLine($"Your damage multiplier increased ({previousDamageMultiplier}x --> [bold]{player.Stats.DamageMultiplier}x[/]).");
        }
        else if (dealerWins)
        {
            var dealerDamage = dealer.Stats.GetDamage(dealerCardSum);
            var isDead = player.Stats.TakeDamage(dealerDamage);
            
            var previousDamageMultiplier = dealer.Stats.DamageMultiplier;
            AnsiConsole.MarkupLine($"[red]You take [bold]{dealerDamage}[/] damage and have [bold]{player.Stats.CurrentHealth}[/] health left.[/]");
            
            dealer.Stats.IncreaseDamageMultiplier();
            AnsiConsole.MarkupLine($"Dealer's damage multiplier increased ({previousDamageMultiplier}x --> [bold]{dealer.Stats.DamageMultiplier}x[/]).");
        }
        else
        {
            AnsiConsole.MarkupLine("[yellow]No one takes damage.[/]");
        }
    }

    private void PerformGameState(ref string gameMessageMarkupText, Player player, Dealer dealer)
    {
        switch (CurrentGameState)
        {
            case GameState.Play:
                switch (CurrentTurn)
                {
                    case Turn.Player:
                        gameMessageMarkupText = player.PerformTurn(this, dealer);
                        break;
                    
                    case Turn.Dealer:
                        dealer.PerformTurn();
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
                CheckWinCondition(player, dealer);
            }

            PerformGameState(ref gameMessageMarkupText, player, dealer);
        }
    }
}