using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace MergeTwo
{
    public class LogicTest
    {
        [Test]
        public void Test()
        {
            EqualIconTest();
            IsCanMergeTest();

            var state = new State();
            Logic.FillFieldIcons(state);

            FillStateTest(state);
            MergeTest(state);
            CalculateIconsAmountTest();
            OrdersTest();
            GetNextOrdersTest();
            UseEnergyIcon(state);
        }

        void GetNextOrdersTest()
        {
            State state = new State();
            Logic.FillFieldIcons(state);
            List<Order> configOrders = new List<Order>
            {
                new Order { ID = 0 },
                new Order { ID = 1 },
                new Order { ID = 2 },
            };
            List<Order> result = Logic.GetNextOrders(configOrders, state);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(0, result[0].ID);
            Assert.AreEqual(1, result[1].ID);

            state.ClaimedOrders.Add(1);
            result = Logic.GetNextOrders(configOrders, state);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(0, result[0].ID);
            Assert.AreEqual(2, result[1].ID);

            state.ClaimedOrders.Add(0);
            result = Logic.GetNextOrders(configOrders, state);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(2, result[0].ID);

            state.ClaimedOrders = new List<int>();
            configOrders = new List<Order>
            {
                new Order { ID = 0 },
                new Order { ID = 1 },
                new Order { ID = 2 },
            };
            result = Logic.GetNextOrders(configOrders, state, 1, 0);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].ID);
            Assert.AreEqual(0, result[1].ID);
        }

        void OrdersTest()
        {
            State state = new State();
            Logic.FillFieldIcons(state);

            Order order = new Order
            {
                IconsToCollect = new List<IconOrder>
                {
                    new IconOrder { Icon = new Icon{ IconType = IconType.Glass, Value = 1 }, Amount = 3 },
                    new IconOrder { Icon = new Icon{ IconType = IconType.Cake, Value = 2 }, Amount = 2 },
                },
                ID = 5
            };

            state.ListField[1].Icons[1].IconType = IconType.Glass;
            state.ListField[1].Icons[1].Value = 1;
            Assert.IsFalse(Logic.IsOrderReady(order, state));

            state.ListField[1].Icons[2].IconType = IconType.Glass;
            state.ListField[1].Icons[2].Value = 1;

            state.ListField[2].Icons[2].IconType = IconType.Glass;
            state.ListField[2].Icons[2].Value = 1;

            state.ListField[0].Icons[2].IconType = IconType.Glass;
            state.ListField[0].Icons[2].Value = 0;

            state.ListField[0].Icons[1].IconType = IconType.Broom;
            state.ListField[0].Icons[1].Value = 1;

            state.ListField[3].Icons[2].IconType = IconType.Cake;
            state.ListField[3].Icons[2].Value = 2;

            state.ListField[4].Icons[1].IconType = IconType.Cake;
            state.ListField[4].Icons[1].Value = 2;
            Assert.IsTrue(Logic.IsOrderReady(order, state));

            var claimedOrders = Logic.ClaimOrder(order, state);
            Assert.AreEqual(5, claimedOrders.Count);
            Assert.AreEqual(new Pos { x = 1, y = 1 }, claimedOrders[0]);
            Assert.AreEqual(new Pos { x = 4, y = 1 }, claimedOrders[4]);

            Assert.AreEqual(state.ListField[1].Icons[1].IconType, IconType.None);
            Assert.AreEqual(state.ListField[1].Icons[1].Value, 0);

            Assert.AreEqual(state.ListField[1].Icons[2].IconType, IconType.None);
            Assert.AreEqual(state.ListField[1].Icons[2].Value, 0);

            Assert.AreEqual(state.ListField[3].Icons[2].IconType, IconType.None);
            Assert.AreEqual(state.ListField[3].Icons[2].Value, 0);

            Assert.AreEqual(state.ListField[0].Icons[1].IconType, IconType.Broom);
            Assert.AreEqual(state.ListField[0].Icons[1].Value, 1);

            Assert.AreEqual(5, state.ClaimedOrders[0]);
            Assert.AreEqual(1, state.Stars);
        }


        void CalculateIconsAmountTest()
        {
            State state = new State();
            Logic.FillFieldIcons(state);

            state.ListField[1].Icons[1].IconType = IconType.Glass;
            state.ListField[1].Icons[1].Value = 1;
            Assert.AreEqual(1, Logic.CalculateAmount(new Icon { IconType = IconType.Glass, Value = 1 }, state));

            state.ListField[1].Icons[2].IconType = IconType.Glass;
            state.ListField[1].Icons[2].Value = 1;

            state.ListField[2].Icons[2].IconType = IconType.Glass;
            state.ListField[2].Icons[2].Value = 1;

            state.ListField[0].Icons[2].IconType = IconType.Glass;
            state.ListField[0].Icons[2].Value = 0;

            state.ListField[0].Icons[1].IconType = IconType.Broom;
            state.ListField[0].Icons[1].Value = 1;
            Assert.AreEqual(3, Logic.CalculateAmount(new Icon { IconType = IconType.Glass, Value = 1 }, state));
        }

        void MergeTest(State state)
        {
            state.ListField[1].Icons[1].IconType = IconType.Glass;
            state.ListField[2].Icons[2].IconType = IconType.Glass;
            Logic.Merge(state.ListField[1].Icons[1], state.ListField[2].Icons[2], state);

            Assert.AreEqual(IconType.Glass, state.ListField[2].Icons[2].IconType);
            Assert.AreEqual(IconType.None, state.ListField[1].Icons[1].IconType);
            Assert.AreEqual(1, state.ListField[2].Icons[2].Value);
            Assert.AreEqual(0, state.ListField[1].Icons[1].Value);
        }

        void FillStateTest(State state)
        {
            Assert.AreEqual(9, state.ListField.Count);
            Assert.AreEqual(7, state.ListField[2].Icons.Count);
            Assert.AreEqual(new Pos(4, 4), state.ListField[4].Icons[4].Pos);
        }

        void IsCanMergeTest()
        {
            var a = new Icon { IconType = IconType.Cake, Value = 1, Pos = new Pos(1, 1) };
            var b = new Icon { IconType = IconType.Cake, Value = 1, Pos = new Pos(1, 2) };
            var iconConfig = new IconTypeConfig { Sprites = new List<Sprite> { null, null, null } };
            Assert.IsTrue(Logic.IsCanMerge(a, b, iconConfig));

            a = new Icon { IconType = IconType.Cake, Value = 2 };
            b = new Icon { IconType = IconType.Cake, Value = 2 };
            Assert.IsFalse(Logic.IsCanMerge(a, b, iconConfig));

            a = new Icon { IconType = IconType.Cake, Value = 1 };
            b = new Icon { IconType = IconType.Cake, Value = 2 };
            Assert.IsFalse(Logic.IsCanMerge(a, b, iconConfig));

            a = new Icon { IconType = IconType.Cauldron, Value = 2 };
            b = new Icon { IconType = IconType.Cake, Value = 2 };
            Assert.IsFalse(Logic.IsCanMerge(a, b, iconConfig));

            a = new Icon { IconType = IconType.Cake, Value = 1, Pos = new Pos(1, 1) };
            b = new Icon { IconType = IconType.Cake, Value = 1, Pos = new Pos(1, 1) };
            Assert.IsFalse(Logic.IsCanMerge(a, b, iconConfig));
        }

        void EqualIconTest()
        {
            Assert.IsTrue(Logic.IsEqual(new Icon { IconType = IconType.Broom, Value = 2 }, new Icon { IconType = IconType.Broom, Value = 2 }));
            Assert.IsFalse(Logic.IsEqual(new Icon { IconType = IconType.Cake, Value = 2 }, new Icon { IconType = IconType.Broom, Value = 2 }));
            Assert.IsFalse(Logic.IsEqual(new Icon { IconType = IconType.Broom, Value = 0 }, new Icon { IconType = IconType.Broom, Value = 2 }));
        }

        private void UseEnergyIcon(State state)
        {
            List<int> energyIconValues = new List<int>() { 3, 9, 27 };
            state.ListField[1].Icons[1].IconType = IconType.Energy;
            state.ListField[1].Icons[1].Value = 0;
            Logic.UseEnergyIcon(state.ListField[1].Icons[1], energyIconValues, state);
            Assert.AreEqual(3, state.Energy);
            Assert.AreEqual(IconType.None, state.ListField[1].Icons[1].IconType);
            Assert.AreEqual(0, state.ListField[1].Icons[1].Value);

            state.ListField[2].Icons[1].IconType = IconType.Energy;
            state.ListField[2].Icons[1].Value = 1;
            Logic.UseEnergyIcon(state.ListField[2].Icons[1], energyIconValues, state);
            Assert.AreEqual(12, state.Energy);
            Assert.AreEqual(IconType.None, state.ListField[2].Icons[1].IconType);
            Assert.AreEqual(0, state.ListField[2].Icons[1].Value);
        }
    }
}
