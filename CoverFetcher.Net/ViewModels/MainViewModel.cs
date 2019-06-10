using CoverFetcher.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Configuration;

namespace CoverFetcher.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ItunesRepository itunesRepository;
        private readonly SettingsConfig settings;

        public MainViewModel ()
	    {
            settings = ((CoverFetcher.App)App.Current).Configuration.Get<SettingsConfig>();

            itunesRepository = new ItunesRepository();
            Refresh = new RelayCommand(LoadCover);
            Save = new RelayCommand(UpdateTags);
            Cancel = new RelayCommand(ReadTags);
            SaveCover = new RelayCommand(WriteCoverToFile);

            Countries = new[] {
                new Country("United States", "US"),
                new Country("United Kingdom", "UK"),
                new Country("Belgium", "BE"),
                new Country("France", "FR"),
                new Country("Germany", "DE"),
                new Country("Netherlands", "NL"),
            };

            SelectedCountry = Countries.FirstOrDefault(c => c.Code == settings?.ItunesSearch?.DefaultCountry) ?? Countries[0];
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

        private byte[] coverImageBytes;
        private BitmapImage cover;
        public BitmapImage Cover { get { return cover; } set { cover = value; RaisePropertyChanged("Cover"); } }

        private SearchStatus status;
        public SearchStatus Status { get { return status; } set { status = value; RaisePropertyChanged("Status"); } }

        public IList<Country> Countries { get; private set; }

        public Country SelectedCountry { get; set; }


        private string filePath;
        public string FilePath 
        { 
            set
            {
                filePath = value;
                ReadTags();
            }
        }

        public ICommand Refresh { get; private set; }
        public ICommand Save { get; private set; }
        public ICommand Cancel { get; private set; }
        public ICommand SaveCover { get; private set; }

        private void ReadTags()
        {
            try
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
                file.Dispose();

                LoadCover();
            }
            catch(Exception ex)
            {
                SendErrorMessage(ex.Message);
            }
        }

        private void LoadCover()
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    Status = SearchStatus.Searching;
                }));

                itunesRepository.FindCover(string.IsNullOrWhiteSpace(AlbumArtist) ? Artist : AlbumArtist,
                    string.IsNullOrWhiteSpace(Album) ? Title : Album, SelectedCountry.Code).ContinueWith(task =>
                    {
                        try
                        {
                            coverImageBytes = task.Result;

                            // Execute on UI Thread
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                if (coverImageBytes != null)
                                {
                                    BitmapImage coverImage = LoadImage(coverImageBytes);

                                    Cover = coverImage;
                                    Status = SearchStatus.Found;
                                }
                                else
                                {
                                    Cover = null;
                                    Status = SearchStatus.NotFound;
                                }
                            }));
                        }
                        catch (AggregateException ex)
                        {
                            SendErrorMessage(ex.InnerException.Message);
                        }
                        catch (Exception ex)
                        {
                            SendErrorMessage(ex.Message);
                        }
                    });
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.Message);
            }
        }

        private BitmapImage LoadImage(byte[] data)
        {
            MemoryStream memoryStream = new MemoryStream(data);
            BitmapImage image = new BitmapImage();

            image.BeginInit();
            image.StreamSource = memoryStream;
            image.EndInit();

            return image;
        }

        private void UpdateTags()
        {
            try
            {
                TagLib.File file = TagLib.File.Create(filePath);

                if (coverImageBytes != null)
                {
                    TagLib.Picture picture = new TagLib.Picture(new ByteArrayFileAbstraction("test", coverImageBytes));
                    TagLib.Id3v2.AttachedPictureFrame coverPictureFrame = new TagLib.Id3v2.AttachedPictureFrame(picture);
                    coverPictureFrame.MimeType = MediaTypeNames.Image.Jpeg;
                    coverPictureFrame.Type = TagLib.PictureType.FrontCover;
                    file.Tag.Pictures = new TagLib.IPicture[] { coverPictureFrame };
                }
                else
                {
                    file.Tag.Pictures = new TagLib.IPicture[0];
                }

                file.Tag.Performers = new string[] { Artist };
                file.Tag.Title = Title;
                file.Tag.AlbumArtists = new string[] { AlbumArtist };
                file.Tag.Album = Album;

                if (Genre != null)
                    file.Tag.Genres = new string[] { Genre };

                file.Tag.Year = uint.Parse(Year);

                file.Save();
                file.Dispose();
            }
            catch (Exception ex)
            {
                SendErrorMessage(ex.Message);
            }
        }

        private void WriteCoverToFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "JPEG (.jpg)|*.jpg";

            string artist = (AlbumArtist != null) ? AlbumArtist : Artist;
            string album = (Album != null) ? Album : Title;
            string defaultFileName = settings.ExportSettings.DefaultFileNamePattern
                .Replace("{artist}", artist)
                .Replace("{album}", album);

            dialog.FileName = defaultFileName;
            if (dialog.ShowDialog() == true)
            {
                string filename = dialog.FileName;

                File.WriteAllBytes(filename, coverImageBytes);
            }
        }

        private void SendErrorMessage(string message)
        {
            Messenger.Default.Send(new ShowErrorMessage() { Message = message });
        }
    }
}