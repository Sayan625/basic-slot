using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System;
using System.Collections;
public class SlotController : MonoBehaviour
{

    [SerializeField] private SlotView slotView;
    [SerializeField] private int betIndex;

    private List<Tween> alltweens = new List<Tween>();
    void Start()
    {
        slotView.PopulateReels();



        //grid height=no icons*iconheight + (no icons-1)*space
        //slot pos=slot transformheight/2+space
    }

    internal  IEnumerator  PopulateSlotAndCheckResult(ResultGameData resultGameData, InitGameData initGameData, Action<int> setFreeSpin,Action<bool> isMatched)
    {
        slotView.PopulateReels(resultGameData.ResultReel);
        Debug.Log(alltweens.Count+"tweens");
        for (int i = 0; i <  alltweens.Count; i++)
        {
            alltweens[i].Pause();
            alltweens[i] = slotView.slots[i].DOLocalMoveY(-220+10, 0.5f).SetEase(Ease.OutElastic);
            yield return new WaitForSeconds(0.25f);

        }

        if (resultGameData.linesToEmit.Count > 0)
        {
            slotView.GeneratePayLine(initGameData.Lines, resultGameData.linesToEmit);
            isMatched(true);
        }
        if(resultGameData.symbolsToEmit.Count>0){
            slotView.PopulateIconAnimation(resultGameData.symbolsToEmit);
            isMatched(true);

        }
        if(resultGameData.freeSpins>0)
        setFreeSpin((int)resultGameData.freeSpins);    
        yield return null;
    }

  
    internal void DepopulateAnimation()
    {

        for (int i = 0; i < slotView.allAnimTweens.Count; i++)
        {
            slotView.allAnimTweens[i].Kill();

        }
        slotView.allAnimTweens.Clear();
        for (int i = 0; i < slotView.animatedIcons.Count; i++)
        {
            slotView.animatedIcons[i].image.transform.localScale= Vector3.one;
        }
         slotView.animatedIcons.Clear();
    }


    internal void StartSpinAnimation()
    {
        KillAllTween();
        DepopulateAnimation();
        slotView.clearLine();
        for (int i = 0; i < slotView.slots.Length; i++)
        {
            // slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
            Tweener tweener = slotView.slots[i].DOLocalMoveY(-1200 + 200, 0.075f).SetLoops(-1, LoopType.Restart).SetDelay(0).SetEase(Ease.InOutBounce);
            tweener.Play();
            alltweens.Add(tweener);
        }

        Debug.Log("safsa"+alltweens.Count);
    }


    internal void KillAllTween()
    {
        for (int i = 0; i < alltweens.Count; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
}
