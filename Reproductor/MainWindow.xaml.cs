﻿using System;
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

using Microsoft.Win32;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using System.Windows.Threading;

namespace Reproductor
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        DispatcherTimer timer;

        //Lector de archivos
        AudioFileReader reader;
        //Comunicacion con la tarjeta de audio
        //exclusivo para salidas
        WaveOut output;

        bool dragging = false;
        //VolumeSampleProvider volume;
        EfectoVolumen efectoVolumen;
        EfectoFadeIn efectoFadeIn;
        FadeOut efectoFadeOut;
        public MainWindow()
        {
            InitializeComponent();
            ListarDispositivosSalida();
            btnReproducir.IsEnabled = false;
            btnDetener.IsEnabled = false;
            btnPausa.IsEnabled = false;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            /*if(efectoFadeIn != null)
            {
                lblMuestras.Text = efectoFadeIn.muestrasLeidas.ToString();
            }*/
            lblTiempoActual.Text = reader.CurrentTime.ToString().Substring(0, 8);
            if(!dragging)
            {
                sldReproduccion.Value = reader.CurrentTime.TotalSeconds;
            }
        }

        void ListarDispositivosSalida()
        {
            cbDispositivoSalida.Items.Clear();
            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                WaveOutCapabilities capacidades = WaveOut.GetCapabilities(i);
                cbDispositivoSalida.Items.Add(capacidades.ProductName);
            }
            cbDispositivoSalida.SelectedIndex = 0;

        }

        private void BtnExaminar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                txtDirectorio.Text = openFileDialog.FileName;
                btnReproducir.IsEnabled = true;
            }
        }

        private void btnReproducir_Click(object sender, RoutedEventArgs e)
        {
            if (output != null && output.PlaybackState == PlaybackState.Paused)
            {
                //retomar la reproducción
                output.Play();
            }
            else
            {
                if (txtDirectorio.Text != null && txtDirectorio.Text != string.Empty)
                {
                    reader = new AudioFileReader(txtDirectorio.Text);

                    //volume = new VolumeSampleProvider(reader);
                    //volume.Volume = (float)(sldVolumen.Value);

                    float duracion = float.Parse(txtDuracionFadeIn.Text);
                    float inicioFadeOut = float.Parse(txtInicioFadeOut.Text);
                    float duracionFadeOut = float.Parse(txtDuracionFadeOut.Text);

                    efectoFadeOut = new FadeOut(reader, inicioFadeOut, duracionFadeOut);
                    efectoFadeIn = new EfectoFadeIn(reader, duracion);

                    efectoVolumen = new EfectoVolumen(efectoFadeOut);
                    efectoVolumen.Volumen = (float)(sldVolumen.Value);
                    

                    output = new WaveOut();
                    output.DeviceNumber = cbDispositivoSalida.SelectedIndex;
                    output.PlaybackStopped += Output_PlaybackStopped;

                    //output.Init(volume);
                    output.Init(efectoVolumen);
                    output.Play();

                    /*//Cambiar el volumen del output
                    output.Volume = (float)(sldVolumen.Value);*/

                    lblTiempoTotal.Text = reader.TotalTime.ToString().Substring(0, 8);
                    lblTiempoActual.Text = reader.CurrentTime.ToString().Substring(0, 8);

                }
            }
            btnReproducir.IsEnabled = false;
            btnDetener.IsEnabled = true;
            btnPausa.IsEnabled = true;

            sldReproduccion.Maximum = reader.TotalTime.TotalSeconds;
            sldReproduccion.Value = reader.CurrentTime.TotalSeconds;

            timer.Start();
        }

        private void Output_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            timer.Stop();
            reader.Dispose();
            output.Dispose();
        }

        private void btnPausa_Click(object sender, RoutedEventArgs e)
        {
            if (output != null)
            {
                output.Pause();
                btnReproducir.IsEnabled = true;
                btnPausa.IsEnabled = false;
                btnDetener.IsEnabled = true;
            }
        }

        private void btnDetener_Click(object sender, RoutedEventArgs e)
        {
            if (output != null)
            {
                output.Stop();
                btnReproducir.IsEnabled = true;
                btnDetener.IsEnabled = false;
                btnPausa.IsEnabled = false;
            }
        }
        private void SldTiempo_DragStarted(object sender, RoutedEventArgs e)
        {
            dragging = true;
        }
       private void SldTiempo_DragCompleted(object sender, RoutedEventArgs e)
       {
            dragging = false;
            if(reader != null && output != null && output.PlaybackState != PlaybackState.Stopped)
            {
                reader.CurrentTime = TimeSpan.FromSeconds(sldReproduccion.Value);
            }
       }

        private void SldVolumen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(output != null && output.PlaybackState != PlaybackState.Stopped)
            {
                efectoVolumen.Volumen = (float)(sldVolumen.Value);
                /*output.Volume = (float)(sldVolumen.Value);*/
                //volume.Volume = (float)(sldVolumen.Value);
                
            }
        }

        private void TxtDuracionFadeIn_Copy_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
