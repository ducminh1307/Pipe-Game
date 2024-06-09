using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Pipe _cellPrefab;
    [SerializeField] private List<LevelData> _levels;

    private bool hasGameFinished;
    private bool isGameOver;
    private int currentLevel;
    private int turns;
    private Pipe[,] pipes; // Khai bao mang 2 chieu
    private List<Pipe> startPipes;

    private WaitForSeconds timeCheck = new WaitForSeconds(.1f);
    private WaitForSeconds timeOut = new WaitForSeconds(2f);

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;

        currentLevel = PlayerPrefs.GetInt("level", 0);

        SpawnLevel();
    }

    // Ham sinh cap do choi
    private void SpawnLevel()
    {
        if (currentLevel >= _levels.Count)
        {
            UIManager.Instance.FinishedGame();
            return;
        }

        UIManager.Instance.InGame();
        UIManager.Instance.UpdateLevelText(currentLevel + 1);

        turns = _levels[currentLevel].turns;
        UIManager.Instance.UpdateTurnText(turns);

        // Khoi tao mang 2 chieu co chieu dai la _level.column phan tu, chieu rong la _level.row phan tu
        pipes = new Pipe[_levels[currentLevel].row, _levels[currentLevel].column];
        startPipes = new List<Pipe>();

        for (int i = 0; i < _levels[currentLevel].row; i++)
        {
            for (int j = 0; j <  _levels[currentLevel].column; j++)
            {
                //Tinh toan toa do sinh ra cua Cell
                Vector2 spawnPos = new Vector2(j + .5f, i + .5f);

                //Tao Cell voi toa do da tinh
                Pipe tempPipe = Instantiate(_cellPrefab);
                tempPipe.transform.position = spawnPos;

                //Khoi tao pipe bang gia tri dua vao cac phan tu cua _level.datas
                tempPipe.Init(_levels[currentLevel].data[i * _levels[currentLevel].column + j]);

                //Dua du lieu cua Cell da tao vao trong mang 2 chieu pipes
                pipes[i, j] = tempPipe;

                //Kiem tra neu la pipe_1 ti dua no vao list startPipes va dat no la ong nuoc bat dau
                if (tempPipe.pipeType == 1)
                    startPipes.Add(tempPipe);
            }
        }

        //Dat kich thuoc cua camera bang gia tri lon nhat cua row va column
        Camera.main.orthographicSize = Mathf.Max(_levels[currentLevel].row, _levels[currentLevel].column);

        //Tinh toan va dat vi tri camera o giua level duoc tao ra
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = _levels[currentLevel].column /2f;
        cameraPos.y = _levels[currentLevel].row /2f;
        Camera.main.transform.position = cameraPos;
    }

    private void Update()
    {
        //Neu hoan thanh level thi khong lam gi
        if (hasGameFinished || isGameOver) return;

        //Lay toa do cua chuot khi nhan vao man hinh
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Lam tron cac toa do cua chuot de chuyen qua gia tri cua row va columm
        int row = Mathf.FloorToInt(mousePos.y);
        int col = Mathf.FloorToInt(mousePos.x);

        //Kiem tra neu khong an chuot vao cac ong nuoc thi khong lam gi
        if (row < 0 || col < 0) return;
        if (row >= _levels[currentLevel].row || col >= _levels[currentLevel].column) return;

        //Khi an vao cac ong nuoc thi se lam cho cac ong nuoc xoay
        if (Input.GetMouseButtonDown(0))
        {
            if (pipes[row, col].pipeType == 1 || pipes[row, col].pipeType == 2)
                return;
            pipes[row, col].UpdateInput();
            turns--;
            UIManager.Instance.UpdateTurnText(turns);
            CheckLose();
            StartCoroutine(ShowHint());
        }
    }

    //Sau 0.1s kiem tra xem co thang khong
    private IEnumerator ShowHint()
    {
        yield return timeCheck;
        CheckFill();
        checkWin();
    }

    //Kiem tra de tao cac ong co nuoc
    private void CheckFill()
    {
        // Dua cac ong ve trang thai rong
        for (int i = 0; i < _levels[currentLevel].row; i++)
        {
            for (int j = 0; j < _levels[currentLevel].column; j++)
            {
                Pipe tempPipe = pipes[i, j];

                if (tempPipe.pipeType != 0) tempPipe.isFilled = false;
            }
        }

        Queue<Pipe> check = new Queue<Pipe>(); // Khoi tao mot hang doi de luu tru cac Pipe
        HashSet<Pipe> finished = new HashSet<Pipe>(); // Tao mot tap hop chua cac Pipe khong trung nhau khi duyet xog

        foreach (var pipe in startPipes) 
        { 
            check.Enqueue(pipe); // Them cac pipe_1 vao trong hang doi
        }

        while (check.Count > 0)
        {
            Pipe pipe = check.Dequeue(); // Lay pipe dau tien ra khoi hang doi
            finished.Add(pipe); // dua pipe do vao trong HashSet

            List<Pipe> connected = pipe.ConnectPipes(); //Lay cac ong co ket noi voi ong dau

            foreach (var connectedPipe in connected)
            {
                // Kiem tra neu ong khong co trong HashSet thi them ong do vao trong hang doi
                if (!finished.Contains(connectedPipe)) check.Enqueue(connectedPipe);
            }
        }

        // Doi trang thai cac ong co noi voi nhau sang "true"
        foreach (var pipe in finished)
        {
            if (pipe.canFill)
                pipe.isFilled = true;
        }

        // Hien thi cac ong co trang thai isFilled
        for (int i = 0; i < _levels[currentLevel].row; i++)
        {
            for (int j = 0; j < _levels[currentLevel].column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                tempPipe.UpdateFilled();
            }
        }
    }

    //Kiem tra da thang chua
    private void checkWin()
    {
        for (int i =  0; i < _levels[currentLevel].row; i++)
        {
            for (int j = 0; j < _levels[currentLevel].column; j++)
            {
                //Neu con ong nuoc chua co nuoc thi khong lam gi
                if (!pipes[i, j].isFilled) return; 
            }
        }
        hasGameFinished = true;
        currentLevel++;
        PlayerPrefs.SetInt("level", currentLevel);
        StartCoroutine(GameFinished());
    }

    //2 giay sau khi win game load lai scene
    private IEnumerator GameFinished()
    {
        yield return timeOut;
        SceneManager.LoadScene(0);
    }

    private void CheckLose()
    {
        if (turns <= 0)
        {
           isGameOver = true;
           StartCoroutine(LoseGame());
        }
    }

    private IEnumerator LoseGame()
    {
        yield return timeOut;
        UIManager.Instance.LoseGame();
    }

    public void ReloadSence()
    {
        SceneManager.LoadScene(0);
    }
}
