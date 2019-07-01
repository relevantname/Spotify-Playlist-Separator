using SpotifyExtension.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SpotifyExtension.Model;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using SpotifyExtension.Tools;
using System.Windows;

namespace SpotifyExtension
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Private Properties
        private string userID;  // UserID that binded to UID Textbox on MainWindow.xaml
        private User currentUser;   // If user enters a valid Spotify User ID to the UID Textbox on MainWindow.xaml this User object will filled. This property binded to CurrentAccountOwnerName label on MainWindow.xaml.

        private List<Playlist_Simplified> simplifiedPlaylists = new List<Playlist_Simplified>();    // This is the user's all public playlists, but 'Playlist_Simplified' does not contains playlist's tracks.
        private List<Playlist_Full> fullPlaylists = new List<Playlist_Full>();  // This is the user's all public playlists with playlist's tracks. This list is binded to 'Playlists' DataGridView.
        private Playlist_Full selectedPlaylist; // This is changed when the user selects(clicks) a playlist from 'Playlists' DataGridView. This playlist's tracks are binded to 'Tracks' DataGridView.

        private List<Playlist_Full> savedPlaylists = new List<Playlist_Full>(); // This list is filled from user's local computer harddisk if playlist's are saved before.

        private List<Playlist_Full> compareResults = new List<Playlist_Full>(); // When user clicks the 'Compare' button, this list is generated via comparing the current playlists and savedplaylists. This playlist's tracks are consists of tracks that not existed on savedplaylists.
        private Playlist_Full selectedPlaylist_CompareResult;   // This is changed when the user selects(clicks) a playlist from 'ComparedPlaylists' DataGridView. This playlist's tracks are binded to 'ComparedTracks' DataGridView.

        private ICommand _saveCommand;  // This is the command that binded to 'Save' button on MainWindow.xaml.
        private ICommand _compareCommand;   // This is the command that binded to 'Compare' button on MainWindow.xaml.
        private ICommand _getUser;  // This is the command that binded to 'Login' button on MainWindow.xaml.
        private ICommand _saveCompareResults; 

        private bool isProgressing;
        #endregion

        #region Public Properties
        public string UserID
        {
            get { return userID; }
            set
            {
                userID = value;
                NotifyPropertyChanged();
            }
        }
        public User CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                NotifyPropertyChanged();
            }
        }

        public List<Playlist_Simplified> SimplifiedPlaylists
        {
            get { return simplifiedPlaylists; }
            set
            {
                simplifiedPlaylists = value;
                NotifyPropertyChanged();
            }
        }
        public List<Playlist_Full> FullPlaylists
        {
            get { return fullPlaylists; }
            set
            {
                fullPlaylists = value;
                NotifyPropertyChanged();
            }
        }
        public Playlist_Full SelectedPlaylist
        {
            get { return selectedPlaylist; }
            set
            {
                selectedPlaylist = value;
                NotifyPropertyChanged();
            }
        }

        public List<Playlist_Full> CompareResults
        {
            get { return compareResults; }
            set
            {
                compareResults = value;
                NotifyPropertyChanged();
            }
        }
        public Playlist_Full SelectedPlaylistCompareResult
        {
            get { return selectedPlaylist_CompareResult; }
            set
            {
                selectedPlaylist_CompareResult = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                if(_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(p => SavePlaylists(), p => currentUser != null && fullPlaylists.Count > 0);
                }
                return _saveCommand;
            }
        }
        public ICommand CompareCommand
        {
            get
            {
                if(_compareCommand == null)
                {
                    _compareCommand = new RelayCommand(p => Compare_Playlists(), p => currentUser != null && fullPlaylists.Count > 0 && savedPlaylists.Count > 0);
                }
                return _compareCommand;
            }
        }
        public ICommand SaveCompareResults
        {
            get
            {
                if (_saveCompareResults == null)
                {
                    _saveCompareResults = new RelayCommand(p => Save_CompareResults(), p => currentUser != null && fullPlaylists.Count > 0 && savedPlaylists.Count > 0 && compareResults.Count>0);
                }
                return _saveCompareResults;
            }
        }
        public ICommand GetUser
        {
            get
            {
                if(_getUser == null)
                {
                    _getUser = new RelayCommand(p => CheckUserID(), p => true);
                }
                return _getUser;
            }
        }

        public bool IsProgressing
        {
            get { return !isProgressing; }
            set
            {
                isProgressing = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        public MainViewModel(){ IsProgressing = false; } // Default Constructor

        // Get currentUser's all public playlists(simplified) then get all tracks of this playlists by href(on returned playlists) and finally cobine all playlists and tracks to the Playlist_Full list.
        private async void GetUserPlaylists()
        {
            IsProgressing = true;

            // ServiceManager returns all public playlist's.(But not with tracks, only tracks href)
            var pLists = await ServiceManager.Instance.GetPlayLists(currentUser.id);

            // If ServiceManager sucessfully returns the playlists.
            if (pLists != null)
            {
                simplifiedPlaylists = pLists.ToList();
                List<Playlist_Full> fPlaylist = new List<Model.Playlist_Full>();

                // We get tracks of all playlists with playlist id.
                for (int i = 0; i < simplifiedPlaylists.Count; i++)
                {
                    var tList = await ServiceManager.Instance.GetPlaylistTracks(currentUser.id, simplifiedPlaylists[i].id);
                    if (tList != null)
                        fPlaylist.Add(new Playlist_Full(simplifiedPlaylists[i], tList.ToList()));
                }
                FullPlaylists = fPlaylist;
            }

            IsProgressing = false;
        }

        // Creates a new(if not exists) directory inside MyDocuments, and saves all playlist's(with tracks)
        private void SavePlaylists()
        {
            IsProgressing = true;

            string playlistID;
            string path_myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderName = "SpotifyPlaylists";
            Directory.CreateDirectory(Path.Combine(path_myDocuments, folderName));

            // Convert all playlist's to json files(with tracks) and save into directory with playlist's id's.
            if (fullPlaylists.Count <= 0)
                MessageBox.Show("There is no playlists to save!");

            foreach (Playlist_Full playlist in fullPlaylists)
            {
                playlistID = playlist.id + ".json";
                string json = JsonConvert.SerializeObject(playlist);
                File.WriteAllText(Path.Combine(path_myDocuments, folderName, playlistID), json);
            }

            IsProgressing = false;
        }

        // Gets the saved playlist's from directory, converts them to 'Playlist_Full' objects and puts 'savedPlaylists' list.
        private async Task<bool> GetSavedPlaylists()
        {
            IsProgressing = true;

            // Clearing the savedPlaylists list to prevent appending.
            savedPlaylists.Clear();

            string path_myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string folderName = "SpotifyPlaylists";

            // Get all paths inside directory.
            string[] filePaths = Directory.GetFiles(Path.Combine(path_myDocuments, folderName), "*.json", SearchOption.TopDirectoryOnly);

            // Loop through all .json files, converts from .json to 'Playlist_Full' objects then adds to 'savedPlaylists' list.
            for (int i = 0; i < filePaths.Length; i++)
            {
                string jsonText = File.ReadAllText(filePaths[i]);
                Playlist_Full playlist = JsonConvert.DeserializeObject<Playlist_Full>(jsonText);
                savedPlaylists.Add(playlist);
            }

            IsProgressing = false;
            return true;
        }

        // Compares two 'PlaylistTrack' list, and add different tracks to the newly created 'newTracklist' list.
        private List<PlaylistTrack> Compare_PlaylistTracks(List<PlaylistTrack> upToDateTracklist, List<PlaylistTrack> existingTracklist)
        {
            List<PlaylistTrack> newTracklist = new List<PlaylistTrack>();
            foreach(PlaylistTrack playlistTrack in upToDateTracklist)
            {
                if (playlistTrack.track == null)
                    continue;

                try
                {
                    if (!existingTracklist.Exists(pTrack => pTrack.track != null ? pTrack.track.id == playlistTrack.track.id : true))
                        newTracklist.Add(playlistTrack);
                }
                catch
                {
                    continue;
                }
            }

            return newTracklist;
        }

        // Compare saved playlists(from local PC) and current playlists(from Spotify) and saves the result set to 'compareResults' list.
        private async void Compare_Playlists()
        {
            List<PlaylistTrack> compareResult = new List<PlaylistTrack>();  // For tracks
            List<Playlist_Full> compareResults = new List<Playlist_Full>(); // For playlists

            // Get saved playlists from documents
            bool result = await GetSavedPlaylists();

            // If there is a saved playlists and if Spotify playlists are taken from server.
            if (result && fullPlaylists.Count>0)
            {
                // Loop through each plasylist taken from spotify web API and compare if this playlist saved before.
                foreach(Playlist_Full newPlaylist in fullPlaylists)
                {
                    Playlist_Full savedPlaylist = savedPlaylists.Find(pList => pList.id == newPlaylist.id);
                    if (savedPlaylist == null)
                        continue;

                    compareResult = Compare_PlaylistTracks(newPlaylist.trackList, savedPlaylist.trackList);
                    compareResults.Add(new Playlist_Full(new Playlist_Simplified(newPlaylist.id, newPlaylist.name + " -CompareResult", newPlaylist.tracks), compareResult));
                }
            }

            CompareResults = compareResults;
        }

        private async void Save_CompareResults()
        {
            List<string> trackUris = new List<string>();
            foreach (Playlist_Full pListFull in compareResults)
            {
                if(pListFull.trackList.Count > 0)
                {
                    trackUris.Clear();
                    foreach (PlaylistTrack pTrack in pListFull.trackList)
                    {
                        if(pTrack.track != null)
                            trackUris.Add(pTrack.track.uri);
                    }

                    await ServiceManager.Instance.CreatePlaylistAndAddTracks(currentUser.id, pListFull.name, trackUris);
                }
            }
        }

        // When the User Enters Spotify User ID and click to 'Login' button, this method is triggered.
        private async void CheckUserID()
        {
            if (userID.Length > 0)
            {
                // If entered userID is valid ServiceManager returns the spotify user.
                User cUser = await ServiceManager.Instance.GetUserByUserID(userID);
                if (cUser.display_name.Length <= 0)
                {
                    CurrentUser = null;
                    return;
                }

                CurrentUser = cUser;
                GetUserPlaylists();
            }
            else
                CurrentUser = null;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
