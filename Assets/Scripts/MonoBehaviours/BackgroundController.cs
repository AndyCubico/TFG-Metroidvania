using System.Collections;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bg;
    [SerializeField] private float transitionTime;

    [SerializeField] private Color[] cloud = new Color[2];

    [SerializeField] private Color[] rainLight = new Color[2];
    [SerializeField] private Color[] rainMedium = new Color[2];
    //[SerializeField] private Color[] rainHeavy = new Color[2];

    [SerializeField] private Color[] sunny = new Color[2];

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(BlendColor(rainLight, sunny, transitionTime));
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(BlendColor(rainLight, cloud, transitionTime));
        }
    }

    public IEnumerator BlendColor(Color[] source, Color[] target, float time)
    {
        float percentage = 0;

        Color topColor = source[0];
        Color bottomColor = source[1];

        while (percentage < 1f)
        {
            topColor = Color.Lerp(source[0], target[0], percentage);   // Top color
            bottomColor = Color.Lerp(source[1], target[1], percentage);   // Bottom color

            bg.material.SetColor("_TopColor", topColor);
            bg.material.SetColor("_BottomColor", bottomColor);
            percentage += Time.deltaTime / time;

            yield return null;
        }
    }
}
