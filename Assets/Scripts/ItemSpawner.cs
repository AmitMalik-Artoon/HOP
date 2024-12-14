using UnityEngine;

/// <summary>
/// Responsible for spawning items at a specified position based on rarity probabilities.
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    [Header("Item Containers")]
    [SerializeField] private ItemsContainer _itemsContainer; // Container for different item lists

    private void Awake()
    {
        // Subscribe to item spawn event
        EventManager.Instance.AddEventListener<Transform>(GamePlayEvents.ItemSpawnAtPos, OnItemSpawnRequest);
    }

    /// <summary>
    /// Event handler for item spawn requests.
    /// </summary>
    /// <param name="sender">The sender of the event (unused).</param>
    /// <param name="eventArgs">The transform where the item should be spawned.</param>
    private void OnItemSpawnRequest(object sender, Transform eventArgs)
    {
        SpawnItemAtPos(eventArgs);
    }

    /// <summary>
    /// Spawns an item at the specified position based on rarity probabilities.
    /// </summary>
    /// <param name="pos">The transform indicating the spawn position.</param>
    private void SpawnItemAtPos(Transform pos)
    {
        float randomValue = Random.Range(0f, 100f); // Random value between 0 and 100

        ItemInfo itemInfo;
        if (randomValue < 50)
        {
            // Common item probability
            itemInfo = GetRandomItem(_itemsContainer.commonItemsList);
        }
        else if (randomValue < 80)
        {
            // Rare item probability
            itemInfo = GetRandomItem(_itemsContainer.rareItemsList);
        }
        else
        {
            // Epic item probability
            itemInfo = GetRandomItem(_itemsContainer.epicItemsList);
        }

        SpawnItem(itemInfo, pos);
    }

    /// <summary>
    /// Spawns the specified item at the given position.
    /// </summary>
    /// <param name="itemInfo">Information about the item to spawn.</param>
    /// <param name="pos">The transform indicating the spawn position.</param>
    private void SpawnItem(ItemInfo itemInfo, Transform pos)
    {
        // Instantiate the item prefab and set its position and parent
        Item item = Instantiate(itemInfo.prefab, pos.position, Quaternion.identity);
        item.transform.SetParent(pos);

        // Initialize item properties
        item.SetItemProps(itemInfo.score);
    }

    /// <summary>
    /// Selects a random item from the given list.
    /// </summary>
    /// <param name="itemList">The list of items to select from.</param>
    /// <returns>A randomly selected item.</returns>
    private ItemInfo GetRandomItem(System.Collections.Generic.List<ItemInfo> itemList)
    {
        return itemList[Random.Range(0, itemList.Count)];
    }
}
