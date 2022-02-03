using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MineSweeper : MonoBehaviour
{    
    [SerializeField] private Field _basicField;
    [SerializeField] private SpriteRenderer _border, _borderTop, _panel;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _bombsText, _timeText;
    [SerializeField] private Transform _grid;
    [SerializeField] private RectTransform _canvasTop, _canvasPanel;

    public Sprite[] Images;
    public Sprite[] Sprites;
    
    private Field[,] _fields;
    private int _remaining, _flagged, _totalMines, _sizeX, _sizeY;
    private bool _gameOver;
    private float _canvasX = 4f;
    private Camera _cam;
    private Coroutine _timer = null;

    private void Awake() {
        Field.OnLeftClick += OnLeftClick;
        Field.OnRightClick += OnRightClick;
        _cam = Camera.main;
    }

    private void Start() {
        _sizeX = _sizeY = 9;
        _totalMines = 10;
        Restart();
    }

#region Generation    

    public void ChangeSize(int s)
    {
        switch (s)
        {
            case 0:
            _cam.transform.position = new Vector3(3.5f, 2.5f, -10 );
            _cam.orthographicSize = 15;
            _sizeX = _sizeY = 9;
            _totalMines = 10;
            _canvasX = 4;
            break;

            case 1:
            _cam.transform.position = new Vector3(6.5f, 6.5f, -10 );
            _cam.orthographicSize = 20;
            _sizeX = _sizeY = 16;
            _totalMines = 40;
            _canvasX = 7.5f;
            break;

            case 2:
            _cam.transform.position = new Vector3(13.5f, 6.5f, -10 );
            _cam.orthographicSize = 20;
            _sizeX = 30;
            _sizeY = 16;
            _totalMines = 99;
            _canvasX = 14.5f;
            break;

            default:
            break;
        }

        Restart();
    }
    
    public void Restart()
    {
        foreach (Transform child in _grid)
        {
            GameObject.Destroy(child.gameObject);
        } 

        _fields = null;
        GenerateGrid();

        _remaining = _sizeX * _sizeY - _totalMines;
        _flagged = 0;

        _timeText.text = "000";
        _bombsText.text = _totalMines.ToString("D3");

        _gameOver = false;
        _image.sprite = Images[0];

        if(_timer != null)
        {
            StopCoroutine(_timer);
            _timer = null;
        }
    }
    
    private void GenerateGrid()
    {
        _fields = new Field[_sizeX, _sizeY];

        for (int r = 0; r < _sizeX; r++)
        {
            for (int c = 0; c < _sizeY; c++)
            {
                _fields[r, c] = Instantiate(_basicField, new Vector3(r, c, 0), Quaternion.identity, _grid);
            }
        }

        ResizePanels();

        RandomizeBombs();
    }

    private void ResizePanels()
    {
        _border.size = new Vector2(_sizeX + 1, _sizeY + 1);
        _borderTop.size = new Vector2(_sizeX + 1, 3);
        _panel.size = new Vector2(_sizeX + 1, 2);
        _borderTop.transform.position = new Vector3(-1, _sizeY + 2.5f, 1);
        _canvasTop.sizeDelta = new Vector2(_sizeX, 2);
        _canvasPanel.sizeDelta = new Vector2(_sizeX, 1);
        _canvasTop.position = new Vector3(_canvasX, _canvasTop.position.y, _canvasTop.position.z);
        _canvasPanel.position = new Vector3(_canvasX, _canvasPanel.position.y, _canvasPanel.position.z);
    }

    private void RandomizeBombs()
    {
        int bombs = 0;
        while (bombs < _totalMines)
        {
            int xr = UnityEngine.Random.Range(0,_sizeX);
            int xc = UnityEngine.Random.Range(0,_sizeY);
            if (!_fields[xr,xc].IsBomb)
            {
                bombs ++;
                _fields[xr,xc].IsBomb = true;
            }
        }
    }

    public int CalculateBombs(int x, int y)
    {
        int bombs = 0;

        for (int i = x-1; i < x+2; i++)
        {
            if(i > -1 && i < _sizeX)
            {
                for (int j = y-1; j < y+2; j++)
                {
                    if(j > -1 && j < _sizeY && _fields[i,j].IsBomb) bombs++;
                }
            }
        }

        if(!_fields[x,y].IsOpened) _remaining --;
        _fields[x,y].Rend.sprite = Sprites[bombs];
        _fields[x,y].IsOpened = true;

        return bombs; 
    }

    public void Uncover(int x, int y, bool[,] visited)
    {
        if(x >= 0 && y >=0 && x < _sizeX && y < _sizeY)
        {
            if (visited[x, y])
                return;

            visited[x, y] = true;
            if(CalculateBombs(x,y) > 0)
                return;

            for (int i = x-1; i < x+2; i++)
            {
                for (int j = y-1; j < y+2; j++)
                {
                    Uncover(i, j, visited);
                }
            }
        }    
    }
#endregion
#region Gameplay

    private void OnRightClick(Field field)
    {
        if(!_gameOver)
        {
            if(_timer == null) _timer = StartCoroutine(Timer());

            if(field.Rend.sprite == Sprites[12])
            {
                field.Rend.sprite = Sprites[11];
                _flagged ++;
            }
            else
            {
                field.Rend.sprite = Sprites[12];
                _flagged --;
            }
            _bombsText.text = (_totalMines - _flagged) < 0? (_totalMines - _flagged).ToString("D2") : (_totalMines - _flagged).ToString("D3");
        } 
            
    }
    
    private void OnLeftClick(Field field)
    {
        if(!_gameOver)
        {
            if(_timer == null) _timer = StartCoroutine(Timer());
            if(field.IsBomb)
            {
                _gameOver = true;
                field.IsOpened = true;
                field.Rend.sprite = Sprites[10];
                foreach (var item in _fields)
                {
                    if(item.IsBomb && item != field) 
                    {
                        item.Rend.sprite = Sprites[9];
                        item.IsOpened = true;
                    }
                }        
                _image.sprite = Images[2];
            }
            else
            {
                Uncover((int)field.transform.position.x, (int)field.transform.position.y, new bool[_sizeX, _sizeY]);
                if(_remaining < 1) 
                {
                    _gameOver = true;
                    _image.sprite = Images[1];
                }
            }
        }
    }

    private IEnumerator Timer()
    {
        int time = 0;
        while(!_gameOver && time <= 999)
        {
            time += 1;
            _timeText.text = time.ToString("D3");
            yield return new WaitForSeconds(1);
        }
        _timer = null;
    }

#endregion

    public void Back()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void OnDestroy() {
        Field.OnLeftClick -= OnLeftClick;
        Field.OnRightClick -= OnRightClick;
    }

}

