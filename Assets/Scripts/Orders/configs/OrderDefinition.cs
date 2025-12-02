using System.Collections.Generic;
using MergeIt.Core.Configs.Elements;
using MergeIt.Game;
using UnityEngine;

[CreateAssetMenu(fileName = "OrderDefinition", menuName = "Game/OrderDefinition")]
public class OrderDefinition : ScriptableObject
{
    public int OrderId;
    public List<Order> RequiredItems;
    public ElementConfig reward;
}