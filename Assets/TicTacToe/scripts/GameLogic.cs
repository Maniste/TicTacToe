using UnityEngine;
using UnityEngine.UI;

enum GameState
{ 
   e_GameStart,
   e_PlayerTurn,
   e_AITurn,
   e_RetryScreen
};

[RequireComponent(typeof(AI))]
public class GameLogic : MonoBehaviour
{
    private GameState _gameState;

    [SerializeField]
    private Text _gameBoardUI = null;

    [SerializeField]
    private Canvas _startScreen = null;

    [SerializeField]
    private Canvas _roundScreem = null;

    [SerializeField]
    private Canvas _retryScreenUI = null;

    private int[,] _gameBoard;

    private int _selectedRow = 0;
    private int _selectedCell = 0;

    public bool ShouldAIRandomizeFirstMove = false;
    private bool _hasFirstMoveBeenDone = false;

    private AI _playerAI = null;

    private bool LeftInput()
    {
        bool state;

        if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") > 0)
            state = true;
        else
            state = false;

        return state;
    }

    private bool RightInput()
    {
        bool state;

        if (Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") < 0)
            state = true;
        else
            state = false;

        return state;
    }

    private bool UpInput()
    {
        bool state;

        if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") < 0)
            state = true;
        else
            state = false;

        return state;
    }

    private bool DownInput()
    {
        bool state;

        if (Input.GetButtonDown("Vertical") && Input.GetAxisRaw("Vertical") > 0)
            state = true;
        else
            state = false;

        return state;
    }

    private void Awake()
    {
        _playerAI = GetComponent<AI>();

        if (_gameBoardUI == null)
            _gameBoardUI = transform.GetChild(1).gameObject.GetComponent<Text>();

        if (_startScreen == null)
            _startScreen = transform.GetChild(2).gameObject.GetComponent<Canvas>();

        if (_roundScreem == null)
            _roundScreem = transform.GetChild(3).gameObject.GetComponent<Canvas>();

        if (_retryScreenUI == null)
            _retryScreenUI = transform.GetChild(4).gameObject.GetComponent<Canvas>();


    }

    void Start()
    {
        _gameState = global::GameState.e_GameStart;

        _gameBoard = new int[3, 3];

        InitGame();

    }

    private void InitGame()
    {
        for(int i = 0; i < _gameBoard.GetLength(0); i++)
        {
            for (int p = 0; p < _gameBoard.GetLength(1); p++)
                _gameBoard[i, p] = 0;
        }
    }

    private void ChangeState()
    {
        switch (_gameState)
        {
            case global::GameState.e_GameStart:
                _gameState = global::GameState.e_PlayerTurn;
                break;
            case global::GameState.e_PlayerTurn:
                _gameState = global::GameState.e_AITurn;
                UpdateRoundUI((int)_gameState);
                break;
            case global::GameState.e_AITurn:
                _gameState = global::GameState.e_PlayerTurn;
                UpdateRoundUI((int)_gameState);
                break;
            default:
                Debug.Log("STATE NOT VALID, NOT CHANGING STATE");
                break;
        }
    }

    private bool CheckGameState()
    {
        
        for (int player = 1; player < 3; player++)
        {
            for (int i = 0; i < _gameBoard.GetLength(0); i++)
            {
                if (CheckRowState(player))
                {
                    int whoWon = player;
       
                    GameFinished(whoWon);
                    _gameState = global::GameState.e_RetryScreen;
                    return true;
                }
                else if (CheckIfDraw(_gameBoard))
                {
                    GameFinished(0);
                    _gameState = global::GameState.e_RetryScreen;
                    return true;
                }
            }
        }

        return false;
    }

    public bool CheckRowState(int playerId)
    {
        for (int row = 0; row < _gameBoard.GetLength(0); row++)
        {
            if (_gameBoard[row, 0] == playerId && _gameBoard[row, 1] == playerId && _gameBoard[row, 2] == playerId)
                return true;
            else if (_gameBoard[0, row] == playerId && _gameBoard[1, row] == playerId && _gameBoard[2, row] == playerId)
                return true;
            else if (_gameBoard[0, 0] == playerId && _gameBoard[1, 1] == playerId && _gameBoard[2, 2] == playerId)
                return true;
            else if (_gameBoard[0, 2] == playerId && _gameBoard[1, 1] == playerId && _gameBoard[2, 0] == playerId)
                return true;
        }
        return false;
    }

    public bool CheckIfDraw(int[,] board)
    {
        for(int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                if (board[y, x] == 0)
                    return false;
            }
        }

        return true;
    }

    private void GameFinished(int whoWon)
    {
        _roundScreem.gameObject.SetActive(false);
        _retryScreenUI.gameObject.SetActive(true);
        //GameBoardUI.enabled = false;

        if (whoWon == 1)
            _retryScreenUI.transform.GetChild(0).GetComponent<Text>().text = "Player Won";
        else if (whoWon == 2)
            _retryScreenUI.transform.GetChild(0).GetComponent<Text>().text = "AI Won";
        else if (whoWon == 0)
            _retryScreenUI.transform.GetChild(0).GetComponent<Text>().text = "Draw!";
    }

    private void RestartGame()
    {
        for (int i = 0; i < _gameBoard.GetLength(0); i++)
        {
            for (int p = 0; p < _gameBoard.GetLength(1); p++)
                _gameBoard[i, p] = 0;
        }


        _roundScreem.gameObject.SetActive(true);
        _retryScreenUI.gameObject.SetActive(false);
        _hasFirstMoveBeenDone = false;

        int firstTurn = Random.Range(0, 2);

        if (firstTurn == 0)
        {
            _roundScreem.transform.GetChild(0).GetComponent<Text>().text = "Player Turn";
            _gameState = global::GameState.e_AITurn;
            ChangeState();
        }
        else if (firstTurn == 1)
        {
            _roundScreem.transform.GetChild(0).GetComponent<Text>().text = "AI Turn";
            _gameState = global::GameState.e_PlayerTurn;
            ChangeState();
        }
    }

    private void GameStartInput()
    {
        if (_startScreen.gameObject.activeSelf == true)
            _startScreen.gameObject.SetActive(true);

        if (Input.GetButton("Jump"))
        {
            _startScreen.gameObject.SetActive(false);
            _roundScreem.gameObject.SetActive(true);
            _gameBoardUI.enabled = true;

            int firstTurn = Random.Range(0, 2);

            if (firstTurn == 0)
            {
                _roundScreem.transform.GetChild(0).GetComponent<Text>().text = "Player Turn";
                _gameState = global::GameState.e_AITurn;
                ChangeState();
            }
            else if (firstTurn == 1)
            {
                _roundScreem.transform.GetChild(0).GetComponent<Text>().text = "AI Turn";
                _gameState = global::GameState.e_PlayerTurn;
                ChangeState();
            }

        }
    }

    private void PlayerInput()
    {

        if (LeftInput())
        {
            _selectedCell++;
            if (_selectedCell > 2)
                _selectedCell = 0;
        }
        else if (RightInput())
        {
            _selectedCell--;
            if (_selectedCell < 0)
                _selectedCell = 2;
        }
        else if (UpInput())
        {
            _selectedRow++;
            if (_selectedRow > 2)
                _selectedRow = 0;
        }
        else if (DownInput())
        {
            _selectedRow--;
            if (_selectedRow < 0)
                _selectedRow = 2;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (!_hasFirstMoveBeenDone)
                _hasFirstMoveBeenDone = true;

            if (_gameBoard[_selectedRow, _selectedCell] != 0)
                return;

            _gameBoard[_selectedRow, _selectedCell] = 1;

            //incase your last move is a win
            UpdateBoardUI();

            if (CheckGameState())
                return;

            ChangeState();
        }

        UpdateBoardUI();
    }

    private void AIInput()
    {
        if (!_hasFirstMoveBeenDone && ShouldAIRandomizeFirstMove)
        {
            _gameBoard = _playerAI.DoFirstRandomMove(_gameBoard);
            _hasFirstMoveBeenDone = true;
            Debug.Log("Randomize Move");
        }
        else
            _gameBoard = _playerAI.DoMove(_gameBoard);

        UpdateBoardUI();

        if (CheckGameState())
            return;

        ChangeState();
    }

    private void RetryInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            RestartGame();
        }
    }

    private void UpdateBoardUI()
    {
        string gameBoardString = " ";
        for (int i = 0; i < _gameBoard.GetLength(0); i++)
        {
            for (int p = 0; p < _gameBoard.GetLength(0); p++)
            {
                string charToAdd = "_";
                int currentCell = _gameBoard[i, p];

                if (_gameBoard[i, p] == 1)
                    charToAdd = "X";
                else if (_gameBoard[i, p] == 2)
                    charToAdd = "O";


                if (_gameState == GameState.e_PlayerTurn && _selectedCell == p && _selectedRow == i)
                {
                    gameBoardString += "{";
                    gameBoardString += charToAdd;
                    gameBoardString += "}";
                    continue;
                }
                gameBoardString += "[";
                gameBoardString += charToAdd;
                gameBoardString += "]";
            }
            gameBoardString += "\n";
              
        }

        _gameBoardUI.text = gameBoardString;
    }

    private void UpdateRoundUI(int currentTurn)
    {
        if (currentTurn == 1)
        {
            _roundScreem.transform.GetChild(0).GetComponent<Text>().text = "Player Turn";
        }
        else if (currentTurn == 2)
        {
            _roundScreem.transform.GetChild(0).GetComponent<Text>().text = "AI Turn";
        }
    }

    void Update()
    {
        switch(_gameState)
        {
            case global::GameState.e_GameStart:
                GameStartInput();
                break;
            case global::GameState.e_PlayerTurn:
                PlayerInput();
                break;
            case global::GameState.e_AITurn:
                AIInput();
                break;
            case global::GameState.e_RetryScreen:
                RetryInput();
                break;
            default:
                GameStartInput();
                break;
        }
    }
}
