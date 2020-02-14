using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
namespace Reproductor
{
    class FadeOut : ISampleProvider
    {
        private ISampleProvider fuente;

        private int muestrasLeidas = 0;
        private float segundosTranscurridos = 0;

        private float duracion;
        private float inicio;
        public FadeOut(ISampleProvider fuente, float inicio, float duracion)
        {
            this.fuente = fuente;
            this.inicio = inicio;
            this.duracion = duracion;
        }
        public WaveFormat WaveFormat
        {
            get
            {
                return fuente.WaveFormat;
            }
        }
        public int Read(float[] buffer, int offset, int count)
        {
            int read = fuente.Read(buffer, offset, count);

            //Aplicar el efecto
            muestrasLeidas += read;
            segundosTranscurridos = (float)(muestrasLeidas) / (float)(fuente.WaveFormat.SampleRate) / (float)(fuente.WaveFormat.Channels);

            if (segundosTranscurridos >= inicio)
            {
                float factorEscala = (((inicio + duracion) - segundosTranscurridos)) / duracion;
                if (((inicio + duracion) - segundosTranscurridos) <= 0.0f)
                {
                    factorEscala = 0.0f;
                }
                
                for (int i = 0; i < read; i++)
                {
                        buffer[i + offset] *= factorEscala;
                }
            }

            return read;
        }
    }
}
