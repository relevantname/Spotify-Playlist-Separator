using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyExtension.Model
{
    public class Playlist_Simplified
    {
        public string id { get; set; }
        public string name { get; set; }
        public PlaylistTracks tracks { get; set; }

        public Playlist_Simplified()
        {
            
        }
        public Playlist_Simplified(string id, string name, PlaylistTracks tracks)
        {
            this.id = id;
            this.name = name;
            this.tracks = tracks;
        }
        //public event PropertyChangedEventHandler PropertyChanged;

        //public string ID
        //{
        //    get { return id; }
        //    set
        //    {
        //        id = value;
        //        NotifyPropertyChanged();
        //    }
        //}

        //public string Name
        //{
        //    get { return name; }
        //    set
        //    {
        //        name = value;
        //        NotifyPropertyChanged();
        //    }
        //}

        //public PlaylistTracks Tracks
        //{
        //    get { return tracks; }
        //    set
        //    {
        //        tracks = value;
        //        NotifyPropertyChanged();
        //    }
        //}

        //private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        //{
        //    if(PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
