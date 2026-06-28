using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraDirector : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineCamera _camera;
    [SerializeField] private GameBootstrapper _gameBootstrapper;
    private TurnManager _turnManager;

    private Dictionary<Character, Transform> _characterViews = new Dictionary<Character, Transform>();

    private void Start()
    {
        GameBootstrapper.OnGameStart += Initialize;
    }

    private void Initialize()
    {
        _turnManager = _gameBootstrapper.GetTurnManager();
        _turnManager.OnTurnStarted += HandleTurnStarted;
    }

    public void RegisterCharacter(Character logic, Transform viewTransform)
    {
        _characterViews[logic] = viewTransform;
    }

    private void HandleTurnStarted(Character activeCharacter)
    {
        if (_characterViews.TryGetValue(activeCharacter, out Transform targetView))
        {
            _camera.Follow = targetView;
        }
    }

    private void OnDestroy()
    {
        GameBootstrapper.OnGameStart -= Initialize;
        _turnManager.OnTurnStarted -= HandleTurnStarted;
    }
}
