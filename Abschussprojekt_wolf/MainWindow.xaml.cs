using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using NAudio.Wave;
using System.Media;
using System.Threading;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Win32;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Controls.Primitives;
//using System.Text.Json;

namespace Abschussprojekt_wolf
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double sound;
        string mp3Directory = @"..\..\mp3s";
        string volumePath = @"..\..\Volume.txt";
        string jsonPath = @"..\..\Saves.json";
        string newDir = @"..\..\mp3s";
        TimeSpan timer;
        TimeSpan time;
        DispatcherTimer dtClockTime;
        string start = "▶";
        string pause = "⏸";
        string weiter = "⏯";
        string shuffle = "🔁";
        string title;
        string artist;
        string album;
        int selction;
        bool v = true;
        bool y = false;
        bool x = true;
        object women;
        string button;
        Musik musikClass;
        TimeSpan editDuration;
        Random rand = new Random();
        List<string> morePaths = new List<string>();
        List<Musik> saveList = new List<Musik>();
        CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        ObservableCollection<Musik> musikList = new ObservableCollection<Musik>();
        AlreadyExists exist = new AlreadyExists();
        LoadingWindow load = new LoadingWindow();
        HelpWindow w = new HelpWindow();
        public MainWindow()
        {
            InitializeComponent();
            v = false;
            load.Show();
            SliderValue();
            dtgPlaylist.ItemsSource = musikList;
            FullTimer();
            SearchForFiles();
            load.Close();
        }
        private void SliderValue()
        {
            string vol = File.ReadAllText(volumePath);
            sound = Convert.ToDouble(vol);
            sldVolume.Value = sound;
            mediaElement.Volume = sldVolume.Value;
        }
        private void SearchForFiles()
        {
            //DirectoryInfo pwd = new DirectoryInfo(mp3Directory);
            using (StreamReader r = new StreamReader(jsonPath))
            {
                var jsonRead = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<List<Musik>>(jsonRead);
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        try
                        {
                            FileAttributes attributes = File.GetAttributes(item.Path);
                            musikList.Add(item);
                            saveList.Add(item);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show($"Der Song {item.Titel} wurde aus dem Ordner entfernt!\n");
                        }
                    }
                }
            }
            var json = JsonConvert.SerializeObject(saveList);
            File.WriteAllText(jsonPath, json);
        }
        private void FullTimer()
        {
            dtClockTime = new DispatcherTimer();
            timer = new TimeSpan();
            dtClockTime.Interval = TimeSpan.FromSeconds(1);
            dtClockTime.Tick += DtClockTime_Tick;
        }
        private void ClearTXT()
        {
            txtTitel.Text = "";
            txtArtist.Text = "";
            txtAlbum.Text = "";
            chbNoAlbum.IsChecked = false;
            txtAlbum.IsEnabled = true;
        }
        private void btnMusicAdd1_Click(object sender, RoutedEventArgs e)
        {
            if (btnEdit.Content.ToString() == "finish Edit")
            {
                MessageBox.Show("Beende bitte zuerst deine Bearbeitung :)");
                goto Skip;
            }
            if (txtArtist.Text == "" || txtTitel.Text == "" || txtAlbum.Text == "" && txtAlbum.IsEnabled == true)
            {
                MessageBox.Show("Bitte füllen Sie jedes der Obrigen Feld aus\n wenn sie kein Album wollen dann drücken Sie auf 'No Album'.");
            }
            else
            {
                dialog.IsFolderPicker = false;
                dialog.Multiselect = false;
                dialog.InitialDirectory = "C:\\";
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    try
                    {
                        Mp3FileReader reader = new Mp3FileReader(dialog.FileName);
                        TimeSpan laenge = reader.TotalTime;
                        int secounds = (int)laenge.TotalSeconds;
                        TimeSpan duration = TimeSpan.FromSeconds(secounds);
                        string currentFile = System.IO.Path.GetFileName(dialog.FileName);
                        string pathFile = System.IO.Path.Combine(newDir, currentFile);
                        try
                        {
                            File.Copy(dialog.FileName, pathFile);
                            y = true;
                        }
                        catch (Exception)
                        {
                            exist.yesNo = 3;
                            exist.ShowDialog();
                            exist.Close();
                        }
                        if (exist.yesNo == 1 || y == true)
                        {
                            Musik musik = new Musik(txtTitel.Text, txtArtist.Text, txtAlbum.Text, duration, pathFile);
                            AddForAdd(musik);
                            y = false;
                        }
                        else if (exist.yesNo == 0)
                        {
                            ClearTXT();
                        }
                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("Bitte wählen Sie eine .mp3 Datei aus.");
                    }
                }
                else
                {
                    MessageBox.Show("Keine Datei ausgewäglt.");
                }
            }
        Skip:;
        }
        private void AddForAdd(Musik musik)
        {
            musikList.Add(musik);
            saveList.Add(musik);
            var json = JsonConvert.SerializeObject(saveList);
            File.WriteAllText(jsonPath, json);
            ClearTXT();
        }
        private void btnMusikRemove_Click(object sender, RoutedEventArgs e)
        {
            if (btnPlay.Content.ToString() == start && btnEdit.Content.ToString() == "Edit")
            {
                if (dtgPlaylist.SelectedValue != (object)-1)
                {
                    Musik selected = (Musik)dtgPlaylist.SelectedItem;
                    using (StreamReader r = new StreamReader(jsonPath))
                    {
                        var jsonRead = r.ReadToEnd();
                        var items = JsonConvert.DeserializeObject<List<Musik>>(jsonRead);
                        if (items != null)
                        {
                            foreach (var item in items)
                            {
                                if (item.Path == selected.Path)
                                {
                                    morePaths.Add(item.Path);
                                }
                            }
                            if (morePaths.Count <= 1)
                            {
                                File.Delete(selected.Path);
                            }
                        }
                    }
                    musikList.Remove(selected);
                    saveList.Remove(selected);
                    var json = JsonConvert.SerializeObject(saveList);
                    File.WriteAllText(jsonPath, json);
                    morePaths.Clear();
                }
                else
                {
                    MessageBox.Show("Bitte wählen Sie ein Musikstück aus das Sie enfernen wollen.");
                }
            }
            else
            {
                MessageBox.Show("Du kannst keine Musik entfernen die gerade spielt oder bearbeitet wird :/");
            }
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (btnPlay.Content.ToString() != start)
            {
                MessageBox.Show("Musik die gerade läuft, kann nicht bearbeitet werden.");
            }
            else if (btnEdit.Content.ToString() == "Edit")
            {
                try
                {
                    musikClass = (Musik)dtgPlaylist.SelectedItem;
                    txtTitel.Text = musikClass.Titel;
                    txtArtist.Text = musikClass.Artist;
                    txtAlbum.Text = musikClass.Album;
                    editDuration = musikClass.Length;
                    title = txtTitel.Text;
                    artist = txtArtist.Text;
                    album = txtAlbum.Text;
                    if (txtAlbum.Text == "")
                    {
                        txtAlbum.IsEnabled = false;
                        chbNoAlbum.IsChecked = true;
                    }
                    btnResetEdit.Visibility = Visibility.Visible;
                    btnEdit.Content = "finish Edit";
                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Wählen Sie ein Musikstück aus.");
                }
            }
            else
            {
                if (txtArtist.Text == "" || txtTitel.Text == "" || txtAlbum.Text == "" && txtAlbum.IsEnabled == true)
                {
                    MessageBox.Show("Bitte füllen Sie jedes der Obrigen Feld aus\n wenn sie kein Album wollen dann drücken Sie auf 'No Album'.");
                }
                else if (dtgPlaylist.SelectedItem != null)
                {

                    Musik musik = new Musik(txtTitel.Text, txtArtist.Text, txtAlbum.Text, editDuration, musikClass.Path);
                    musikList.Remove(musikClass);
                    saveList.Remove(musikClass);
                    AddForAdd(musik);
                    btnResetEdit.Visibility = Visibility.Hidden;
                    btnEdit.Content = "Edit";
                }
            }
        }
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnPlay.Content.ToString() == start)
                {
                    Musik path = (Musik)dtgPlaylist.SelectedItem;
                    women = dtgPlaylist.SelectedItem;
                    Uri mp3 = new Uri(path.Path, UriKind.Relative);
                    mediaElement.Source = mp3;
                    mediaElement.Play();
                    Animation((Musik)women);
                    button = pause;
                    lblNowPlaying.Content = $"Now Playing: {path.Titel}";
                    btnPlay.Content = pause;
                }
                else if (btnPlay.Content.ToString() == pause)
                {
                    Animation((Musik)women);
                    mediaElement.Pause();
                    btnPlay.Content = weiter;
                    button = weiter;
                }
                else if (btnPlay.Content.ToString() == weiter)
                {
                    Animation((Musik)women);
                    mediaElement.Play();
                    btnPlay.Content = pause;
                    button = pause;
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Bitte wähle einen Song aus.");
            }

        }
        private void dtgPlaylist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (women != dtgPlaylist.SelectedItem)
            {
                btnPlay.Content = start;
            }
            else if (button == weiter)
            {
                btnPlay.Content = weiter;
            }
            else if (button == pause)
            {
                btnPlay.Content = pause;
            }
        }
        private void Animation(Musik path)          //Macht die Animations sachen und etc
        {
            try
            {
                Mp3FileReader reader = new Mp3FileReader(path.Path);
                time = reader.TotalTime;
                int secounds = (int)time.TotalSeconds;
                pgbMusik.Maximum = secounds;
                Duration duration = new Duration(TimeSpan.FromSeconds(secounds));
                DoubleAnimation doubleanimation = new DoubleAnimation(secounds, duration);
                if (btnPlay.Content.ToString() == start || x == false)
                {
                    doubleanimation.From = 0;
                    pgbMusik.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
                    dtClockTime.Stop();
                    FullTimer();
                    dtClockTime.Start();
                    if (x == false)
                    {
                        Uri mp3 = new Uri(path.Path, UriKind.Relative);
                        mediaElement.Source = mp3;
                        mediaElement.Play();
                        x = true;
                    }
                }
                else if (btnPlay.Content.ToString() == pause)
                {
                    doubleanimation.BeginTime = null;
                    pgbMusik.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
                    dtClockTime.Stop();
                }
                else if (btnPlay.Content.ToString() == weiter)
                {
                    pgbMusik.BeginAnimation(ProgressBar.ValueProperty, doubleanimation);
                    dtClockTime.Start();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Keine Musik ausgewählt");
                btnPlay.Content = start;
            }

        }

        private void DtClockTime_Tick(object sender, EventArgs e)
        {
            timer = timer.Add(TimeSpan.FromSeconds(1));
            int ytime = (int)time.TotalSeconds;
            TimeSpan xtime = TimeSpan.FromSeconds(ytime);
            lblTimer.Content = $"{timer.ToString()}/{xtime.ToString()}";
            if (timer >= xtime)
            {
                dtClockTime.Stop();
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            x = false;
            Animation((Musik)women);
            btnPlay.Content = pause;
        }

        private void sldVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (v == false)
            {
                mediaElement.Volume = sldVolume.Value;
                sound = sldVolume.Value;
                string vol = Convert.ToString(sound);
                File.WriteAllText(volumePath, vol);
            }
        }

        private void chbNoAlbum_Clicked(object sender, RoutedEventArgs e)
        {
            if (chbNoAlbum.IsChecked == true)
            {
                txtAlbum.Text = "";
                txtAlbum.IsEnabled = false;
            }
            else
            {
                txtAlbum.IsEnabled = true;
            }
        }

        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            w.Show();
        }

        private void btnResetEdit_Click(object sender, RoutedEventArgs e)
        {
            txtTitel.Text = title;
            txtArtist.Text = artist;
            txtAlbum.Text = album;
            if (album == "")
            {
                txtAlbum.IsEnabled = false;
                chbNoAlbum.IsChecked = true;
            }
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void dtgPlaylist_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() == "Path")
            {
                e.Cancel = true;
            }
        }
        private void btnShuffle_Click(object sender, RoutedEventArgs e)
        {
            selction = dtgPlaylist.SelectedIndex;
            while (selction == dtgPlaylist.SelectedIndex)
            {
                selction = rand.Next(0, musikList.Count);
            }
            dtgPlaylist.SelectedIndex = selction;
            btnPlay.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
        }



        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}