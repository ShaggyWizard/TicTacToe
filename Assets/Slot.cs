using System;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] TMPro.TMP_Text predictionText;
    [SerializeField] TMPro.TMP_Text stateText;
    public void SetPrediction(int prediction, int weight)
    {
        predictionText.text = "" + prediction + " : "+ weight;
    }
    public event Action<Slot> OnClick;
    public void Click()
    {
        OnClick?.Invoke(this);
    }
    public Player State { get; private set; }


    public void PlaceX()
    {
        stateText.text = "X";
        State = Player.X;
    }
    public void PlaceO()
    {
        stateText.text = "O";
        State = Player.O;
    }
    public void Empty()
    {
        State = Player.N;
    }
}
public enum Player
{
    N,
    X,
    O
}