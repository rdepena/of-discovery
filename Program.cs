using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Openfin.Desktop;

namespace of_discovery
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var runtimeOptions = new RuntimeOptions
            {
                Version = "whatever",
                RuntimeConnectOptions = RuntimeConnectOptions.UseExternal,
                Port = 9696,
                PortDiscoveryMode = PortDiscoveryMode.None
            };

            var runtime = Runtime.GetRuntimeInstance(runtimeOptions);
            //make sure we log incomming/outgoing messages
            SetMessageLoging(runtime);

            //Connect to the runtime.
            runtime.Connect(()=> 
            {
                //Just comment/uncomment functions to observe.
                //Subscribe(runtime);
                //Unsubscribe(runtime);
                //Send(runtime);
                //SubscribeListener(runtime);
            });
            while(true) 
            {
                ;
            }
        }
        private static void Subscribe(Runtime runtime)
        {
            runtime.InterApplicationBus.subscribe(runtime.Options.UUID, "uuid-topic", InterAppBusListener);
            runtime.InterApplicationBus.subscribe("*", "hello:of:notification", InterAppBusListener);
            runtime.InterApplicationBus.subscribe("some-fake-uuid", "fake-uuid-topic", InterAppBusListener);
        }
        private static void Unsubscribe(Runtime runtime)
        {
            runtime.InterApplicationBus.unsubscribe(runtime.Options.UUID, "uuid-topic", InterAppBusListener);
            runtime.InterApplicationBus.unsubscribe("*", "star-topic", InterAppBusListener);
            runtime.InterApplicationBus.unsubscribe("some-fake-uuid", "fake-uuid-topic", InterAppBusListener);
        }
        private static void Send(Runtime runtime)
        {
            var Data = new { stuff = "Stuff" };
            runtime.InterApplicationBus.send(runtime.Options.UUID, "fake-topic", JObject.FromObject(Data));            
        }
        private static void SubscribeListener(Runtime runtime)
        {
            runtime.InterApplicationBus.addSubscribeListener((uuid, topic) =>
            {
                Console.WriteLine($"someone subscribed to {uuid}, {topic}");
            });
        }
        private static void UnsubscribeListener(Runtime runtime)
        {
            runtime.InterApplicationBus.addUnsubscribeListener((uuid, topic) =>
            {
                Console.WriteLine($"someone unsubscribe to {uuid}, {topic}");
            });
        }
        private static void SetMessageLoging(Runtime runtime)
        {
            runtime.OnIncommingMessage += (sender, e) =>
            {
                Console.WriteLine("OnIncommingMessage");
                Console.WriteLine(e.MessageContent);
            };

            runtime.OnOutgoingMessage += (sender, e) =>
            {
                Console.WriteLine("OnOutgoingMessage");
                Console.WriteLine(e.MessageContent);
            };
        }
        public static void FocusedEventListener(Ack ack)
        {
            Console.WriteLine("Focused");
        }

        public static void InterAppBusListener(string sender, string topic, object data)
        {
            Console.WriteLine($"Got {data} from {sender} on {topic}");
        }
    }
}
