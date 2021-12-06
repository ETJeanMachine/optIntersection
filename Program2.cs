using System.Threading.Tasks;

public class Program2 {
    public static void Main(string[] args) {
        Intersection intersection = new Intersection(1000);
        while (intersection.QueueSize != 0) {
            if(intersection.SafeToGoEastWest) {
                Task.Run(intersection.TravelEastWest);
            } else {
                Task.Run(intersection.TravelNorthSouth);
            }
        }
    }
}