using System;
using System.Collections.Generic;
using UnityEngine;

public enum TurnState
{
    FreeExploration,
    Combat,
}

public class TurnManager
{
    public TurnState CurrentState { get; private set; }

    private GameGrid _gameGrid;

    private List<Character> _allCharacters = new List<Character>();
    private Character _playerCharacter;
    private int _currentCharacterIndex = 0;

    private float _transitionTimer = 1.5f;

    public event Action<Character> OnTurnStarted;

    public TurnManager(Character playerCharacter, GameGrid gameGrid)
    {
        _playerCharacter = playerCharacter;
        _gameGrid = gameGrid;
        _allCharacters.Add(playerCharacter);
        CurrentState = TurnState.FreeExploration;
    }

    public void RegisterCharacter(Character character)
    {
        if (!_allCharacters.Contains(character))
        {
            _allCharacters.Add(character);
        }
    }

    public void UnregisterCharacter(Character character) 
    {
        if (_allCharacters.Contains(character))
        {
            _allCharacters.Remove(character);
        }
    }

    public void TickFreeTurn()
    {
        if (CurrentState != TurnState.FreeExploration)
            return;

        _playerCharacter.ResetTurn();

        foreach (var character in _allCharacters)
        {
            if (character != _playerCharacter && !character.IsDead)
            {
                ProcessNonCombatAI(character);
            }
        }

        Debug.Log("Світ зробив один логічний тік.");
    }

    public void EnterCombat()
    {
        CurrentState = TurnState.Combat;

        foreach (var character in _allCharacters)
        {
            character.ResetTurn();
        }
        Debug.Log("Почався покроковий бій!");
    }

    public void ForceEndTurn()
    {
        if (CurrentState == TurnState.Combat)
        {
            AdvanceToNextCharacter();
        }
    }

    private void AdvanceToNextCharacter()
    {
        _currentCharacterIndex = (_currentCharacterIndex + 1) % _allCharacters.Count;
        Character nextChar = _allCharacters[_currentCharacterIndex];

        if (nextChar.IsDead)
        {
            AdvanceToNextCharacter();
            return;
        }

        nextChar.ResetTurn();
        _transitionTimer = 1.5f;
        OnTurnStarted?.Invoke(nextChar);
        Debug.Log($"Хід переходить до: {nextChar.Name}");
    }

    private void ProcessNonCombatAI(Character npc)
    {
        // Тимчасова заглушка для ШІ поза боєм
    }

    public void CheckAndAdvanceCombatTurn()
    {
        Character activeChar = _allCharacters[_currentCharacterIndex];

        if (activeChar.HasExhaustedTurn())
        {
            AdvanceToNextCharacter();
        }
    }

    public void CheckForCombatTriggers()
    {
        foreach (var character in _allCharacters)
        {
            if (!character.IsDead && character is EnemyBrain enemy)
            {
                if (enemy.TryDetectPlayer(_gameGrid))
                {
                    EnterCombat();
                    Debug.Log("БІЙ ПОЧАТО!");
                    break;
                }
            }
        }
    }

    public void Update(float deltaTime)
    {
        if (CurrentState != TurnState.Combat) return;

        if (_transitionTimer > 0)
        {
            _transitionTimer -= deltaTime;
            return;
        }

        Character activeChar = _allCharacters[_currentCharacterIndex];

        if (activeChar.IsMovingVisually) return;

        if (activeChar is EnemyBrain enemy)
        {
            enemy.ProcessTurn(_gameGrid);
        }

        CheckAndAdvanceCombatTurn();
    }
}
