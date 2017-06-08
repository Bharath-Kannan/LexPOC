using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Media;
using Java.IO;
using System.Threading.Tasks;
using Amazon.Lex.Model;
using static Android.Views.ViewGroup;
using Android.Views;
using Android.Graphics.Drawables;

namespace Test1
{
    [Activity(Label = "Test1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        Android.Widget.Button _start;
        byte[] audioBuffer = new byte[100000];
        TextView[] textViewArray = new TextView[20];
        int textcount = 0;
        LinearLayout main;
        ScrollView content;
        LinearLayout chat;
       

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);
          
            _start = FindViewById<Android.Widget.Button>(Resource.Id.start);
            main = FindViewById<LinearLayout>(Resource.Id.parentLayout);
            chat = FindViewById<LinearLayout>(Resource.Id.chat);
            content = FindViewById<ScrollView>(Resource.Id.scroll);
            var fromLayoutParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            var toLayoutParams = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

            var audRecorder = new AudioRecord(AudioSource.Mic, 11025, ChannelIn.Mono, Encoding.Pcm16bit, audioBuffer.Length);
            for (int i = 0; i < 20; i++)
            {
                
                textViewArray[i] = new TextView(this);
                textViewArray[i].SetPadding(50,30,50,30);
               chat.AddView(textViewArray[i]);
               
            }
            

            _start.Click += async delegate
          {
              if(audRecorder.RecordingState==RecordState.Stopped) 
              {
                  _start.Text = "STOP RECORDING";
                  try
                  {
                      
                      audRecorder.StartRecording();
                      await Task.Factory.StartNew(() => audRecorder.Read(audioBuffer, 0, audioBuffer.Length));
                  }
                  catch (Exception ex)
                  {
                      throw ex;
                  }
              }
              if (audRecorder.RecordingState == RecordState.Recording)
              {
                  try
                  {
                      _start.Text = "START RECORDING";                      
                      audRecorder.Stop();
                      PostContentResponse response= await Authenticator.Main(audioBuffer);
                      System.IO.Stream response_audio_stream = response.AudioStream;


                      fromLayoutParams.SetMargins(10, 10, 10, 10);
                      fromLayoutParams.Gravity = GravityFlags.Left; 
                      textViewArray[textcount].LayoutParameters = fromLayoutParams;
                      textViewArray[textcount].Text = response.InputTranscript;
                      GradientDrawable request = new GradientDrawable();
                      request.SetCornerRadius(10);
                      request.SetColor(global::Android.Graphics.Color.ParseColor("#1E90FF"));
                      textViewArray[textcount].Background = request;
                      textcount++;

                      toLayoutParams.SetMargins(10, 10, 10, 10);
                      toLayoutParams.Gravity = GravityFlags.Right;
                      textViewArray[textcount].LayoutParameters = toLayoutParams;
                      textViewArray[textcount].Text = response.Message;
                     
                      GradientDrawable reply = new GradientDrawable();
                      reply.SetCornerRadius(10);
                      reply.SetColor(global::Android.Graphics.Color.ParseColor("#696969"));
                      textViewArray[textcount].Background = reply;
                      textcount++;
                      Array.Clear(audioBuffer, 0, audioBuffer.Length);
                      if (response_audio_stream.Length== -1)//If no audio response
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
                      else if (response_audio_stream.Length == 0)//Request Fullfilled
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
                          var response_audio = Authenticator.ConvertStreamToBytes(response_audio_stream);
                          PlayAudioTrack(response_audio);
                      }
                  }
                  catch (Exception ex)
                  {
                      throw ex;
                  }
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


