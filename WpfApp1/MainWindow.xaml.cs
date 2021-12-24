using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{
    
    public partial class MainWindow : Window
    {

        private DispatcherTimer TimerTrack;
        private double TimerCounter = 0;
        private Loading_Track loading_Track;
        private Logik_player logik_Player;

        private bool Start = false; //проверка есть ли треки или создан ли объект logik_Player
        private TimeSpan ChekPosition_music;
        private bool Check_pause_play = true;
        private bool PlayTreck = true;
        private bool RePlayTrack = false;
        private bool ForwardTrack = false;

        private bool CheckRePlayTrack = true;
        private bool CheckForwardTrack = true;
        private bool MainWindowState = false;

        public MainWindow()
        {
            InitializeComponent();
            loading_Track = new Loading_Track();
            SetStartingVallueVolume();
            TimerTrack = new DispatcherTimer(); 
        }        

        //timer
        private void timerForNow_Tick(object sender, EventArgs e)
        {
            string a = PlayerElement.Position.ToString(); // функция преобразования
            
            string c = "0,";
            if (a.Length > 8)
            {
                for (int i = 9; i < 16; i++)
                    c += a[i];
            }
            else
            {
                c = "0";
            }

            int ad = (int.Parse(a[3].ToString() + a[4].ToString()) * 60) + int.Parse(a[6].ToString() + a[7].ToString());
            double b = 0;
            if (ad == 0)
            {
                 b = (Double.Parse(c));
            }
            else
            {
                 b = (Double.Parse(ad.ToString()) + Double.Parse(c));
            }


            // преобразование в время

            int Time = 0;
            double TimeD = 0;
            Double.TryParse(b.ToString(), out TimeD);
            Time = (int)TimeD;
            int Second = Time % 60;
            int Min = Time / 60;

            

            if(Min<10)
            a = "0"+ Min.ToString() + ":";
            if (Min >= 10)
            a = Min.ToString() + ":";
            if (Second < 10)
            a += "0" + Second.ToString();
            if (Second >= 10)
            a += Second.ToString();

            TimerCounter = Double.Parse(b.ToString());


            CurrentTime.Text = a;
            TrackSlider.Value = TimerCounter;
            CurrentTimeD.Text = a;

            if (TrackSlider.Value >= TrackSlider.Maximum)
            {
             
                PlayTreck = true;
                ChekPosition_music = TimeSpan.FromSeconds(0);
                Check_pause_play = true;
                PlayerElement.Stop();
                TimerTrack.Stop();

                if (RePlayTrack) // режим проигрует тот же трек снова
                {
                    
                    TimerTrack.Interval = TimeSpan.FromMilliseconds(250);
                    TimerTrack.Tick += timerForNow_Tick;
                    TimerTrack.Start();
                    Check_pause_play = false;

                    PlayerElement.Source = new Uri(logik_Player.ThisTrack());
                    PlayerElement.Play();
                    PlayerElement.Position = ChekPosition_music;
                    
                }
                if (ForwardTrack) // режим проигруем треки до конца плейлиста
                {
                    PlayerElement.Source = new Uri(logik_Player.TrackForward());
                    PlayerElement.Play();
                    PlayerNull();
                    PlayTreck = false;

                    Check_pause_play = false;
                    TimerTrack.Interval = TimeSpan.FromMilliseconds(250);
                    TimerTrack.Tick += timerForNow_Tick;
                    TimerTrack.Start();
                }
               
            }
        }

        // служебные функции 

        private void PlayerNull()
        {
            Check_pause_play = false;
            ChekPosition_music = TimeSpan.FromSeconds(0);
        }      

        private void SetStartingVallueVolume()
        {
            PlayerElement.Volume = 0.5;
            Volume.Value = 0.5;           
        }       
       

        private void PlayerElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            string time = PlayerElement.NaturalDuration.ToString();
            string temporary_variable1="";
            string milisecond = "0,";
            for (int i = 3; i < 8; i++)
                temporary_variable1 += time[i];
            for (int i = 9; i < time.Length; i++)
                milisecond += time[i];

            MaxTimeAudioV.Text = temporary_variable1;
            MaxTimeAudioN.Text = temporary_variable1;
            // плеер
            
            TrackSlider.Maximum = double.Parse(temporary_variable1[0].ToString() + temporary_variable1[1].ToString())* 60 + double.Parse(temporary_variable1[3].ToString() + temporary_variable1[4].ToString())+ double.Parse( milisecond);
            TrackSlider.Value = 0;

            // название трека
           string variable_file = logik_Player.ThisTrack();
            string name = "";
            int count=0;
            for (int i = variable_file.Length - 1; variable_file[i] != '\\'; i--)
                count++;
            count = variable_file.Length - count;
            for (int i = count; i < variable_file.Length; i++)
             name += variable_file[i];

            NameTrackUp.Text = name;
            NameTrack.Text = name;
        }

        private void SetNullTimeAudio()
        {
            MaxTimeAudioV.Text = "00:00";
            MaxTimeAudioN.Text = "00:00";
            CurrentTime.Text = "00:00";
            CurrentTimeD.Text = "00:00";
        }

        // навигация 
        private void Сollapse_Player(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void full_screen_Player(object sender, RoutedEventArgs e)
        {
            if (!MainWindowState)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                MainWindowState = true;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                MainWindowState = false;
            }
        }

        private void Close_Player(object sender, RoutedEventArgs e)
        {
            Application app = Application.Current;
            app.Shutdown();
        }

        //загрузка путей треков
        private void TreakLoading(object sender, RoutedEventArgs e)
        {
            loading_Track.Load_Track();
            logik_Player = new Logik_player(loading_Track.GetListTrack(), loading_Track.GetCurrentTrack());
            if (loading_Track.GetCurrentTrack() != null)
            {
                Start = true;
                Check_pause_play = true; 
            }
            loading_Track  = new Loading_Track(); // економия памяти
        }
        // навигация по трекам
        private void Back(object sender, RoutedEventArgs e)
        {
            if (Start)
            {
                PlayerElement.Source = new Uri(logik_Player.TrackBack());               
                PlayerElement.Play();
                PlayerNull();

                PlayTreck = false;

               
                TimerTrack.Interval = TimeSpan.FromMilliseconds(250);
                TimerTrack.Tick += timerForNow_Tick;
                TimerTrack.Start();
            }
        }

        private void Play(object sender, RoutedEventArgs e)
        {            
                if (PlayTreck)
                {
                    if (Start)
                    {
                        TimerTrack.Interval = TimeSpan.FromMilliseconds(250);
                        TimerTrack.Tick += timerForNow_Tick;
                        TimerTrack.Start();
                        PlayTreck = false;
                        TrackSlider.Value = 0;
                    }
                    

                }

                if (Check_pause_play)
                {
                    if (Start)
                    {
                        PlayerElement.Source = new Uri(logik_Player.ThisTrack());
                        PlayerElement.Play();
                        PlayerElement.Position = ChekPosition_music;
                        
                    }

                    Check_pause_play = false;
                    ButtonPlay.Content = "False";
                }
                else
                {
                    if (Start)
                    {
                        PlayerElement.Pause();                        
                        ChekPosition_music = PlayerElement.Position;
                    }

                    Check_pause_play = true;
                    ButtonPlay.Content = "True";
                }                
        }
        

        private void forward(object sender, RoutedEventArgs e)
        {
            if (Start)
            {
                //делегирование                 

                PlayerElement.Source = new Uri(logik_Player.TrackForward());                
                PlayerElement.Play();
                PlayerNull();
                PlayTreck = false;

                


                TimerTrack.Interval = TimeSpan.FromMilliseconds(250);
                TimerTrack.Tick += timerForNow_Tick;
                TimerTrack.Start();
            }
        }

        private void Stop(object sender, RoutedEventArgs e)
        {            
            TrackSlider.Value = 0;
            TimerTrack.Stop();
            Check_pause_play = true;
            ChekPosition_music = TimeSpan.FromSeconds(0);
            SetNullTimeAudio();
            PlayTreck = true;
            PlayerElement.Close();
        }
       

        private void VolumeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PlayerElement.Volume = Volume.Value;
        }

        private void TrackValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimerCounter = TrackSlider.Value;    // проблема заедания не решена
            ChekPosition_music = TimeSpan.FromSeconds(TrackSlider.Value);
            PlayerElement.Position = ChekPosition_music;
        }

        private void MoveThumb(object sender, DragCompletedEventArgs e)
        {
            TimerCounter = TrackSlider.Value;
            ChekPosition_music = TimeSpan.FromSeconds(TrackSlider.Value);
            PlayerElement.Position = ChekPosition_music;
        }


        private void TrackSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           // TimerCounter = TrackSlider.Value;
           // ChekPosition_music = TimeSpan.FromSeconds(TrackSlider.Value);
           // PlayerElement.Position = ChekPosition_music;
        }

        private void TrackSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TimerCounter = TrackSlider.Value;
           // ChekPosition_music = TimeSpan.FromSeconds(TrackSlider.Value);
           // PlayerElement.Position = ChekPosition_music;
        }  
        
        
        private void RePlay_Track(object sender, RoutedEventArgs e)
        {
           /*
            Binding binding = new Binding();

            binding.ElementName = "Mouse_Click"; // элемент-источник
            binding.Path = new PropertyPath("RepeatBehavior"); // свойство элемента-источника
            replay.SetBinding(TextBlock.TextProperty, binding); // установка привязки для элемента-приемника
            
           */
    
            if (CheckRePlayTrack)
            {
                replay.Content = "true";
                RePlayTrack = true;
                CheckRePlayTrack = false;
            }
            else
            {
                replay.Content = "false";
                RePlayTrack = false;
                CheckRePlayTrack = true;
            }
        }

        private void Forward_Track(object sender, RoutedEventArgs e)
        {           

            if (CheckForwardTrack)
            {
                ForwardTrack = true;
                CheckForwardTrack = false;
                forward_track.Content = "True";
            }
            else
            {
                ForwardTrack = false;
                CheckForwardTrack = true;
                forward_track.Content = "False";
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void TrackSlider_IsMouseCaptureWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            MessageBox.Show("Yess!!!");
        }

       

        //private void Slider_DragCompleted()
        //{
        //    DoWork(((Slider)sender).Value);
        //    this.dragStarted = false;
        //}


    }
}
