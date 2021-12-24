using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    class Logik_player
    {
        // пути треков
        List<string> treks_P;
        string currentSong;
        // текущий трек
        int ThisTrackCounter = 0;

        public Logik_player(List<string> treks_P, string currentSong)
        {
            this.treks_P = new List<string>();
            foreach (var element in treks_P)
            this.treks_P.Add(element);
            this.currentSong = currentSong;
                        
            foreach (string element in treks_P)
            {
                if (element == currentSong)
                    break;
                else
                    ThisTrackCounter++;
            }
        }

        public string TrackBack()
        {
            if (ThisTrackCounter != 0)
            {
                ThisTrackCounter--;
                currentSong = treks_P[ThisTrackCounter];
            }

            return treks_P[ThisTrackCounter];
        }


        public string ThisTrack()
        {
            return currentSong;
        }

        public string TrackForward()
        {
            if (ThisTrackCounter != treks_P.Count - 1)
            {
                ThisTrackCounter++;
                currentSong = treks_P[ThisTrackCounter];
            }            
            return treks_P[ThisTrackCounter];
        }
    }
}
