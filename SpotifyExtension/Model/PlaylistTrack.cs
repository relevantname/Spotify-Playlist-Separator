using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyExtension.Model
{
    public class PlaylistTrack
    {
        public string added_at { get; set; }
        public Track track { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;

        //public DateTime AddedDate
        //{
        //    get { return addedDate; }
        //    set
        //    {
        //        addedDate = value;
        //        NotifyPropertyChanged();
        //    }
        //}
        //public Track Track
        //{
        //    get { return track; }
        //    set
        //    {
        //        track = value;
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
