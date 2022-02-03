using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Guess : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _randomText, _guessText, _scoreText, _highscoreText, _dificultyText, _attemptsText;
    [SerializeField] private GameObject _restartGO, _winGO;
    [SerializeField] private Image _background;  

    private Color _defaultColor = new Color(.5f, .5f, .5f, 1);
    private Color _red = new Color(1, 0, 0f, .5f);
    private Color _green = new Color(0, 1, 0, .5f);

    private int _randomNumber, _score, _limit, _attempts, _highscore = 0;
    
    private void Start() {
        Dificulty(1);   
    }

    private int GetRandomNumber()
    {
        return UnityEngine.Random.Range(1,_limit + 1);
    }
    
    public void Dificulty(int i)
    {
        switch (i)
        {
            case 0:
            _limit = 10;
            _dificultyText.color = Color.green;
            break;

            case 1:
            _limit = 20;
            _dificultyText.color = Color.yellow;
            break;

            case 2:
            _limit = 30;
            _dificultyText.color = Color.red;
            break;
            default:
            break;
        }

        Restart();
    }
    
    public void Restart()
    {
        _attempts = 10;
        _attemptsText.text = _attempts.ToString();
        _background.color = _defaultColor;
        _randomNumber = GetRandomNumber();
        _score =_limit;
        _scoreText.text = $"Score: {_score}";
        _dificultyText.text = $"1 - {_limit}";
        _guessText.text = "Start Guessing!";
        _randomText.text = "??";
        _restartGO.SetActive(false);
        _winGO.SetActive(false);
    }
    
    public void EnterNumber(string number)
    {  
        if(Int32.TryParse(number, out int nr))
        {
            if(nr < 1 || nr > _limit) _guessText.text = $"Choose a number between 1 and {_limit} FFS!";
            else if(nr == _randomNumber) 
            {
                _guessText.text = "Correct!";
                if(_score > _highscore) _highscore = _score;
                _highscoreText.text = $"Highscore: {_highscore.ToString()}";
                _background.color = _green;
                _randomText.text = number;
                _winGO.SetActive(true);
            }    
            else
            {
                _guessText.text = nr < _randomNumber? "Too Low!" : "Too High";
                _background.color = _red;
                _score --;
                _attempts--;
                _attemptsText.text = _attempts.ToString();
                _scoreText.text = $"Score: {_score.ToString()}";
                if(_attempts < 1) _restartGO.SetActive(true);
            } 
        }
    }

    public void Back(){UnityEngine.SceneManagement.SceneManager.LoadScene(0);}
   
}
