using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CoverFetcher.Net
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            string filename = (string)((DataObject)e.Data).GetFileDropList()[0];

            TagLib.File file = TagLib.File.Create(filename);
            string artist = file.Tag.FirstPerformer;
            string albumArtist = file.Tag.FirstAlbumArtist;
            string title = file.Tag.Title;
            string album = file.Tag.Album;
        }
    }
}
