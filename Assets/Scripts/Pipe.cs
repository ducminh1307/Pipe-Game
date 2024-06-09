using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class Pipe : MonoBehaviour
{
    //[HideInInspector] public bool isFilled;
    public bool isFilled;
    public bool canFill { get; private set; }
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

        //Neu la o trong va pipe_1 thi se co nuoc
        if (pipeType == 0 || pipeType == 1)
        {
            isFilled = true;
            canFill = true;
        }

        //Neu la Cell rong thi khong lam gi
        if (pipeType == 0)
            return;

        //Neu la pipe_1 hoac pipe_2 thi xoay theo ket qua chia lay phan nguyen neu la cac pipe con lai thi xoay random 90, 180, 270 do
        if (pipeType != 0)
            rotation = pipe / 10;
        //else
        //    rotation = Random.Range(minRotation, maxRotation + 1);

        currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);

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

    /// <summary>
    /// Xoay ong nuoc
    /// </summary>
    public void UpdateInput()
    {
        //Neu la Cell rong, pipe_1, pipe_2 thi khong xoay duoc
        if (pipeType == 0 || pipeType == 1 || pipeType == 2)
            return;

        //Xoay them 90 do
        rotation = (rotation + 1) % (maxRotation + 1); // phai cong chinh len phep chia lay du voi 4 thi moi xoay ong voi cac goc 0, 90, 180, 270
        currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);
    }

    public void UpdateFilled()
    {
        //Neu la Cell rong thi khong lam gi
        if (pipeType == 0) return;

        //Bat / tat sprite co nuoc cua pipe theo isFilled
        emptySprite.gameObject.SetActive(!isFilled);
        filledSprite.gameObject.SetActive(isFilled);
    }

    /// <summary>
    /// Kiem tra va dua ra ket qua cac ong nuoc duoc noi voi Pipe
    /// </summary>
    /// <returns>Cac ong nuoc duoc noi</returns>
    public List<Pipe> ConnectPipes()
    {
        //Khoi tao list result de chua cac pipe ket noi voi Pipe
        List<Pipe> result = new List<Pipe>();
        //Khoi tao so ong nuoc da lien ket
        int connectedPipe = 0;

        //Lay tung collider cua Pipe ra kiem tra
        foreach (var box in connectBoxes)
        {
            //Lay tat ca cac Pipe co collider cham vao collider cua Pipe
            Collider2D[] hits = Physics2D.OverlapCircleAll(box.transform.position, 0.15f);
            for (int i = 0; i < hits.Length; i++)
            {
                result.Add(hits[i].transform.parent.parent.GetComponent<Pipe>()); //Them Pipe noi voi Pipe vao result
                //(collier la con cua pipe, pipe la con cua Cell, Cell chua component Pipe)
                connectedPipe++;
            }
        }

        //Tru di cac dau cua ong nuoc
        connectedPipe -= connectBoxes.Count;

        //Kiem tra neu tat ca cac ong nuoc duoc noi thi canFill = true
        if (connectedPipe == connectBoxes.Count)
            canFill = true;
        else
            canFill = false;

        return result;
    }
}
