using System;
using System.Collections.Generic;

namespace LiarsBarBots.Utils
{
    public static class DeckBotStrategy
    {
        public static void SelectCards(List<Card> cards, int roundCard)
        {
            if (cards.Count == 1)
            {
                cards[0].Selected = true;
                return;
            }

            var hasDevilCard = HasDevilCard(cards);

            var matchingCards = CountMatchingCards(cards, roundCard);
            var junkCards = cards.Count - matchingCards;
            if (hasDevilCard) junkCards -= 1;

            // all shit cards
            if (junkCards == cards.Count)
            {
                var selIdx = UnityEngine.Random.Range(0, cards.Count);
                cards[selIdx].Selected = true;
                return;
            }
            // 3/4 shit cards, 1/2 round/devil cards
            if (junkCards == 4 || junkCards == 3)
            {
                var rVal = UnityEngine.Random.value;
                if (junkCards == 3 && rVal > 0.9f)
                {
                    SelectJunkCards(cards, junkCards, roundCard);
                } else if (rVal > 0.3f)
                {
                    // place the matching card
                    if (matchingCards > 0)
                    {
                        SelectMatchingCards(cards, 1, roundCard);
                    } else
                    {
                        SelectDevilCard(cards);
                    }
                }
                else
                {
                    SelectJunkCards(cards, 1, roundCard);
                }
                return;
            }

            if (junkCards == 2)
            {
                if (UnityEngine.Random.value > 0.5f)
                {
                    SelectJunkCards(cards, 2, roundCard);
                } 
                else if (hasDevilCard)
                {
                    SelectDevilCard(cards);
                } 
                else
                {
                    var amount = matchingCards > 2 ? 2 : 1;
                    SelectMatchingCards(cards, amount, roundCard);
                }

                return;
            }

            if (junkCards == 1)
            {
                if (hasDevilCard && UnityEngine.Random.value > 0.5f)
                {
                    SelectDevilCard(cards);
                }
                else if (UnityEngine.Random.value > 0.5f)
                {
                    SelectJunkCards(cards, 1, roundCard);
                }
                else
                {
                    var amount = matchingCards > 2 ? 2 : 1;
                    SelectMatchingCards(cards, amount, roundCard);
                }
                
                return;
            }

            // All of our cards match!
            if (matchingCards == cards.Count)
            {
                var amount = matchingCards;
                if (matchingCards > 3)
                {
                    amount = UnityEngine.Random.value > 0.5f ? 2 : 3;
                }
                SelectMatchingCards(cards, amount, roundCard);
                return;
            }
            
            // All of our cards match and we have a devil card!
            if (matchingCards == cards.Count - 1 && hasDevilCard)
            {
                if (UnityEngine.Random.value > 0.5f)
                {
                    var amount = matchingCards;
                    if (matchingCards > 3)
                    {
                        amount = UnityEngine.Random.value > 0.5f ? 2 : 3;
                    }
                    SelectMatchingCards(cards, amount, roundCard);
                } else
                {
                    SelectDevilCard(cards);
                }
                return;
            }
        }

        static void SelectDevilCard(List<Card> cards)
        {
            for (int i = 0; i<cards.Count; i++)
            {
                var card = cards[i];
                if (card.cardtype == -1 || card.Devil)
                {
                    card.Selected = true;
                    return;
                }
            }
        }

        static void SelectMatchingCards(List<Card> cards, int amount, int roundCard)
        {
            var amountSelected = 0;
            for (int i = 0; amountSelected < amount; i++)
            {
                var card = cards[i];
                if ((card.cardtype == roundCard || card.cardtype == 4) && !card.Devil)
                {
                    card.Selected = true;
                    amountSelected++;
                }
            }
        }

        static void SelectJunkCards(List<Card> cards, int amount, int roundCard)
        {
            var amountSelected = 0;
            for (int i = 0; amountSelected < amount; i++)
            {
                var card = cards[i];
                if (card.cardtype != roundCard && card.cardtype != -1 && card.cardtype != 4 && !card.Devil)
                {
                    card.Selected = true;
                    amountSelected++;
                }
            }
        }

        public static float CalculateCurrentBetChance(bool isDevil, int roundCard, List<Card> initPlayerCards, int currentBet, int playerCount)
        {
            var totalCards = 20; // 6(KQA) + 2 Jokers
            var possibleMatchingCards = 8;
            if (isDevil && currentBet == 1 && !HasDevilCard(initPlayerCards))
            {
                // it might be a devil card
                possibleMatchingCards += 1;
            }
            var myMatchingCards = CountMatchingCards(initPlayerCards, roundCard);

            // Calculate ratio of matching cards in the current round
            var cardsInRound = playerCount * 5;
            float p = cardsInRound / (float)totalCards;

            // Calculate chance of single card
            var unknownMatchingCards = possibleMatchingCards - myMatchingCards;
            var cardChance = (unknownMatchingCards / (float)possibleMatchingCards) * p;
            return (float)Math.Pow(cardChance, currentBet);
        }

        public static int CountMatchingCards(List<Card> cards, int targetCardType)
        {
            var matchingCardCount = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                var cardType = card.cardtype;
                if ((cardType == 4 || cardType == targetCardType) && !card.Devil)
                {
                    matchingCardCount++;
                }
            }
            return matchingCardCount;
        }

        public static bool HasDevilCard(List<Card> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                if (card.cardtype == -1 || card.Devil) return true;
            }
            return false;
        }
    }
}
