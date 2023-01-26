using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeView : MonoBehaviour
{
    [SerializeField] private GameObject _decreaseButton;
    [SerializeField] private GameObject _increaseButton;
    [SerializeField] private Game _game;
    [SerializeField] private TMPro.TMP_Text _text;
    public void UpdateView()
    {
        _decreaseButton.SetActive(true);
        _increaseButton.SetActive(true);
        if (_game.Size == _game.MinSize)
        {
            _decreaseButton.SetActive(false);
        }
        if (_game.Size == _game.MaxSize)
        {
            _increaseButton.SetActive(false);
        }
        _text.text = $"{_game.Size}x{_game.Size}";
    }
}
