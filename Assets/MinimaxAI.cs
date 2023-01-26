using System;
using System.Collections.Generic;
using System.Linq;

public class MinimaxAI : IBot
{
    private bool _isRootMax;
    public void Place(Slot[,] slots, int depth)
    {
        _isRootMax = Utils.IsMax(slots);
        var myLetter = _isRootMax ? Player.X : Player.O;

        var targetEval = _isRootMax ? int.MinValue : int.MaxValue;

        Player[,] board = Utils.CopyBoard(Utils.GetBoard(slots));
        List<Option> options = GetOptions(slots, myLetter, depth, ref targetEval, board);

        Slot targetSlot = options.ToList()[UnityEngine.Random.Range(0, options.Count)].slot;

        var bestOptions = options.Where(option => option.score == targetEval).ToList();
        if (bestOptions.Count != 0)
        {
            int bestWeight = _isRootMax ? bestOptions.Max(option => option.weight) : bestOptions.Min(option => option.weight);
            var bestWeightedOptions = bestOptions.Where(option => option.weight == bestWeight).ToList();
            if (bestWeightedOptions.Count != 0)
            {
                targetSlot = bestWeightedOptions[UnityEngine.Random.Range(0, bestWeightedOptions.Count)].slot;
            }
        }

        if (myLetter == Player.X)
            targetSlot.PlaceX();
        else
            targetSlot.PlaceO();
    }

    private List<Option> GetOptions(Slot[,] slots, Player letter, int depth, ref int targetEval, Player[,] board)
    {
        List<Option> optionsList = new List<Option>();
        var size = slots.GetLength(0);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (slots[x, y].State != Player.N)
                {
                    slots[x, y].SetPrediction(0, 0);
                    continue;
                }

                Player[,] childBoard = Utils.CopyBoard(board);
                childBoard[x, y] = letter;

                Minimax(out int score, out int weight, childBoard, Utils.GetCurrentStep(slots) + 1, depth, !_isRootMax);

                targetEval = _isRootMax ? Math.Max(targetEval, score) : Math.Min(targetEval, score);

                slots[x, y].SetPrediction(score, weight);

                optionsList.Add(new Option(slots[x,y], score, weight));
            }
        }

        return optionsList;
    }

    private void Minimax(out int score, out int weight, Player[,] board, int step, int depth, bool isMax, int alpha = int.MinValue, int beta = int.MaxValue)
    {
        Player winner = Utils.CheckWin(board);

        weight = 0;

        if (winner != Player.N)
        {
            score = Utils.GetWinnerScore(winner, board.Length, step);
            weight = score;
            return;
        }

        if (depth == 0 || step == board.Length)
        {
            score = 0;
            return;
        }

        var maxEval = int.MinValue;
        var minEval = int.MaxValue;
        var size = board.GetLength(0);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (board[x,y] != Player.N)
                    continue;

                Player[,] childBoard = Utils.CopyBoard(board);
                childBoard[x, y] = isMax ? Player.X : Player.O;

                Minimax(out int childScore, out int childWeight, childBoard, step + 1, depth - 1, !isMax, alpha, beta);

                maxEval = Math.Max(maxEval, childScore);
                minEval = Math.Min(minEval, childScore);
                weight += childWeight;

                if (isMax)
                    alpha = Math.Max(alpha, childScore);
                else
                    beta = Math.Min(beta, childScore);
                if (beta <= alpha)
                    break;
            }
        }
        score = isMax ? maxEval : minEval;
        return;
    }


    public struct Option
    {
        public Slot slot;
        public int score;
        public int weight;

        public Option(Slot slot, int score, int weight)
        {
            this.slot = slot;
            this.score = score;
            this.weight = weight;
        }
    }

}