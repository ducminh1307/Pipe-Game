using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LevelData _level;
    [SerializeField] private Pipe _cellPrefab;

    private bool hasGameFinished;
    private Pipe[,] pipes; // Khai bao mang 2 chieu
    private List<Pipe> startPipes;


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
}
