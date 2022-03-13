using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coroutineManager : MonoBehaviour
{
    public float startdelay = 0f;
    public float slowdown_duration = 5f;
    public GameObject pausetext;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(gameslowdown());
        StartCoroutine(gameresume());
    }

    IEnumerator gameslowdown()
    {
        yield return new WaitForSecondsRealtime(startdelay);
        Time.timeScale = 0.3f;
        pausetext.SetActive(true);

    }

    IEnumerator gameresume()
    {
        yield return new WaitForSecondsRealtime(slowdown_duration);
        Time.timeScale = 1;
        pausetext.SetActive(false);

    }
    // Update is called once per frame
    void Update()
    {

    }
}
