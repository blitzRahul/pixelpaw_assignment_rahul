using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using Windows.Storage;

namespace pixelpaw
{
    public class SessionData
    {
        public SessionData() { }
        public string LoginTime { get; set; }
        public string SessionNumber { get; set; }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        // Collection to hold login times
        public ObservableCollection<SessionData> LoginTimes { get; private set; }

        //to generate session ID
        private int _sessionCounter;

        public MainViewModel()
        {
            LoginTimes = new ObservableCollection<SessionData>();

            //loading data
            LoadLoginTimes();
        }

        // Adds a new login time
        public void AddLoginTime(DateTime loginTime)
        {
            var sessionData = new SessionData
            {
                LoginTime = loginTime.ToString("yyyy-MM-dd | hh:mm tt"), 
                SessionNumber = $"Session {_sessionCounter + 1}"
            };

            //add it to login times
            LoginTimes.Add(sessionData);

            //increase session counter
            _sessionCounter++;

            //now save it
            SaveLoginTimes();
        }

        // load from file
        private void LoadLoginTimes()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                var jsonFileTask = localFolder.TryGetItemAsync("loginTimes.json").AsTask();

                //wait for the file to finish loading
                jsonFileTask.Wait();
                var jsonFile = jsonFileTask.Result as StorageFile;

                if (jsonFile == null)
                {
                    return; 
                }

                //now read the file data
                var jsonContentTask = FileIO.ReadTextAsync(jsonFile).AsTask();
                jsonContentTask.Wait();
                string jsonContent = jsonContentTask.Result;

                //convert the JSON data to an ObservableCollection<SessionData>
                var sessions = JsonSerializer.Deserialize<ObservableCollection<SessionData>>(jsonContent);

                //if data is there add the data to LoginTimes
                if (sessions != null)
                {
                    foreach (var session in sessions)
                    {
                        LoginTimes.Add(session);
                    }
                    _sessionCounter = LoginTimes.Count;
                }
            }
            catch (Exception ex)
            {
            }
        }

        // Saves login times to local storage synchronously
        private void SaveLoginTimes()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                //we can replace existing since we are going to get everything while loading anyways
                var createFileTask = localFolder.CreateFileAsync("loginTimes.json", CreationCollisionOption.ReplaceExisting).AsTask();
                createFileTask.Wait(); 
                StorageFile jsonFile = createFileTask.Result;

                //serialize the json data and store it
                string jsonContent = JsonSerializer.Serialize(LoginTimes);
                var writeTextTask = FileIO.WriteTextAsync(jsonFile, jsonContent).AsTask();
                writeTextTask.Wait(); 
            }
            catch (Exception ex)
            {
            }
        }

        //this function is for debugging (just removes everything)
        public void RemoveAllLoginTimes()
        {
            try
            {
                
                LoginTimes.Clear();
                _sessionCounter = 0;

                // remove json file
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var getFileTask = localFolder.TryGetItemAsync("loginTimes.json").AsTask();
                getFileTask.Wait();
                var jsonFile = getFileTask.Result as StorageFile;

                if (jsonFile != null)
                {
                    var deleteTask = jsonFile.DeleteAsync().AsTask();
                    deleteTask.Wait(); 
                }

            }
            catch (Exception ex)
            {
            }
        }

        // notify view of change
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
