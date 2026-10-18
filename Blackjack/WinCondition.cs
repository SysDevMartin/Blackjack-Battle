using Spectre.Console;

namespace Blackjack;

public static class WinCondition
{
    public static void CheckRoundWinCondition(Player player, Dealer dealer)
    {
        Game.CurrentGameState = Game.GameState.Stand;
        
        var playerCardSum = player.Hand.GetEffectiveCardSum();
        var dealerCardSum = dealer.Hand.GetEffectiveCardSum();
        
        var playerDealerSumDifference = playerCardSum - dealerCardSum;
        var isPlayerOverTargetSum = playerCardSum > Settings.TargetCardSum;
        var isDealerOverTargetSum = dealerCardSum > Settings.TargetCardSum;
        
        var playerWins = false;
        var dealerWins = false;

        if (playerCardSum == dealerCardSum)
        {
            playerWins = false;
            dealerWins = false;
        }
        else if (isPlayerOverTargetSum && isDealerOverTargetSum)
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
            if (player.Stats.IsDoubleDownActive || dealer.Stats.IsDoubleDownActive)
            {
                playerDamage *= 2;
            }
            dealer.Stats.TakeDamage(playerDamage);
            
            var previousDamageMultiplier = player.Stats.DamageMultiplier;
            AnsiConsole.MarkupLine($"[green]Dealer takes [bold]{playerDamage}[/] damage and has [bold]{dealer.Stats.CurrentHealth}[/] health left.[/]");
            
            player.Stats.IncreaseDamageMultiplier();
            AnsiConsole.MarkupLine($"Your damage multiplier increased ({previousDamageMultiplier}x --> [bold]{player.Stats.DamageMultiplier}x[/]).");
        }
        else if (dealerWins)
        {
            var dealerDamage = dealer.Stats.GetDamage(dealerCardSum);
            if (dealer.Stats.IsDoubleDownActive || player.Stats.IsDoubleDownActive)
            {
                dealerDamage *= 2;
            }
            player.Stats.TakeDamage(dealerDamage);
            
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

    public static void CheckGameWinCondition(Game game, Player player, Dealer dealer)
    {
        var isGameOver = false;
        
        if (!player.Stats.IsAlive && !dealer.Stats.IsAlive)
        {
            // Both player and dealer are dead.
            isGameOver = true;
            AnsiConsole.MarkupLine("\n[bold][yellow]Game Over! Both you and the dealer died.[/][/]");
        }
        else if (!dealer.Stats.IsAlive)
        {
            // Dealer is dead.
            isGameOver = true;
            AnsiConsole.MarkupLine("\n[bold][green]Victory! You defeated the dealer.[/][/]");
        }
        else if (!player.Stats.IsAlive)
        {
            // Player is dead.
            isGameOver = true;
            AnsiConsole.MarkupLine("\n[bold][red]Game Over! You've been defeated by the dealer.[/][/]");
        }

        if (isGameOver)
        {
            Game.CurrentGameState = Game.GameState.GameOver;
            var menuChoice = Menu.PromptGameOverMenuChoice();
            
            switch (menuChoice)
            {
                case Menu.GameOverMenuChoice.PlayAgain:
                    game.ResetGame(player, dealer);
                    break;
            
                case Menu.GameOverMenuChoice.Quit:
                    Game.CurrentGameState = Game.GameState.Quit;
                    break;
            }
        }
    }
}