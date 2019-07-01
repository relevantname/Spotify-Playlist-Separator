using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyExtension.Model
{
    public class Track
    {
        public string id { get; set; }
        public string name { get; set; }
        public string uri { get; set; }

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

        //private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        //{
        //    if(PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
