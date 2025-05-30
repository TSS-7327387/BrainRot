using UnityEngine;

public class MergableObject : MonoBehaviour
{
    public int objectType;
    public GameObject nextTierPrefab;
    private bool hasMerged = false;
    public bool released=false;
    public AudioClip sound;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!released && collision.gameObject.CompareTag("Ceiling"))
        {
            return;
        }
        //if(!released)GameManager.Instance.UpdateScore(5);
        //released = true;
        if (hasMerged || nextTierPrefab==null) return;

        MergableObject other = collision.gameObject.GetComponent<MergableObject>();
        if (other != null && !other.hasMerged && other.objectType == objectType)
        {
            hasMerged = true;
            other.hasMerged = true;

            Vector3 mergePos = (transform.position + other.transform.position) / 2f;

            GameManager.Instance.powerManager.containerItems.Remove(other.transform);
            GameManager.Instance.powerManager.containerItems.Remove(transform);
            Destroy(gameObject);
            Destroy(other.gameObject);
            if(GameManager.Instance.oSpawner.rank==objectType && objectType<GameManager.Instance.oSpawner.spawnableObjects.Length-1)
                GameManager.Instance.oSpawner.rank = objectType+1;
            AudioManager.instance.PlaySFX(sound);
            GameManager.Instance.ShowEffect(mergePos,objectType*5,nextTierPrefab);
            GameManager.Instance.oSpawner.CheckMergable(this);
        }
    }
}
