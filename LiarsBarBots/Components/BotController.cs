using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using LiarsBarBots.Utils;

namespace LiarsBarBots.Components
{
    public class BotController : MonoBehaviour
    {
        public Manager manager;
        private PlayerStats playerStats;
        private DiceGameProxy diceGameProxy;
        private DeckGameProxy blorfGameProxy;
        private ChaosGameProxy chaosGameProxy;
        private bool turnFinished;
        public bool canPlay = true;

        void Start()
        {
            playerStats = gameObject.GetComponent<PlayerStats>();
            switch (manager.mode)
            {
                case CustomNetworkManager.GameMode.LiarsDice:
                    {
                        var diceGamePlay = playerStats.GetComponent<DiceGamePlay>();
                        diceGameProxy = new DiceGameProxy(diceGamePlay);
                        if (playerStats.isServer)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                diceGamePlay.DiceValues.Add(UnityEngine.Random.Range(1, 7));
                            }
                        }
                        break;
                    }
                case CustomNetworkManager.GameMode.LiarsDeck:
                    {
                        var blorfGamePlay = playerStats.GetComponent<BlorfGamePlay>();
                        blorfGameProxy = new DeckGameProxy(blorfGamePlay);
                        break;
                    }
                case CustomNetworkManager.GameMode.LiarsChaos:
                    {
                        var chaosGamePlay = playerStats.GetComponent<ChaosGamePlay>();
                        chaosGameProxy = new ChaosGameProxy(chaosGamePlay);
                        break;
                    }
            }
        }

        void Update()
        {
            if (playerStats.Dead || playerStats.Winner || !canPlay) return;
            var alivePlayerCount = manager.Players.Count((x) => !x.Dead);
            if (alivePlayerCount <= 1) return;

            switch (manager.mode)
            {
                case CustomNetworkManager.GameMode.LiarsDice:
                    {
                        playLiarsDice();
                        break;
                    }
                case CustomNetworkManager.GameMode.LiarsDeck: 
                    {
                        playLiarsDeck();
                        break;
                    }
                case CustomNetworkManager.GameMode.LiarsChaos:
                    {
                        playLiarsChaos();
                        break;
                    }
            }
        }

        void playLiarsDice()
        {
            var diceGameManager = manager.DiceGame;
            if (diceGameManager.NetworkCalledLiar || diceGameManager.NetworkCalledSpotOn) return;

            if (playerStats.HaveTurn && !turnFinished)
            {
                diceGameProxy.diceGamePlay.NetworkLooking = true;
                diceGameProxy.diceGamePlay.animator.SetBool("Look", true);
                if (manager.CountDown > 28f) return;
                diceGameProxy.diceGamePlay.NetworkLooking = false;
                diceGameProxy.diceGamePlay.animator.SetBool("Look", false);

                turnFinished = true;
                StartCoroutine(WaitForTurnEnd());
                var isTraditional = diceGameManager.DiceMode == DiceGamePlayManager.dicemode.Traditional;
                var currentBid = (diceGameManager.LastCount, diceGameManager.LastDice);
                var yourDice = diceGameProxy.diceGamePlay.DiceValues.ToList();

                var alivePlayerCount = manager.Players.Count((x) => !x.Dead);
                var turnStats = DiceBotStrategy.CalculateDiceProbability(isTraditional, alivePlayerCount * 5, currentBid, yourDice);

                var bid = turnStats.best_bid.bid;
                Plugin.Logger.LogInfo($"Best Bid probability: {turnStats.best_bid.probability}\nbest bid: {bid.amount} {bid.face}\ncurrent bid probability: {turnStats.current_bid_probability}\nexpected total: {turnStats.expected_total}");

                if (diceGameManager.BetPlaced)
                {
                    if (isTraditional && turnStats.expected_total == diceGameManager.LastCount)
                    {
                        diceGameProxy.CallSpotOn();
                        return;
                    }
                    if (turnStats.current_bid_probability < UnityEngine.Random.value || bid.amount == 0) {
                        
                        diceGameProxy.CallLiar();
                        return;
                    }
                }

                diceGameProxy.PlaceBet(Math.Min(20,bid.amount), Math.Min(6, Math.Max(1, bid.face)));
            }
        }

        private IEnumerator WaitForTurnEnd()
        {
            yield return new WaitForSeconds(3f);
            turnFinished = false;
        }

        void playLiarsDeck()
        {
            var blorfGameManager = manager.BlorfGame;
            var isDevil = blorfGameManager.DeckMode == BlorfGamePlayManager.deckmode.Devil;
            var activeCards = GetActiveCards(blorfGameProxy.blorfGamePlay.Cards);
            if (playerStats.HaveTurn && !turnFinished)
            {
                if (manager.CountDown > 28f) return;

                turnFinished = true;
                StartCoroutine(WaitForTurnEnd());

                if (blorfGameManager.LastBetPlayer != null)
                {
                    // We can call liar
                    if (manager.Players.Where((PlayerStats X) => !X.Fnished && !X.Dead).Count() == 2 && blorfGameManager.LastBetPlayer != gameObject && blorfGameManager.LastBetPlayer.GetComponent<BlorfGamePlay>().Cards.Where((GameObject X) => X.activeSelf).Count() == 0)
                    {
                        blorfGameProxy.CallLiar();
                        return;
                    }
                    var alivePlayerCount = manager.Players.Count((x) => !x.Dead);
                    var allCards = GetAllCards(blorfGameProxy.blorfGamePlay.Cards);
                    var truthProbability = DeckBotStrategy.CalculateCurrentBetChance(isDevil, blorfGameManager.NetworkRoundCard, allCards, blorfGameManager.LastRound.Count, alivePlayerCount);
                    var randValue = UnityEngine.Random.value;

                    Plugin.Logger.LogInfo($"Truth Probability: {truthProbability}, rand: {randValue}");
                    if (truthProbability < randValue)
                    {
                        blorfGameProxy.CallLiar();
                        return;
                    }
                }

                // we need to place 1-3 cards
                DeckBotStrategy.SelectCards(activeCards, blorfGameManager.NetworkRoundCard);
                blorfGameProxy.ThrowCards();
            }
        }

        void playLiarsChaos()
        {
            var chaosGameManager = manager.ChaosGame;
            if (playerStats.HaveTurn && !turnFinished)
            {
                if (manager.CountDown > 28f) return;
                turnFinished = true;
                StartCoroutine(WaitForTurnEnd());

                var activeCards = GetActiveCards(chaosGameProxy.chaosGamePlay.Cards);
                if (chaosGameManager.LastBetPlayer != null)
                {
                    // We can call liar
                    if (manager.Players.Where((PlayerStats X) => !X.Fnished && !X.Dead).Count() == 2 && chaosGameManager.LastBetPlayer != gameObject && chaosGameManager.LastBetPlayer.GetComponent<ChaosGamePlay>().Cards.Where((GameObject X) => X.activeSelf).Count() == 0)
                    {
                        chaosGameProxy.CallLiar();
                        return;
                    }
                    var isSafe = false;
                    var matchingCardCount = ChaosBotStrategy.CountMatchingCards(activeCards, chaosGameManager.NetworkRoundCard);
                    if (matchingCardCount == activeCards.Count)
                    {
                        // If we aren't the last player, then we can play safe
                        isSafe = manager.Players.Any(x => {
                            var cgp = x.GetComponent<ChaosGamePlay>();
                            return GetActiveCards(cgp.Cards).Count == activeCards.Count;
                        });
                    }
                    if (!isSafe)
                    {
                        var alivePlayerCount = manager.Players.Count((x) => !x.Dead);
                        var allCards = GetAllCards(chaosGameProxy.chaosGamePlay.Cards);
                        var truthProbability = ChaosBotStrategy.CalculateCurrentBetChance(chaosGameManager.NetworkRoundCard, allCards, alivePlayerCount);
                        var randValue = UnityEngine.Random.value;

                        Plugin.Logger.LogInfo($"Truth Probability: {truthProbability}, rand: {randValue}");
                        if (truthProbability < randValue)
                        {
                            chaosGameProxy.CallLiar();
                            return;
                        }
                    } else
                    {
                        Plugin.Logger.LogInfo($"Playing Safe!");
                    }
                }

                // we need to place 1 card
                var randCardNum = UnityEngine.Random.Range(0, activeCards.Count);
                activeCards[randCardNum].Selected = true;
                chaosGameProxy.ThrowCards();
            }
        }

        static List<Card> GetActiveCards(List<GameObject> cards)
        {
            List<Card> activeCards = new List<Card>();
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                if (card.activeSelf)
                {
                    activeCards.Add(card.GetComponent<Card>());
                }
            }
            return activeCards;
        }

        static List<Card> GetAllCards(List<GameObject> cards)
        {
            List<Card> resultCards = new List<Card>();
            for (int i = 0; i<cards.Count;i++)
            {
                resultCards.Add(cards[i].GetComponent<Card>());
            }
            return resultCards;
        }
    }
}