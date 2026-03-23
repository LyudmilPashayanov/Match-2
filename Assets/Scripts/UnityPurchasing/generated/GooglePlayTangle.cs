// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("PEum/EHf+5n+frqdBBjlZ/3SMOKoGpm6qJWekbIe0B5vlZmZmZ2Ym5GTIJZ0bSlX1DNqyjrCLjjhLG10eT6JEKZwEN6cOiF7IgSSRHzVbYFNn8VnXM4ItYD5ofB2wkB9EJ/ea7t22we5m8WTgh0D08oSV3OVTN6MyRrScjVWKKqjkVtVgqiycQzCLwAamZeYqBqZkpoamZmYMB/xSSsUILU1h4gP+fKV/byPp5nwnVambjwlm1LV3VuPIvO7Zc8MsUedPwXp5pIMnKVvXHaw2JvLWiBKuszMY7rBqAODcfYtfbfPtJDcK0CZcSi2v3Q++SuE4f3fRjw9xgRinewjDm0YvwhFhHSCpFC7RxSkn6pUXs3HivPvBxgUQcAK6zOWVZqbmZiZ");
        private static int[] order = new int[] { 12,12,10,7,4,6,11,12,12,13,10,13,12,13,14 };
        private static int key = 152;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
