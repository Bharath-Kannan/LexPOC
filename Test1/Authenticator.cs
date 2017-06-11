using Amazon.Lex;
using Amazon.Lex.Model;
using System;    
using System.IO;
using System.Threading.Tasks;
using LexPOC;

namespace LexPOC
{
    public class AWSLexConnector
    {
       
        public async static Task<PostContentResponse> Main(byte [] args)
        {
            #region variables

            string accesskeyid = "";
            string secretaccesskeyid = "";
            var region = Amazon.RegionEndpoint.USEast1;
            var lex_client = new AmazonLexClient(accesskeyid, secretaccesskeyid, region);
            var byteArray = args;
            Stream stream = new MemoryStream(byteArray);
            #endregion

            try
            {
                PostContentRequest request = new PostContentRequest();
                var response = new PostContentResponse();
                request.BotAlias = "$LATEST";
                request.BotName = "OrderCake";
                request.UserId = "geethu95";
                request.Accept = "audio/mpeg";
                request.ContentType = "audio/l16; rate=16000; channels=1";
                request.InputStream = stream;
                response = await lex_client.PostContentAsync(request);
                return response;

            }
            catch (Exception ex)
            {
                
                throw;
            }
           
        }
        //Method to convert audio stream to bytes
        public static byte[]  ConvertStreamToBytes(Stream stream)
        {
            try
            {
                byte[] buffer = new byte[stream.Length];
                    for (int totalBytesCopied = 0; totalBytesCopied < (int)stream.Length;)
                        totalBytesCopied += stream.Read(buffer, totalBytesCopied, (int)(stream.Length) - totalBytesCopied);
                    return buffer;
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        
    }

    }
}

