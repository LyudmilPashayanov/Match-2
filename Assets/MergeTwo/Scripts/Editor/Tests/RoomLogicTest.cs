using NUnit.Framework;
using System.Collections.Generic;

namespace MergeTwo
{
    public class RoomLogicTest
    {
        [Test]
        public void Test()
        {
            TestIsPiecePutchsed();
            TestPurhcsePiece();
        }

        private static void TestPurhcsePiece()
        {
            State state = new State();
            state.Stars = 3;
            state.CurrentRoom = 0;
            List<ConfigRoom> rooms = new List<ConfigRoom>() {
                new ConfigRoom {
                    Pieces = new List<Piece> { new Piece { ID = 15, Price = 3 } },
                }
            };

            Assert.IsTrue(Logic.Room.IsCanPurchasePiece(15, state, rooms));
            Logic.Room.BuyPiece(15, state, rooms);

            Assert.AreEqual(1, state.PurchasedPieces.Count);
            Assert.AreEqual(15, state.PurchasedPieces[0]);
            Assert.AreEqual(0, state.Stars);

            Assert.IsFalse(Logic.Room.IsCanPurchasePiece(15, state, rooms));

            Logic.Room.CheckFotRoomComplition(state, rooms);
            Assert.AreEqual(1, state.CurrentRoom);
            Assert.IsTrue(state.IsRoomCompleted);
        }

        private static void TestIsPiecePutchsed()
        {
            State state = new State();
            Assert.IsFalse(Logic.Room.IsPiecePurchased(2, state));
            state.PurchasedPieces.Add(2);
            Assert.IsTrue(Logic.Room.IsPiecePurchased(2, state));
        }
    }
}
