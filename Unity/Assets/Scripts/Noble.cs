using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Noble : Character {
    public Texture2D texAngry;
    public Texture2D texSurprise;

    protected override void Awake()
    {
        base.Awake();

        switch (character)
        {
            case Dialog.CHARACTER.NOBLE_1:
                texSurprise = Resources.Load("Img/Characters/catlady/face_surprise", typeof(Texture2D)) as Texture2D;
                texAngry = Resources.Load("Img/Characters/catlady/face_angry", typeof(Texture2D)) as Texture2D;
                break;
            case Dialog.CHARACTER.NOBLE_2:
                texSurprise = Resources.Load("Img/Characters/dogman/face_surprise", typeof(Texture2D)) as Texture2D;
                texAngry = Resources.Load("Img/Characters/dogman/face_angry", typeof(Texture2D)) as Texture2D;
                break;
            case Dialog.CHARACTER.NOBLE_3:
                texSurprise = Resources.Load("Img/Characters/bunnyman/face_surprise", typeof(Texture2D)) as Texture2D;
                texAngry = Resources.Load("Img/Characters/bunnyman/face_angry", typeof(Texture2D)) as Texture2D;
                break;
            case Dialog.CHARACTER.NOBLE_4:
                texSurprise = Resources.Load("Img/Characters/birdlady/face_surprise", typeof(Texture2D)) as Texture2D;
                texAngry = Resources.Load("Img/Characters/birdlady/face_angry", typeof(Texture2D)) as Texture2D;
                break;
        }
    }

	void Start () {
	
	}
	
	void Update () {
	
	}

    public override void SetAnimation(ANIMATION anim)
    {
        if (currentAnimation == anim) return;
        currentAnimation = anim;

        StopAllCoroutines();

        DOTween.Kill(this.GetInstanceID() + "s");
        DOTween.Kill(this.GetInstanceID() + "r");

        switch (anim)
        {
            case ANIMATION.NORMAL:
                head.DOScale(originalScale, 1.0f);
                head.transform.localRotation = Quaternion.identity;

                head.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texFaceNeutral);

                transform.position = originalPosition;
                break;
            case ANIMATION.TALKING:
                Quaternion q = head.localRotation;
                q.eulerAngles = new Vector3(0, 0, -punchRotationScale.z / 2);
                head.transform.localRotation = q;
                head.DOPunchScale(punchScaleSize, punchScaleDuration, 1, 1).SetLoops(-1).SetId(this.GetInstanceID() + "s").SetEase(Ease.Linear);
                head.DOPunchRotation(punchRotationScale, punchRotationDuration, 1, 1).SetLoops(-1).SetId(this.GetInstanceID() + "r").SetEase(Ease.Linear);

                head.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texFaceNeutral);

                StartCoroutine(StartShake());
                break;
            case ANIMATION.ANGRY:
                head.DOScale(originalScale, 1.0f);
                head.transform.localRotation = Quaternion.identity;

                transform.position = originalPosition;

                head.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texAngry);
                break;
            case ANIMATION.SURPRISED:
                head.DOScale(originalScale, 1.0f);
                head.transform.localRotation = Quaternion.identity;

                transform.position = originalPosition;

                head.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texSurprise);
                break;
        }
    }
}
