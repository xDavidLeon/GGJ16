using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Player : Character {
    public int score = 0;
    public string cname;
    private Texture2D texGlow;

    protected override void Awake()
    {
        base.Awake();

        switch (character)
        {
            case Dialog.CHARACTER.P_1:
                texGlow = Resources.Load("Img/Characters/player/player1_glow", typeof(Texture2D)) as Texture2D;
                cname = "Señor Rojo";
                break;
            case Dialog.CHARACTER.P_2:
                texGlow = Resources.Load("Img/Characters/player/player2_glow", typeof(Texture2D)) as Texture2D;
                cname = "Señor Azul";
                break;
            case Dialog.CHARACTER.P_3:
                texGlow = Resources.Load("Img/Characters/player/player3_glow", typeof(Texture2D)) as Texture2D;
                cname = "Señor Verde";
                break;
            case Dialog.CHARACTER.P_4:
                texGlow = Resources.Load("Img/Characters/player/player4_glow", typeof(Texture2D)) as Texture2D;
                cname = "Señor Rosa";
                break;
        }

        body.GetComponent<MeshRenderer>().material.SetTexture("_EmissiveTex", texGlow);
    }

    void Start () {
	
	}

    void Update () {
	
	}

    public static int CompareId(Player c1, Player c2)
    {
        return c1.score.CompareTo(c2.score);
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

                body.GetComponent<MeshRenderer>().material.DOColor(new Color(0, 0, 0, 0), "_ColorEmissive", 1.0f);

                transform.position = originalPosition;
                break;
            case ANIMATION.TALKING:

                body.GetComponent<MeshRenderer>().material.DOColor(new Color(characterColor.r, characterColor.g, characterColor.b, 1.0f), "_ColorEmissive", 1.0f);

                StartCoroutine(StartShake());
                break;
        }
    }

}
