using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UFOController : MonoBehaviour
{
    bool collapseBase = true, collapseMiddle = true, startBaseCollapse = true, startMiddleCollapse = true;
    public GameObject UFOBase, UFOMiddle;
    public CanvasGroup infoCanvas;
    public GameObject infoImage; 
    public Text infoText;
    public Animator infoAnim;
    public PlayerUI timer;
    public UFOColorOscillate BaseColor, MiddleColor;


    float fallSpeedBase = 0f, fallSpeedMiddle = 0f;
    public float gravity = 30f;

    [SerializeField]
    bool startFallingBase = false, startFallingMiddle = false;
    // Start is called before the first frame update
    void Start()
    {
        BaseColor.enabled = false;
        MiddleColor.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.GetCurrentTime() < 80f && collapseBase)
        {
            collapseBase = false;
            startFallingBase = true;
        }
        if (timer.GetCurrentTime() < 40f && collapseMiddle)
        {
            collapseMiddle = false;
            startFallingMiddle = true;
        }

        if(timer.GetCurrentTime() < 90f && startBaseCollapse)
        {
            startBaseCollapse = false;
            BaseColor.enabled = true;
            infoText.text = "Move to higher platform before it collapse!";
            infoText.fontSize = 40;
            infoCanvas.alpha = 1;
            infoAnim.SetTrigger("Play");
            StartCoroutine(HideAllUI());
        }
        if (timer.GetCurrentTime() < 50f && startMiddleCollapse)
        {
            startMiddleCollapse = false;
            MiddleColor.enabled = true;
            infoText.text = "Move to higher platform before it collapse!";
            infoText.fontSize = 40;
            infoCanvas.alpha = 1;
            infoAnim.SetTrigger("Play");
            StartCoroutine(HideAllUI());
        }


        if (startFallingBase)
        {
            fallSpeedBase += gravity * Time.deltaTime;   // accelerate
            UFOBase.transform.position += Vector3.down * fallSpeedBase * Time.deltaTime;
        }

        if (startFallingMiddle)
        {
            fallSpeedMiddle += gravity * Time.deltaTime;   // accelerate
            UFOMiddle.transform.position += Vector3.down * fallSpeedMiddle * Time.deltaTime;
        }

        if(timer.GetCurrentTime() <= 0f)
        {
            LastManRanking.Instance.OnRaceTimeUp();
        }
    }

    IEnumerator HideAllUI()
    {
        yield return new WaitForSeconds(10f);

        float t = 0f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, t / 0.5f);

            if (infoCanvas != null)
                infoCanvas.alpha = alpha;

            yield return null;
        }

        if (infoCanvas != null)
            infoCanvas.alpha = 0;
    }
}
