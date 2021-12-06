using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Intersection {

    private readonly object intersection = new object();
    private Queue<object> eastWestQueue = new Queue<object>();
    private Queue<object> northSouthQueue = new Queue<object>();
    public bool SafeToGoEastWest { get; private set; }
    public bool SafeToGoNorthSouth { get; private set; }
    public int QueueSize {
        get {
            return eastWestQueue.Count + northSouthQueue.Count;
        }
    }

    public Intersection(int n) {
        SafeToGoEastWest = true;
        SafeToGoNorthSouth = false;
        Random rnd = new Random();
        for (int i = 0; i < n; i++) {
            switch (rnd.Next(0, 2)) {
                case 0:
                    eastWestQueue.Enqueue(new object());
                    break;
                case 1:
                    northSouthQueue.Enqueue(new object());
                    break;
            }
        }
    }

    public void StartTrafficSignal() {
        Task.Run(() => {
            while (true) {
                SafeToGoEastWest = SafeToGoNorthSouth;
                SafeToGoNorthSouth = !SafeToGoEastWest;
                Thread.Sleep(500);
            }
        });
    }

    public void TravelEastWest() {
        lock (intersection) {
            if (SafeToGoEastWest && eastWestQueue.Count != 0) {
                Thread.Sleep(10);
                eastWestQueue.Dequeue();
            }
        }
    }

    public void TravelNorthSouth() {
        lock (intersection) {
            if (SafeToGoNorthSouth && northSouthQueue.Count != 0) {
                Thread.Sleep(10);
                northSouthQueue.Dequeue();
            }
        }
    }
}