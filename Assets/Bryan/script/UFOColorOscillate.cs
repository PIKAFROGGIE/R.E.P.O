using UnityEngine;

public class UFOColorOscillate : MonoBehaviour
{
    public MeshRenderer rend;
    public Color targetColor = Color.red;
    public float speed = 2f;

    Color originalColor1, originalColor2;

    void Start()
    {
        if (!rend) rend = GetComponent<MeshRenderer>();
        if (rend.materials.Length == 1)
        {
            originalColor1 = rend.materials[0].color;
        }
        else if (rend.materials.Length > 1)
        {
            originalColor1 = rend.materials[0].color;
            originalColor2 = rend.materials[4].color;
        }
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        if(originalColor1 != null)
            rend.materials[0].color = Color.Lerp(originalColor1, targetColor, t);
        if (originalColor2 != null)
            rend.materials[4].color = Color.Lerp(originalColor2, targetColor, t);
    }
}

