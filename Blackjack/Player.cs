using Spectre.Console;

namespace Blackjack;

public class Player
{
    public bool IsStanding { get; set; }
    public readonly Hand Hand = new();
    
    public readonly Stats Stats = new(Game.Settings.StartingMaxHealth, Game.Settings.StartingDamageMultiplier);
    private readonly Color _borderColor = Color.Green;

    public void DisplayPanel()
    {
        var name = "YOU";
        var phrase = string.Empty;
        var revealCards = true;
        
        Display.DisplayPlayerPanel(
            Hand, Stats,
            name,
            phrase,
            _borderColor,
            revealCards,
            IsStanding);
    }
    
    public string ProcessTurn(Game game, Dealer dealer)
    {
        var actionMessage = String.Empty;
        var playerChoice = Menu.PromptTurnActionChoice();
        
        switch (playerChoice)
        {
            case Menu.TurnActionChoice.Hit:
                if (Hand.Cards.Count < Game.Settings.MaxCardCount)
                {
                    IsStanding = false;
                    Hand.AddRandomCard();
                    Game.CurrentTurn = Game.Turn.Dealer;
                }
                else
                {
                    actionMessage = "[red]You cannot ask for more cards. You have reached the limit.[/]";
                }
                break;
            
            case Menu.TurnActionChoice.Stand:
                IsStanding = true;
                Game.CurrentTurn = Game.Turn.Dealer;
                break;
            
            case Menu.TurnActionChoice.Quit:
                Menu.PromptGameQuit();
                break;
        }

        return actionMessage;
    }
}