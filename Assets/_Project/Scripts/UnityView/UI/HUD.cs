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

    private void Start()
    {
        GameBootstrapper.OnGameStart += Initialize;
    }

    public void Initialize()
    {
        _turnManager = _gameBootstrapper.GetTurnManager();

        _playerCharacter = _playerController.GetPlayerCharacter();

        _playerCharacter.OnResourcesChanged += UpdatePoints;
        _playerCharacter.OnHealthChanged += UpdateHP;

        UpdateHP(_playerCharacter.CurrentHealth, _playerCharacter.MaxHealth);
        UpdatePoints();
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

        _hpTxt.text = $"{_playerCharacter.CurrentHealth}/{_playerCharacter.MaxHealth}";

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
        GameBootstrapper.OnGameStart -= Initialize;

        if (_playerCharacter != null)
        {
            _playerCharacter.OnHealthChanged -= UpdateHP;
            _playerCharacter.OnResourcesChanged -= UpdatePoints;
        }
    }

    public void SkipTurn()
    {
        _turnManager.ForceEndTurn();
    }
}