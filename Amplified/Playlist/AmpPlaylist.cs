using Amplified.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amplified.Playlist
{
    public class AmpPlaylist : INotifyPropertyChanged
    {
        public AmpPlaylist()
        {
            PlaylistItemQueue = new System.Collections.ObjectModel.ObservableCollection<AmpPlaylistItem>();
        }

        public void EnqueueFile(FileInfo f)
        {
            PlaylistItemQueue?.Add(new AmpPlaylistItem(f.FullName, this));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public System.Collections.ObjectModel.ObservableCollection<AmpPlaylistItem> PlaylistItemQueue { get; set; }

        public AmpPlaylistItem CurrentTrackPlaylistItem
        {
            get;
            set;
        }

        public string CurrentTrack { get; set; } = "<No Track Playing>";

        internal AmpPlaylistItem Front()
        {
            CurrentTrackPlaylistItem = PlaylistItemQueue.Count() > 0 ? PlaylistItemQueue.First() : null;
            if (CurrentTrackPlaylistItem != null)
            {
                PropertyChanged.OnPropertyChanged(() => CurrentTrackPlaylistItem);
                CurrentTrack = CurrentTrackPlaylistItem.DiskItem.FileInfo.FullName;
                PropertyChanged.OnPropertyChanged(() => CurrentTrack);
            }
            return CurrentTrackPlaylistItem;
        }
    }
}
