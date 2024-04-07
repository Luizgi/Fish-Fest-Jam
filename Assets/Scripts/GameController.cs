using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    IEnumerator CReturn()
    {
        yield return new WaitForSeconds(2f * Time.deltaTime);
        SceneManager.LoadScene(0);
    }

    public void Return()
    {
        StartCoroutine(CReturn());
    }
}
