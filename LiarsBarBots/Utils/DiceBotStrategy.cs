using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiarsBarBots.Utils
{
    using DiceBet = (int amount, int face);

    public static class DiceBotStrategy
    {
        public struct DiceTurnStats
        {
            public float current_bid_probability;
            public int expected_total;
            public (DiceBet bid, float probability) best_bid;
        }

        public static int CountMatchingDice(bool isTraditional, DiceBet bid, List<int> dice)
        {
            int matchingDice = 0;
            for (int i = 0; i < dice.Count; i++)
            {
                var die = dice[i];
                if (die == bid.face)
                {
                    matchingDice++;
                }
                else if (die == 1 && isTraditional)
                {
                    matchingDice++;
                }
            }
            return matchingDice;
        }

        public static float CalculateBidProbability(bool isTraditional, int unknownDiceCount, int requiredFromUnknown)
        {
            float p = isTraditional ? 2f / 6f : 1f / 6f;
            var currentBidProbability = 0f;
            for (var i = requiredFromUnknown; i <= unknownDiceCount; i++)
            {
                var combinations = MathHelpers.comb(unknownDiceCount, i);
                var probability = combinations * MathF.Pow(p, i) * MathF.Pow(1f - p, unknownDiceCount - i);
                currentBidProbability += probability;
            }

            return currentBidProbability;
        }

        public static (DiceBet bet, float probability) CalculateBestBet(bool isTraditional, int totalDice, DiceBet currentBid, List<int> yourDice)
        {
            List<DiceBet> validBets = new List<DiceBet>();
            for (int q = currentBid.amount; q <= totalDice; q++)
            {
                var f = q == currentBid.amount ? currentBid.face + 1 : 1;
                for (; f < 7; f++)
                {
                    if (q == currentBid.amount && f <= currentBid.face) continue;
                    validBets.Add((q, f));
                }
            }

            float bestBidProbability = 0f;
            DiceBet bestBid = (0, 0);
            int unknownDice = totalDice - yourDice.Count;
            foreach (var bid in validBets)
            {
                // calc matching dice
                var matchingDice = CountMatchingDice(isTraditional, bid, yourDice);

                var requiredFromUnknown = bid.amount - matchingDice;
                if (requiredFromUnknown < 0) requiredFromUnknown = 0;

                float p = isTraditional ? 2f / 6f : 1f / 6f;

                var betProbability = CalculateBidProbability(isTraditional, unknownDice, requiredFromUnknown);
                if (betProbability > bestBidProbability)
                {
                    bestBidProbability = betProbability;
                    bestBid = bid;
                }
                else if (betProbability == bestBidProbability)
                {
                    if (bid.amount > bestBid.amount || bid.face > bestBid.face)
                    {
                        bestBidProbability = betProbability;
                        bestBid = bid;
                    }
                }
            }

            return (bestBid, bestBidProbability);
        }

        public static DiceTurnStats CalculateDiceProbability(bool isTraditional, int totalDice, DiceBet currentBid, List<int> yourDice)
        {
            int unknownDice = totalDice - yourDice.Count;

            var matchingDice = CountMatchingDice(isTraditional, currentBid, yourDice);

            float p = isTraditional ? 2f / 6f : 1f / 6f;
            var expected_from_unknown = unknownDice * p;
            var expected_total = expected_from_unknown + matchingDice;

            int requiredFromUnknown = currentBid.amount - matchingDice;
            if (requiredFromUnknown < 0) requiredFromUnknown = 0;

            var currentBidProbability = CalculateBidProbability(isTraditional, unknownDice, requiredFromUnknown);
            var best_bid = CalculateBestBet(isTraditional, totalDice, currentBid, yourDice);
            return new DiceTurnStats { best_bid = best_bid, current_bid_probability = currentBidProbability, expected_total = Mathf.CeilToInt(expected_total) };
        }
    }

    public static class MathHelpers
    {
        public static long comb(int x, int y)
        {
            return perm(x, y) / Factorial(y);
        }

        public static long perm(int x, int y)
        {
            return FactorialDivision(x, x - y);
        }

        private static long FactorialDivision(int topFactorial, int divisorFactorial)
        {
            long result = 1;
            for (int i = topFactorial; i > divisorFactorial; i--)
                result *= i;
            return result;
        }

        private static long Factorial(int i)
        {
            if (i <= 1) return 1;
            return i * Factorial(i - 1);
        }
    }
}
