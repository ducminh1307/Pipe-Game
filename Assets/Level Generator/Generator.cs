using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Generator : MonoBehaviour
{
    public static Generator Instance;

    [Header("Elements")]
    [SerializeField] private LevelData _level;
    [SerializeField] private SpawnCell _cellPrefab;

    [Header("Datas")]
    [SerializeField] private int _row, _col;
    [SerializeField] private int turns;

    private bool hasGameFinished;
    private SpawnCell[,] pipes; // Khai bao mang 2 chieu
    private List<SpawnCell> startPipes;

    private WaitForSeconds timeCheck = new WaitForSeconds(.1f);

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance);
        Instance = this;

        CreateLevelData();
        SpawnLevel();
    }

    // Ham sinh cap do choi
    private void SpawnLevel()
    {
        // Khoi tao mang 2 chieu co chieu dai la _level.column phan tu, chieu rong la _level.row phan tu
        pipes = new SpawnCell[_level.row, _level.column];
        startPipes = new List<SpawnCell>();

        for (int i = 0; i < _level.row; i++)
        {
            for (int j = 0; j < _level.column; j++)
            {
                //Tinh toan toa do sinh ra cua Cell
                Vector2 spawnPos = new Vector2(j + .5f, i + .5f);

                //Tao Cell voi toa do da tinh
                SpawnCell tempPipe = Instantiate(_cellPrefab);
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

        //Dat kich thuoc cua camera bang gia tri lon nhat cua row va column
        Camera.main.orthographicSize = Mathf.Max(_level.row, _level.column);

        //Tinh toan va dat vi tri camera o giua level duoc tao ra
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = _level.column / 2f;
        cameraPos.y = _level.row / 2f;
        Camera.main.transform.position = cameraPos;
    }

    private void CreateLevelData()
    {
        _level.row = _row;
        _level.column = _col;
        _level.data = new List<int>();

        for (int i = 0; i < _level.row; i++)
        {
            for (int j = 0; j < _level.column; j++)
            {
                _level.data.Add(0);
            }
        }
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
        if (row >= _level.row || col >= _level.column) return;

        //Khi an vao cac ong nuoc thi se lam cho cac ong nuoc xoay
        if (Input.GetMouseButtonDown(0))
        {
            pipes[row, col].UpdateInput();
            StartCoroutine(ShowHint());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pipes[row, col].Init(0);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            pipes[row, col].Init(1);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            pipes[row, col].Init(2);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            pipes[row, col].Init(3);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            pipes[row, col].Init(4);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            pipes[row, col].Init(5);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            pipes[row, col].Init(6);
        }

        StartCoroutine(ShowHint());
    }

    //Sau 0.1s kiem tra xem co thang khong
    private IEnumerator ShowHint()
    {
        yield return timeCheck;
        ResetStartPipe();
        CheckFill();
    }

    //Kiem tra de tao cac ong co nuoc
    private void CheckFill()
    {
        // Dua cac ong ve trang thai rong
        for (int i = 0; i < _level.row; i++)
        {
            for (int j = 0; j < _level.column; j++)
            {
                SpawnCell tempPipe = pipes[i, j];

                if (tempPipe.pipeType != 0) tempPipe.isFilled = false;
            }
        }

        Queue<SpawnCell> check = new Queue<SpawnCell>(); // Khoi tao mot hang doi de luu tru cac Pipe
        HashSet<SpawnCell> finished = new HashSet<SpawnCell>(); // Tao mot tap hop chua cac Pipe khong trung nhau khi duyet xog

        foreach (var pipe in startPipes)
        {
            check.Enqueue(pipe); // Them cac pipe_1 vao trong hang doi
        }

        while (check.Count > 0)
        {
            SpawnCell pipe = check.Dequeue(); // Lay pipe dau tien ra khoi hang doi
            finished.Add(pipe); // dua pipe do vao trong HashSet

            List<SpawnCell> connected = pipe.ConnectPipes(); //Lay cac ong co ket noi voi ong dau

            foreach (var connectedPipe in connected)
            {
                // Kiem tra neu ong khong co trong HashSet thi them ong do vao trong hang doi
                if (!finished.Contains(connectedPipe)) check.Enqueue(connectedPipe);
            }
        }

        // Doi trang thai cac ong co noi voi nhau sang "true"
        foreach (var filled in finished)
        {
            filled.isFilled = true;
        }

        // Hien thi cac ong co trang thai isFilled
        for (int i = 0; i < _level.row; i++)
        {
            for (int j = 0; j < _level.column; j++)
            {
                SpawnCell tempPipe = pipes[i, j];
                tempPipe.UpdateFilled();
            }
        }
    }

    //Reset start pipe
    public void ResetStartPipe()
    {
        startPipes = new List<SpawnCell> ();

        for (int i = 0; i < _level.row; i++)
        {
            for (int j = 0; j < _level.column; j++)
            {
                if (pipes[i, j].pipeType == 1)
                {
                    startPipes.Add(pipes[i, j]);
                }
            }
        }
    }

    // Luu du lieu
    public void SaveData()
    {
        for (int i = 0; i < _level.row; ++i)
        {
            for (int j = 0; j < _level.column; ++j)
            {
                _level.data[i * _level.column + j] = pipes[i, j].pipeData;
            }
        }

        _level.turns = turns;

        // Cap nhat du lieu cua ScriptableObject level
        EditorUtility.SetDirty(_level);
    }
}
