using Spectre.Console;

namespace Blackjack;

public class Display
{
    public static void DisplayGameLogoPanel()
    {
        var figlet = new FigletText("Blackjack Battle").Color(Color.DarkSeaGreen4_1);
        AnsiConsole.Write(figlet);
        AnsiConsole.MarkupLine("  [darkseagreen4]Get the sum of your cards as close as possible to [bold]21[/] without going over.[/]");
        AnsiConsole.MarkupLine("  [darkseagreen4]Winning a round deals damage to the opponent and increases the damage multiplier.[/]");
        AnsiConsole.MarkupLine("  [darkseagreen4]If the dealer's health is depleted, you win. If your health is depleted, you lose.[/]");
        AnsiConsole.MarkupLine("  [darkseagreen4]Purple cards are Aces (either 1 or 11, depending on what's best for your hand).[/]\n");
    }

    public static void DisplayPlayerPanel(
        Hand hand,
        Stats stats,
        string playerName,
        string playerPhrase,
        Color borderColor,
        bool revealCards,
        bool isStanding
        )
    {
        if (isStanding)
        {
            playerName += " (STANDING)";
            borderColor = Color.LightGoldenrod1;
        }
        
        var cardPanel = GetCardPanel(hand, revealCards);
        var damageMultiplierText = $"{stats.DamageMultiplier}x";
        var cardSum = hand.GetEffectiveCardSum();
        var visibleCardSum = cardSum - hand.Cards[1].EffectiveValue;
        var displayedCardSum = revealCards ? $"{cardSum}" : $"{visibleCardSum} + ?";
        var headerText = playerPhrase.Length > 0 ? $"{playerName} - {playerPhrase}" : playerName;
        var effectiveDamageText = revealCards ? $"{cardSum * stats.DamageMultiplier}" : $"{visibleCardSum * stats.DamageMultiplier}+";
        var percentageHealth = Math.Round((stats.CurrentHealth / (float)stats.MaxHealth) * 100);
        
        var rows = new Rows(
            new Markup($"Health: [bold]{stats.CurrentHealth}[/] / [bold]{stats.MaxHealth}[/] [dim]({percentageHealth}%)[/]"),
            new Markup($"Damage multiplier: [bold]{damageMultiplierText}[/]"),
            new Rule { Style = Style.Parse("gray30") },
            cardPanel,
            new Rule { Style = Style.Parse("gray30") },
            new Markup($"[white]Sum: {displayedCardSum} [dim]({effectiveDamageText} damage)[/][/]")
            );

        var panel = new Panel(rows)
        {
            Width = Game.Settings.GamePanelWidth
        }
        .Header(headerText)
        .BorderColor(borderColor);
        
        AnsiConsole.Write(panel);
    }

    private static Panel GetCardPanel(Hand hand, bool revealCards)
    {
        var grid = new Grid();
        grid.AddColumns(hand.Cards.Count);

        var cardPanelList = new List<Panel>();

        for (int i = 0; i < hand.Cards.Count; i++)
        {
            var card = hand.Cards[i];
            var reveal = (i != Dealer.HiddenCardIndex) || revealCards;
            var cardPanel = card.GetCardPanel(reveal);
            cardPanelList.Add(cardPanel);
        }
        grid.AddRow(cardPanelList.ToArray());

        var panel = new Panel(grid)
        {
            Width = Game.Settings.GamePanelWidth
        }
        .NoBorder();

        return panel;
    }
}