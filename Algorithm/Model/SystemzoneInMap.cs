using NetTopologySuite.Geometries;

namespace CollectSystemzone.Model
{
    public class SystemzoneInMap
    {
        public int Id { get; set; }
        public Geometry Geom { get; set; }

        public string Name { get; set; }

        public int MyProperty { get; set; }

        public double WeightNumber { get; set; }

    }
}