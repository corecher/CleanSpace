using UnityEngine;
[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 0)]
public class Item : ScriptableObject
{
    public string id;
    public int count;
    public int maxWeight;
    public int minWeight;
}
