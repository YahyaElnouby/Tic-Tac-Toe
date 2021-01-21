using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


public class GameControl : MonoBehaviour
{
    public GameObject X, O;
    public Text info;
    public GameObject restartButton;
    public float time;
    
    public enum Phase {Empty, HUMAN_X, AI_O};

    Phase Turn;

    //To keep track of elements when it's empty, X, or O
    public GameObject[] allCreated;
    public Phase[] player;
    public int depth;
    private void Awake()
    {

        //X is the first player to start
        Turn = Phase.HUMAN_X;
        info.text = "Player's Turn";
        depth = 4;
        //Keep track of the cell's state
        for(int i = 0; i < player.Length; i++)
        {
            player[i] = Phase.Empty;
        }

        restartButton.SetActive(false);
        time = 10;
    }
    private void Update()
    {
        if (Turn == Phase.AI_O)
        {
            time += Time.deltaTime * 2;
            if (time >= 11)
            {
                int bestScore = -1, bestPosition = -1, score = 0;
                for (int i = 0; i < player.Length; i++)
                {
                    if (player[i] == Phase.Empty)
                    {
                        player[i] = Phase.AI_O;
                        if(player.Length == 25)
                            score = minimax(Phase.HUMAN_X, player, depth, -1000, +1000);
                        else
                            score = minimax(Phase.HUMAN_X, player, 10, -1000, +1000);
                        player[i] = Phase.Empty;

                        if (bestScore < score)
                        {
                            bestScore = score;
                            bestPosition = i;
                        }
                    }
                }
                if (bestPosition > -1)
                {
                    Vector3 position = new Vector3(allCreated[bestPosition].transform.position.x, allCreated[bestPosition].transform.position.y - 0.3f);
                    Destroy(allCreated[bestPosition]);//remove the empty cell
                    allCreated[bestPosition] = Instantiate(O, position, Quaternion.identity);
                    player[bestPosition] = Phase.AI_O;

                }

                if (CheckWon(Phase.AI_O))
                {
                    Turn = Phase.Empty;
                    info.text = "Computer Won!";
                    restartButton.SetActive(true);
                }
                else
                {
                    Turn = Phase.HUMAN_X;
                    info.text = "Player's Turn";
                    time = 10;
                    depth++;
                }
            }
        }

        if (CheckDraw())
        {
            Turn = Phase.Empty;
            info.text = "Tie!";
            restartButton.SetActive(true);
        }
    }
    public void CreateShape(GameObject cell, int id)
    {
        //replace the cell with either X or O depending on Player playing
        if(Turn == Phase.HUMAN_X)
        {
           
            allCreated[id] = Instantiate(X, new Vector3(allCreated[id].transform.position.x + 0.1f, allCreated[id].transform.position.y - 0.2f), Quaternion.identity);
            player[id] = Phase.HUMAN_X;
            Destroy(cell);//remove the empty cell

            if (CheckWon(Phase.HUMAN_X))
            {
                Turn = Phase.Empty;
                info.text = "Player Won!";
                restartButton.SetActive(true);
            }
            else
            {
                Turn = Phase.AI_O;
                info.text = "Computer's Turn";
                time = 10;
            }
        }
        

        if (CheckDraw())
        {
            Turn = Phase.Empty;
            info.text = "Tie!";
            restartButton.SetActive(true);
        }

    }

    private bool CheckDraw()
    {
        bool human, computer, isEmpty, isDraw = false;

        human = CheckWon(Phase.HUMAN_X);
        computer = CheckWon(Phase.AI_O);
        isEmpty = CheckEmpty();

        if(!human & !computer & !isEmpty)
        {
            isDraw = true;
        }

        return isDraw;
    }

    private bool CheckEmpty()
    {
        bool isEmpty = false;

        for(int i = 0; i < player.Length; i++)
        {
           if(player[i] == Phase.Empty)
            {
                isEmpty = true;
                break;
            }
        }
        return isEmpty;
    }

    private bool CheckWon(Phase currentPlayer)
    {
        bool won = false;

        if (allCreated.Length == 9)
        {
            int[,] winConditions = new int[8, 3] { { 0, 1, 2 }, {3, 4, 5 }, {6, 7, 8}, //rows
                                               {0, 3, 6}, {1, 4, 7}, { 2, 5, 8}, //columns
                                               {0, 4, 8}, {2, 4, 6 } }; //diagonals

            for (int i = 0; i < 8; i++)
            {
                if (player[winConditions[i, 0]] == currentPlayer &
                   player[winConditions[i, 1]] == currentPlayer &
                   player[winConditions[i, 2]] == currentPlayer)
                {
                    won = true;
                    break;
                }
            }
            return won;
        }
        else if(allCreated.Length == 16)
        {
            int[,] winConditions = new int[10, 4] { {0, 1, 2, 3}, {4, 5, 6, 7 }, {8, 9, 10, 11}, {12, 13, 14, 15}, //rows
                                               {0, 4, 8, 12}, {1, 5, 9, 13}, {2, 6, 10, 14}, {3, 7, 11, 15}, //columns
                                               {0, 5, 10, 15}, {3, 6, 9, 12} }; //diagonals

            for (int i = 0; i < 10; i++)
            {
                if (player[winConditions[i, 0]] == currentPlayer &&
                   player[winConditions[i, 1]] == currentPlayer &&
                   player[winConditions[i, 2]] == currentPlayer &&
                   player[winConditions[i, 3]] == currentPlayer)
                {
                    won = true;
                    break;
                }
            }
            return won;
        }
        else if(allCreated.Length == 25)
        {
            int[,] winConditions = new int[12, 5] { {0, 1, 2, 3, 4}, {5, 6, 7, 8, 9}, {10, 11, 12, 13, 14}, {15, 16, 17, 18, 19}, {20, 21, 22, 23, 24}, //rows
                                               {0, 5, 10, 15, 20}, {1, 6, 11, 16, 21}, {2, 7, 12, 17, 22}, {3, 8, 13, 18, 23}, {4, 9, 14, 19, 24}, //columns
                                               {0, 6, 12, 18, 24}, {4, 8, 12, 16, 20} }; //diagonals

            for (int i = 0; i < 12; i++)
            {
                if (player[winConditions[i, 0]] == currentPlayer &&
                   player[winConditions[i, 1]] == currentPlayer &&
                   player[winConditions[i, 2]] == currentPlayer &&
                   player[winConditions[i, 3]] == currentPlayer &&
                   player[winConditions[i, 4]] == currentPlayer)
                {
                    won = true;
                    break;
                }
            }
            return won;
        }
        return won;
    }

    private int minimax(Phase currentPlayer, Phase[] board, int depth, int alpha, int beta)
    {
        if (CheckDraw())
            return 0;

        if (CheckWon(Phase.AI_O))
            return +1;
        
        if (CheckWon(Phase.HUMAN_X))
            return -1;
        if (depth <= 0 && board.Length != 9)
            return (-1 * depth);

        int score;

        if (currentPlayer == Phase.AI_O)
        {
            for(int i = 0; i < board.Length; i++)
            {
                if(board[i] == Phase.Empty)
                {
                    board[i] = Phase.AI_O;
                    score = minimax(Phase.HUMAN_X, board, --depth, alpha, beta);
                    board[i] = Phase.Empty;
                   
                    if(score > alpha)
                        alpha = score;

                    if (alpha > beta)
                        break;
                }
            }
            return alpha;
        }
        else
        {
            for (int i = 0; i < board.Length; i++)
            {
                if (board[i] == Phase.Empty)
                {
                    board[i] = Phase.HUMAN_X;
                    score = minimax(Phase.AI_O, board, --depth, alpha, beta);
                    board[i] = Phase.Empty;

                    if (score < beta)
                        beta = score;

                    if (alpha > beta)
                        break;
                }
            }
            return beta;
        }
    }

    public void Easy()
    {
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Difficulties");
        for (int i = 0; i < tmp.Length; i++)
        {
            Destroy(tmp[i]);
        }

        Instantiate(Resources.Load("Prefabs/3x3") as GameObject);
    }

    public void Normal()
    {
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Difficulties");
        for (int i = 0; i < tmp.Length; i++)
        {
            Destroy(tmp[i]);
        }

        Instantiate(Resources.Load("Prefabs/4x4") as GameObject);
    }

    public void Hard()
    {
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Difficulties");
        for (int i = 0; i < tmp.Length; i++)
        {
            Destroy(tmp[i]);
        }

        Instantiate(Resources.Load("Prefabs/5x5") as GameObject);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
