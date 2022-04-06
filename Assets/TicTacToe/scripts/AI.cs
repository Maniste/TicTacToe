using UnityEngine;

public class AI : MonoBehaviour
{
    GameLogic _logic = null;

    private void Awake()
    {
        _logic = GetComponent<GameLogic>();
    }

    public int[,]DoFirstRandomMove(int[,] board)
    {
        int randomX = Random.Range(0, 3);
        int randomY = Random.Range(0, 3);
        board[randomY, randomX] = 2;
        return board;
    }

    public int[,] DoMove(int[,] board)
    {
        int bestScore = -99;
        int bestMoveY = 0;
        int bestMoveX = 0;

        for (int y = 0; y < board.GetLength(0); y++)
        {
            for (int x = 0; x < board.GetLength(1); x++)
            {
                //is there a spot
                if (board[y, x] == 0)
                {
                    board[y, x] = 2;
                    int score = Minimax(board, 0, false);
                    board[y, x] = 0;

                    if (score > bestScore)
                    {
                        bestScore = score;

                        bestMoveY = y;
                        bestMoveX = x;
                    }

                }
            }
        }

        board[bestMoveY, bestMoveX] = 2;

        return board;
    }

    private int Minimax(int[,] board, int depth, bool isMaximising)
    {
        if (_logic.CheckRowState(1))
            return -1;
        else if (_logic.CheckRowState(2))
            return 1;
        else if (_logic.CheckIfDraw(board))
            return 0;


        int bestScore = -99;

        if (isMaximising)
        {
            for (int y = 0; y < board.GetLength(0); y++)
            {
                for (int x = 0; x < board.GetLength(1); x++)
                {
                    //is there a spot
                    if (board[y, x] == 0)
                    {
                        board[y, x] = 2;
                        int score = Minimax(board, depth + 1, false);
                        board[y, x] = 0;

                        if (score > bestScore)
                            bestScore = score;

                    }
                }
            }
            return bestScore;
        }
        else
        {
            bestScore = 99;

            for (int y = 0; y < board.GetLength(0); y++)
            {
                for (int x = 0; x < board.GetLength(1); x++)
                {
                    //is there a spot
                    if (board[y, x] == 0)
                    {
                        board[y, x] = 1;
                        int score = Minimax(board, depth + 1, true);
                        board[y, x] = 0;

                        if (score < bestScore)
                            bestScore = score;

                    }
                }
            }

            return bestScore;
        }
    }


}
