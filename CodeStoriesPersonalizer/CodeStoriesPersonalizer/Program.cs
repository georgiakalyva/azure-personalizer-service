using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeStoriesPersonalizer
{
    class Program
    {
        // The key specific to your personalizer resource instance; e.g. "0123456789abcdef0123456789ABCDEF"
        private static readonly string ApiKey = "your key";


        private static readonly string ServiceEndpoint = "your endpoint";
        static void Main(string[] args)
        {
            //SimulateRandomUserPreferences();
            int iteration = 1;
            bool runLoop = true;

            // Get the actions list to choose from personalizer with their features.
            IList<RankableAction> actions = GetActions();

            // Initialize Personalizer client.
            PersonalizerClient client = InitializePersonalizerClient(ServiceEndpoint);

            do
            {
                Console.WriteLine("\nIteration: " + iteration++);

                // Get context information from the user.
                string userProfession = GetUserProfession();
                string UserPreference = GetUsersPreference();

                // Create current context from user specified data.
                IList<object> currentContext = new List<object>() {
                new { professiom = userProfession },
                new { preference = UserPreference }
                };

                // Exclude an action for personalizer ranking. This action will be held at its current position.
                // This simulates a business rule to force the action "juice" to be ignored in the ranking.
                // As juice is excluded, the return of the API will always be with a probability of 0.
                IList<string> excludeActions = new List<string> { "http://codestories.gr/index.php/2018/10/21/297/" };

                // Generate an ID to associate with the request.
                string eventId = Guid.NewGuid().ToString();

                // Rank the actions
                var request = new RankRequest(actions, currentContext, excludeActions, eventId);
                RankResponse response = client.Rank(request);

                Console.WriteLine("\nPersonalizer service thinks you should read this: " + response.RewardActionId + ". Is this correct? (y/n)");

                float reward = 0.0f;
                string answer = GetKey();

                if (answer == "Y")
                {
                    reward = 1;
                    Console.WriteLine("\nGreat! Enjoy this article.");
                }
                else if (answer == "N")
                {
                    reward = 0;
                    Console.WriteLine("\nSorry, but try again we can do better!");
                }
                else
                {
                    Console.WriteLine("\nEntered choice is invalid. Service assumes that you didn't like the article.");
                }

                Console.WriteLine("\nPersonalizer service ranked the actions with the probabilities as below:");
                foreach (var rankedResponse in response.Ranking)
                {
                    Console.WriteLine(rankedResponse.Id + " " + rankedResponse.Probability);
                }

                // Send the reward for the action based on user response.
                client.Reward(response.EventId, new RewardRequest(reward));

                Console.WriteLine("\nPress q to break, any other key to continue:");
                runLoop = !(GetKey() == "Q");

            } while (runLoop);
        }

        static PersonalizerClient InitializePersonalizerClient(string url)
        {
            PersonalizerClient client = new PersonalizerClient(
                new ApiKeyServiceClientCredentials(ApiKey))
            { Endpoint = url };

            return client;
        }

        static IList<RankableAction> GetActions()
        {
            IList<RankableAction> actions = new List<RankableAction>
        {
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/03/04/web-role-vs-worker-role-azure/", Features =  new List<object>() { new { category = "Azure", words =  316 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/03/20/free-ebook-azure-strategy-implementation/", Features =  new List<object>() { new { category = "eBook", words =  87  } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/04/10/azure-storage-replication/", Features =  new List<object>() { new { category = "Azure", words =  427 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/04/22/azure-ad-b2c-series-part-1-registering-a-b2c-app/", Features =  new List<object>() { new { category = "Azure", words =  219 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/04/25/video-tutorial-how-to-create-a-wordpress-site-on-azure/", Features =  new List<object>() { new { category = "Video Tutorial", words =  40  } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/04/26/move-azure-resources-from-one-subscription-resource-group-to-another/", Features =  new List<object>() { new { category = "Azure", words =  188 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/05/05/gdpr-requirements-sql-platform/", Features =  new List<object>() { new { category = "GDPR", words =  73  } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/05/13/ngrok-public-urls-for-local-web-server/", Features =  new List<object>() { new { category = "Tools", words =  273 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/05/13/backup-all-sql-server-databases-at-once/", Features =  new List<object>() { new { category = "SQL Server", words =  205 } }},
            /*new RankableAction   { Id = "http://codestories.gr/index.php/2018/05/20/getting-started-with-luis-ai/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  901 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/05/23/authentication-with-azure-ad-b2c-on-mobile-workshop/", Features =  new List<object>() { new { category = "Azure", words =  117 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/05/24/replacing-database-table-rows-with-incremental-values/", Features =  new List<object>() { new { category = "SQL Server", words =  123 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/05/30/azure-serverless-computing-cookbook/", Features =  new List<object>() { new { category = "Azure", words =  73  } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/06/20/building-cloud-native-applications-with-node-js-and-azure-ebook/", Features =  new List<object>() { new { category = "Azure", words =  48  } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/09/02/cognitive-services-qna-maker/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  1393    } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/09/06/cognitive-services-bing-web-search/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  655 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/09/11/cognitive-services-bing-autosuggest-api/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  505 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/09/15/cognitive-services-text-to-speech/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  2327    } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/09/16/cognitive-services-speech-to-text/", Features =  new List<object>() { new { category = "Speech", words =  534 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/09/26/how-to-generate-realistic-data-for-your-application/", Features =  new List<object>() { new { category = "Tools", words =  124 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/09/28/how-to-schedule-a-database-backup-operation-in-sql-server/", Features =  new List<object>() { new { category = "SQL Server", words =  292 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/10/03/test-the-office365-smtp-relay-service-powershell/", Features =  new List<object>() { new { category = "Office365", words =  148 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/10/05/set-up-remote-desktop-access-rdp-on-a-linux-vm-on-azure/", Features =  new List<object>() { new { category = "Azure", words =  423 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/10/07/dockerize-an-asp-net-core-application/", Features =  new List<object>() { new { category = "Docker", words =  256 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/10/11/codeweek-event-chatting-with-l-u-i-s/", Features =  new List<object>() { new { category = "Workshops", words =  223 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/10/13/cognitive-services-bing-visual-search/", Features =  new List<object>() { new { category = "Search", words =  1119    } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/10/14/cognitive-services-bing-video-search/", Features =  new List<object>() { new { category = "Search", words =  754 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/10/17/get-the-results-from-your-luis-endpoint/", Features =  new List<object>() { new { category = "Language", words =  438 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/10/21/297/", Features =  new List<object>() { new { category = "Search", words =  685 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/11/04/getting-started-with-azure-machine-learning-studio/", Features =  new List<object>() { new { category = "Machine Learning", words =  738 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/11/12/create-a-simple-movie-recommender-using-azure-ml/", Features =  new List<object>() { new { category = "Machine Learning", words =  1577    } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/11/15/publish-your-azure-ml-predictive-experiment/", Features =  new List<object>() { new { category = "Machine Learning", words =  340 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/11/15/infocom-world-2018-intelligent-business-apps/", Features =  new List<object>() { new { category = "Conferences", words =  149 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/11/18/microsoft-skypeathon-2018-demystifying-artificial-intelligence/", Features =  new List<object>() { new { category = "Video Tutorial", words =  56  } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2018/12/26/sentiment-analysis-on-twitter-posts-using-flow/", Features =  new List<object>() { new { category = "Language", words =  417 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/01/02/create-cognitive-services-key-for-all-apis-in-the-azure-portal/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  162 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/01/07/create-a-coffee-house-recommendation-service-using-azure-ml/", Features =  new List<object>() { new { category = "Machine Learning", words =  1053    } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/01/08/artificial-intelligence/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  1026    } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/01/13/video-tutorial-coffee-house-recommendation-service-with-azure-ml/", Features =  new List<object>() { new { category = "Machine Learning", words =  148 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/01/21/cognitive-services-bing-news-search/", Features =  new List<object>() { new { category = "Search", words =  923 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/01/26/bing-local-business-search/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  542 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/01/29/get-translator-text-api-supported-languages/", Features =  new List<object>() { new { category = "Language", words =  356 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/02/03/use-the-translator-text-api-to-translate-text/", Features =  new List<object>() { new { category = "Language", words =  368 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/02/08/transliterate-text-with-the-translator-text-api/", Features =  new List<object>() { new { category = "Language", words =  468 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/02/11/use-the-translator-api-to-detect-language/", Features =  new List<object>() { new { category = "Language", words =  351 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/02/18/use-the-translator-api-to-break-text-into-sentences/", Features =  new List<object>() { new { category = "Language", words =  348 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/02/21/my-gitignore-file-for-net-development/", Features =  new List<object>() { new { category = "Github", words =  664 } }},
            new RankableAction   { Id = "http://codestories.gr/index.php/2019/03/05/using-text-analytics-api-to-extract-keywords-sentiment-and-more/", Features =  new List<object>() { new { category = "Language", words =  827 } }},*/
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/03/09/use-the-content-moderator-to-find-objectionable-material/", Features =  new List<object>() { new { category = "Language", words =  1528    } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/03/11/tips-for-a-successful-technical-presentation/", Features =  new List<object>() { new { category = "Presentations", words =  789 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/03/14/use-the-content-moderator-to-find-objectionable-material-in-images/", Features =  new List<object>() { new { category = "Language", words =  1431    } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/03/17/generate-a-pfx-certificate-using-the-windows-certificate-store/", Features =  new List<object>() { new { category = "Security", words =  271 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/03/22/generate-a-pfx-certificate-using-openssl/", Features =  new List<object>() { new { category = "Security", words =  324 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/03/25/use-custom-vision-ai-to-recognize-brands/", Features =  new List<object>() { new { category = "Vision", words =  563 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/04/14/create-a-simple-bot-using-the-microsoft-bot-framework/", Features =  new List<object>() { new { category = "Bots", words =  558 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/04/29/create-a-knowledge-base-with-a-qna-maker/", Features =  new List<object>() { new { category = "Knowledge", words =  331 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/05/04/create-a-qna-bot/", Features =  new List<object>() { new { category = "Bots", words =  376 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/05/26/object-detection-using-custom-vision-ai/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  689 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/05/27/resistance-to-change/", Features =  new List<object>() { new { category = "Business", words =  284 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/06/04/build-a-sentiment-model-using-ml-net/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  854 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/06/26/create-a-healthcare-bot/", Features =  new List<object>() { new { category = "Bots", words =  355 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/07/28/face-detection-app-with-tracking-js/", Features =  new List<object>() { new { category = "Vision", words =  3322    } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/09/24/create-an-rss-reader-application/", Features =  new List<object>() { new { category = ".NET General", words =  232 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/09/26/net-core-3-released/", Features =  new List<object>() { new { category = "News", words =  292 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/10/01/add-your-bot-to-microsoft-teams-tabs-and-channels/", Features =  new List<object>() { new { category = "Bots", words =  278 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/10/02/introducing-immersive-reader-a-new-azure-cognitive-service/", Features =  new List<object>() { new { category = "Language", words =  179 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/10/03/create-a-daily-news-bot-using-rss-feeds/", Features =  new List<object>() { new { category = "Bots", words =  1250    } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/10/07/introducing-the-form-recognizer-document-insights-made-easy/", Features =  new List<object>() { new { category = "Vision", words =  416 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/10/10/azure-ai-comes-to-office365-business-applications/", Features =  new List<object>() { new { category = "Office365", words =  264 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/10/15/introducing-the-personalizer-personalized-experiences-made-easy/", Features =  new List<object>() { new { category = "Decision", words =  327 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/10/19/attending-now-at-work-london-2019/", Features =  new List<object>() { new { category = "Conferences", words =  251 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/10/20/introducing-ink-recognizer/", Features =  new List<object>() { new { category = "Vision", words =  297 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/11/01/microsoft-and-qualcomm-release-vision-ai-developer-kit/", Features =  new List<object>() { new { category = "Vision", words =  224 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/11/10/find-an-openhack-near-you/", Features =  new List<object>() { new { category = "News", words =  174 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/11/18/attending-the-web-summit-2019-experience/", Features =  new List<object>() { new { category = "Conferences", words =  847 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/11/20/real-time-speech-transcription-and-translation-has-never-been-easier/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  414 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/11/25/stay-on-top-of-your-ai-game-with-microsoft-ai-school/", Features =  new List<object>() { new { category = "News", words =  239 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2019/12/01/microsoft-bot-framework-4-6-new-features/", Features =  new List<object>() { new { category = "Bots", words =  203 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2020/01/30/transform-handwriting-to-text-part-1/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  1372    } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2020/02/05/transform-handwriting-to-text-with-the-azure-ink-recognizer-part-2/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  235 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2020/03/16/transform-handwriting-to-text-with-the-azure-ink-recognizer-part-3/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  850 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2020/03/23/embed-your-azure-bot-in-any-website/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  109 } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2020/04/04/create-a-chat-bot-from-a-knowledge-base-in-minutes-video/", Features =  new List<object>() { new { category = "Bots", words =  60  } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2020/04/04/create-your-own-healthcare-assistant-video/", Features =  new List<object>() { new { category = "Bots", words =  53  } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2020/04/12/introduction-to-ai-and-azure-cognitive-services/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  38  } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2020/05/08/create-your-own-healthcare-digital-assistant-without-code-video/", Features =  new List<object>() { new { category = "Bots", words =  87  } }},
            //new RankableAction   { Id = "http://codestories.gr/index.php/2020/05/08/introduction-to-ai-and-cognitive-services-video/", Features =  new List<object>() { new { category = "Artificial Intelligence", words =  110 } }},
    
            };

            return actions;
        }

        static void SimulateRandomUserPreferences() 
        {
            for (int i = 0; i < 1000; i++)
            {
                Random random = new Random(DateTime.Now.Millisecond);

                IList<RankableAction> actions = GetActions();

                PersonalizerClient client = InitializePersonalizerClient(ServiceEndpoint);

                IList<object> currentContext = new List<object>() {
                    new { professiom = random.Next(1, 5)    },
                    new { preference = random.Next(1, 3) }
                    };
                IList<string> excludeActions = new List<string> { "http://codestories.gr/index.php/2018/10/21/297/" };

                // Generate an ID to associate with the request.
                string eventId = Guid.NewGuid().ToString();

                var request = new RankRequest(actions, currentContext, excludeActions, eventId);
                RankResponse response = client.Rank(request);
                client.Reward(response.EventId, new RewardRequest(random.Next(0, 2)));
            }
        }
        static string GetUserProfession()
        {
            string[] UserProfession = new string[] { "Developer", "IT Pro", "Student", "Other" };

            Console.WriteLine("\nWhat is your profession (enter number)? 1. Developer 2. IT Pro 3. Student 4. Other");
            if (!int.TryParse(GetKey(), out int Index) || Index < 1 || Index > UserProfession.Length)
            {
                Console.WriteLine("\nEntered value is invalid. Setting feature value to " + UserProfession[4] + ".");
                Index = 1;
            }

            return UserProfession[Index - 1];
        }
            
        static string GetUsersPreference()
        {
            string[] Preference = new string[] { "Laptop stickers are cool", "Laptop stickers are unprofessional" };

            Console.WriteLine("\n Laptop Stickers are (enter number)? 1. cool 2. unprofessional");
            if (!int.TryParse(GetKey(), out int tasteIndex) || tasteIndex < 1 || tasteIndex > Preference.Length)
            {
                Console.WriteLine("\nEntered value is invalid. Setting feature value to " + Preference[0] + ".");
                tasteIndex = 1;
            }

            return Preference[tasteIndex - 1];
        }

        private static string GetKey()
        {
            return Console.ReadKey().Key.ToString().Last().ToString().ToUpper();
        }


    }
}
