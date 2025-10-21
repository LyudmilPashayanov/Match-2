using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace MergeTwo
{
    public class FileManager
    {
        public static State GetState()
        {
            if (File.Exists(Constant.SavePath))
            {
                byte[] bytes = File.ReadAllBytes(Constant.SavePath);
                string stateString = Encoding.UTF8.GetString(bytes);
                var state = JsonConvert.DeserializeObject<State>(stateString);
                return state;
            }
            Config config = GameContext.GetInstance<Config>();
            config.InitialState.ListField = GetFieldFromSO(config);
            string serializeState = JsonConvert.SerializeObject(config.InitialState);
            return JsonConvert.DeserializeObject<State>(serializeState);
        }

        public static void SaveState()
        {
            var state = GameContext.GetInstance<State>();
            SaveState(state, Constant.SavePath);
        }

        public static void SaveState(State state, string path)
        {
            string stateString = JsonConvert.SerializeObject(state);
            byte[] stateBytes = Encoding.UTF8.GetBytes(stateString);
            File.WriteAllBytes(path, stateBytes);
        }

        public static List<IconList> GetFieldFromSO(Config config)
        {
            string intialFieldStr = config.InitialStateFile.text;
            return JsonConvert.DeserializeObject<List<IconList>>(intialFieldStr);
        }

        public static void SaveFieldToResources(State state)
        {
            string listField = JsonConvert.SerializeObject(state.ListField);
            string path = $"{Application.dataPath}/MergeTwo/SO/initial-field.txt";
            int count = 0;
            while (File.Exists(path) || count > 1000)
            {
                path = $"{Application.dataPath}/MergeTwo/SO/initial-field_{count}.txt";
                count++;
            }
            File.WriteAllText(path, listField);
        }

        public static void DeleteSave()
        {
            if (File.Exists(Constant.SavePath))
                File.Delete(Constant.SavePath);
        }
    }
}