using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Player CheckWin(Player[,] board)
    {
        var size = board.GetLength(0);
        Player winner;

        //diagonal '\'
        winner = CheckLine(board, 0, 0, 1, 1);
        if (winner != Player.N)
            return winner;

        //diagonal '/'
        winner = CheckLine(board, size - 1, 0, -1, 1);
        if (winner != Player.N)
            return winner;

        for (int i = 0; i < size; i++)
        {
            //horizontal
            winner = CheckLine(board, i, 0, 0, 1);
            if (winner != Player.N)
                return winner;

            //vertical
            winner = CheckLine(board, 0, i, 1, 0);
            if (winner != Player.N)
                return winner;
        }

        return Player.N;
    }
    private static Player CheckLine(Player[,] board, int startX, int startY, int deltaX, int deltaY)
    {
        Player firstValue = board[startX, startY];

        if (firstValue == Player.N)
            return Player.N;

        for (int i = 1; i < board.GetLength(0); i++)
        {
            if (board[startX + i * deltaX , startY + i * deltaY] != firstValue)
            {
                return Player.N;
            }
        }
        return firstValue;
    }
    public static bool CheckPossibilityForPlayerAt(Player player, Player[,] board, int targetX, int targetY)
    {
        var size = board.GetLength(0);

        Player opponent = player == Player.X ? Player.O : Player.X;

        bool diagonal1 = targetX == targetY;
        bool diagonal2 = targetX == size - targetY - 1;
        bool horizontal = true;
        bool vertical = true;

        for (int i = 0; i < size && diagonal1; i++)
        {
            if (board[i, i] == opponent)
                diagonal1 = false;
        }
        for (int i = 0; i < size && diagonal2; i++)
        {
            if (board[i, size - i - 1] == opponent)
                diagonal2 = false;
        }
        for (int x = 0; x < size && horizontal; x++)
        {
            if (board[x, targetY] == opponent)
                horizontal = false;
        }
        for (int y = 0; y < size && vertical; y++)
        {
            if (board[targetX, y] == opponent)
                vertical = false;
        }

        return diagonal1 || diagonal2 || horizontal || vertical;
    }

    public static int GetWinnerScore(Player winner, int boardSize, int currentStep)
    {
        if (winner == Player.X)
        {
            return boardSize + 1 - currentStep;
        }
        else
        {
            return currentStep - boardSize - 1;
        }
    }
    public static Player[,] CopyBoard(Player[,] board)
    {
        var size = board.GetLength(0);
        Player[,] copy = new Player[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                copy[x, y] = board[x, y];
            }
        }
        return copy;
    }
    public static Player[,] GetBoard(Slot[,] slots)
    {
        var size = slots.GetLength(0);
        Player[,] board = new Player[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                board[x, y] = slots[x, y].State;
            }
        }
        return board;
    }
    public static bool IsMax(Slot[,] slots)
    {
        return GetCurrentStep(slots) % 2 == 0;
    }
    public static int GetCurrentStep(Slot[,] slots)
    {
        var step = 0;
        foreach (var slot in slots)
        {
            if (slot.State != Player.N)
                step++;
        }
        return step;
    }
    public static bool HasSpace(Slot[,] slots)
    {
        foreach (var slot in slots)
        {
            if (slot.State == Player.N)
                return true;
        }
        return false;
    }
}
