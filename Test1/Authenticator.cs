using Amazon.Lex;
using Amazon.Lex.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Test1
{
    public class Authenticator
    {
       
        public async static Task<byte[]> Main(byte [] args)
        {
            string accesskeyid = "AKIAJHYYM27C3VEXKFNQ";
            string secretaccesskeyid = "ywh9ceZAbu5APUWtatH7LLq44MZ2c932dVUGfLlY";
            var region = Amazon.RegionEndpoint.USEast1;
            var lex_client = new AmazonLexClient(accesskeyid, secretaccesskeyid, region);
            var byteArray = args;
            Stream stream = new MemoryStream(byteArray);
            try
            {
                // PostContentRequest request = new PostContentRequest();
                PostTextRequest request = new PostTextRequest();
                var response = new PostContentResponse();
                request.BotAlias = "$LATEST";
                request.BotName = "OrderFlowers";
                request.UserId = "geethu95";
                request.InputText = "i would like to order some flowers";
               // request.Accept = "audio/mpeg";
              //  request.ContentType = "audio/l16; rate=16000; channels=1";
              //  request.InputStream = stream;
               // response = await lex_client.PostContentAsync(request);
               var resp= await lex_client.PostTextAsync(request);
                var response_bytes = ConvertStreamToBytes(response.AudioStream);
                return response_bytes;

            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
        //method to convert audio stream to bytes
        public static byte[]  ConvertStreamToBytes(Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            for (int totalBytesCopied = 0; totalBytesCopied < stream.Length;)
                totalBytesCopied += stream.Read(buffer, totalBytesCopied, Convert.ToInt32(stream.Length) - totalBytesCopied);
            return buffer;
        
    }

    }
}

