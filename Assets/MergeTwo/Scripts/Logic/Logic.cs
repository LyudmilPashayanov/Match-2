using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MergeTwo
{
    public class Logic
    {
        public static bool IsEqual(Icon a, Icon b) => a.IconType == b.IconType && a.Value == b.Value;

        public static bool IsCanMerge(Icon a, Icon b, IconTypeConfig iconConfig)
        {
            if (a == null || b == null)
                Debug.Log("NULL");

            if (a.IconType == IconType.None || b.IconType == IconType.None)
                return false;

            if (a.Pos.x == b.Pos.x && a.Pos.y == b.Pos.y)
                return false;

            if (IsEqual(a, b))
            {
                if (a.Value < iconConfig.Sprites.Count - 1 || b.Value < iconConfig.Sprites.Count - 1)
                {
                    return true;
                }
            }
            return false;
        }

        public static void FillFieldIcons(State state)
        {
            state.ListField = new List<IconList>();
            for (int i = 0; i < 9; i++)
            {
                state.ListField.Add(new IconList());
                for (int j = 0; j < 7; j++)
                {
                    state.ListField[i].Icons.Add(new Icon
                    {
                        IconType = IconType.None,
                        Pos = new Pos(i, j)
                    });
                }
            }
        }

        public static Icon Merge(Icon a, Icon b, State state)
        {
            state.ListField[b.Pos.x].Icons[b.Pos.y].Value += 1;
            state.ListField[a.Pos.x].Icons[a.Pos.y].IconType = IconType.None;
            state.ListField[a.Pos.x].Icons[a.Pos.y].Value = 0;

            return state.ListField[b.Pos.x].Icons[b.Pos.y];
        }

        public static int CalculateAmount(Icon icon, State state)
        {
            int amount = 0;
            foreach (var column in state.ListField)
            {
                foreach (var filedIcon in column.Icons)
                {
                    if (IsEqual(filedIcon, icon))
                        amount += 1;
                }
            }
            return amount;
        }

        public static bool IsOrderReady(Order order, State state)
        {
            foreach (var iconOrder in order.IconsToCollect)
            {
                int amount = CalculateAmount(iconOrder.Icon, state);
                if (amount < iconOrder.Amount)
                    return false;
            }

            return true;
        }

        public static List<Pos> ClaimOrder(Order order, State state)
        {
            List<Pos> claimedIcons = new();
            foreach (var iconToCollect in order.IconsToCollect)
            {
                for (int i = 0; i < iconToCollect.Amount; i++)
                {
                    Icon icon = GetFirstIcon(iconToCollect.Icon, state);
                    Pos pos = icon.Pos;
                    claimedIcons.Add(pos);
                    state.ListField[pos.x].Icons[pos.y].IconType = IconType.None;
                    state.ListField[pos.x].Icons[pos.y].Value = 0;
                }
            }

            state.Stars += order.OrderReward;
            state.ClaimedOrders.Add(order.ID);
            return claimedIcons;
        }

        static Icon GetFirstIcon(Icon icon, State state)
        {
            foreach (var column in state.ListField)
            {
                foreach (var fieldIcon in column.Icons)
                {
                    if (IsEqual(fieldIcon, icon))
                        return fieldIcon;
                }
            }
            return null;
        }



        public static List<Pos> GetOrderIconPositions(Order order, State state)
        {
            List<Pos> icons = new();
            foreach (var iconToCollect in order.IconsToCollect)
            {
                var similarIcons = new Queue<Icon>(GetAllSimilarIcons(iconToCollect.Icon, state));
                for (int i = 0; i < iconToCollect.Amount; i++)
                {
                    if (similarIcons.Count > 0)
                    {
                        Icon icon = similarIcons.Dequeue();
                        icons.Add(icon.Pos);
                    }
                }
            }
            return icons;
        }

        static List<Icon> GetAllSimilarIcons(Icon icon, State state)
        {
            List<Icon> icons = new();
            foreach (var column in state.ListField)
            {
                foreach (var fieldIcon in column.Icons)
                {
                    if (IsEqual(fieldIcon, icon))
                        icons.Add(fieldIcon);
                }
            }
            return icons;
        }

        public static List<Order> GetNextOrders(List<Order> configOrders, State state, int displayedId = -1, int displayedIndex = -1)
        {
            List<Order> orders = new();
            bool isSwap = false;
            for (int j = 0; j < configOrders.Count; j++)
            {
                Order order = configOrders[j];
                if (!state.ClaimedOrders.Contains(order.ID))
                {
                    orders.Add(order.Clone());

                    if (displayedId != -1 && order.ID == displayedId && orders.Count - 1 != displayedIndex)
                    {
                        isSwap = true;
                    }

                    if (orders.Count == Constant.MaxOrdersAmount)
                        break;
                }

            }

            if (isSwap)
                orders.Reverse();

            return orders;
        }

        public static void RandomizeState(State state, List<IconTypeConfig> iconTypeConfigs, List<Icon> icons)
        {
            foreach (var listIcon in state.ListField)
            {
                foreach (var icon in listIcon.Icons)
                {
                    IconTypeConfig config = iconTypeConfigs.First(i => i.IconType == icon.IconType);
                    int index = Random.Range(0, icons.Count);
                    icon.IconType = icons[index].IconType;
                    icon.Value = icons[index].Value;

                    if (IsSpawner(icon.IconType))
                    {
                        icon.Capacity = config.SpawnerCapacities[icon.Value];
                    }
                }
            }
        }

        public static bool IsCanAddIcon(State state)
        {
            for (int i = 0; i < state.ListField.Count; i++)
            {
                IconList iconList = state.ListField[i];
                for (int j = 0; j < iconList.Icons.Count; j++)
                {
                    if (iconList.Icons[j].IconType == IconType.None)
                        return true;
                }
            }
            return false;
        }

        public static Icon GetFreeIcon(State state)
        {
            for (int i = 0; i < state.ListField.Count; i++)
            {
                IconList iconList = state.ListField[i];
                for (int j = 0; j < iconList.Icons.Count; j++)
                {
                    if (iconList.Icons[j].IconType == IconType.None)
                        return iconList.Icons[j];
                }
            }
            return null;
        }

        public static bool IsSpawner(IconType iconType) => (int)iconType >= 1000;

        public static bool IsCanSpawn(Icon icon, State state)
        {
            if (!IsSpawner(icon.IconType))
                return false;

            if (state.Energy <= 0)
                return false;

            if (state.ListField[icon.Pos.x].Icons[icon.Pos.y].Capacity > 0)
                return true;           

            return false;
        }

        public static void SpawnIcon(Icon spawner, State state, List<IconTypeConfig> iconTypeConfigs)
        {
            Icon freeIcon = GetFreeIcon(state);
            List<SpawnableIcon> weightedIcons = new();
            var iconConfig = iconTypeConfigs.First(i => i.IconType == spawner.IconType);
            var iconsToSpawnFromConfig = iconConfig.IconsToSpawn;
            foreach (var icon in iconsToSpawnFromConfig)
            {
                for (int i = 0; i < icon.Weight; i++)
                {
                    weightedIcons.Add(icon);
                }
            }
            SpawnableIcon target = weightedIcons[Random.Range(0, weightedIcons.Count)];

            freeIcon.IconType = target.IconType;
            freeIcon.Value = target.IconValue;

            state.FlyIconAnimation = new AnimationItem
            {
                IconType = freeIcon.IconType,
                Value = freeIcon.Value,
                From = spawner.Pos,
                To = freeIcon.Pos,
            };

            state.ListField[spawner.Pos.x].Icons[spawner.Pos.y].Capacity -= 1;

            if (state.ListField[spawner.Pos.x].Icons[spawner.Pos.y].Capacity == 0)
            {
                state.ListField[spawner.Pos.x].Icons[spawner.Pos.y].RechargeTime = GetTimestamp() + iconConfig.EnergyDelay;
            }

            state.Energy -= 1;
        }

        public static long GetTimestamp()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            return now.ToUnixTimeSeconds();
        }

        public static bool IsEnergyIcon(Icon icon)
        {
            return icon.IconType == IconType.Energy;
        }

        public static void UseEnergyIcon(Icon icon, List<int> energyIconsValues, State state)
        {
            if (!IsEnergyIcon(icon))
                return;

            state.Energy += energyIconsValues[icon.Value];
            state.ListField[icon.Pos.x].Icons[icon.Pos.y].Value = 0;
            state.ListField[icon.Pos.x].Icons[icon.Pos.y].IconType = IconType.None;
        }

        public static void GiveReward(State state, Config config, int room)
        {
            var rewardIcons = config.Rooms[room].RewardIcons;
            foreach (var item in rewardIcons)
            {
                state.IconToTake.Add(item.Clone());
            }
        }

        public static Icon CollectIcon(State state)
        {
            Icon freeIcon = GetFreeIcon(state);
            freeIcon.IconType = state.IconToTake[0].IconType;
            freeIcon.Value = state.IconToTake[0].Value;

            state.IconToTake.RemoveAt(0);
            return freeIcon;
        }

        public static void TryRechargeIcons(State state, List<IconTypeConfig> iconTypeConfigs, List<Pos> recharges)
        {
            long now = GetTimestamp();
            for (int i = 0; i < state.ListField.Count; i++)
            {
                for (int j = 0; j < state.ListField[i].Icons.Count; j++)
                {
                    Icon icon = state.ListField[i].Icons[j];
                    if (IsSpawner(icon.IconType) && icon.Capacity == 0 && icon.RechargeTime < now)
                    {
                        var iconConfig = iconTypeConfigs.First(i => i.IconType == icon.IconType);
                        icon.Capacity = iconConfig.SpawnerCapacities[icon.Value];
                        icon.RechargeTime = 0;
                        recharges.Add(icon.Pos);
                    }
                }
            }
        }

        public static void RemoveIcon(State state, Pos pos)
        {
            state.ListField[pos.x].Icons[pos.y].IconType = IconType.None;
            state.ListField[pos.x].Icons[pos.y].Value = 0;
        }

        public static class Room
        {
            public static bool IsPiecePurchased(int id, State state)
            {
                if (state.PurchasedPieces.Contains(id))
                    return true;
                return false;
            }

            public static void BuyPiece(int id, State state, List<ConfigRoom> rooms)
            {
                int price = GetPiecePrice(id, rooms);
                if (state.Stars >= price)
                {
                    state.PurchasedPieces.Add(id);
                    state.Stars -= price;
                }
                else
                {
                    Debug.LogError("There is not enough stars");
                }
            }

            public static void CheckFotRoomComplition(State state, List<ConfigRoom> rooms)
            {
                List<int> piecesIds = rooms[state.CurrentRoom].Pieces.Select(p => p.ID).ToList();
                bool isAllCollected = piecesIds.All(id => state.PurchasedPieces.Contains(id));
                if (isAllCollected)
                {
                    state.CurrentRoom += 1;
                    state.IsRoomCompleted = true;
                }
            }

            private static int GetPiecePrice(int id, List<ConfigRoom> rooms)
            {
                foreach (var room in rooms)
                {
                    foreach (var piece in room.Pieces)
                    {
                        if (piece.ID == id)
                        {
                            return piece.Price;
                        }
                    }
                }
                Debug.LogError($"There is no price for piece ID=>{id}");
                return 0;
            }

            public static bool IsCanPurchasePiece(int id, State state, List<ConfigRoom> rooms)
            {
                if (IsPiecePurchased(id, state))
                    return false;
                int price = GetPiecePrice(id, rooms);
                return state.Stars >= price;
            }

            public static (int current, int max, string name) GetRoomProgress(Config config, State state)
            {
                ConfigRoom room = config.Rooms[state.CurrentRoom];

                int purchsedPieces = state.PurchasedPieces.Count;
                for (int i = 0; i < state.CurrentRoom; i++)
                {
                    purchsedPieces -= config.Rooms[i].Pieces.Count;
                }

                return (purchsedPieces, room.Pieces.Count, room.Name);
            }

            public static List<Piece> GetPiecesToPurchase(Config config, State state)
            {
                var pieces = new List<Piece>();
                ConfigRoom room = config.Rooms[state.CurrentRoom];

                foreach (var piece in room.Pieces)
                {
                    if (IsCadAddPiece(piece, state.PurchasedPieces))
                    {
                        pieces.Add(piece);
                    }
                }

                return pieces;
            }

            private static bool IsCadAddPiece(Piece piece, List<int> purchasedPieces)
            {
                if (purchasedPieces.Contains(piece.ID))
                {
                    return false;
                }

                foreach (var item in piece.Conditions)
                {
                    if (!purchasedPieces.Contains(item))
                    {
                        return false;
                    }
                }
                return true;
            }

            public static bool IsShouldShowAnimation(int id, State state)
            {
                return !state.AnimatedPieces.Contains(id);
            }

            public static void AnimatePiece(int id, State state)
            {
                state.AnimatedPieces.Add(id);
            }

            public static Sprite GetLocationImage(int roomIndex, State state, Config config)
            {
                ConfigRoom configRoom = config.Rooms[roomIndex];
                if (roomIndex < state.CurrentRoom)
                    return configRoom.CollectedSprite;

                if (roomIndex == state.CurrentRoom)
                    return configRoom.WorkInProgressSprite;

                return configRoom.LockedSprite;
            }

            public static void RoomAnimationPlayed(State state)
            {
                state.IsRoomCompleted = false;
            }
        }
    }
}
