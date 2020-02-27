using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
namespace Reproductor
{
    class EfectoDelay : ISampleProvider
    {
        private ISampleProvider fuente;

        public int offsetMilisegundos;
        private List<float> muestras = new List<float>();
        private int tamañoBuffer;

        private int cantidadMuestrasTranscurridas = 0;
        private int cantidadMuestrasBorradas = 0;
        public EfectoDelay(ISampleProvider fuente, int offsetMilisegundos)
        {
            this.fuente = fuente;
            this.offsetMilisegundos = offsetMilisegundos;

            tamañoBuffer = fuente.WaveFormat.SampleRate * 20 * fuente.WaveFormat.Channels;
        }
        //frecuencia de muestreo, profundidad de bits, cantidad de canales.
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

            return read;
        }
    }
}
