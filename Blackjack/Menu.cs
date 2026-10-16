using Spectre.Console;

namespace Blackjack;

public static class Menu
{
    public enum TurnActionChoice
    {
        Hit,
        Stand,
        DoubleDown,
        Quit,
        None
    }
    
    public enum AfterRoundChoice
    {
        Continue,
        None
    }

    public enum GameOverMenuChoice
    {
        PlayAgain,
        Quit,
        None
    }
    
    public static TurnActionChoice PromptTurnActionChoice()
    {
        AnsiConsole.WriteLine("");
        
        string[] availableChoices =
        [
            "[bold][chartreuse3]Hit[/][/] [dim]- Get a card from the dealer.[/]",
            "[bold][gold3_1]Stand[/][/] [dim]- Stay at current card sum.[/]",
            "[bold][darkorange]Double Down[/][/] [dim]- Double your damage dealt AND damage taken this round.[/]",
            "[bold][red]Quit[/][/]"
        ];
        
        var selectedMenuItem = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]Your turn. What's your decision?[/]")
                .AddChoices(availableChoices));
        
        switch (selectedMenuItem)
        {
            case "[bold][chartreuse3]Hit[/][/] [dim]- Get a card from the dealer.[/]":
                return TurnActionChoice.Hit;
            
            case "[bold][gold3_1]Stand[/][/] [dim]- Stay at current card sum.[/]":
                return TurnActionChoice.Stand;
            
            case "[bold][darkorange]Double Down[/][/] [dim]- Double your damage dealt AND damage taken this round.[/]":
                return TurnActionChoice.DoubleDown;
            
            case "[bold][red]Quit[/][/]":
                return TurnActionChoice.Quit;
        }

        return TurnActionChoice.None;
    }
    
    public static AfterRoundChoice PromptAfterRoundChoice()
    {
        AnsiConsole.WriteLine("");
            
        string[] availableChoices =
        {
            "[bold][chartreuse3]Continue[/][/]"
        };
            
        var selectedMenuItem = AnsiConsole.Prompt(
            new SelectionPrompt<string>().AddChoices(availableChoices));
            
        switch (selectedMenuItem)
        {
            case "[bold][chartreuse3]Continue[/][/]":
                return AfterRoundChoice.Continue;
        }

        return AfterRoundChoice.None;
    }
    
    public static void PromptGameQuit()
    {
        AnsiConsole.WriteLine("");
        if (AnsiConsole.Confirm("[bold]Quit the game?[/] [gray50]All progress will be lost.[/]"))
        {
            Game.CurrentGameState = Game.GameState.Quit;
        }
    }

    public static GameOverMenuChoice PromptGameOverMenuChoice()
    {
        AnsiConsole.WriteLine("");
        
        string[] availableChoices =
        [
            "[bold][chartreuse3]Play Again[/][/]",
            "[bold][red]Quit[/][/]"
        ];
        
        var selectedMenuItem = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold]Play again?[/]")
                .AddChoices(availableChoices));
        
        switch (selectedMenuItem)
        {
            case "[bold][chartreuse3]Play Again[/][/]":
                return GameOverMenuChoice.PlayAgain;
            
            case "[bold][red]Quit[/][/]":
                return GameOverMenuChoice.Quit;
        }

        return GameOverMenuChoice.None;
    }
}