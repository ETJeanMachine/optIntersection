using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace optIntersection {
    class Program {

        static readonly object intersection = new object();
        static readonly object SafeToGoEastWest = new object();
        static readonly object SafeToGoNorthSouth = new object();
        static Queue<int> carQueue = new Queue<int>();
        static List<int> readyToGo = new List<int>();
        static Stopwatch stopWatch = new Stopwatch();
        const int EastWest = 0;
        const int NorthSouth = 1;

        /// <summary>
        /// This generates an integer queue of size n with either 1's or 0's;
        /// with 0 signifying a Easterly/Westerly bound car and 1 signifying a
        /// Northerly/Southerly bound car.
        /// </summary>
        /// <param name="n">The size of the queue we are generating.</param>
        private static void GenerateQueue(int n) {
            Random rnd = new Random();
            // Generating our random queue.
            for (int i = 0; i < n; i++)
                carQueue.Enqueue(rnd.Next(0, 2));
        }

        /// <summary>
        /// This method loops indefinitely, changing the light signal for
        /// east/west and north/south every 500ms. It pulses all waiting cars to
        /// go when the signal changes.
        /// </summary>
        public static void StartTrafficSignal() {
            bool signal = Convert.ToBoolean(NorthSouth);
            while (true) {
                Thread.Sleep(500);
                if (signal)
                    lock (SafeToGoNorthSouth) {
                        Console.WriteLine("It's now safe to go North/South!");
                        Monitor.PulseAll(SafeToGoNorthSouth);
                    }
                else
                    lock (SafeToGoEastWest) {
                        Console.WriteLine("It's now safe to go East/West!");
                        Monitor.PulseAll(SafeToGoEastWest);
                    }
                signal = !signal;
            }
        }


        /// <summary>
        /// This method allows, once the intersection is clear, and the light
        /// signals it is safe to travel East/West, a car to pass through the
        /// intersection, which takes 10ms.
        /// </summary>
        public static void TravelEastWest() {
            lock (SafeToGoEastWest) {
                Monitor.Wait(SafeToGoEastWest);
                // Locking off any other cars from entering the intersection.
                lock (intersection) {
                    Thread.Sleep(10);
                    Console.WriteLine($"Traveled East/West! {stopWatch.ElapsedMilliseconds}");
                    readyToGo.Remove(EastWest);
                }
            }
        }

        /// <summary>
        /// This method allows, once the intersection is clear, and the light
        /// signals it is safe to travel North/South, a car to pass through the
        /// intersection, which takes 10ms.
        /// </summary>
        public static void TravelNorthSouth() {
            lock (SafeToGoNorthSouth) {
                Monitor.Wait(SafeToGoNorthSouth);
                // Locking off any other cars from entering the intersection.
                lock (intersection) {
                    Thread.Sleep(10);
                    Console.WriteLine($"Traveled North/South! {stopWatch.ElapsedMilliseconds}");
                    readyToGo.Remove(NorthSouth);
                }
            }
        }

        /// <summary>
        /// Main method of the program. Goes through a list of randomly
        /// generated cars, and depending on their value, it assigns them to run
        /// either TravelEastWest or TravelNorthSouth. Once done, it waits for
        /// all tasks to finish, then exits out.
        /// </summary>
        /// <param name="args">Command line arguments; unused.</param>
        public static void Main2(string[] args) {
            GenerateQueue(1000);
            Task.Run(StartTrafficSignal);
            List<Task> carsToGo = new List<Task>();
            stopWatch.Start();
            while (carQueue.Count != 0) {
                int car = carQueue.Peek();
                switch (car) {
                    case EastWest:
                        carsToGo.Add(Task.Run(() => TravelEastWest()));
                        readyToGo.Add(EastWest);
                        break;
                    case NorthSouth:
                        carsToGo.Add(Task.Run(() => TravelNorthSouth()));
                        readyToGo.Add(NorthSouth);
                        break;
                }
            }
            Task.WaitAll(carsToGo.ToArray());
        }
    }
}