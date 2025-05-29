
using DG.Tweening;
using UnityEngine;

public class Ceiling : MonoBehaviour
{
    public bool gameOver = false;
    public GameObject vfx;
    /*private void OnEnable()
    {
        gameOver = false;
    }*/
    private void Start()
    {
        GameManager.OnGameOver += GameOver;
        GameManager.OnGameContinue += ContinueGame;
    }
    void GameOver()
    {
        gameOver = true;
        vfx.SetActive(true);
    }
    void ContinueGame()
    {
        vfx.SetActive(false);
        DOVirtual.DelayedCall(2, () => { gameOver = false; });
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (gameOver) return;
        print("Gameobject is"+ collision.gameObject.name);
        if (collision.gameObject.TryGetComponent(out MergableObject mg))
        {
            if (mg.released)
            {
                gameOver = true;
                vfx.SetActive(true);
                GameManager.Instance.GameOver();
            }
        }
    }

}
