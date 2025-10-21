using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace MergeTwo
{
    public class SpawnersTest
    {
        [Test]
        public void Test()
        {
            var state = new State();
            Logic.FillFieldIcons(state);

            Assert.IsTrue(Logic.IsCanAddIcon(state));
            FillState(state);

            Assert.IsFalse(Logic.IsCanAddIcon(state));

            state.ListField[2].Icons[2].IconType = IconType.None;
            Assert.IsTrue(Logic.IsCanAddIcon(state));
            Assert.AreEqual(state.ListField[2].Icons[2], Logic.GetFreeIcon(state));
        }

        private static void FillState(State state)
        {
            for (int i = 0; i < state.ListField.Count; i++)
            {
                IconList iconList = state.ListField[i];
                for (int j = 0; j < iconList.Icons.Count; j++)
                {
                    iconList.Icons[j] = new Icon { IconType = IconType.Glass };
                }
            }
        }

        [Test]
        public void Test_IsSpawner()
        {
            Assert.IsTrue(Logic.IsSpawner(IconType.Fridge));
            Assert.IsFalse(Logic.IsSpawner(IconType.Envelope));
        }

        [Test]
        public void Test_IsCanSpawn()
        {
            var state = new State();
            Logic.FillFieldIcons(state);
            FillState(state);
            state.Energy = 10;

            state.ListField[2].Icons[2] = new Icon { IconType = IconType.Fridge, Pos = new Pos(2, 2) };
            state.ListField[3].Icons[3] = new Icon { IconType = IconType.Bag, Capacity = 1, Pos = new Pos(3, 3) };

            Assert.IsFalse(Logic.IsCanSpawn(state.ListField[2].Icons[2], state));
            Assert.IsFalse(Logic.IsCanSpawn(state.ListField[1].Icons[1], state));
            Assert.IsTrue(Logic.IsCanSpawn(state.ListField[3].Icons[3], state));
            state.Energy = 0;
            Assert.IsFalse(Logic.IsCanSpawn(state.ListField[3].Icons[3], state));
        }

        [Test]
        public void Test_SpawnIcon()
        {
            var state = new State();
            Logic.FillFieldIcons(state);
            FillState(state);
            state.Energy = 10;

            //target icon
            state.ListField[1].Icons[1] = new Icon { IconType = IconType.None };
            //spawner
            state.ListField[2].Icons[2] = new Icon { IconType = IconType.Fridge, Pos = new Pos(2, 2), Capacity = 12 };

            List<IconTypeConfig> configs = new List<IconTypeConfig> {
                new IconTypeConfig {
                    IconType = IconType.Fridge,
                    IconsToSpawn = new List<SpawnableIcon>
                    {
                        new SpawnableIcon {
                            IconType = IconType.Cake, IconValue = 1, Weight = 1
                        }
                    },
                    EnergyDelay = 500
                }
            };

            Logic.SpawnIcon(state.ListField[2].Icons[2], state, configs);

            Assert.AreEqual(IconType.Cake, state.ListField[1].Icons[1].IconType);
            Assert.AreEqual(1, state.ListField[1].Icons[1].Value);

            Assert.IsNotNull(state.FlyIconAnimation);
            Assert.AreEqual(state.ListField[2].Icons[2].Pos, state.FlyIconAnimation.From);
            Assert.AreEqual(state.ListField[1].Icons[1].Pos, state.FlyIconAnimation.To);
            Assert.AreEqual(IconType.Cake, state.FlyIconAnimation.IconType);
            Assert.AreEqual(1, state.FlyIconAnimation.Value);
            Assert.AreEqual(state.ListField[2].Icons[2].Capacity, 11);
            Assert.AreEqual(9, state.Energy);

            state.ListField[3].Icons[1] = new Icon { IconType = IconType.None };
            state.ListField[2].Icons[2].Capacity = 1;
            Logic.SpawnIcon(state.ListField[2].Icons[2], state, configs);
            Assert.AreEqual(state.ListField[2].Icons[2].Capacity, 0);
           
            Assert.AreEqual(state.ListField[2].Icons[2].RechargeTime, Logic.GetTimestamp() + 500);
        }

        [Test]
        public void Test_Recharge()
        {
            var state = new State();
            Logic.FillFieldIcons(state);
            FillState(state);

            long unixTimestamp = Logic.GetTimestamp();

            //spawner
            state.ListField[2].Icons[2] = new Icon { IconType = IconType.Fridge, Pos = new Pos(2, 2), Capacity = 0, RechargeTime = unixTimestamp - 500 };

            List<IconTypeConfig> configs = new List<IconTypeConfig> {
                new IconTypeConfig {
                    IconType = IconType.Fridge,
                    IconsToSpawn = new List<SpawnableIcon>
                    {
                        new SpawnableIcon {
                            IconType = IconType.Cake, IconValue = 1, Weight = 1
                        }
                    },
                    EnergyDelay = 500,
                    SpawnerCapacities = new List<int> { 20 }
                }
            };

            List<Pos> recharges = new List<Pos>();
            Logic.TryRechargeIcons(state, configs, recharges);

            Assert.AreEqual(20, state.ListField[2].Icons[2].Capacity);
            Assert.AreEqual(0, state.ListField[2].Icons[2].RechargeTime);
        }
    }
}
