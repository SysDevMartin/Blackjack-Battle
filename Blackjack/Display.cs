using Spectre.Console;

namespace Blackjack;

public static class Display
{
    public static void DisplayGameLogoPanel()
    {
        var figlet = new FigletText("Blackjack Battle").Color(Color.DarkSeaGreen4_1);
        AnsiConsole.Write(figlet);
        AnsiConsole.MarkupLine("  [darkseagreen4]Get the sum of your cards as close as possible to [bold]21[/] without going over.[/]");
        AnsiConsole.MarkupLine("  [darkseagreen4]Winning a round deals damage to the opponent and increases the damage multiplier.[/]");
        AnsiConsole.MarkupLine("  [darkseagreen4]If the dealer's health is depleted, you win. If your health is depleted, you lose.[/]");
        AnsiConsole.MarkupLine("  [darkseagreen4]Purple cards are Aces (either 1 or 11, depending on what's best for the hand).[/]\n");
    }

    public static void DisplayPlayerPanel(
        Hand hand,
        Stats stats,
        string playerName,
        string playerPhrase,
        string portraitAsciiArt,
        Color borderColor,
        string textColorName,
        bool revealCards,
        bool isStanding,
        bool isDoubleDownActive
        )
    {
        var mainPanelBorderColor = borderColor;
        var portraitPanelBorderColor = borderColor;
        
        if (isStanding)
        {
            playerName += " (STANDING)";
            mainPanelBorderColor = Color.LightGoldenrod1;
        }

        var grid = new Grid();
        grid.AddColumn();
        grid.AddColumn();
        
        var cardPanel = GetCardPanel(hand, revealCards);
        var damageMultiplierText = $"{stats.DamageMultiplier}x";
        var cardSum = hand.GetEffectiveCardSum();
        var visibleCardSum = cardSum - hand.Cards[Dealer.HiddenCardIndex].EffectiveValue;
        var displayedCardSum = revealCards ? $"{cardSum}" : $"{visibleCardSum} + ?";
        var headerText = playerPhrase.Length > 0 ? $"{playerName} - {playerPhrase}" : playerName;
        var effectiveDamageText = revealCards ? $"{cardSum * stats.DamageMultiplier}" : $"{visibleCardSum * stats.DamageMultiplier}+";
        var percentageHealth = Math.Round((stats.CurrentHealth / (float)stats.MaxHealth) * 100);
        var sumText = $"[white]Sum: {displayedCardSum}[/]";

        if (isDoubleDownActive)
        {
            effectiveDamageText = revealCards
                ? $"{cardSum * stats.DamageMultiplier * 2}"
                : $"{visibleCardSum * stats.DamageMultiplier * 2}+";
            sumText += $" [darkorange]({effectiveDamageText} DAMAGE)[/]";
        }
        else
        {
            sumText += $" [dim]({effectiveDamageText} damage)[/]";
        }
        
        var rows = new Rows(
            new Markup($"Health: [bold]{stats.CurrentHealth}[/] / [bold]{stats.MaxHealth}[/] [dim]({percentageHealth}%)[/]"),
            new Markup($"Damage multiplier: [bold]{damageMultiplierText}[/]"),
            new Rule { Style = Style.Parse("gray30") },
            cardPanel,
            new Rule { Style = Style.Parse("gray30") },
            new Markup(sumText)
        );

        var panel = new Panel(rows)
        {
            Width = Settings.GamePanelWidth
        }
        .Header(headerText)
        .BorderColor(mainPanelBorderColor);

        var portraitPanel = GetPortraitPanel(portraitAsciiArt, portraitPanelBorderColor, textColorName);
        grid.AddRow(panel, portraitPanel);
        
        //AnsiConsole.Write(panel);
        AnsiConsole.Write(grid);
    }

    public static Panel GetPortraitPanel(string asciiArt, Color borderColor, string textColorName)
    {
        var markup = new Markup($"[{textColorName}]{asciiArt.Trim("\r\n").Trim("\n")}[/]");
        return new Panel(markup)
            {
                Width = 25,
                Height = 10
            }
            .BorderColor(borderColor);
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
            Width = Settings.GamePanelWidth
        }
        .NoBorder();

        return panel;
    }
}