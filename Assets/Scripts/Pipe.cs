using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pipe : MonoBehaviour
{

    [HideInInspector] public bool IsFilled;
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

    public void Init(int pipe)
    {
        pipeType = pipe % 10;

        currentPipe = Instantiate(_pipePrefabs[pipeType], transform);
        currentPipe.transform.localPosition = Vector3.zero;

        if (pipeType == 1 || pipeType == 2)
            rotation = pipe / 10;
        else
            rotation = Random.Range(minRotation, maxRotation + 1);

        currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);

        if (pipe == 0 || pipeType == 1)
            IsFilled = true;

        if (pipeType == 0)
            return;

        emptySprite = currentPipe.GetChild(0).GetComponent<SpriteRenderer>();
        emptySprite.gameObject.SetActive(!IsFilled);
        filledSprite = currentPipe.GetChild(1).GetComponent<SpriteRenderer>();
        filledSprite.gameObject.SetActive(IsFilled);

        connectBoxes = new List<Transform>();
        for (int i = 2; i < currentPipe.childCount; i++)
        {
            connectBoxes.Add(currentPipe.GetChild(i));
        }
    }
}
