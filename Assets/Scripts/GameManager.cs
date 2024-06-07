using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LevelData _level;
    [SerializeField] private Pipe _cellPrefab;

    private bool hasGameFinished;
    private Pipe[,] pipes; // Khai bao mang 2 chieu
    private List<Pipe> startPipes;

    private WaitForSeconds timeCheck = new WaitForSeconds(.1f);

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;

        SpawnLevel();
    }

    // Ham sinh cap do choi
    private void SpawnLevel()
    {
        pipes = new Pipe[_level.row, _level.column]; // Khoi tao mang 2 chieu co chieu dai la _level.column phan tu, chieu rong la _level.row phan tu
        startPipes = new List<Pipe>();

        for (int i = 0; i < _level.row; i++)
        {
            for (int j = 0; j <  _level.column; j++)
            {
                //Tinh toan toa do sinh ra cua Cell
                Vector2 spawnPos = new Vector2(j + .5f, i + .5f);

                //Tao Cell voi toa do da tinh
                Pipe tempPipe = Instantiate(_cellPrefab);
                tempPipe.transform.position = spawnPos;

                //Khoi tao pipe bang gia tri dua vao cac phan tu cua _level.datas
                tempPipe.Init(_level.data[i * _level.column + j]);

                //Dua du lieu cua Cell da tao vao trong mang 2 chieu pipes
                pipes[i, j] = tempPipe;

                //Kiem tra neu la pipe_1 ti dua no vao list startPipes va dat no la ong nuoc bat dau
                if (tempPipe.pipeType == 1)
                    startPipes.Add(tempPipe);
            }
        }

        //Dat kich thuoc cua camera bang row neu lon hon column hoac nguoc lai
        Camera.main.orthographicSize = Mathf.Max(_level.row, _level.column);

        //Tinh toan va dat vi tri camera o giua level duoc tao ra
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = _level.column /2f;
        cameraPos.y = _level.row /2f;
        Camera.main.transform.position = cameraPos;
    }

    private void Update()
    {
        //Neu hoan thanh level thi khong lam gi
        if (hasGameFinished) return;

        //Lay toa do cua chuot khi nhan vao man hinh
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Lam tron cac toa do cua chuot de chuyen qua gia tri cua row va columm
        int row = Mathf.FloorToInt(mousePos.y);
        int col = Mathf.FloorToInt(mousePos.x);

        //Kiem tra neu khong an chuot vao cac ong nuoc thi khong lam gi
        if (row < 0 || col < 0) return;
        if (row >=  _level.row || col >= _level.column) return;

        //Khi an vao cac ong nuoc thi se lam cho cac ong nuoc xoay
        if (Input.GetMouseButtonDown(0))
        {
            pipes[row, col].UpdateInput();
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
        for (int i = 0; i < _level.row; i++)
        {
            for (int j = 0; j < _level.column; j++)
            {
                Pipe tempPipe = pipes[i, j];

                if (tempPipe.pipeType != 0) tempPipe.isFilled = false;
            }
        }

        Queue<Pipe> check = new Queue<Pipe>();
        HashSet<Pipe> finished = new HashSet<Pipe>();

        foreach (var pipe in startPipes) 
        { 
            check.Enqueue(pipe);
        }

        while (check.Count > 0)
        {
            Pipe pipe = check.Dequeue();
            finished.Add(pipe);
            List<Pipe> connected = pipe.ConnectPipes();
            foreach (var connectedPipe in connected)
            {
                if (!finished.Contains(connectedPipe)) check.Enqueue(connectedPipe);
            }
        }

        foreach (var filled in finished)
        {
            filled.isFilled = true;
        }

        for (int i = 0; i < _level.row; i++)
        {
            for (int j = 0; j < _level.column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                tempPipe.UpdateFilled();
            }
        }
    }

    //Kiem tra da thang chua
    private void checkWin()
    {
        for (int i =  0; i < _level.row; i++)
        {
            for (int j = 0; j < _level.column; j++)
            {
                //Neu con ong nuoc chua co nuoc thi khong lam gi
                if (!pipes[i, j].isFilled) return; 
            }
        }
        hasGameFinished = true;
        StartCoroutine(GameFinished());
    }

    //2 giay sau khi win game load lai scene
    private IEnumerator GameFinished()
    {
        WaitForSeconds timeWin = new WaitForSeconds(2f);
        yield return timeWin;
        SceneManager.LoadScene(0);
    }
}
