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
        #region variable declaration
        private MediaPlayer mediaPlayer = new MediaPlayer();
        ImageButton _start;
        ImageButton _clear;
        byte[] audioBuffer = new byte[100000];
        LinearLayout main, chat;
        ScrollView content;
        TextView record_text;
        #endregion


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            #region initialization
            _start = FindViewById<Android.Widget.ImageButton>(Resource.Id.start);
            _clear = FindViewById<ImageButton>(Resource.Id.clear);
            record_text = FindViewById<TextView>(Resource.Id.recordText);
            main = FindViewById<LinearLayout>(Resource.Id.parentLayout);
            chat = FindViewById<LinearLayout>(Resource.Id.chat);
            content = FindViewById<ScrollView>(Resource.Id.scroll);
            content.PageScroll(FocusSearchDirection.Up);
            var requestLayout = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            var responseLayout = new LinearLayout.LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);
            var audRecorder = new AudioRecord(AudioSource.Mic, 11025, ChannelIn.Mono, Encoding.Pcm16bit, audioBuffer.Length);
            #endregion

            _clear.Click += delegate
           {
               if (chat.ChildCount > 0)
                   chat.RemoveAllViews();
           };
                _start.Click += async delegate
          {
              if(audRecorder.RecordingState==RecordState.Stopped) 
              {
                  _start.SetImageResource(Resource.Drawable.stop);
                  try
                  {
                      record_text.Visibility = Android.Views.ViewStates.Visible;
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
                      record_text.Visibility = Android.Views.ViewStates.Invisible;
                      _start.SetImageResource(Resource.Drawable.start);
                      audRecorder.Stop();
                      PostContentResponse response = await Authenticator.Main(audioBuffer);
                      System.IO.Stream response_audio_stream = response.AudioStream;
                      requestLayout.SetMargins(20, 10, 10, 10);
                      requestLayout.Gravity = GravityFlags.Left;
                      TextView request_Text = new TextView(this);
                      request_Text.LayoutParameters = requestLayout;
                      request_Text.Text = response.InputTranscript;
                      GradientDrawable request = new GradientDrawable();
                      request.SetCornerRadius(50);
                      request_Text.SetPadding(50, 30, 50, 30);
                      request.SetColor(global::Android.Graphics.Color.ParseColor("#1E90FF"));
                      request_Text.Background = request;
                      chat.AddView(request_Text);

                      responseLayout.SetMargins(10, 10, 10, 10);
                      responseLayout.Gravity = GravityFlags.Right;
                      TextView response_Text = new TextView(this);
                      response_Text.LayoutParameters = responseLayout;
                      response_Text.Text = response.Message;
                      GradientDrawable reply = new GradientDrawable();
                      reply.SetCornerRadius(50);
                      response_Text.SetPadding(50, 30, 50, 30);
                      reply.SetColor(global::Android.Graphics.Color.ParseColor("#FCFCFC"));
                      response_Text.SetTextColor(global::Android.Graphics.Color.ParseColor("#333333"));
                      response_Text.Background = reply;
                      chat.AddView(response_Text);
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


