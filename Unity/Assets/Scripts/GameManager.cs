using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using XboxCtrlrInput;
using UnityEngine.EventSystems;

public class GameManager : MonoSingleton<GameManager> {

    public enum GAME_STATE
    {
        INTRO = -1,
        INTERVENTION = 0,
        ANSWERS = 1,
        RESULTS = 2,
        ENDING = 3
    };

    [Header ("Game Elements")]
    public Noble[] nobles;
    public Player[] players;
    public AudioClip[] emptyAnswers;
    public AudioClip audioBanquet;

    [Header ("UI")]
    public CanvasGroup panelAnswers;
    public CanvasGroup fade;
    public Image textDialogUICanvas;
    public Text textDialogUI;
    public RectTransform[] cursors;
    public RectTransform clock;
    public Image clockTimer;
    public List<ButtonAnswer> buttons;
    public CanvasGroup panelScores;
    public CanvasGroup panelEnding;
    public Text scoresText0;
    public Text scoresText1;
    public Text scoresText2;
    public Text scoresText3;

    [Header("Intro")]
    public List<Sprite> sprIntroShots;
    public List<float> introDurations;
    public Image imgIntroA, imgIntroB;

    [Header ("Game Settings")]
    public GAME_STATE currentGameState = GAME_STATE.INTRO;
    public float dialogDuration = 3.0f;
    private float timerAnswerPhase = 0.0f;
    public float answerPhaseDuration = 10.0f;

    [Header ("External Data")]
    public DialogDatabase dialogDatabase;
    public AudioClip[] clipsQuestions;
    //public List<AudioClip> clipsAnswers;

    private AudioSource audioSource;
    private int currentInterventionPos = 0;
    private Intervention currentIntervention = null;

    private Answer selectedD0, selectedD1, selectedD2, selectedD3;
    private PointerEventData pd0, pd1, pd2, pd3;

    private float pX1, pX2, pX3, pX4, pY1, pY2, pY3, pY4;

    private Answer muteAnswer;

    protected override void Awake()
    {
        base.Awake();
        if (GameManager.instance != this) return;

        muteAnswer = new Answer();
        muteAnswer.text = "...";
        muteAnswer.id_dialog = -1;

        fade.alpha = 1;
        panelAnswers.alpha = 0;
        clockTimer.fillAmount = 0;

        audioSource = GetComponent<AudioSource>();
    }

    void Start ()
    {
        Init();
    }

    public void Init()
    {
        textDialogUI.text = "";

        cursors[0].anchoredPosition = new Vector3(cursors[0].parent.GetComponent<RectTransform>().rect.width / 2 - 32, cursors[0].parent.GetComponent<RectTransform>().rect.height / 2 + 32, 0);
        cursors[1].anchoredPosition = new Vector3(cursors[1].parent.GetComponent<RectTransform>().rect.width / 2 + 32, cursors[1].parent.GetComponent<RectTransform>().rect.height / 2 + 32, 0);
        cursors[2].anchoredPosition = new Vector3(cursors[2].parent.GetComponent<RectTransform>().rect.width / 2 - 32, cursors[2].parent.GetComponent<RectTransform>().rect.height / 2 - 32, 0);
        cursors[3].anchoredPosition = new Vector3(cursors[3].parent.GetComponent<RectTransform>().rect.width / 2 + 32, cursors[3].parent.GetComponent<RectTransform>().rect.height / 2 - 32, 0);

        fade.alpha = 1;
        clock.DOMoveY(3.5f, 0.0f);

        for (int i = 0; i < players.Length; i++) players[i].score = 0;

        SetGameState(currentGameState);
    }

    public void SetGameState(GAME_STATE state)
    {
        currentGameState = state;
        switch(state)
        {
            case GAME_STATE.INTRO:
                PhaseIntro();
                break;
            case GAME_STATE.INTERVENTION:
                PhaseIntervention();
                break;
            case GAME_STATE.ANSWERS:
                PhaseAnswers();
                break;
            case GAME_STATE.RESULTS:
                PhaseResults();
                break;
            case GAME_STATE.ENDING:
                PhaseEnding();
                break;
        }
    }

    void Update () {
        UpdateCursors();

        switch (currentGameState)
        {
            case GAME_STATE.ANSWERS:

                //EventSystem.current.RaycastAll(pd0, raycastResults0);
                //EventSystem.current.RaycastAll(pd1, raycastResults1);
                //EventSystem.current.RaycastAll(pd2, raycastResults2);
                //EventSystem.current.RaycastAll(pd3, raycastResults3);

                timerAnswerPhase += Time.deltaTime;
                if (timerAnswerPhase >= answerPhaseDuration)
                {
                    timerAnswerPhase = 5;

                    //if (raycastResults0.Count > 0)
                    //{
                    //    // Do something on buttons
                    //}
                    
                    SetGameState(GAME_STATE.RESULTS);
                }
                clockTimer.fillAmount = timerAnswerPhase / answerPhaseDuration;
                break;
        }

    }

    private void UpdateCursors()
    {
        Vector3 cursor0pos = cursors[0].anchoredPosition;
        Vector3 cursor1pos = cursors[1].anchoredPosition;
        Vector3 cursor2pos = cursors[2].anchoredPosition;
        Vector3 cursor3pos = cursors[3].anchoredPosition;

        cursor0pos += new Vector3(XCI.GetAxis(XboxAxis.LeftStickX, XboxController.First), XCI.GetAxis(XboxAxis.LeftStickY, XboxController.First), 0) * Time.deltaTime * 300;
        cursor1pos += new Vector3(XCI.GetAxis(XboxAxis.LeftStickX, XboxController.Second), XCI.GetAxis(XboxAxis.LeftStickY, XboxController.Second), 0) * Time.deltaTime * 300;
        cursor2pos += new Vector3(XCI.GetAxis(XboxAxis.LeftStickX, XboxController.Third), XCI.GetAxis(XboxAxis.LeftStickY, XboxController.Third), 0) * Time.deltaTime * 300;
        cursor3pos += new Vector3(XCI.GetAxis(XboxAxis.LeftStickX, XboxController.Fourth), XCI.GetAxis(XboxAxis.LeftStickY, XboxController.Fourth), 0) * Time.deltaTime * 300;

        VecClamped(ref cursor0pos);
        VecClamped(ref cursor1pos);
        VecClamped(ref cursor2pos);
        VecClamped(ref cursor3pos);

        cursors[0].anchoredPosition = cursor0pos;
        cursors[1].anchoredPosition = cursor1pos;
        cursors[2].anchoredPosition = cursor2pos;
        cursors[3].anchoredPosition = cursor3pos;

        pX1 = (cursors[0].anchoredPosition.x / cursors[0].parent.GetComponent<RectTransform>().rect.width);
        pY1 = (cursors[0].anchoredPosition.y / cursors[0].parent.GetComponent<RectTransform>().rect.height);

        pX2 = (cursors[1].anchoredPosition.x / cursors[1].parent.GetComponent<RectTransform>().rect.width);
        pY2 = (cursors[1].anchoredPosition.y / cursors[1].parent.GetComponent<RectTransform>().rect.height);

        pX3 = (cursors[2].anchoredPosition.x / cursors[2].parent.GetComponent<RectTransform>().rect.width);
        pY3 = (cursors[2].anchoredPosition.y / cursors[2].parent.GetComponent<RectTransform>().rect.height);

        pX4 = (cursors[3].anchoredPosition.x / cursors[3].parent.GetComponent<RectTransform>().rect.width);
        pY4 = (cursors[3].anchoredPosition.y / cursors[3].parent.GetComponent<RectTransform>().rect.height);
        //pointer.position = Camera.main.WorldToScreenPoint(someWorldPosition);

        if (pd0 == null)
            pd0 = new PointerEventData(EventSystem.current);

        //pd0.position = new Vector2(Screen.width * pX1, Screen.height * pY1);
        pd0.position = cursors[0].position;

        if (pd1 == null)
            pd1 = new PointerEventData(EventSystem.current);

        //pd1.position = new Vector2(Screen.width * pX2, Screen.height * pY2);
        pd1.position = cursors[1].position;

        if (pd2 == null)
            pd2 = new PointerEventData(EventSystem.current);

        //pd2.position = new Vector2(Screen.width * pX3, Screen.height * pY3);
        pd2.position = cursors[2].position;

        if (pd3 == null)
            pd3 = new PointerEventData(EventSystem.current);

        //pd3.position = new Vector2(Screen.width * pX4, Screen.height * pY4);
        pd3.position = cursors[3].position;

        int bId0 = Mathf.FloorToInt(pX1 * 2) * 8 + Mathf.FloorToInt(pY1 * 8);
        int bId1 = Mathf.FloorToInt(pX2 * 2) * 8 + Mathf.FloorToInt(pY2 * 8);
        int bId2 = Mathf.FloorToInt(pX3 * 2) * 8 + Mathf.FloorToInt(pY3 * 8);
        int bId3 = Mathf.FloorToInt(pX4 * 2) * 8 + Mathf.FloorToInt(pY4 * 8);

        FocusButton(bId0, players[0]);
        FocusButton(bId1, players[1]);
        FocusButton(bId2, players[2]);
        FocusButton(bId3, players[3]);

        if (XCI.GetButtonDown(XboxButton.A, XboxController.First))
        {
            Answer a1 = dialogDatabase.answers[bId0];
            if (a1 != selectedD1 && a1 != selectedD2 && a1 != selectedD3) selectedD0 = a1;
            SelectButton(bId0, players[0]);
        }
        if (XCI.GetButtonDown(XboxButton.A, XboxController.Second))
        {
            Answer a1 = dialogDatabase.answers[bId1];
            if (a1 != selectedD0 && a1 != selectedD2 && a1 != selectedD3) selectedD1 = a1;
            SelectButton(bId1, players[1]);
        }
        if (XCI.GetButtonDown(XboxButton.A, XboxController.Third))
        {
            Answer a1 = dialogDatabase.answers[bId2];
            if (a1 != selectedD1 && a1 != selectedD0 && a1 != selectedD3) selectedD2 = a1;
            SelectButton(bId2, players[2]);
        }
        if (XCI.GetButtonDown(XboxButton.A, XboxController.Fourth))
        {
            Answer a1 = dialogDatabase.answers[bId3];
            if (a1 != selectedD1 && a1 != selectedD2 && a1 != selectedD0) selectedD3 = a1;
            SelectButton(bId3, players[3]);
        }


    }

    public void PhaseIntro()
    {
        StartCoroutine(PhaseIntroCoroutine());
    }

    public void MoveInHierarchy(Transform t, int delta)
    {
        int index = t.GetSiblingIndex();
        t.SetSiblingIndex(index + delta);
    }

    public IEnumerator PhaseIntroCoroutine()
    {
        fade.alpha = 1;
        AudioSource src = Camera.main.GetComponent<AudioSource>();
        yield return new WaitForSeconds(2.0f);
        AudioClip a = Resources.Load<AudioClip>("Audio/champagne_rag_intro");
        if (a != null)
        {
            src.clip = a;
            src.loop = false;
            src.Play();
        }
        for (int i = 0; i < sprIntroShots.Count; i++)
        {
            Sprite s = sprIntroShots[i];
            if (i%2 == 0)
            {
                MoveInHierarchy(imgIntroA.transform, i+1);
                imgIntroA.sprite = s;
                imgIntroA.color = new Color(1, 1, 1, 0);
                imgIntroA.DOFade(1.0f, 1.0f).SetEase(Ease.Linear);
            }
            else
            {
                MoveInHierarchy(imgIntroB.transform, i + 1);
                imgIntroB.sprite = s;
                imgIntroB.color = new Color(1, 1, 1, 0);
                imgIntroB.DOFade(1.0f, 1.0f).SetEase(Ease.Linear);
            }

            if (i == 12) src.DOFade(0.0f, 3.0f);
            yield return new WaitForSeconds(introDurations[i]);
        }

        imgIntroA.DOFade(0.0f, 1.0f).SetEase(Ease.Linear);
        imgIntroB.DOFade(0.0f, 1.0f).SetEase(Ease.Linear);

        fade.DOFade(0, 5.0f).SetEase(Ease.Linear).SetDelay(1.0f);

        yield return new WaitForSeconds(3.0f);
        src.clip = audioBanquet;
        src.loop = true;
        src.Play();
        src.DOFade(1.0f, 3.0f);

        yield return new WaitForSeconds(3.0f);
        SetGameState(GAME_STATE.INTERVENTION);
    }

    public void PhaseEnding()
    {
        StartCoroutine(PhaseEndingCoroutine());
    }

    public IEnumerator PhaseEndingCoroutine()
    {
        List<Player> pScores = new List<Player>();
        foreach (Player p in players) pScores.Add(p);
        System.Comparison<Player> cmp = new System.Comparison<Player>(Player.CompareId);
        pScores.Sort(cmp);

        scoresText0.text = "El " + pScores[0].cname + " encontró pretendienta de alta cuna y jamás trabajó en su vida.";
        scoresText1.text = "El " + pScores[1].cname + " fue invitado a jugar al pádel con el Señor Perro.";
        scoresText2.text = "El " + pScores[2].cname + " nunca fue invitado a otros ritos nupciales.";
        scoresText3.text = "El " + pScores[3].cname + " murió en un \"desafortunado\" accidente de caza semanas después.";

        panelScores.DOFade(1.0f, 1.0f);

        yield return new WaitForSeconds(15.0f);

        panelEnding.DOFade(1.0f, 1.0f);

        yield return new WaitForSeconds(5.0f);
        panelAnswers.DOFade(0.0f, 0.0f);
        panelEnding.DOFade(0.0f, 1.0f);
        yield return new WaitForSeconds(1.0f);
        SetGameState(GAME_STATE.INTRO);
    }

    public void PhaseIntervention()
    {
        fade.alpha = 0;
        if (currentInterventionPos >= dialogDatabase.interventions.Count) SetGameState(GAME_STATE.ENDING);

        panelAnswers.DOFade(0.0f, 0.5f);
        textDialogUICanvas.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f);

        currentIntervention = dialogDatabase.interventions[currentInterventionPos];

        StartCoroutine(LaunchIntervention(currentIntervention, 0.0f));

        currentInterventionPos++;
    }

    public IEnumerator LaunchIntervention(Intervention intervention, float delay)
    {
        yield return new WaitForSeconds(delay);
        currentIntervention = intervention;

        for (int i = 0; i < intervention.questions_ids.Count; i++)
        {
            LaunchQuestion(dialogDatabase.GetQuestion(intervention.questions_ids[i]));
            yield return new WaitForSeconds(5.0f);
            ClearAnimations();
        }

        SetGameState(GAME_STATE.ANSWERS);
    }
    
    public void PhaseAnswers()
    {
        ClearButtons();
        timerAnswerPhase = 0;
        panelAnswers.DOFade(1.0f, 0.5f);
        textDialogUICanvas.GetComponent<CanvasGroup>().DOFade(0.0f, 0.5f);

        float p = (float)dialogDatabase.interventions.IndexOf(currentIntervention) / (float) dialogDatabase.interventions.Count;
        answerPhaseDuration = 10 - (p * 5);
       
        clock.DOMoveY(2.55f, 1.0f);

        selectedD0 = null;
        selectedD1 = null;
        selectedD2 = null;
        selectedD3 = null;
    }

    public void PhaseResults()
    {
        panelAnswers.DOFade(0.0f, 3.0f);
        textDialogUICanvas.GetComponent<CanvasGroup>().DOFade(1.0f, 0.5f);

        clock.DOMoveY(3.5f, 1.0f);
        clockTimer.fillAmount = 0;

        StartCoroutine(PhaseResultsCoroutine());
    }

    public IEnumerator PhaseResultsCoroutine()
    {
        if (selectedD0 == null) selectedD0 = muteAnswer;
        if (selectedD1 == null) selectedD1 = muteAnswer;
        if (selectedD2 == null) selectedD2 = muteAnswer;
        if (selectedD3 == null) selectedD3 = muteAnswer;

        LaunchAnswer(selectedD0, 0);

        yield return new WaitForSeconds(5.0f);

        LaunchAnswer(selectedD1, 1);

        yield return new WaitForSeconds(5.0f);

        LaunchAnswer(selectedD2, 2);

        yield return new WaitForSeconds(5.0f);

        LaunchAnswer(selectedD3, 3);

        yield return new WaitForSeconds(5.0f);

        SetMassNobleAnimation(Character.ANIMATION.NORMAL);
        SetGameState(GAME_STATE.INTERVENTION);
    }

    public void SetMassNobleAnimation(Character.ANIMATION anim)
    {
        for (int i = 0; i < nobles.Length; i++)
        {
            nobles[i].SetAnimation(anim);
        }
    }

    public void ClearDialog()
    {
        textDialogUI.text = "";
    }

    public void ClearAnimations()
    {
        for (int i = 0; i < nobles.Length; i++) nobles[i].SetAnimation(Character.ANIMATION.NORMAL);
        for (int i = 0; i < players.Length; i++) players[i].SetAnimation(Character.ANIMATION.NORMAL);
    }

    public void LaunchQuestion(Question d, int playerN = -1)
    {
        ClearDialog();
        ClearAnimations();
        textDialogUI.DOText(d.text, dialogDuration).SetEase(Ease.Linear);

        Character c = null;
        c = nobles[(int)d.id_character - 1];
        int id_audio = d.id_dialog;
        if (clipsQuestions.Length > id_audio && clipsQuestions[id_audio] != null) audioSource.PlayOneShot(clipsQuestions[id_audio]);

        textDialogUICanvas.DOColor(c.characterColor, 1.0f);
        c.SetAnimation(Character.ANIMATION.TALKING);
    }

    public void LaunchAnswer(Answer d, int playerN = -1)
    {
        if (d == muteAnswer)
        {
            LaunchDialogEmpty(playerN);
            return;
        }

        ClearDialog();
        ClearAnimations();
        textDialogUI.DOText(d.text, dialogDuration).SetEase(Ease.Linear);

        Character c = null;
        c = players[playerN];
        int id_audio = 0;
        string id_s = "";

        int answer_pos = dialogDatabase.answers.IndexOf(d);
        id_audio = answer_pos + playerN*4;
        id_s = id_audio.ToString();
        if (id_audio < 10) id_s = "0" + id_s;
        AudioClip a = Resources.Load<AudioClip>("Audio/Answers/" + id_s);
        if (a != null) audioSource.PlayOneShot(a);
        
        //if (clipsAnswers.Count > id_audio && clipsAnswers[id_audio] != null) audioSource.PlayOneShot(clipsAnswers[id_audio]);
        textDialogUICanvas.DOColor(c.characterColor, 1.0f);

        c.SetAnimation(Character.ANIMATION.TALKING);

        // Intervention score

        int score = currentIntervention.scores[answer_pos];
        players[playerN].score += score;
        if (score > 0) SetMassNobleAnimation(Character.ANIMATION.SURPRISED);
        else if (score < 0) SetMassNobleAnimation(Character.ANIMATION.ANGRY);
        else SetMassNobleAnimation(Character.ANIMATION.NORMAL);
    }

    public void LaunchDialogEmpty(int playerN)
    {
        ClearDialog();
        ClearAnimations();
        textDialogUI.DOText(muteAnswer.text, dialogDuration).SetEase(Ease.Linear);

        Character c = null;

            c = players[playerN];
        AudioClip a = emptyAnswers[playerN];
            //int id_audio = dialogDatabase.answers.IndexOf((Answer)d) * playerN;
            //string id_s = id_audio.ToString();
            //if (id_audio < 10) id_s = "0" + id_s;
            //AudioClip a = Resources.Load<AudioClip>("Audio/Answers/" + id_s);
            if (a != null) audioSource.PlayOneShot(a);
            ////if (clipsAnswers.Count > id_audio && clipsAnswers[id_audio] != null) audioSource.PlayOneShot(clipsAnswers[id_audio]);
        textDialogUICanvas.DOColor(c.characterColor, 1.0f);
        c.SetAnimation(Character.ANIMATION.TALKING);

        SetMassNobleAnimation(Character.ANIMATION.NORMAL);
        //c.SetAnimation(Character.ANIMATION.TALKING);
    }


    private void FocusButton(int id, Player p)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].id == id)
            {
                buttons[i].playersFocusing.Add(p);
            }
            else buttons[i].playersFocusing.Remove(p);
        }
    }

    private void SelectButton(int id, Player p)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i].id == id)
            {
                buttons[i].playerSelected = p;
                buttons[i].SetColor(p.characterColor);
            }
            else
            {
                if (buttons[i].playerSelected == p) buttons[i].playerSelected = null;
            }
        }
    }

    private void ClearButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].Reset();
        }
    }

    private Vector3 VecClamped(ref Vector3 v)
    {
        v.x = Mathf.Clamp(v.x, 0.001f, Screen.width * 0.799f);
        v.y = Mathf.Clamp(v.y, 0.001f, Screen.height * 0.799f);

        return v;
    }

}
