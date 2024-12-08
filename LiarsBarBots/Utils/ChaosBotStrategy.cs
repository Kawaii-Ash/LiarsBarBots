using System.Collections.Generic;

namespace LiarsBarBots.Utils
{
    public static class ChaosBotStrategy
    {
        // chaos: 5(KQ), M, C
        // In chaos, we only have 3 cards and can only place 1 at a time
        public static float CalculateCurrentBetChance(int roundCard, List<Card> initPlayerCards, int playerCount)
        {
            var totalCards = 12;
            var possibleMatchingCards = 7;
            var myMatchingCards = CountMatchingCards(initPlayerCards, roundCard);
            var cardsInRound = playerCount * 3;
            float p = cardsInRound / (float)totalCards;

            var unknownMatchingCards = possibleMatchingCards - myMatchingCards;
            return (unknownMatchingCards / (float)possibleMatchingCards) * p;
        }

        // 3 = Chaos
        // 4 = Master
        public static int CountMatchingCards(List<Card> cards, int targetCardType)
        {
            var matchingCardCount = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                var cardType = cards[i].cardtype;
                if (cardType == targetCardType || cardType == 3 || cardType == 4) matchingCardCount++;
            }
            return matchingCardCount;
        }
    }
}
