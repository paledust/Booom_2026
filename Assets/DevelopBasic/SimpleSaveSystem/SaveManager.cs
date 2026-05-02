using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;

namespace SimpleSaveSystem{
    public enum Serializer{Json, Binary}
    public static class SaveManager{
        public const string SAVEFILE_DIRECTOR = "/saves/";
        public const string GLOBALFILE_NAME = "Global.sav";
        public const string SAVEFILE_NAME = "Data.sav";
        public static GlobalSaveData globalSaveData;
        public static void CleanGameState(int slotIndex){
            string filePath = Application.persistentDataPath + SAVEFILE_DIRECTOR + $"/{slotIndex}/" + SAVEFILE_NAME;
            if(File.Exists(filePath)) File.Delete(filePath);
        }
        public static async void SaveGameState(int slotIndex){
            string globalFolderPath = Application.persistentDataPath + SAVEFILE_DIRECTOR;
            string folderPath = Application.persistentDataPath + SAVEFILE_DIRECTOR + $"/{slotIndex}/";

        //To save, we first Load
            var saveData = Load<SaveData>(folderPath, SAVEFILE_NAME, Serializer.Json);
            if(saveData == null) saveData = new SaveData();

            EventHandler.Call_OnBeginSave();
        //Capture State
            ISaveable[] saveables = Service.FindComponentsOfTypeIncludingDisable<ISaveable>();
            await Task.Run(()=>{
                foreach(var saveable in saveables){
                    saveable.CaptureState(ref saveData);
                }
            //Save to file
                Save(folderPath, SAVEFILE_NAME, saveData, Serializer.Json);

            //Save Global
                Save(globalFolderPath, GLOBALFILE_NAME, globalSaveData, Serializer.Json);
            });
            Debug.Log($"{saveables.Length} saveables in scene are saved into Save Slot {slotIndex}.");
            await Task.Delay(500);
            
            EventHandler.Call_OnCompleteSave();
        }

        public static void LoadGameState(int slotIndex){
            string path = Application.persistentDataPath + SAVEFILE_DIRECTOR + $"/{slotIndex}/";
            var saveData = Load<SaveData>(path, SAVEFILE_NAME, Serializer.Json);
            if(saveData == null){
                Debug.LogWarning("No Valid Save Data");
                return;
            }

            ISaveable[] saveables = Service.FindComponentsOfTypeIncludingDisable<ISaveable>();
            foreach(ISaveable saveable in saveables){
                saveable.RestoreState(saveData);
            }
            Debug.Log($"{saveables.Length} saveables in scene are loaded from Save Slot {slotIndex}.");
        }
        
        public static void Initialize(){
            string folderPath = Application.persistentDataPath + SAVEFILE_DIRECTOR;
            string globalFilePath = Application.persistentDataPath + SAVEFILE_DIRECTOR + GLOBALFILE_NAME;

        // Create save folder if not found
            if(!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        // Create global save file if not found
            if(!File.Exists(globalFilePath)){
                globalSaveData = new GlobalSaveData();
                Save(folderPath, GLOBALFILE_NAME, globalSaveData, Serializer.Json);
            }
            else{
                globalSaveData = Load<GlobalSaveData>(folderPath, GLOBALFILE_NAME, Serializer.Json);
                if(globalSaveData==null) globalSaveData = new GlobalSaveData();
            }
        }
    #region Save and Load File
        static void Save<T>(string folderPath, string fileName, T saveData, Serializer serializer){
            if(!Directory.Exists(folderPath)){
                Directory.CreateDirectory(folderPath);
            }
            SerializeData<T>(folderPath+fileName, saveData, serializer);
        }
        static T Load<T>(string folderPath, string fileName, Serializer serializer){
            if(!File.Exists(folderPath+fileName)){
                return default(T);
            }

            var data = DeserializeData<T>(folderPath+fileName, serializer);
            return data;
        }
    #endregion

    #region Serialization
        static void SerializeData<T>(string filePath, T saveData, Serializer serializer){
            switch(serializer){
                case Serializer.Json:
                    string data = JsonConvert.SerializeObject(saveData);
                    File.WriteAllText(filePath, data);
                    break;
                default:
                    FileStream file = File.Open(filePath, FileMode.Create);
                    BinaryFormatter formatter = GetBinaryFormatter();
                    formatter.Serialize(file, saveData);
                    file.Close();
                    break;
            }
        }
        static T DeserializeData<T>(string filePath, Serializer serializer){
            switch(serializer){
                case Serializer.Json:
                    string saveData = File.ReadAllText(filePath);

                    try{
                        return JsonConvert.DeserializeObject<T>(saveData);
                    }
                    catch{
                        Debug.LogError("Save File Corrupted");
                        return default(T);
                    }
                default:
                    FileStream file = File.Open(filePath, FileMode.Open);
                    BinaryFormatter formatter = GetBinaryFormatter();

                    T data;
                    try{
                        data = (T)formatter.Deserialize(file);
                    }
                    catch{
                        Debug.LogError("Save File Corrupted");
                        data = default(T);
                    }
                    file.Close();

                    return data;
            }
        }
    #endregion
        // Formerly in SerializationManager - moved here to be shared by all Platform objects
        static BinaryFormatter GetBinaryFormatter()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            SurrogateSelector selector = new SurrogateSelector();
            Vector3SerializationSurrogate vector3Surrogate = new Vector3SerializationSurrogate();
            QuaternionSerializationSurrogate quaternionSurrogate = new QuaternionSerializationSurrogate();

            selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
            selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternionSurrogate);

            formatter.SurrogateSelector = selector;

            return formatter;
        }
    }
}