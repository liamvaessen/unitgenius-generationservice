using Newtonsoft.Json;
using System;
using System.Threading;
using UnitGenius_GenerationService.Model;
using UnitGenius_GenerationService.Service;
using Microsoft.Extensions.Configuration;




class Program
{
    static void Main(string[] args)
    {
        // Set up configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        IConfiguration configuration = builder.Build();

        

        #region Running and Shutdown handling
        // Create a ManualResetEvent
        var resetEvent = new ManualResetEvent(false);

        // Register for the ProcessExit event
        AppDomain.CurrentDomain.ProcessExit += (s, e) =>
        {
            Console.WriteLine("Graceful shutdown...");
            resetEvent.Set();
        };

        // Register for the CancelKeyPress event
        Console.CancelKeyPress += (s, e) =>
        {
            Console.WriteLine("Graceful shutdown...");
            resetEvent.Set();

            // Prevent the process from terminating immediately
            e.Cancel = true;
        };
        #endregion Running and Shutdown handling

        

        #region Instantiation of Services
        // Create a new instances of the Services
        while (true)
        {
            try
            {
                Console.WriteLine("Trying to instantiate services...");
                string apikey = configuration["OpenAI_ApiKey"] ?? throw new ArgumentNullException("OpenAI_ApiKey");
                // Create a new instance of the UnitTestGenerationService
                var unitTestGenerationService = new UnitTestGenerationService(apikey);

                string rabbitmqHost = configuration["RABBITMQ_HOST"] ?? throw new ArgumentNullException("RABBITMQ_HOST");
                int rabbitmqPort = int.TryParse(configuration["RABBITMQ_PORT"], out int port) ? port : 5672; // default RabbitMQ port
                string rabbitmqUser = configuration["RABBITMQ_USERNAME"] ?? throw new ArgumentNullException("RABBITMQ_USERNAME");
                string rabbitmqPass = configuration["RABBITMQ_PASSWORD"] ?? throw new ArgumentNullException("RABBITMQ_PASSWORD");
                string rabbitmqVhost = configuration["RABBITMQ_VHOST"] ?? "/"; // default RabbitMQ virtual host

                

                // Create a new instance of the MessageReceiverHelper
                var requestedMessageReceiver = new MessageReceiverHelper(
                    callback: async message =>
                    {
                        Console.WriteLine("Received message...");

                        try
                        {
                            // Deserialize the received message into a GenerationRequest object
                            GenerationRequest request = JsonConvert.DeserializeObject<GenerationRequest>(message);

                            // Generate the unit test and add to result of the request
                            request = await unitTestGenerationService.GenerateAsync(request);
                            

                            // Serialize the request object into a JSON string
                            string completedMessage = JsonConvert.SerializeObject(request);

                            // Log the completion of the request
                            Console.WriteLine($"Request {request.RequestId} has been completed.");
                            Console.WriteLine($"Result: {request.Result}");

                            // Create a new instance of the MessagePublisherHelper
                            var completedMessagePublisher = new MessagePublisherHelper(
                                queueName: "generationRequests_Completed",
                                hostName: rabbitmqHost,
                                hostPort: rabbitmqPort,
                                hostUsername: rabbitmqUser,
                                hostPassword: rabbitmqPass,
                                virtualHost: rabbitmqVhost);

                            // Publish the completed message to the RabbitMQ queue
                            completedMessagePublisher.PublishMessage(completedMessage);

                            Console.WriteLine("Published message...");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error during processing of message: {ex.Message}");
                        }
                    },
                    queueName: "generationRequests_Requested",
                    hostName: rabbitmqHost,
                    hostPort: rabbitmqPort,
                    hostUsername: rabbitmqUser,
                    hostPassword: rabbitmqPass,
                    virtualHost: rabbitmqVhost);

                Console.WriteLine("Services instantiated successfully.");

                while (true)
                {
                    // Block the main thread until the application is about to shut down
                    resetEvent.WaitOne();

                    Console.WriteLine("Running...");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during instantiation of services: {ex.Message}");
                Console.WriteLine("Retrying...");
                if (resetEvent.WaitOne(500))  // Wait for 5 seconds before retrying, or until the application is about to shut down
                {
                    Console.WriteLine("Shutdown signal received. Exiting...");
                    return;  // If the application is about to shut down, exit the program
                }
            }
        }
        #endregion Instantiation of Services

        // Block the main thread until the application is about to shut down
        resetEvent.WaitOne();
    }
}