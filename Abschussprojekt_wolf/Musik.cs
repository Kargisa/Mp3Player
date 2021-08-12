using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abschussprojekt_wolf
{
    class Musik
    {
        public string Titel { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public TimeSpan Length { get; set; }
        public string Path { get; set; } 

        public Musik(string inTitle, string inKuenstler, string inAlbum, TimeSpan inLaenge, string inPath)
        {
            Titel = inTitle;
            Artist = inKuenstler;
            Album = inAlbum;
            Length = inLaenge;
            Path = inPath;
        }

    }
}
