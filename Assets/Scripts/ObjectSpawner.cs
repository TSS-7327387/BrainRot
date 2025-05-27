using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject[] spawnableObjects;
    public Transform spawnPoint;
    public HorizontalDrag hDrag;
    public int rank;
    public bool gameOver;
    public Transform piecesParent;
    public GameObject[] piecesUI;
    void Start()
    {
        SpawnRandomObject();
        GameManager.OnGameOver += GameOver;
        GameManager.OnGameContinue += ContinueGame;
    }

    private List<int> shuffledIndices = new List<int>();
    private int currentIndex = 0;

    public void SpawnRandomObject()
    {
        if (gameOver) return;

        // Generate or refill the shuffle bag if needed
        if (shuffledIndices.Count == 0 || currentIndex >= shuffledIndices.Count)
        {
            GenerateShuffledIndices(rank);
            currentIndex = 0;
        }

        /*  int index = shuffledIndices[currentIndex];
          currentIndex++;*/
        GameObject piece = Instantiate(spawnableObjects[shuffledIndices[currentIndex++]], spawnPoint.position, Quaternion.identity, piecesParent);
        hDrag.StartDraging(piece);
        GameManager.Instance.powerManager.containerItems.Add(piece.transform);

        // Show the next object's index
        int nextIndex = GetNextObjectIndex();
        if (nextIndex != -1)
        {
            foreach(GameObject obj in piecesUI)
            {
                obj.SetActive(false);
            }
            piecesUI[nextIndex].SetActive(true);
        }
        else
        {
            Debug.Log("No next object index. Reshuffling.");
        }
    }

    private void GenerateShuffledIndices(int playerRank)
    {
        shuffledIndices.Clear();

        int maxExclusiveIndex = (playerRank > 3)
            ? Mathf.Clamp(playerRank - 1, 4, spawnableObjects.Length)
            : Mathf.Min(3, spawnableObjects.Length);

        for (int i = 0; i < maxExclusiveIndex; i++)
        {
            shuffledIndices.Add(i);
        }

        // Shuffle the list
        for (int i = 0; i < shuffledIndices.Count; i++)
        {
            int randIndex = Random.Range(i, shuffledIndices.Count);
            int temp = shuffledIndices[i];
            shuffledIndices[i] = shuffledIndices[randIndex];
            shuffledIndices[randIndex] = temp;
        }
    }
    public int GetNextObjectIndex()
    {
        if (gameOver) return -1;

        // Check if we've reached the end of the shuffled list
        if (shuffledIndices.Count == 0 || currentIndex >= shuffledIndices.Count)
        {
            GenerateShuffledIndices(rank);
            currentIndex = 0;
        }

        // Now we are guaranteed to have a next object index
        return shuffledIndices[currentIndex];
    }
    public void GameOver()
    {
        gameOver = true;
        DOVirtual.DelayedCall(1,()=>{ piecesParent.gameObject.SetActive(false); }) ;
    }
    public void ContinueGame()
    {
        RemoveHalfObjects();
        gameOver = false;
        piecesParent.gameObject.SetActive(true);

    }
    public void RemoveHalfObjects()
    {
        int removeCount = piecesParent.childCount / 2;
        Transform[] objectsToRemove = new Transform[removeCount];

        // Collect references to the objects to remove
        for (int i = 0; i < removeCount; i++)
        {
            objectsToRemove[i] = piecesParent.GetChild(i);
        }

        for (int i = 0; i < removeCount; i++)
        {
            if (objectsToRemove[i] != null)
            {
                Destroy(objectsToRemove[i].gameObject);
            }
        }
    }
    public void NewObject(GameObject nextTierPrefab,Vector3 mergePos)
    {

        GameManager.Instance.powerManager.containerItems.Add(Instantiate(nextTierPrefab, mergePos, Quaternion.identity, piecesParent).transform);
    }
}