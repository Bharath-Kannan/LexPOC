using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Media;
using Java.IO;

namespace Test1
{
    [Activity(Label = "Test1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        Button _start;
        Button _stop;

        byte[] audioBuffer = new byte[100000];
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
            _start = FindViewById<Button>(Resource.Id.start);
            _stop = FindViewById<Button>(Resource.Id.stop);

            var audRecorder = new AudioRecord(
              AudioSource.Mic,
              11025,
              ChannelIn.Mono,
              Android.Media.Encoding.Pcm16bit,
              audioBuffer.Length
            );

            _start.Click += delegate
           {
               _stop.Enabled = !_stop.Enabled;
               _start.Enabled = !_start.Enabled;
               audioBuffer = new byte[100000];
               audRecorder.StartRecording();
               audRecorder.Read(audioBuffer, 0, audioBuffer.Length);
           };
            _stop.Click += async delegate
            {
                _start.Enabled = !_start.Enabled;
                _stop.Enabled = !_stop.Enabled;
                audRecorder.Stop();
                var response_audio = await Authenticator.Main(audioBuffer);
                audioBuffer = null;
                PlayAudioTrack(response_audio);
            };
        }


        //play back the response
        public void PlayAudioTrack(byte[] audio_response_Buffer)
        {
            try
            {
                File tempMp3 = File.CreateTempFile("tempvoice", "mp3", CacheDir);
                tempMp3.DeleteOnExit();
                FileOutputStream fos = new FileOutputStream(tempMp3);
                fos.Write(audio_response_Buffer);
                fos.Close();
                mediaPlayer.Reset();
                FileInputStream fis = new FileInputStream(tempMp3);
                mediaPlayer.SetDataSource(fis.FD);
                mediaPlayer.Prepare();
                mediaPlayer.Start();

            }
            catch (Exception)
            {

                throw;
            }

        }

    }

}


