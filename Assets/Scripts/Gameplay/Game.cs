/*
 * File: Game.cs
 * Description: Manages game state, score, and round logic. Use this to track points and update your score UI.
 * Author: Seifer Albacete, UnPAUSE
 */

using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }

    [Header( "Score Settings" )]
    [SerializeField] TextMeshProUGUI _scoreTextP1;
    [SerializeField] TextMeshProUGUI _scoreTextP2;

    int _scoreP1;
    int _scoreP2;

    void Awake()
    {
        if ( Instance != null && Instance != this )
        {
            Destroy( gameObject );
            return;
        }

        Instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore( PlayerNumber player )
    {
        switch ( player )
        {
            case PlayerNumber.Player1:
                _scoreP1++;
                break;

            case PlayerNumber.Player2:
                _scoreP2++;
                break;
        }

        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if ( _scoreTextP1 != null )
        {
            _scoreTextP1.text = _scoreP1.ToString();
        }

        if ( _scoreTextP2 != null )
        {
            _scoreTextP2.text = _scoreP2.ToString();
        }
    }
}
