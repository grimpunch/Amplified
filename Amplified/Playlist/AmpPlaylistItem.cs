using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amplified.IO;
using Amplified.Utils;

namespace Amplified.Playlist
{
    public class AmpPlaylistItem : INotifyPropertyChanged
    {
        public AmpPlaylistItem(string absPath, AmpPlaylist ownerPlaylist )
        {
            DiskItem = new AmpDiskItem(new System.IO.FileInfo(absPath));
            OwnerPlaylist = ownerPlaylist;
        }
        public AmpPlaylistItem(FileInfo f, AmpPlaylist ownerPlaylist)
        {
            DiskItem = new AmpDiskItem(f);
            OwnerPlaylist = ownerPlaylist;
        }

        public NAudio.Wave.AudioFileReader GetAudioReader()
        {
            if (DiskItem.AudioFileReader == null)
            {
                DiskItem.AudioFileReader = new NAudio.Wave.AudioFileReader(DiskItem.FileInfo.FullName);
                PropertyChanged.OnPropertyChanged(() => DiskItem.AudioFileReader);
            }
            return DiskItem.AudioFileReader;
        }

        public AmpPlaylist OwnerPlaylist { get; set; } = null;
        public event PropertyChangedEventHandler PropertyChanged;
        public AmpDiskItem DiskItem {get;set;} = null;
    }
}
