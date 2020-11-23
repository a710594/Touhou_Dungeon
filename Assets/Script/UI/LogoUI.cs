using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ByTheTale.StateMachine;

public class LogoUI : MachineBehaviour
{
    public CanvasGroup OriginalGroup;
    public Image ClubLogo;
    public Image TitleLogo;
    public Button Button;
    public Button LoadButton;
    public Button NewButton;
    public Button StaffButton;
    public Button CloseStaffButton;
    public GameObject ClickLabel;
    public GameObject PlayGroup;
    public GameObject StaffGroup;

    //private IEnumerator ShowLogo()
    //{
    //    OriginalGroup.alpha = 0;
    //    OriginalGroup.DOFade(1, 1);
    //    yield return new WaitForSeconds(2.0f);
    //    OriginalGroup.DOFade(0, 1);

    //    yield return new WaitForSeconds(1.0f);

    //    ClubLogo.DOFade(1, 1);
    //    yield return new WaitForSeconds(2.0f);
    //    ClubLogo.DOFade(0, 1);

    //    yield return new WaitForSeconds(1.0f);

    //    TitleLogo.DOFade(1, 1);
    //    yield return new WaitForSeconds(1.0f);
    //    ClickLabel.SetActive(true);
    //}

    public override void AddStates()
    {
        AddState<OriginalState>();
        AddState<ClubState>();
        AddState<TitleState>();
        AddState<PlayState>();

        SetInitialState<OriginalState>();
    }

    //
    //State
    //
    private class LogoState : State
    {
        protected LogoUI parent;

        public override void Enter()
        {
            parent = (LogoUI)machine;
        }

        public virtual void ScreenOnClick() { }
    }

    private class OriginalState : LogoState
    {
        private Coroutine _coroutine;

        public override void Enter()
        {
            base.Enter();
            _coroutine = parent.StartCoroutine(Show());
        }

        public override void ScreenOnClick()
        {
            parent.StopCoroutine(_coroutine);
            parent.OriginalGroup.DOKill();
            parent.OriginalGroup.alpha = 0;
            parent.ChangeState<ClubState>();
        }

        private IEnumerator Show()
        {
            parent.OriginalGroup.DOFade(1, 1);
            yield return new WaitForSeconds(2.0f);
            parent.OriginalGroup.DOFade(0, 1);
            yield return new WaitForSeconds(1.0f);
            parent.ChangeState<ClubState>();
        }
    }

    private class ClubState : LogoState
    {
        private Coroutine _coroutine;

        public override void Enter()
        {
            base.Enter();
            _coroutine = parent.StartCoroutine(Show());
        }

        public override void ScreenOnClick()
        {
            parent.StopCoroutine(_coroutine);
            parent.ClubLogo.DOKill();
            parent.ClubLogo.color = new Color(1, 1, 1, 0);
            parent.ChangeState<TitleState>();
        }

        private IEnumerator Show()
        {
            parent.ClubLogo.DOFade(1, 1);
            yield return new WaitForSeconds(2.0f);
            parent.ClubLogo.DOFade(0, 1);
            yield return new WaitForSeconds(1.0f);
            parent.ChangeState<TitleState>();
        }
    }

    private class TitleState : LogoState
    {
        private Coroutine _coroutine;

        public override void Enter()
        {
            base.Enter();
            _coroutine = parent.StartCoroutine(Show());
        }

        public override void ScreenOnClick()
        {
            parent.StopCoroutine(_coroutine);
            parent.TitleLogo.DOKill();
            parent.TitleLogo.color = new Color(1, 1, 1, 0);
            parent.ClickLabel.SetActive(false);
            parent.ChangeState<PlayState>();
            AudioSystem.Instance.Play("Forest");

            //if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Explore)
            //{
            //    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
            //    {
            //        ExploreController.Instance.SetFloorFromMemo();
            //    });
            //}
            //else if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Villiage)
            //{
            //    MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Villiage);
            //}
            //else
            //{
            //    if (!ProgressManager.Instance.Memo.FirstFlag) //尚未結束新手教學
            //    {
            //        parent.gameObject.SetActive(false);
            //        Plot_1 plot_1 = new Plot_1();
            //        plot_1.Start();
            //    }
            //}
        }

        private IEnumerator Show()
        {
            parent.TitleLogo.DOFade(1, 1);
            yield return new WaitForSeconds(1.0f);
            parent.ClickLabel.SetActive(true);
            AudioSystem.Instance.Play("Forest");
        }
    }

    private class PlayState : LogoState
    {
        private Coroutine _coroutine;

        public override void Enter()
        {
            base.Enter();
            _coroutine = parent.StartCoroutine(Show());
        }

        public override void ScreenOnClick()
        {
        }

        private IEnumerator Show()
        {
            yield return null;
            parent.PlayGroup.SetActive(true);
            if (!ProgressManager.Instance.Memo.FirstFlag) //尚未結束新手教學
            {
                parent.LoadButton.gameObject.SetActive(false);
            }
        }
    }

    private void OnClick() 
    {
        ((LogoState)currentState).ScreenOnClick();
    }

    private void LoadOnClick()
    {
        if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Explore)
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
            {
                ExploreController.Instance.SetFloorFromMemo();
            });
        }
        else if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Villiage)
        {
            MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Villiage);
        }
    }

    private void NewOnClick()
    {
        ConfirmUI.Open("請問你確定要重新開始遊戲嗎？", "確定", "取消", ()=> 
        {
            GameSystem.Instance.ClearMemo();
            gameObject.SetActive(false);
            Plot_1 plot_1 = new Plot_1();
            plot_1.Start();
        }, null);
    }

    private void StaffOnClick()
    {
        StaffGroup.SetActive(true);
    }

    private void CloseStaffOnClick()
    {
        StaffGroup.SetActive(false);
    }

    private void Awake()
    {
        OriginalGroup.alpha = 0;
        ClubLogo.color = new Color(1, 1, 1, 0);
        TitleLogo.color = new Color(1, 1, 1, 0);
        ClickLabel.SetActive(false);
        PlayGroup.SetActive(false);

        //StartCoroutine(ShowLogo());

        Button.onClick.AddListener(OnClick);
        LoadButton.onClick.AddListener(LoadOnClick);
        NewButton.onClick.AddListener(NewOnClick);
        StaffButton.onClick.AddListener(StaffOnClick);
        CloseStaffButton.onClick.AddListener(CloseStaffOnClick);
    }
}
