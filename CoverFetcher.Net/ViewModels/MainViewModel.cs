using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CoverFetcher.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ItunesRepository itunesRepository;

        public MainViewModel ()
	    {
            itunesRepository = new ItunesRepository();
	    }

        private string artist;
        public string Artist { get { return artist; } set { artist = value; RaisePropertyChanged("Artist"); } }

        private string title;
        public string Title { get { return title; } set { title = value; RaisePropertyChanged("Title"); } }

        private string albumArtist;
        public string AlbumArtist { get { return albumArtist; } set { albumArtist = value; RaisePropertyChanged("AlbumArtist"); } }

        private string album;
        public string Album { get { return album; } set { album = value; RaisePropertyChanged("Album"); } }

        private string track;
        public string Track { get { return track; } set { track = value; RaisePropertyChanged("Track"); } }

        private string disc;
        public string Disc { get { return disc; } set { disc = value; RaisePropertyChanged("Disc"); } }

        private string year;
        public string Year { get { return year; } set { year = value; RaisePropertyChanged("Year"); } }

        private string genre;
        public string Genre { get { return genre; } set { genre = value; RaisePropertyChanged("Genre"); } }

        private BitmapImage cover;
        public BitmapImage Cover { get { return cover; } set { cover = value; RaisePropertyChanged("Cover"); } }

        private string filePath;
        public string FilePath 
        { 
            set
            {
                filePath = value;
                ReadTags();
            }
        }

        private void ReadTags()
        {
            TagLib.File file = TagLib.File.Create(filePath);
            Artist = file.Tag.FirstPerformer;
            Title = file.Tag.Title;
            AlbumArtist = file.Tag.FirstAlbumArtist;
            Album = file.Tag.Album;
            Track = "" + file.Tag.Track;
            Disc = "" + file.Tag.Disc;
            Year = "" + file.Tag.Year;
            Genre = file.Tag.FirstGenre;
            Cover = null;

            itunesRepository.FindCover(string.IsNullOrWhiteSpace(AlbumArtist) ? Artist : AlbumArtist, 
                string.IsNullOrWhiteSpace(Album) ? Title : Album).ContinueWith(task =>
                {
                    // Execute on UI Thread
                    Application.Current.Dispatcher.Invoke((Action)(() => { Cover = task.Result; }));
                });
        }
    }
}
