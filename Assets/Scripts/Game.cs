using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Audio;

public class Game : MonoBehaviour
{
    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    public static bool startingAtlevelZero;
    public static int startingLevel;

    public Canvas hud_canvas;
    public Canvas pause_canvas;

    public int scoreOneLine = 40;
    public int scoreTwoLine = 100;
    public int scoreThreeLine = 300;
    public int scoreFourLine = 1200;

    public int currentLevel = 0;
    private int numLinesCleared = 0;

    public static float fallSpeed = 1.0f;
    public static bool isPaused;


    public TextMeshProUGUI hud_score;
    public TextMeshProUGUI hud_level;
    public TextMeshProUGUI hud_lines;

    public static int currentScore = 0;

    private int numberOfRowsthisTurn = 0;

    public AudioSource audioSource;


    public AudioClip clearedLineSound;

    private GameObject previewTetromino;
    private GameObject nextTetromino;
    private GameObject savedTetromino;
    private GameObject ghostTetromino;


    private bool gameStarted = false;
    private int startingHighScore;
    private int startingHighScore2;
    private int startingHighScore3;

    private Vector2 previewTetrominoPosition = new Vector2(-6.5f, 16);
    private Vector2 savedTetrominoPosition = new Vector2(-6.5f, 10);

    public int maxSwaps = 2;
    private int currentSwaps = 0;

   
    void Start()
    {
        currentScore = 0;

        hud_score.text = "0";
        currentLevel = startingLevel;
        hud_level.text=currentLevel.ToString();
        hud_lines.text = "0";

        currentLevel = startingLevel;

        SpawnNextTetromino();

        audioSource = GetComponent<AudioSource>();
        startingHighScore = PlayerPrefs.GetInt("highscore");
        startingHighScore2 = PlayerPrefs.GetInt("highscore2");
        startingHighScore3 = PlayerPrefs.GetInt("highscore3");
    }

    void Update()
    {
        UpdateScore();

        UpdateUI();

        UpdateLevel();

        UpdateSpeed();

        CheckUserImput();
    }

    void CheckUserImput() 
    {
        if(Input.GetKeyUp(KeyCode.P))
        {
            if (Time.timeScale==1)
            {
                PauseGame();
                
            }
            else
            {
                ResumeGame();
                
            }
            
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            GameObject tempNextTetromino = GameObject.FindGameObjectWithTag("currentActiveTetromino");

            Object.FindFirstObjectByType<Game>().SaveTetromino(tempNextTetromino.transform);
        }


    }

    void PauseGame() 
    {
        Time.timeScale = 0;
        audioSource.Pause();
        isPaused = true;
        hud_canvas.enabled= false;
        pause_canvas.enabled= true;

    }

    void ResumeGame() 
    {
        Time.timeScale = 1;      
        isPaused = false;
        audioSource.Play();
        hud_canvas.enabled = true;
        pause_canvas.enabled = false;
    }

    void UpdateLevel() 
    {
        if ((startingAtlevelZero == true)||(startingAtlevelZero)==false && numLinesCleared/ 10>startingLevel)
        {
            currentLevel = numLinesCleared / 10;
        }
    }
    void UpdateSpeed() 
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    public void UpdateUI() 
    {
        hud_score.text = currentScore.ToString();
        hud_level.text = currentLevel.ToString();
        hud_lines.text = numLinesCleared.ToString();
    }

    public void UpdateScore() 
    {
        if (numberOfRowsthisTurn > 0) 
        {
            if (numberOfRowsthisTurn==1)
            {
                ClearedOneLine();
            }
            else if (numberOfRowsthisTurn == 2)
            {
                ClearedTwoLine();
            }
            else if (numberOfRowsthisTurn == 3)
            {
                ClearedThreeLine();
            }
            else if (numberOfRowsthisTurn == 4)
            {
                ClearedFourLine();
            }

            numberOfRowsthisTurn = 0;

            PlayLineClearedSound();
        }
    }

    public void ClearedOneLine() 
    {
        currentScore += scoreOneLine + (currentLevel * 20);
        numLinesCleared++;
    }

    public void ClearedTwoLine()
    {
        currentScore += scoreTwoLine + (currentLevel * 25);
        numLinesCleared += 2;
    }
    public void ClearedThreeLine()
    {
        currentScore += scoreThreeLine + (currentLevel * 30);
        numLinesCleared += 3;
    }

    public void ClearedFourLine()
    {
        currentScore += scoreFourLine + (currentLevel * 40);
        numLinesCleared += 4;
    }

    public void PlayLineClearedSound() 
    {
        audioSource.PlayOneShot(clearedLineSound);
    }

    public void UpdateHighScore() 
    {
        if (currentScore>startingHighScore)
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", startingHighScore);
            PlayerPrefs.SetInt("highscore1", currentScore);

            PlayerPrefs.SetInt("highscore", currentScore);
        }
        else if (currentScore>startingHighScore2)
        {
            PlayerPrefs.SetInt("highscore3", startingHighScore2);
            PlayerPrefs.SetInt("highscore2", currentScore);
        }
        else if (currentScore > startingHighScore3)
        {
            PlayerPrefs.SetInt("highscore3", currentScore);
        }

        PlayerPrefs.SetInt("lastscore", currentScore);


    }

    bool CheckIsValidPosition(GameObject tetromino) 
    {
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round (mino.position);
            if (!CheckIsInsideGrid(pos))
            {
                return false;
            }
            if (GetTransformformGridPosition(pos)!=null && GetTransformformGridPosition(pos).parent!=tetromino.transform)
            {
                return false;
            }
        }

        return true;

    }

    public bool CheckISAboveGrid(Tetromin tetromino) 
    {
        for (int x = 0; x < gridWidth; x++)
        {
            foreach (Transform mino in tetromino.transform)
            {
                Vector2 pos = Round(mino.position);
                if (pos.y> gridHeight-1)
                {
                    return true;
                }
            }
        }
        return false;

    }

    public bool IsfullRowAt(int y) 
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x,y]==null)
            {
                return false;
            }
        }

        numberOfRowsthisTurn++;

        return true;
    }

    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < gridWidth; x++)
        {
          
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            
        }
    }

    public void MoveRowDown(int y) 
    {
        for (int x = 0; x < gridWidth; x++)
        {
            if (grid[x,y]!=null)
            {
                grid[x, y - 1] = grid[x, y];

                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1, 0);  
            }
        }
    }

    public void MoveallRowsDown(int y)
    {
        for (int i = y; i < gridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow() 
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (IsfullRowAt(y))
            {
                DeleteMinoAt(y);

                MoveallRowsDown(y + 1);

                --y;
            }
        }
    }

    public void UpdateGrid(Tetromin tetromino) 
    {
        for (int y = 0;y< gridHeight;y++) 
        {
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x,y].parent==tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }
        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);

            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformformGridPosition(Vector2 pos) 
    {
        if (pos.y>gridHeight -1)
        {
            return null;
        }
        else
        {
            return grid[(int)pos.x, (int)pos.y]; 
        }
    }

    public void SpawnNextTetromino()
    {
        if (!gameStarted)
        {
            gameStarted = true;


            nextTetromino = Instantiate(Resources.Load<GameObject>(GetRandomTetromino()), new Vector2(5.0f, 20.0f), Quaternion.identity);
            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromin>().enabled = false;
            nextTetromino.tag = "currentActiveTetromino";

            SpawnGhostTetromino();


        }
        else
        {
            previewTetromino.transform.localPosition = new Vector2(5.0f, 20.0f);
            nextTetromino = previewTetromino;
            nextTetromino.GetComponent<Tetromin>().enabled = true;
            nextTetromino.tag = "currentActiveTetromino";

            previewTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
            previewTetromino.GetComponent<Tetromin>().enabled = false;


            SpawnGhostTetromino();

        }
        currentSwaps = 0;        
    }

    public void SpawnGhostTetromino() 
    {
        if (GameObject.FindGameObjectWithTag("currentGhostTetromino") !=null)
        {
            Destroy(GameObject.FindGameObjectWithTag("currentGhostTetromino"));
        }

        ghostTetromino = (GameObject)Instantiate (nextTetromino, nextTetromino.transform.position, Quaternion.identity);

        Destroy(ghostTetromino.GetComponent<Tetromin>());

        ghostTetromino.AddComponent<GhostTetromino>();

    }


    public void SaveTetromino(Transform t) 
    {
        currentSwaps++;

        if (currentSwaps>maxSwaps)
        {
            return;
        }

        if (savedTetromino!=null)
        {
            GameObject tempSavedTetromino = GameObject.FindGameObjectWithTag("currentSavingTetromino");

            tempSavedTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);

            if (!CheckIsValidPosition (tempSavedTetromino))
            {
                tempSavedTetromino.transform.localPosition = savedTetrominoPosition;

                return;
            }

            savedTetromino = (GameObject)Instantiate(t.gameObject);
            savedTetromino.GetComponent <Tetromin>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavingTetromino";

            nextTetromino = (GameObject)Instantiate(tempSavedTetromino);
            nextTetromino.GetComponent<Tetromin>().enabled = true;
            nextTetromino.transform.localPosition = new Vector2(gridWidth / 2, gridHeight);
            nextTetromino.tag = "currentActiveTetromino";

            DestroyImmediate(t.gameObject);
            DestroyImmediate(tempSavedTetromino);

            SpawnGhostTetromino();

        }
        else
        {
            savedTetromino = (GameObject)Instantiate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));
            savedTetromino.GetComponent<Tetromin>().enabled = false;
            savedTetromino.transform.localPosition = savedTetrominoPosition;
            savedTetromino.tag = "currentSavingTetromino";

            DestroyImmediate(GameObject.FindGameObjectWithTag("currentActiveTetromino"));

            SpawnNextTetromino();
        }

    }


    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 8);
        string randomTetrominoName = "Prefabs/Tetromino_T";

        switch (randomTetromino)
        {
            case 1:
                randomTetrominoName = "Prefabs/Tetromino_T";
                break;
            case 2:
                randomTetrominoName = "Prefabs/Tetromino_Long";
                break;
            case 3:
                randomTetrominoName = "Prefabs/Tetromino_Square";
                break;
            case 4:
                randomTetrominoName = "Prefabs/Tetromino_J";
                break;
            case 5:
                randomTetrominoName = "Prefabs/Tetromino_L";
                break;
            case 6:
                randomTetrominoName = "Prefabs/Tetromino_S";
                break;
            case 7:
                randomTetrominoName = "Prefabs/Tetromino_Z";
                break;
        }

        return randomTetrominoName;
    }

    public void GameOver() 
    {
        UpdateHighScore();
        SceneManager.LoadScene("GameOver");
    }

}