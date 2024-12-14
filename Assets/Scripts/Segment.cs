using UnityEngine;

internal class Segment : MonoBehaviour
{
    public Segment NextSegment;
    public void SetSegmentData(Segment segment,bool readyToSpawn,float spawnProb)
    {
        NextSegment = segment;
        if (!readyToSpawn) return;
        if(Random.value<spawnProb)
        {
            EventManager.Instance.Dispatch<Transform>(GamePlayEvents.ItemSpawnAtPos,null,transform);
        }
    }
    private void OnDisable()
    {
        NextSegment = null;
    }
}
