
using DG.Tweening;
using UnityEngine;

public class Ceiling : MonoBehaviour
{
    public bool gameOver;
    private void Start()
    {

        GameManager.OnGameOver += GameOver;
        GameManager.OnGameContinue += ContinueGame;
    }
    void GameOver()
    {
        gameOver = true;
    }
    void ContinueGame()
    {
        DOVirtual.DelayedCall(1.5f, () => { gameOver = false; });
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameOver) return;
        if (collision.gameObject.TryGetComponent(out MergableObject mg))
        {
            if (mg.released)
            {
                GameManager.Instance.GameOver();
            }
        }
    }
}
