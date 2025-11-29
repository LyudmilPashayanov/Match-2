using System.Collections.Generic;
using MergeIt.Game;
using UnityEngine;

[CreateAssetMenu(fileName = "OrderDefinition", menuName = "Game/OrderDefinition")]
public class OrderDefinition : ScriptableObject
{
    public int OrderId;
    public List<Order> RequiredItems;
}