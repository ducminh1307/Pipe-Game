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
    private Pipe[,] pipes;
    private List<Pipe> startPipes;


    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;

        SpwanLevel();
    }

    private void SpwanLevel()
    {
        pipes = new Pipe[_level.row, _level.column];
        startPipes = new List<Pipe>();

        for (int i = 0; i < _level.row; i++)
        {
            for (int j = 0; j <  _level.column; j++)
            {
                Vector2 spawnPos = new Vector2(j + .5f, i + .5f);
                Pipe tempPipe = Instantiate(_cellPrefab);
                tempPipe.transform.position = spawnPos;
                tempPipe.Init(_level.data[i * _level.column + j]);

                pipes[i, j] = tempPipe;

                if (tempPipe.pipeType == 1)
                {
                    startPipes.Add(tempPipe);
                }
            }
        }

        Camera.main.orthographicSize = Mathf.Max(_level.row, _level.column);

        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = _level.column /2f;
        cameraPos.y = _level.row /2f;
        Camera.main.transform.position = cameraPos;
    }
}
