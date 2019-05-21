using System.Data;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Windows;

public class AmpAppMainGUIViewModel
{
    #region Properties

    #endregion

    #region Fields
    private WaveOutEvent m_audioDevice;
    private AudioFileReader m_audioFile;
    #endregion

    #region Constructor
    public AmpAppMainGUIViewModel()
    {

    }

    #endregion

    #region Methods
    public void OnPlayPause(RoutedEventArgs e)
    {
        if (m_audioDevice != null && m_audioFile != null)
        {
            m_audioDevice.Play();
        }
        else
        {
            if (m_audioFile == null)
            {
                InitAudioStream(new AudioFileReader(@"C:\Windows\media\tada.wav"));
                m_audioDevice.Play();
            }
        }
    }

    public void OnStop(RoutedEventArgs e)
    {
        m_audioDevice.Stop();
    }

    public void OnNext(RoutedEventArgs e)
    {

    }

    public void OnQuit(RoutedEventArgs e)
    {
        
    }

    #endregion

    #region Event Handling
    public void OnArgReceived(string arg)
    {
        // received arg.
        Console.WriteLine("Received arg from new instance '{0}'", arg);
    }

    private void OnPlaybackStopped(object sender, StoppedEventArgs stopArgs)
    {
        m_audioDevice.Dispose();
        m_audioDevice = null;
        m_audioFile.Dispose();
        m_audioFile = null;
    }

    public void InitAudioStream(AudioFileReader audioFileReader)
    {
        if (m_audioDevice == null)
        {
            m_audioDevice = new WaveOutEvent();
        }

        if (m_audioFile == null)
        {
            m_audioFile = audioFileReader;
            m_audioDevice.Init(m_audioFile);
        }
    }

    #endregion
}