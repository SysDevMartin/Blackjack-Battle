using Spectre.Console;

namespace Blackjack;

public class Player
{
    private const string TextColorName = "green";
    
    public bool IsStanding { get; set; }
    public readonly Hand Hand = new();
    //public Stats Stats = new(Game.Settings.StartingMaxHealth, Game.Settings.StartingDamageMultiplier);
    public readonly Stats Stats = new(Game.Settings.StartingMaxHealth, 1000);
    
    private readonly Color _borderColor = Color.Green;

    public void Reset()
    {
        IsStanding = false;
        Hand.Cards.Clear();
        Stats.Reset(Game.Settings.StartingMaxHealth, Game.Settings.StartingDamageMultiplier);
    }

    public void DisplayPanel()
    {
        var name = "YOU";
        var phrase = string.Empty;
        var revealCards = true;
        var asciiArt = @"
    _| /-_-\)|/_
   (___\ _ //___\
   (  |\\_/// * \\
    \_| \_((*   *))
    ( |__|_\\  *//
    (o/  _  \_*_/
    //\__|__/\
   // |  | |  |
";
        
        Display.DisplayPlayerPanel(
            Hand, Stats,
            name,
            phrase,
            asciiArt,
            _borderColor,
            TextColorName,
            revealCards,
            IsStanding,
            Stats.IsDoubleDownActive);
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
            
            case Menu.TurnActionChoice.DoubleDown:
                if (!Stats.IsDoubleDownActive)
                {
                    Stats.IsDoubleDownActive = true;
                    Game.CurrentTurn = Game.Turn.Dealer;
                }
                else
                {
                    actionMessage = "[red]You can only Double Down once per round.[/]";
                }
                break;
            
            case Menu.TurnActionChoice.Quit:
                Menu.PromptGameQuit();
                break;
        }

        return actionMessage;
    }
}