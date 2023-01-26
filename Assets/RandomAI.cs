using System;
using System.Collections.Generic;
using System.Linq;

public class RandomAI : IBot
{
    public void Place(Slot[,] slots, int depth)
    {
        var myLetter = Utils.IsMax(slots) ? Player.X : Player.O;

        List<Slot> available = new List<Slot>();

        for (int y = 0; y < slots.GetLength(1); y++)
        {
            for (int x = 0; x < slots.GetLength(0); x++)
            {
                if (slots[x, y].State != Player.N)
                {
                    available.Add(slots[x, y]);
                    continue;
                }
            }
        }

        var targetSlot = available[UnityEngine.Random.Range(0, available.Count)];

        if (myLetter == Player.X)
            targetSlot.PlaceX();
        else
            targetSlot.PlaceO();
    }
}
