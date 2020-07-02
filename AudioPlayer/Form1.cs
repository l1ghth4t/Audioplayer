using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace AudioPlayer
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            InitializeComponent();
            metroStyleManager1.Style = MetroFramework.MetroColorStyle.Red;
            metroStyleManager1.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.StyleManager = metroStyleManager1;
            this.Text = "Audioplayer";
        }

        private static WMPLib.WindowsMediaPlayer player = new WMPLib.WindowsMediaPlayer();

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((player.controls.currentPosition != 0) && (player.controls.currentItem.duration != 0))
                metroProgressBar1.Value = (int)((player.controls.currentPosition / player.controls.currentItem.duration) * 100.0);

            if (this.BackgroundImage == null && player.URL != "")
            {
                try
                {
                    TagLib.File file_TAG = TagLib.File.Create(player.URL);

                    if (file_TAG.Tag.Pictures.Length >= 1)
                    {
                        try
                        {
                            var bin = (byte[])(file_TAG.Tag.Pictures[0].Data.Data);
                            pictureBox1.Image = Image.FromStream(new MemoryStream(bin));
                        }

                        catch (Exception) { }

                        lbTags.Text = "Album: " + file_TAG.Tag.Album +
                                        "\nArtist: " + String.Join(", ", file_TAG.Tag.Performers) +
                                        "\nName: " + file_TAG.Tag.Title +
                                        "\nYear: " + file_TAG.Tag.Year +
                                        "\nDuretion: " + file_TAG.Properties.Duration.ToString("mm\\:ss");
                    }
                }

                catch (Exception ex)
                {
                    timer1.Stop();
                    MetroFramework.MetroMessageBox.Show(this, $"{ex.Message}", "Error of reading metadata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            AudioPosition.Text = player.controls.currentPositionString;
            lbNameAudio.Text = Path.GetFileName(player.URL);
        }

        private void OpenTile_Click(object sender, EventArgs e)
        {
            try
            {
                Openfile();
            }

            catch (Exception ex)
            {
                MetroFramework.MetroMessageBox.Show(this, $"{ex.Message}", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void Openfile()
        {
            OpenFileDialog openSound = new OpenFileDialog()
            {
                Filter = "MP3|*.mp3|WAV|*.wav|FLAC|*.flac|All files|*.*",
                Multiselect = false,
                ValidateNames = true
            };

            if (openSound.ShowDialog() == DialogResult.OK)
            {
                player.URL = openSound.FileName;
                player.controls.play();
            }
        }

        private void PlayTile_Click(object sender, EventArgs e)
        {
            if (PlayTile.Text == "Pause")
            {
                player.controls.pause();
                PlayTile.Text = "Play";
            }

            else
            {
                player.controls.play();
                PlayTile.Text = "Pause";
            }
        }

        private void StopTile_Click(object sender, EventArgs e)
        {
            player.controls.stop();
            metroProgressBar1.Value = 0;
        }

        private void metroProgressBar1_MouseDown(object sender, MouseEventArgs e)
        {
            this.metroProgressBar1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ScrollProgress);
        }

        private void metroProgressBar1_MouseUp(object sender, MouseEventArgs e)
        {
            this.metroProgressBar1.MouseMove -= new System.Windows.Forms.MouseEventHandler(this.ScrollProgress);

            metroProgressBar1.Value = ((e.Location.X * 100) / metroProgressBar1.Width);

            player.controls.currentPosition = ((double)e.Location.X / (double)metroProgressBar1.Width) * player.controls.currentItem.duration;
        }

        private void ScrollProgress(object sender, MouseEventArgs e)
        {
            metroProgressBar1.Value = ((e.Location.X * 100) / metroProgressBar1.Width);
        }
    }
}
