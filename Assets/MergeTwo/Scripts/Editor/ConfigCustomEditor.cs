using System.Text;
using UnityEditor;

namespace MergeTwo
{
    [CustomEditor(typeof(Config))]
    public class ConfigCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Config config = target as Config;
            foreach (var item in config.IconTypeConfigs)
            {
                item.Name = item.IconType.ToString();
            }

            foreach (var item in config.InitialState.ListField)
            {
                item.Name = item.ToString();
            }

            if (config.InitialState == null || config.InitialState.ListField.Count == 0) 
            {
                config.InitialState.ListField = FileManager.GetFieldFromSO(config);
            }

            int id = 1;
            if (config.Orders != null && config.Orders.Count > 0) 
            {
                foreach (var order in config.Orders)
                {                    
                    var name = new StringBuilder();
                    foreach (var item in order.IconsToCollect)
                    {
                       name.Append($"{item.Amount} {item.Icon.IconType} {item.Icon.Value} |");
                    }
                    order.Name = name.ToString();

                    order.ID = id++;
                }
            }
        }
    } 
}
