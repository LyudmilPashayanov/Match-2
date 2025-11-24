using System.Collections.Generic;
using MergeIt.Game;
using UnityEngine;

[CreateAssetMenu(fileName = "OrderList", menuName = "Game/Order List")]
public class OrderList : ScriptableObject
{
    public List<OrderDefinition> OrderDefinitions = new List<OrderDefinition>();
}