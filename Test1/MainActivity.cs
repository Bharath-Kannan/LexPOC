using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Media;
using Java.IO;
using System.Threading.Tasks;

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
            var audRecorder = new AudioRecord(AudioSource.Mic, 11025, ChannelIn.Mono, Encoding.Pcm16bit, audioBuffer.Length);
            //start Recoding on click of start button
            _start.Click += async delegate
          {
              _stop.Enabled = !_stop.Enabled;
              _start.Enabled = !_start.Enabled;
              try
              {
                  audRecorder.StartRecording();
                  await Task.Factory.StartNew(() => audRecorder.Read(audioBuffer, 0, audioBuffer.Length));
              }
              catch (Exception ex)
              {
                  throw ex;
              }

          };
            //stop recording on click of stop button
            _stop.Click += async delegate
           {
               try
               {
                   _start.Enabled = !_start.Enabled;
                   _stop.Enabled = !_stop.Enabled;
                   audRecorder.Stop();
                   var response_audio = await Authenticator.Main(audioBuffer);//send the recorded audio to main method and get the response audio
                   Array.Clear(audioBuffer, 0, audioBuffer.Length);//Clear the audio buffer
                   if (response_audio == null)//If no audio response
                   {
                       AlertDialog.Builder alert = new AlertDialog.Builder(this);
                       alert.SetTitle("Error");
                       alert.SetMessage("There is a problem in processing request..!");
                       alert.SetPositiveButton("OK", (senderAlert, args) =>
                       {

                       });
                       Dialog dialog = alert.Create();
                       dialog.Show();
                   }
                   else if (response_audio.Length == 0)//Request Fullfilled
                   {
                       AlertDialog.Builder alert = new AlertDialog.Builder(this);
                       alert.SetTitle("Success");
                       alert.SetMessage("Request processed successfully!");
                       alert.SetPositiveButton("OK", (senderAlert, args) =>
                       {
                       });
                       Dialog dialog = alert.Create();
                       dialog.Show();
                   }
                   else
                   {
                       PlayAudioTrack(response_audio);
                   }
               }
               catch (Exception ex)
               {
                   throw ex;
               }

           };
        }

        //Play back the response
        public void PlayAudioTrack(byte[] audio_response_Buffer)
        {
            try
            {
                    File tempAudio = File.CreateTempFile("tempvoice", "mp3", CacheDir);
                    tempAudio.DeleteOnExit();
                    FileOutputStream fos = new FileOutputStream(tempAudio);
                    fos.Write(audio_response_Buffer);
                    fos.Close();
                    mediaPlayer.Reset();
                    FileInputStream fis = new FileInputStream(tempAudio);
                    mediaPlayer.SetDataSource(fis.FD);
                    mediaPlayer.Prepare();
                    mediaPlayer.Start();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }

}


