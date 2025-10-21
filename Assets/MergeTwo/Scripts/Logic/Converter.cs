using System.Collections.Generic;

namespace MergeTwo
{
    public class Converter 
    {
        public static List<IconList> ToListField(Icon[,] icons) 
        {
            var newField = new List<IconList>();
            for (int i = 0; i < 9; i++)
            {
                newField.Add(new IconList());
                for (int j = 0; j < 7; j++)
                {
                    newField[i].Icons.Add(icons[i, j]);
                }
            }

            return newField;
        }
    }
}