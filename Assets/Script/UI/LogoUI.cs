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
    public GameObject ClickLabel;

    private IEnumerator ShowLogo()
    {
        OriginalGroup.alpha = 0;
        OriginalGroup.DOFade(1, 1);
        yield return new WaitForSeconds(2.0f);
        OriginalGroup.DOFade(0, 1);

        yield return new WaitForSeconds(1.0f);

        ClubLogo.DOFade(1, 1);
        yield return new WaitForSeconds(2.0f);
        ClubLogo.DOFade(0, 1);

        yield return new WaitForSeconds(1.0f);

        TitleLogo.DOFade(1, 1);
        yield return new WaitForSeconds(1.0f);
        ClickLabel.SetActive(true);
    }

    public override void AddStates()
    {
        AddState<OriginalState>();
        AddState<ClubState>();
        AddState<TitleState>();

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

            if (!ProgressManager.Instance.Memo.FlagList[0]) //尚未結束第一個對話
            {
                parent.gameObject.SetActive(false);
                Plot_1 plot_1 = new Plot_1();
                plot_1.Start();
            }
            else if (!ProgressManager.Instance.Memo.FlagList[1]) //尚未結束第二個對話
            {
                parent.gameObject.SetActive(false);
                Plot_2 plot_2 = new Plot_2();
                plot_2.Start();
            }
            else if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Explore)
            {
                MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Explore, () =>
                {
                    ExploreController.Instance.SetFloorFromMemo();
                });
            }
            else if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.Battle)
            {
                MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle, () =>
                {
                    BattleController.Instance.InitFromMemo();
                });
            }
            else if (MySceneManager.Instance.CurrentScene == MySceneManager.SceneType.FirstBattle)
            {
                MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Battle, () =>
                {
                    BattleController.Instance.InitFromMemo(() =>
                    {
                        Plot_2 plot_2 = new Plot_2();
                        plot_2.Start();
                    }, () =>
                    {
                        MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Logo);
                    });
                });
            }
            else
            {
                MySceneManager.Instance.ChangeScene(MySceneManager.SceneType.Villiage);
            }
        }

        private IEnumerator Show()
        {
            parent.TitleLogo.DOFade(1, 1);
            yield return new WaitForSeconds(1.0f);
            parent.ClickLabel.SetActive(true);
            AudioSystem.Instance.Play("Forest");
        }
    }

    private void OnClick() 
    {
        ((LogoState)currentState).ScreenOnClick();
    }

    private void Awake()
    {
        OriginalGroup.alpha = 0;
        ClubLogo.color = new Color(1, 1, 1, 0);
        TitleLogo.color = new Color(1, 1, 1, 0);
        ClickLabel.SetActive(false);

        //StartCoroutine(ShowLogo());

        Button.onClick.AddListener(OnClick);
    }
}
