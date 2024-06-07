using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pipe : MonoBehaviour
{

    [HideInInspector] public bool isFilled;
    [HideInInspector] public int pipeType;

    [SerializeField] private Transform[] _pipePrefabs;

    private Transform currentPipe;
    private int rotation;

    private SpriteRenderer emptySprite;
    private SpriteRenderer filledSprite;
    private List<Transform> connectBoxes;

    private const int minRotation = 0;
    private const int maxRotation = 3;
    private const int rotationMultiplier = 90;

    /// <summary>
    /// Ham khoi tao pipe
    /// </summary>
    /// <param name="pipe"></param>
    public void Init(int pipe)
    {
        //Tao loai ong bang cach chia lay phan du
        pipeType = pipe % 10;

        //Tao pipe la con cua Cell
        currentPipe = Instantiate(_pipePrefabs[pipeType], transform);
        currentPipe.transform.localPosition = Vector3.zero; // Dua pine ve vi tri mac dinh la (0,0) so voi Cell

        //Neu la Cell rong thi khong lam gi
        if (pipeType == 0)
            return;

        //Neu la pipe_1 hoac pipe_2 thi xoay theo ket qua chia lay phan nguyen neu la cac pipe con lai thi xoay random 90, 180, 270 do
        if (pipeType == 1 || pipeType == 2)
            rotation = pipe / 10;
        else
            rotation = Random.Range(minRotation, maxRotation + 1);

        currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);

        //Neu la pipe_1 thi se co nuoc
        if (pipeType == 1)
            isFilled = true;        

        //Bat/tat sprite co nuoc cua pipe theo isFilled
        emptySprite = currentPipe.GetChild(0).GetComponent<SpriteRenderer>();
        emptySprite.gameObject.SetActive(!isFilled);

        filledSprite = currentPipe.GetChild(1).GetComponent<SpriteRenderer>();
        filledSprite.gameObject.SetActive(isFilled);

        //Lay du lieu cac moi noi cua ong tu cac collider tao trong prefab
        connectBoxes = new List<Transform>();
        for (int i = 2; i < currentPipe.childCount; i++)
        {
            connectBoxes.Add(currentPipe.GetChild(i));
        }
    }
}
