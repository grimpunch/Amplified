using Amplified.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amplified.IO
{
    public class AmpDiskItem : INotifyPropertyChanged
    {
        public AmpDiskItem(FileInfo fInfo)
        {
            FileInfo = fInfo;
        }

        #region Properties
        public NAudio.Wave.AudioFileReader AudioFileReader { get; set; } = null;
        public System.IO.FileInfo FileInfo { get; set; } = null;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
