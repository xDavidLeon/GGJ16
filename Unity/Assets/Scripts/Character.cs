using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Character : MonoBehaviour {
    public enum ANIMATION
    {
        NORMAL = 0,
        TALKING = 1,
        ANGRY = 2,
        SURPRISED = 3
    }

    public Dialog.CHARACTER character = Dialog.CHARACTER.NOBLE_1;

    public Color characterColor = Color.black;

    public Transform body;
    public Transform head;

    protected Vector3 originalScale = Vector3.one;
    public Vector3 punchScaleSize = Vector3.one;
    public float punchScaleDuration = 1.0f;

    public Vector3 punchRotationScale = Vector3.one;
    public float punchRotationDuration = 1.0f;

    protected Vector3 originalPosition = Vector3.zero;
    public float shakeDuration = 1.0f;
    public float shakeForce = 1.0f;

    protected ANIMATION currentAnimation = ANIMATION.NORMAL;

    protected Texture2D texBody;
    protected Texture2D texFaceNeutral;

    protected bool isPlayer = false;

    protected virtual void Awake()
    {
        originalPosition = transform.position;

        switch (character)
        {
            case Dialog.CHARACTER.NOBLE_1:
                texBody = Resources.Load("Img/Characters/catlady/body", typeof(Texture2D)) as Texture2D;
                texFaceNeutral = Resources.Load("Img/Characters/catlady/face_neutral", typeof(Texture2D)) as Texture2D;
                break;                                                 
            case Dialog.CHARACTER.NOBLE_2:
                texBody = Resources.Load("Img/Characters/dogman/body", typeof(Texture2D)) as Texture2D;
                texFaceNeutral = Resources.Load("Img/Characters/dogman/face_neutral", typeof(Texture2D)) as Texture2D;
                break;                                                 
            case Dialog.CHARACTER.NOBLE_3:
                texBody = Resources.Load("Img/Characters/bunnyman/body", typeof(Texture2D)) as Texture2D;
                texFaceNeutral = Resources.Load("Img/Characters/bunnyman/face_neutral", typeof(Texture2D)) as Texture2D;
                break;                                                 
            case Dialog.CHARACTER.NOBLE_4:
                texBody = Resources.Load("Img/Characters/birdlady/body", typeof(Texture2D)) as Texture2D;
                texFaceNeutral = Resources.Load("Img/Characters/birdlady/face_neutral", typeof(Texture2D)) as Texture2D;
                break;                                                 
            case Dialog.CHARACTER.P_1:
                texBody = Resources.Load("Img/Characters/player/player1", typeof(Texture2D)) as Texture2D;
                break;                                                 
            case Dialog.CHARACTER.P_2:
                texBody = Resources.Load("Img/Characters/player/player2", typeof(Texture2D)) as Texture2D;
                break;                                                 
            case Dialog.CHARACTER.P_3:
                texBody = Resources.Load("Img/Characters/player/player3", typeof(Texture2D)) as Texture2D;
                break;                                                 
            case Dialog.CHARACTER.P_4:
                texBody = Resources.Load("Img/Characters/player/player4", typeof(Texture2D)) as Texture2D;
                break;
        }

        if ((int)character > 4) isPlayer = true;

        body.GetComponent<MeshRenderer>().material.SetTexture("_MainTex",texBody);
        if (!isPlayer)
        {
            head.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texFaceNeutral);
            originalScale = head.localScale;
        }

    }

    void Start () {
	
	}
	
	void Update () {
	
	}

    public virtual void SetAnimation(ANIMATION anim)
    {
        
    }

    protected IEnumerator StartShake()
    {
        transform.DOShakePosition(shakeDuration, new Vector3(0, shakeForce, 0), 5, 0).SetEase(Ease.Linear);
        yield return new WaitForSeconds(Random.Range(shakeDuration, shakeDuration * 2));
        StartCoroutine(StartShake());
    }

}
