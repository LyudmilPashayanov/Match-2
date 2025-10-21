using UnityEngine;

namespace MergeTwo
{
    public class Constant 
    {
        public static string SavePath = $"{Application.persistentDataPath}/save.dat";  

        public const string Lobby = "Lobby";
        public const string GameScene = "GameScene";

        public const int MaxOrdersAmount = 2;

        public const string IsLobbyAnimation = "is_lobby_animation";
    }
}