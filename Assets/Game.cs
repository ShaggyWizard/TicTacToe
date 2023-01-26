using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField] private Slot _slotPrefab;
    [SerializeField] private RectTransform _slotsContainer;
    [SerializeField] private RectTransform _dividersContainer;
    [SerializeField] private float _width;

    [Header("Game settings")]
    [SerializeField] private int _size = 3;
    [SerializeField] private int _minSize = 3;
    [SerializeField] private int _maxSize;
    [SerializeField] private bool _lockInput;
    [SerializeField] private UnityEvent _onXWin;
    [SerializeField] private UnityEvent _onOWin;
    [SerializeField] private UnityEvent _onDraw;
    [SerializeField] private UnityEvent _onGameEnd;


    public int Size => _size;
    public int MinSize => _minSize;
    public int MaxSize => _maxSize;
    public Slot[,] Slots { get; private set; }
    public event Action OnPlayerTurnEnd;


    private IBot _ai;
    private GameType _gameType;

    private enum GameType
    {
        PlayerVsPlayer,
        PlayerVsAi,
        AiVsAi
    }

    public void StartPlayerVsPlayer()
    {
        _gameType = GameType.PlayerVsPlayer;
        GenerateField();
        _lockInput = false;
    }
    public void StartPlayerVsAi(bool aiFirst)
    {
        _gameType = GameType.PlayerVsAi;
        GenerateField();
        _lockInput = false;
        _ai = new MinimaxAI();
        if (aiFirst)
            PlaceAi();
    }
    public void StartAiVsAi()
    {
        _gameType = GameType.AiVsAi;
        GenerateField();
        _lockInput = true;
        _ai = new MinimaxAI();
        PlaceAi();
    }

    public void PlacePlayer(Slot slot)
    {
        if (slot.State != Player.N || _lockInput)
        {
            return;
        }

        if (Utils.IsMax(Slots))
            slot.PlaceX();
        else
            slot.PlaceO();

        if (CheckEnd())
            return;

        if (_gameType == GameType.PlayerVsAi)
            PlaceAi();
    }
    public void PlaceAi()
    {
        StartCoroutine(PlaceAiRoutine());
    }
    private IEnumerator PlaceAiRoutine()
    {
        _lockInput = true;
        yield return null;
        _ai.Place(Slots, DepthData.instance.depth[_size]);
        yield return null;

        if (CheckEnd())
            yield break;

        _lockInput = false;
        if (_gameType == GameType.AiVsAi)
        {
            yield return new WaitForSeconds(0.5f);
            PlaceAi();
        }
    }

    public void GenerateField()
    {
        if (_size < 1) { return; }

        Slots = new Slot[_size, _size];

        for (int y = 0; y < _size; y++)
        {
            var row = new GameObject();
            row.transform.SetParent(_slotsContainer);
            var layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.childForceExpandHeight = true;
            layout.childForceExpandWidth = true;
            layout.GetComponent<RectTransform>().localScale = Vector2.one;

            for (int x = 0; x < _size; x++) 
            {
                Slots[x, y] = Instantiate(_slotPrefab, row.transform);
                Slots[x, y].OnClick += PlacePlayer;
            }
        }

        float space = _dividersContainer.GetComponent<RectTransform>().rect.width / _size;
        for (int i = 1; i < _size; i++)
        {
            var dividerX = new GameObject("divider x " + i, typeof(RectTransform)).GetComponent<RectTransform>();
            dividerX.transform.SetParent(_dividersContainer);
            dividerX.localScale = Vector2.one;
            dividerX.anchorMin = Vector2.zero;
            dividerX.anchorMax = Vector2.up;
            dividerX.sizeDelta = new Vector2(_width, 0);
            dividerX.anchoredPosition = new Vector2(i * space, 0);
            dividerX.gameObject.AddComponent<Image>();


            var dividerY = new GameObject("divider y " + i, typeof(RectTransform)).GetComponent<RectTransform>();
            dividerY.transform.SetParent(_dividersContainer);
            dividerY.localScale = Vector2.one;
            dividerY.anchorMin = Vector2.zero;
            dividerY.anchorMax = Vector2.right;
            dividerY.sizeDelta = new Vector2(0, _width);
            dividerY.anchoredPosition = new Vector2(0, i * space);
            dividerY.gameObject.AddComponent<Image>();
        }
    }
    public void IncreaseSize()
    {
        _size = Mathf.Min(_maxSize, _size + 1);
    }
    public void DecreaseSize()
    {
        _size = Mathf.Max(_minSize, _size - 1);
    }
    private bool CheckEnd()
    {
        var winner = Utils.CheckWin(Utils.GetBoard(Slots));
        if (winner == Player.N)
        {
            if (Utils.HasSpace(Slots))
                return false;

            _onDraw?.Invoke();

            EndGameWithDelay();
            return true;
        }

        if (winner == Player.X)
            _onXWin?.Invoke();
        else if (winner == Player.O)
            _onOWin?.Invoke();

        EndGameWithDelay();
        return true;
    }
    private void Clear()
    {
        foreach (Transform child in _slotsContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _dividersContainer)
        {
            Destroy(child.gameObject);
        }
    }
    private void EndGameWithDelay()
    {
        StartCoroutine(EndGameRoutine());
    }
    private IEnumerator EndGameRoutine()
    {
        yield return new WaitForSeconds(2f);

        Clear();
        _onGameEnd?.Invoke();
    }
}
