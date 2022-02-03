using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Roll : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _currentScores = new TextMeshProUGUI[2];
    [SerializeField] private TextMeshProUGUI[] _totalScores = new TextMeshProUGUI[2];
    [SerializeField] private Image[] _sides = new Image[2];
    [SerializeField] private Sprite[] _dices;
    [SerializeField] private TextMeshProUGUI _winnerTxt;
    [SerializeField] private GameObject _winnerGO;
    [SerializeField] private Image _dice;

    private string[] _players = new string[2]{"PLAYER 1","PLAYER 2"};
    private int[] _totals = new int[2]{0,0};
    private int _currentPlayer, _score = 0;
        
    private void Start() {
        Restart();
    }

    public void Restart()
    {
        _sides[1].CrossFadeAlpha(.5f, .01f, true);
        for (int i = 0; i < _players.Length; i++)
        {
            _currentScores[i].text = "0";
            _totalScores[i].text = "0";
            _totals[i] = 0;
        }
        _dice.sprite = _dices[0];
        _currentPlayer = _score = 0;
        _sides[0].CrossFadeAlpha(.9f, .01f, true);
        _winnerGO.SetActive(false);
    }

    public void RollDice()
    {
        int x = Random.Range(1,6);
        _dice.sprite = _dices[x];
        if(x == 1) SwitchPlayer();
        else
        {
            _score += x;
            _currentScores[_currentPlayer].text = _score.ToString();
        }
    }

    public void Hold()
    {
        _totals[_currentPlayer] += _score;
        _totalScores[_currentPlayer].text = _totals[_currentPlayer].ToString();
        if(_totals[_currentPlayer] >= 100) Win();
        SwitchPlayer();
    }

    public void SwitchPlayer()
    {
        _sides[_currentPlayer].CrossFadeAlpha(.5f, .01f, true);
        _currentPlayer = _currentPlayer == 0? 1 : 0;
        _sides[_currentPlayer].CrossFadeAlpha(.9f, .01f, true);
        _score = 0;
        _currentScores[0].text = _currentScores[1].text = "0";
    }

    public void Win()
    {
        _winnerTxt.text = _players[_currentPlayer] + " WINS!";
        _winnerGO.SetActive(true);
    }

    public void Back(){UnityEngine.SceneManagement.SceneManager.LoadScene(0);}

}




