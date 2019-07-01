using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyExtension.Model
{
    public class PlaylistTracks
    {
        public string href { get; set; }
        public int total { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;

        //public ObservableCollection<PlaylistTrack> Tracks
        //{
        //    get { return tracks; }
        //    set
        //    {
        //        tracks = value;
        //        NotifyPropertyChanged();
        //    }
        //}
        //public int TrackCount
        //{
        //    get { return trackCount; }
        //    set
        //    {
        //        trackCount = value;
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
