using System;
using System.IO;
using System.Text.Json;

namespace ExplorerNav.Services
{
    internal class JsonUtil
    {
        public bool HasError { get; private set; }
        public string? ErrorMessage { get; private set; }

        public string? Serialize<ObjectType>(ObjectType obj, bool pretty = false)
        {
            ResetError();
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = pretty };
                var serialized = JsonSerializer.Serialize(obj, options);
                return serialized;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
            return null;
        }

        //Deserialize<ObjectType> --> 'The collection type 'MusicPlayerA4.Models.ReactiveCollection`1[MusicPlayerA4.Models.PlaylistItem]' is abstract, an interface, or is read only, and could not be instantiated and populated. 
        // Pass List<PlaylistItem> instead
        public ObjectType? Deserialize<ObjectType>(string serialized)
        {
            ResetError();
            try
            {
                var deserialized = JsonSerializer.Deserialize<ObjectType>(serialized);
                return deserialized;
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }
            return default;
        }

        public bool WriteToFile<ObjectType>(ObjectType obj, string filePath, bool pretty = false)
        {
            ResetError();
            try
            {
                string jsonString = Serialize(obj, pretty);

                if (jsonString != null)
                {
                    File.WriteAllText(filePath, jsonString);
                    return true;
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }

            return false;
        }

        public ObjectType? ReadFromFile<ObjectType>(string filePath)
        {
            ResetError();
            try
            {
                string file = File.ReadAllText(filePath);
                return Deserialize<ObjectType>(file);
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = ex.Message;
            }

            return default;
        }

        private void ResetError()
        {
            HasError = false;
            ErrorMessage = null;
        }

    }
}
