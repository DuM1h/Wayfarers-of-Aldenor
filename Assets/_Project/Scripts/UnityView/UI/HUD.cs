using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("HealthBar Settings")]
    [SerializeField] private float changeDelay = 0.5f;
    [SerializeField] private float changeSpeed = 2.5f;

    private float receiveDamageTime;
    private float targetHealthFill;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _mpTxt;
    [SerializeField] private TextMeshProUGUI _apTxt;
    [SerializeField] private TextMeshProUGUI _bpTxt;
    [SerializeField] private TextMeshProUGUI _hpTxt;
    [SerializeField] private Image _hpImage;
    [SerializeField] private Image _hpDamagedImage;

    [Header("References")]
    [SerializeField] private PlayerControllerView _playerController;
    [SerializeField] private GameBootstrapper _gameBootstrapper;
    private TurnManager _turnManager;
    private Character _playerCharacter;


    public void Initialize()
    {
        if (!ValidateReferences())
            return;

        _turnManager = _gameBootstrapper.GetTurnManager();

        _playerCharacter = _playerController.GetPlayerCharacter();

        _playerCharacter.OnResourcesChanged += UpdatePoints;
        _playerCharacter.Stats.OnHealthChanged += UpdateHP;

        UpdateHP(_playerCharacter.Stats.CurrentHealth, _playerCharacter.Stats.MaxHealth);
        UpdatePoints();
    }

    private bool ValidateReferences()
    {
        bool isValid = true;
        if (_mpTxt == null)
        {
            Debug.LogError("HUD: Не призначенe посилання на MpTxt!");
            isValid = false;
        }
        if (_apTxt == null)
        {
            Debug.LogError("HUD: Не призначене посилання на ApTxt!");
            isValid = false;
        }
        if (_bpTxt == null)
        {
            Debug.LogError("HUD: Не призначене посилання на BbTxt!");
            isValid = false;
        }
        if (_hpTxt == null)
        {
            Debug.LogError("HUD: Не призначене посилання на HpTxt!");
            isValid = false;
        }
        if (_hpImage == null)
        {
            Debug.LogError("HUD: Не призначенe посилання на HpImage!");
            isValid = false;
        }
        if (_hpDamagedImage == null)
        {
            Debug.LogError("HUD: Не призначене посилання на HpDamagedImage!");
            isValid = false;
        }
        if (_playerController == null)
        {
            Debug.LogError("HUD: Не призначене посилання на PlayerController!");
            isValid = false;
        }
        if (_gameBootstrapper == null)
        {
            Debug.LogError("HUD: Не призначене посилання на GameBootstrapper!");
            isValid = false;
        }
        return isValid;
    }

    void Update()
    {
        if (Time.time - receiveDamageTime > changeDelay)
        {
            if (_hpDamagedImage.fillAmount > targetHealthFill)
            {
                _hpDamagedImage.fillAmount -= changeSpeed * Time.deltaTime;
                if (_hpDamagedImage.fillAmount < targetHealthFill)
                {
                    _hpDamagedImage.fillAmount = targetHealthFill;
                }
            }
        }
    }

    private void UpdateHP(int newHealth, int maxHealth)
    {
        if (_playerCharacter == null) return;

        _hpTxt.text = $"{_playerCharacter.Stats.CurrentHealth}/{_playerCharacter.Stats.MaxHealth}";

        targetHealthFill = (float)newHealth / (float)maxHealth;
        _hpImage.fillAmount = (float)newHealth / (float)maxHealth;

        if (_hpDamagedImage.fillAmount > targetHealthFill)
        {
            receiveDamageTime = Time.time;
        }
    }

    private void UpdatePoints()
    {
        if (_playerCharacter == null) return;

        _mpTxt.text = $"MP: {_playerCharacter.AvailableMovementPoints}/{_playerCharacter.MaxMovementPoints}";
        _apTxt.text = $"AP: {_playerCharacter.AvailableActionPoints}/{_playerCharacter.MaxActionPoints}";
        _bpTxt.text = $"BP: {_playerCharacter.AvailableBonusActions}/{_playerCharacter.MaxBonusActions}";
    }

    private void OnDestroy()
    {
        if (_playerCharacter != null)
        {
            _playerCharacter.Stats.OnHealthChanged -= UpdateHP;
            _playerCharacter.OnResourcesChanged -= UpdatePoints;
        }
    }

    public void SkipTurn()
    {
        if (_turnManager == null)
            return;
        _turnManager.ForceEndTurn();
    }
}