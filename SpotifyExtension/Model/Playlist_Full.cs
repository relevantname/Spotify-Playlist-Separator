using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyExtension.Model
{
    public class Playlist_Full : Playlist_Simplified
    {
        public List<PlaylistTrack> trackList { get; set; }

        public Playlist_Full()
        {

        }
        public Playlist_Full(Playlist_Simplified simplePlaylist, List<PlaylistTrack> trackList)
        {
            base.id = simplePlaylist.id;
            base.name = simplePlaylist.name;
            base.tracks = simplePlaylist.tracks;
            this.trackList = trackList;
        }
    }
}
