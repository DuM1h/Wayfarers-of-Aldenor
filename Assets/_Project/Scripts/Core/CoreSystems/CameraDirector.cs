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

    public void Initialize()
    {
        if (!ValidateReferences())
            return;
        _turnManager = _gameBootstrapper.GetTurnManager();
        _turnManager.OnTurnStarted += HandleTurnStarted;
    }

    private bool ValidateReferences()
    {
        bool isValid = true;
        if (_camera == null)
        {
            Debug.LogError("CameraDirector: Не призначенe посилання на Camera!");
            isValid = false;
        }
        if (_gameBootstrapper == null)
        {
            Debug.LogError("CameraDirector: Не призначене посилання на GameBootstrapper!");
            isValid = false;
        }
        return isValid;
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
        if ( _turnManager != null ) 
            _turnManager.OnTurnStarted -= HandleTurnStarted;
    }
}
