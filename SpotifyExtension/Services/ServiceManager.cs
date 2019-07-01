using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SpotifyExtension.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpotifyExtension.Services
{
    public class ServiceManager
    {
        public static ServiceManager instance;
        public static HttpClient client = new HttpClient();

        private string tokenURL = "https://accounts.spotify.com/api/token";
        private string authURL = "https://accounts.spotify.com/authorize";

        private string playlistURL = "https://api.spotify.com/v1/users/{0}/playlists";
        private string getPlaylistTracksURL = "https://api.spotify.com/v1/users/{0}/playlists/{1}/tracks";
        private string postPlaylistTracksURL = "https://api.spotify.com/v1/playlists/{0}/tracks";
        private string getUserURL = "https://api.spotify.com/v1/users/{0}";

        public ServiceManager()
        {
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        public static ServiceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceManager();
                }
               
                return instance;
            }
        }

        // Gets the access token from web API.
        private async Task<string> GetAccessToken()
        {
            SpotifyToken token = new SpotifyToken();
            
            using(var message = new HttpRequestMessage(HttpMethod.Post, tokenURL))
            {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", "MDZkNDFlZDkxZDllNGJkY2JmNjE5YzM0MDliZDJiMDQ6MDA0ZTE1ZTNiZDkxNGVlN2JkNWE3MzJlOGU3N2MyYjc=");
                string postString = string.Format("grant_type=client_credentials");

                using (HttpContent content = new StringContent(postString, UTF8Encoding.UTF8, "application/x-www-form-urlencoded"))
                {
                    message.Content = content;

                    var response = await client.SendAsync(message);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContentString = await response.Content.ReadAsStringAsync();
                        token = JsonConvert.DeserializeObject<SpotifyToken>(responseContentString);
                    }
                }
            }

            #region Web Request Method
            //string postString = string.Format("grant_type=client_credentials");
            //byte[] byteArray = Encoding.UTF8.GetBytes(postString);
            //WebRequest request = WebRequest.Create(tokenURL);
            //request.Method = "POST";
            //// Basic MDZkNDFlZDkxZDllNGJkY2JmNjE5YzM0MDliZDJiMDQ6MDA0ZTE1ZTNiZDkxNGVlN2JkNWE3MzJlOGU3N2MyYjc= => this is the code string getted from spotify developers page(Client ID:Secret) and encoded as UTF8
            //request.Headers.Add("Authorization", "Basic MDZkNDFlZDkxZDllNGJkY2JmNjE5YzM0MDliZDJiMDQ6MDA0ZTE1ZTNiZDkxNGVlN2JkNWE3MzJlOGU3N2MyYjc=");
            //request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = byteArray.Length;
            //using (Stream dataStream = request.GetRequestStream())
            //{
            //    dataStream.Write(byteArray, 0, byteArray.Length);
            //    using (WebResponse response = await request.GetResponseAsync())
            //    {
            //        using (Stream responseStream = response.GetResponseStream())
            //        {
            //            using (StreamReader reader = new StreamReader(responseStream))
            //            {
            //                string responseFromServer = reader.ReadToEnd();
            //                token = JsonConvert.DeserializeObject<SpotifyToken>(responseFromServer);
            //            }
            //        }
            //    }
            //}
            #endregion

            return token.access_token;
        }

        public async Task<IEnumerable<Playlist_Simplified>> GetPlayLists(string userID)
        {
            string token = await GetAccessToken();
            string url = string.Format(playlistURL, userID);
            IEnumerable<Playlist_Simplified> playLists = await GetSpotifyType<IEnumerable<Playlist_Simplified>>(token, url);
            return playLists;
        }
        public async Task<IEnumerable<PlaylistTrack>> GetPlaylistTracks(string userID, string trackID)
        {
            string token = await GetAccessToken();
            string url = string.Format(getPlaylistTracksURL, userID, trackID);
            IEnumerable<PlaylistTrack> playlistTracks = await GetSpotifyType<IEnumerable<PlaylistTrack>>(token, url);
            return playlistTracks;
        }
        public async Task<User> GetUserByUserID(string userID)
        {
            string token = await GetAccessToken();
            string url = string.Format(getUserURL, userID);
            User user = await GetSpotifyType<User>(token, url);
            return user;
        }
        public async Task<bool> CreatePlaylistAndAddTracks(string userID, string playlistName, List<string> trackURIs)
        {
            string token = await GetAccessToken();
            string url = string.Format(playlistURL, userID);

            TrackUris trackUris = new TrackUris();
            trackUris.uris = trackURIs;
            string jsonUrls = JsonConvert.SerializeObject(trackUris);

            using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, url))
            {
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                //string playListName = string.Format("{\"name\":\"{0}\"}", playlistName);
                PlaylistName pListName = new PlaylistName();
                pListName.name = playlistName;
                HttpContent content = new StringContent(JsonConvert.SerializeObject(pListName));

                message.Content = content;

                var response = await client.SendAsync(message);
                if (response.IsSuccessStatusCode)
                {
                    var responseContentString = await response.Content.ReadAsStringAsync();
                    Playlist_Simplified newPlayList = JsonConvert.DeserializeObject<Playlist_Simplified>(responseContentString);

                    string urlTracks = string.Format(postPlaylistTracksURL, newPlayList.id);
                    //string jsonUrls = JsonConvert.SerializeObject(trackURIs);

                    message.RequestUri = new Uri(urlTracks);
                    content = new StringContent(jsonUrls);

                    return true;
                }
                else
                {
                    MessageBox.Show(response.ReasonPhrase);
                    return false;
                }
            }
        }

        /// <summary>
        /// Converts Web Response Content from Json String To Object(T)
        /// </summary>
        /// <typeparam name="T">Return Type</typeparam>
        /// <param name="token">Spotify Authentication Token</param>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<T> GetSpotifyType<T>(string token, string url)
        {
            try
            {
                T type = default(T);

                using (var message = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    var response = await client.SendAsync(message);
                    if (response.IsSuccessStatusCode)
                    {
                        using(HttpContent content = response.Content)
                        {
                            var jsonContent = content.ReadAsStringAsync().Result;
                            JObject jsonObject = JObject.Parse(jsonContent);

                            if (typeof(T) != typeof(User))
                                type = JsonConvert.DeserializeObject<T>(jsonObject["items"].ToString());
                            else
                                type = JsonConvert.DeserializeObject<T>(jsonObject.ToString());
                        }
                    }
                    else
                    {
                        MessageBox.Show(response.StatusCode.ToString());
                        return default(T);
                    }
                }

                #region Old WebRequest
                //using (WebResponse response = await request.GetResponseAsync())
                //{
                //    using (Stream dataStream = response.GetResponseStream())
                //    {
                //        using (StreamReader reader = new StreamReader(dataStream))
                //        {
                //            string responseFromServer = reader.ReadToEnd();
                //            JObject jsonObject = JObject.Parse(responseFromServer);

                //            if (typeof(T) != typeof(User))
                //                type = JsonConvert.DeserializeObject<T>(jsonObject["items"].ToString());
                //            else
                //                type = JsonConvert.DeserializeObject<T>(jsonObject.ToString());
                //        }
                //    }
                //}
                #endregion

                return type;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return default(T);
            }
        }
    }
}
