using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameResultView : MonoBehaviour
{
    private string _message;
    private bool _isEndScreen;
    private bool _isRestarting;

    public void ShowStatus(string message)
    {
        _message = message;
        _isEndScreen = false;
    }

    public void ShowEnd(string message)
    {
        _message = message;
        _isEndScreen = true;
    }

    private void OnGUI()
    {
        if (string.IsNullOrEmpty(_message))
            return;

        if (!_isEndScreen)
        {
            GUI.Label(new Rect(16, 16, 340, 36), _message);
            return;
        }

        float width = Mathf.Min(360f, Screen.width - 32f);
        Rect panel = new Rect((Screen.width - width) * 0.5f, Screen.height * 0.35f, width, 150f);
        GUI.Box(panel, string.Empty);
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 24,
            wordWrap = true
        };
        GUI.Label(new Rect(panel.x + 12, panel.y + 12, panel.width - 24, 70), _message, labelStyle);
        if (!_isRestarting && GUI.Button(new Rect(panel.x + 55, panel.y + 95, panel.width - 110, 38), "Грати ще раз"))
            StartCoroutine(RestartScene());
    }

    private IEnumerator RestartScene()
    {
        _isRestarting = true;
        _message = null;
        _isEndScreen = false;
        Time.timeScale = 1f;

        yield return null;
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex >= 0)
            SceneManager.LoadScene(sceneIndex);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
