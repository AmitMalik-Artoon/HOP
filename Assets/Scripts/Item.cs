using UnityEngine;

internal class Item : MonoBehaviour
{
    private int _score;
    public void SetItemProps(int score)
    {
        _score = score;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EventManager.Instance.Dispatch(GamePlayEvents.CollectItemPick, null, _score);
            Destroy(gameObject);
        }
    }
    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
