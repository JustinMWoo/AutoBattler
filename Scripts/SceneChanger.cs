using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{
    #region Singleton
    private static SceneChanger _instance;
    public static SceneChanger Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public Animator animator;

    private string sceneName;

    public void FadeToScene(string sceneName)
    {
        animator.SetTrigger("FadeOut");

        this.sceneName = sceneName;
    }

    private void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneName);
    }
}
