using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class ReadCSV : MonoBehaviour
{
    public static List<T> Read<T>(string path) where T : new()
    {
        List<T> result = new List<T>();

        // Load the CSV file as a TextAsset from the Resources folder
        TextAsset csvFile = Resources.Load<TextAsset>(path);
        if (csvFile == null)
        {
            Debug.LogError($"CSV file not found at path: {path}");
            return result;
        }

        // Read all lines from the CSV file
        string[] lines = csvFile.text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        // Split the header line to get the column names
        string[] headers = lines[0].Split(',');

        // Loop through the data lines and map them to objects of type T
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');

            // Create a new instance of T
            T obj = new T();

            // Use reflection to set the properties of the object based on the headers
            for (int j = 0; j < headers.Length; j++)
            {
                // Get the property matching the header name, ignoring case
                PropertyInfo property = typeof(T).GetProperty(headers[j].Trim(), BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (property != null && values[j] != null)
                {
                    try
                    {
                        // Check if the property type is an enum
                        if (property.PropertyType.IsEnum)
                        {
                            // Parse the string as it is, without changing case
                            object enumValue = Enum.Parse(property.PropertyType, values[j].Trim());
                            property.SetValue(obj, enumValue);
                        }
                        else
                        {
                            // Otherwise, convert the value to the correct type and set it on the object
                            object convertedValue = Convert.ChangeType(values[j].Trim(), property.PropertyType);
                            property.SetValue(obj, convertedValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to set property {headers[j]}: {ex.Message}");
                    }
                }
            }

            result.Add(obj);
        }

        return result;
    }

    void Start()
    {
        // Assuming the CSV file is located at Resources/biome_data.csv
        //List<BiomeData> biomeDataList = ReadCSV.Read<BiomeData>("WeatherSystem");

        //// Example: Print each biome's data
        //foreach (var data in biomeDataList)
        //{
        //    Debug.Log($"Biome: {data.Biomes}, Base Temp: {data.Base}, Min Temp: {data.Min}, Max Temp: {data.Max}, Sunny: {data.Sunny}, Rain: {data.Rain}, Wind: {data.Wind}");
        //}
    }
}