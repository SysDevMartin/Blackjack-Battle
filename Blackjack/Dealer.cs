using Spectre.Console;

namespace Blackjack;

public class Dealer
{
    public const int HiddenCardIndex = 1;
    private const int MinCardSumToStand = 17;
    private const int StandWhenReachingSumLeftToTarget = 4;
    private const int PercentageChanceToIgnoreSumWhenBelowTarget = 50;
    private const int SumLeftToTargetForIgnoreChance = 3;
    
    public bool IsStanding { get; set; }
    public readonly Hand Hand = new();
    
    public readonly Stats Stats = new(Game.Settings.StartingMaxHealth, Game.Settings.StartingDamageMultiplier);
    private readonly Color _borderColor = Color.Red;
    private readonly Random _randomNumberGenerator = new();

    public string GetRandomPhrase()
    {
        string[] availablePhrases =
        {
            "What's the move?",
            "Your call.",
            "Hit or stand?",
            "Think carefully now.",
            "What are you feeling?",
            "Trust your instincts.",
            "Your turn to decide.",
            "What's the plan?",
            "Feeling lucky?",
            "Choose wisely.",
            "Let's hear it.",
            "What'll it be?",
            "The next move is yours.",
            "Go with your gut.",
            "How brave are we today?",
            "This one's on you.",
            "Ready to commit?",
            "What's the strategy?",
            "Take your best shot.",
            "No pressure.",
            "A decision must be made.",
            "The cards are waiting.",
            "What's your play?",
            "Stand proud or push your luck?",
            "Big moment.",
            "Don't overthink it. Or maybe do.",
            "The suspense is real.",
            "What are we doing here?",
            "Feeling confident?",
            "You've got options.",
            "Another card maybe?",
            "Happy with what you've got?",
            "How much do you trust that hand?",
            "Risk it or keep it?",
            "Let's see some courage.",
            "A bold choice could pay off. Or cost you everything.",
            "No guts, no glory.",
            "What's the verdict?",
            "Time to make your move.",
            "The table believes in you. Mostly.",
            "Trust the process.",
            "Feeling dangerous?",
            "One choice changes everything.",
            "Don't let the cards intimidate you.",
            "What's the game plan, chief?",
            "You know something I don't?",
            "I hope you have a strategy.",
            "Winging it again?",
            "This could get interesting.",
            "How lucky do you feel right now?",
            "The next card could be beautiful. Or absolutely devastating.",
            "Make it count.",
            "What does your gut say?",
            "What's fate looking like today?",
            "You've come too far to hesitate.",
            "Take your time.",
            "Not too much time.",
            "What's cooking?",
            "This hand has potential. Or potential problems.",
            "Care to tempt fate?",
            "Ready to roll the dice?",
            "Metaphorically speaking, of course.",
            "What's your secret plan?",
            "I can feel a questionable decision coming.",
            "Thinking hard over there?",
            "Boldness is rewarded. Sometimes.",
            "What's the smart move?",
            "What's the fun move?",
            "Do you believe in destiny?",
            "This is where legends are made. Or cautionary tales.",
            "Feeling brave enough for another card?",
            "Happy to stay where you are?",
            "Don't let me influence you.",
            "I'm just the dealer.",
            "You're driving this bus. Hopefully with a license.",
            "Take a chance.",
            "Play it safe. Or surprise us all.",
            "The cards have done their part.",
            "Now it's your turn.",
            "What's the winning move?",
            "Can you read the table?",
            "Your face says one thing.",
            "Risk versus reward.",
            "The eternal struggle.",
            "Let's make some history. Or at least finish this hand.",
            "What's it going to be, card shark?",
            "You feeling lucky, punk?",
            "This hand isn't getting any younger.",
            "A choice stands before you.",
            "Need another card?",
            "Sticking with that hand?",
            "One more card won't hurt... probably.",
            "You standing on that?",
            "Feeling good enough to stand?",
            "Want to take another shot?",
            "This is where things get interesting.",
            "Another card or are you done?",
            "Happy with what you've got?",
            "Are we pushing our luck today?",
            "Stand firm or chase greatness?",
            "One card away from glory. Or disaster.",
            "Feeling brave enough to hit?",
            "Think you've got enough?",
            "Care for another?",
            "Trust that hand?",
            "Do you dare hit?",
            "Playing it safe?",
            "You sure you want to stop there?",
            "That's your final answer?",
            "Looking for something bigger?",
            "It's all or nothing now.",
            "Go big or go bust.",
            "Will the next card save the day?",
            "Want to see one more?",
            "Last chance to change your fate.",
            "Feeling lucky enough to hit?",
            "Is that hand speaking to you?",
            "Can you resist another card?",
            "Bold players hit.",
            "Smart players stand.",
            "The best players know the difference.",
            "What's it gonna be?",
            "Another card could change everything.",
            "Standing already?",
            "Not tempted by one more card?",
            "This hand isn't over yet.",
            "Are we done here?",
            "The next card has your name on it. Or maybe it doesn't.",
            "Stand and hope?",
            "Hit and pray?",
            "What's your level of confidence right now?",
            "Ready to test your luck?",
            "The cards are calling.",
            "Just one more card...",
            "Hit me with your decision.",
            "Let's find out what you're made of.",
            "Need a card?",
            "One more?",
            "Stand pat?",
            "Risk it?",
            "Push your luck?",
            "Going again?",
            "Stay or play?",
            "Hit me?",
            "That's enough?",
            "Taking a chance?",
            "What's the call?",
            "Another round?"
        };

        var randomIndex = _randomNumberGenerator.Next(availablePhrases.Length);
        return availablePhrases[randomIndex];
    }

    public void DisplayPanel(bool revealCards)
    {
        var name = "DEALER";
        var phrase = Game.CurrentTurn == Game.Turn.Dealer ? "Thinking..." : GetRandomPhrase();
        
        Display.DisplayPlayerPanel(
            Hand,
            Stats,
            name,
            phrase,
            _borderColor,
            revealCards,
            IsStanding);
    }

    private void MakeTurnDecision()
    {
        if (!IsStanding)
        {
            var cardSum = Hand.GetEffectiveCardSum();
            
            if (
                (cardSum >= Game.Settings.TargetCardSum) ||
                (Hand.Cards.Count >= Game.Settings.MaxCardCount)
            )
            {
                IsStanding = true;
            }
            else if (cardSum < MinCardSumToStand)
            {
                // Card is below minimum threshold for standing, so the dealer must hit.
                Hand.AddRandomCard();
            }
            else
            {
                // If the other conditions fail, calculate if the dealer should hit or stand.
                var sumLeftToTarget = Game.Settings.TargetCardSum - cardSum;

                // When the card sum left to the target sum is equal to or greater than a certain value,
                // roll the dice to check if the dealer should gamble with one more card.
                var ignoreSumLeftToTarget = (
                    (sumLeftToTarget >= SumLeftToTargetForIgnoreChance) &&
                    (_randomNumberGenerator.Next(0, 100 + 1) >= PercentageChanceToIgnoreSumWhenBelowTarget)
                );

                if ((sumLeftToTarget > StandWhenReachingSumLeftToTarget) || ignoreSumLeftToTarget)
                {
                    Hand.AddRandomCard();
                }
                else
                {
                    IsStanding = true;
                }
            }
        }

        Game.CurrentTurn = Game.Turn.Player;
    }
    
    public void ProcessTurn()
    {
        AnsiConsole.Status()
            .Start("\nDealer's turn. Making a decision...", ctx =>
            {
                var minRandomSleepTime = 3000;
                var maxRandomSleepTime = 5000;
                var randomSleepTime = _randomNumberGenerator.Next(minRandomSleepTime, maxRandomSleepTime + 1);
                
                Thread.Sleep(randomSleepTime);
                MakeTurnDecision();
                
                Game.CurrentTurn = Game.Turn.Player;
            });
    }
}