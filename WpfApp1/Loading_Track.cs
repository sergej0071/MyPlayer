using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace WpfApp1
{ 

    class Loading_Track
    {
        List<string> traks_P; // база данных
        string currentSong;
        string FormatTrack = "Audio Files|*.mp3; *.wav; *.Ogg; *.ape; *.mp4;";

        public Loading_Track()
        {
            traks_P = new List<string>();
        }

        public List<string> GetListTrack()
        {
            return traks_P;
        }
               
        public string GetCurrentTrack()
        {
            return currentSong;
        }

        

        public void Load_Track()
        {
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            traks_P = new List<string>();

            fileDialog.Filter = FormatTrack;
            var result = fileDialog.ShowDialog();

            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    {
                        var file = fileDialog.FileName;
                        bool CheckFormat = false;
                        currentSong = file;
                        // удаляем последние символы
                        file = FolderDiscover(file);

                        List<Regex> listFromat = FormatTrackFilterForLoadTrack(FormatTrack);

                        //DirectoryInfo(folderPath).GetFiles("*.mp3").Select(x => x.FullName).ToArray()
                        //Directory.EnumerateFiles(file, "*", SearchOption.TopDirectoryOnly)


                        foreach (var element in Directory.EnumerateFiles(file, "*", SearchOption.TopDirectoryOnly))
                        {
                            foreach (var ElementForCheckFormat in listFromat)
                            {
                                if (ElementForCheckFormat.IsMatch(element))
                                    CheckFormat = true;
                            }

                            if (CheckFormat)
                            {
                                traks_P.Add(element);
                                CheckFormat = false;
                            }
                        }
                        // сортировка  по названию

                      
                        break;
                    }
                case System.Windows.Forms.DialogResult.Cancel:

                default:
                   // System.Windows.MessageBox.Show("Неправильный выбор файла");
                    break;
            }
        }

        public string FolderDiscover(string way)
        {
            int count = 0;
            string variable_file;

            for (int i = way.Length-1; way[i] != '\\'; i--)
                count++;
            variable_file = way;
            count = way.Length - count- 1;
            way = "";
            for (int i = 0; i < count; i++)
                way += variable_file[i];

            return way;
        }

        private List<Regex> FormatTrackFilterForLoadTrack(string formatTrack)
        {
            List<Regex> listFromat = new List<Regex>();
            string str_forList="";
            bool Check = false;

            foreach (var element in formatTrack)
            {
                if (element == '.')
                    Check = true;

                if (element == ';')
                {
                    Check = false;
                    listFromat.Add(new Regex(@"\w*." + str_forList));
                    str_forList = "";
                }

                if (Check)
                {
                    str_forList += element;
                }

              
            }
            
            return listFromat;
        }

    }
}
