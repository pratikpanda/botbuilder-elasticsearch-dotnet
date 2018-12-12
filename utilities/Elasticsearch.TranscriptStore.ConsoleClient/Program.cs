using Bot.Builder.Elasticsearch.TranscriptStore.Manager;
using System;

namespace Elasticsearch.TranscriptStore.ConsoleClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            int result;
            string response;
            do
            {
                Console.Clear();
                Console.ResetColor();
                Console.WriteLine("Elasticsearch Transcript Store Manager");
                Console.WriteLine("--------------------------------------");
                Console.WriteLine();
                Console.WriteLine("Please choose from one of the following options [1|2|3]:");
                Console.WriteLine(" 1. Retrieve Conversation Transcript");
                Console.WriteLine(" 2. Delete Conversation Transcript");
                Console.WriteLine(" 3. Exit");
                Console.Write("Your choice: ");

                bool successfullyParsed = int.TryParse(Console.ReadLine(), out result);
                if (successfullyParsed)
                {
                    string channelId = string.Empty;
                    string conversationId = string.Empty;
                    var elasticsearchTranscriptStoreManager = new ElasticsearchTranscriptStoreManager();
                    switch (result)
                    {
                        case 1:
                            Console.Clear();
                            Console.WriteLine("Retrieve Conversation Transcript");
                            Console.WriteLine("--------------------------------");
                            Console.WriteLine();
                            Console.Write("Please provide the channel id: ");
                            channelId = Console.ReadLine();
                            Console.Write("Please provide the conversation id: ");
                            conversationId = Console.ReadLine();
                            var retrieveTranscriptResult = elasticsearchTranscriptStoreManager.RetrieveTranscriptAsync(channelId, conversationId).Result;
                            if (retrieveTranscriptResult.Retrieved)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(retrieveTranscriptResult.Message);

                                var generateTranscriptFileResult = elasticsearchTranscriptStoreManager.GenerateTranscriptFileAsync(retrieveTranscriptResult.Transcript).Result;
                                if (generateTranscriptFileResult.Generated)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.WriteLine(generateTranscriptFileResult.Message);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine(generateTranscriptFileResult.Message);
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(retrieveTranscriptResult.Message);
                            }
                            break;
                        case 2:
                            Console.Clear();
                            Console.WriteLine("Delete Conversation Transcript");
                            Console.WriteLine("------------------------------");
                            Console.WriteLine();
                            Console.Write("Please provide the channel id: ");
                            channelId = Console.ReadLine();
                            Console.Write("Please provide the conversation id: ");
                            conversationId = Console.ReadLine();
                            var deleteTranscriptResult = elasticsearchTranscriptStoreManager.DeleteTranscriptAsync(channelId, conversationId).Result;
                            if (deleteTranscriptResult.Deleted)
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine(deleteTranscriptResult.Message);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(deleteTranscriptResult.Message);
                            }
                            break;
                        case 3:
                            Console.Clear();
                            return;
                        default:
                            Console.Clear();
                            Console.WriteLine("You have selected an invalid option!");
                            break;
                    }
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("You have selected an invalid option!");
                }

                Console.ResetColor();
                Console.Write("Do you want to try again [y|n]: ");
                response = Console.ReadLine().ToLower();
            }
            while ((response == "y" || response == "yes") && result != 3);
        }
    }
}
