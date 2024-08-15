using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using DG.Tweening;
public class SlotView : MonoBehaviour
{
    [Header("slots")]
    [SerializeField] internal Transform[] slots;
    [SerializeField] private GameObject IconPrefab;
    [SerializeField] private SpriteAtlas spriteAtlas;
    [SerializeField] private List<Reel> resultMatrix = new List<Reel>(3);
    [SerializeField] internal List<Icon> animatedIcons = new List<Icon>();
    [SerializeField] internal Vector2 iconSize;
    [SerializeField] private float spacing = 30;
    [SerializeField] private Vector2 slotMatrix;

    [Header("payline")]
    [SerializeField] private GameObject lineObject;
    [SerializeField] private Transform lineCanvas;
    [SerializeField] private Vector2 InitialLinePosition;
    [SerializeField] private int x_Distance;
    [SerializeField] private int y_Distance;
    [SerializeField] private List<GameObject> wininglines;

     internal List<Tweener> allAnimTweens= new List<Tweener>(); 

    internal void PopulateReels()
    {
        clearLine();


        for (int i = 0; i < 15; i++)
        {
            PopulateRow(i);
        }

    }

    internal void PopulateReels(List<List<int>> iconsId)
    {
        clearLine();



        for (int i = 0; i < slotMatrix.x; i++)
        {
            PopulateRow(i, iconsId[iconsId.Count - i - 1]);

        }


    }


    void PopulateRow(int index)
    {
        for (int i = 0; i < slotMatrix.y; i++)
        {
            Image image = Instantiate(IconPrefab, slots[i]).GetComponent<Image>();
            ImageAnimation imageAnimation = image.GetComponent<ImageAnimation>();
            int id = UnityEngine.Random.Range(0, spriteAtlas.spriteCount);
            image.sprite = spriteAtlas.GetSprite(id.ToString());
            Icon icon = new Icon(image, id, imageAnimation);
            if (index < (int)slotMatrix.x)
                resultMatrix[(int)slotMatrix.x - 1 - index].reelIcons.Add(icon);
            icon.image.transform.localPosition = new Vector3(0, (iconSize.y + spacing) * index, 0);

        }
    }

    void PopulateRow(int index, List<int> Ids)
    {
            int idinit = UnityEngine.Random.Range(0, spriteAtlas.spriteCount);

        for (int i = 0; i < slotMatrix.y; i++)
        {
            if(index==slotMatrix.x-1)
                Ids[i]=idinit;
            else
            Ids[i]=UnityEngine.Random.Range(0, spriteAtlas.spriteCount);

            resultMatrix[(int)slotMatrix.x - 1 - index].reelIcons[i].image.sprite = spriteAtlas.GetSprite(Ids[i].ToString());
            resultMatrix[(int)slotMatrix.x - 1 - index].reelIcons[i].id = Ids[i];
        }
    }

    internal void GeneratePayLine(List<List<int>> y_index, List<int> lineIndexs)
    {
        Debug.Log("line list :"+JsonConvert.SerializeObject(y_index));
        Debug.Log("line list :"+JsonConvert.SerializeObject(lineIndexs));
        for (int i = 0; i < lineIndexs.Count; i++)
        {
            GameObject lineObj = Instantiate(lineObject, lineCanvas);
            wininglines.Add(lineObj);
            lineObj.transform.localPosition = new Vector2(InitialLinePosition.x, InitialLinePosition.y);
            UILineRenderer line = lineObj.GetComponent<UILineRenderer>();
            var pointlist = new List<Vector2>();
            for (int j = 0; j < y_index[lineIndexs[i]].Count; j++)
            {
                var points = new Vector2() { x = j * x_Distance, y = y_index[lineIndexs[i]][j] * -y_Distance };
                pointlist.Add(points);
            }
            line.Points = pointlist.ToArray();
        }

    }


    internal void clearLine()
    {
        for (int i = 0; i < wininglines.Count; i++)
        {
            Destroy(wininglines[i]);
        }
    }

    internal void PopulateIconAnimation(List<List<string>> pos)
    {
        Debug.Log("pos :" + JsonConvert.SerializeObject(pos));
        List<string> flatList = new List<string>();
        foreach (var innerList in pos)
        {
            foreach (var str in innerList)
            {
                flatList.Add(str);
            }
        }

        Debug.Log("pos :" + JsonConvert.SerializeObject(flatList));

        for (int i = 0; i < flatList.Count; i++)
        {
            string[] numbers = flatList[i].Split(',');
            var symbol = resultMatrix[int.Parse(numbers[1])].reelIcons[int.Parse(numbers[0])];
            animatedIcons.Add(symbol);
            allAnimTweens.Add(symbol.image.transform.DOScale(1.1f,0.7f).SetLoops(-1, LoopType.Yoyo));
        }

    }



}

