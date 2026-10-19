namespace BlackjackBattle;

public class Hand
{
    public readonly List<Card> Cards = new();
    
    private readonly Random _randomNumberGenerator = new();

    public void Reset()
    {
        Cards.Clear();
    }

    public int GetEffectiveCardSum()
    {
        var sum = 0;
        foreach (var card in Cards)
        {
            sum += card.EffectiveValue;
        }
        return sum;
    }

    private int GetNonAceEffectiveCardSum()
    {
        var sum = 0;
        foreach (var card in Cards.Where(x => !x.IsAce))
        {
            sum += card.EffectiveValue;
        }
        return sum;
    }

    public void AddRandomCard()
    {
        Cards.Add(GetRandomCard());
        RecalculateCardValues(GetNonAceEffectiveCardSum());
    }

    private void RecalculateCardValues(int nonAceCardSum)
    {
        var sum = nonAceCardSum;
        foreach (var card in Cards.Where(x => x.IsAce))
        {
            card.UpdateAceValue(sum);
            sum += card.EffectiveValue;
        }
    }
    
    private Card GetRandomCard()
    {
        // Use MaxCardValue + 1 to account for the fact that the
        // random function starts at value 0, not 1.
        var randomNumber = _randomNumberGenerator.Next(
            Settings.MinCardValue,
            Settings.MaxCardValue + 1
        );
        
        return new Card(randomNumber);
    }
}