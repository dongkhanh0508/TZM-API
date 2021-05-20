using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace TradeMap.Service.Helpers
{
    public static class GeoJsonHelper
    {
        public static JObject FormatToGeoJson(Geometry geometry)
        {
            string geoJson;
            var serializer = GeoJsonSerializer.Create();
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                serializer.Serialize(jsonWriter, geometry);
                geoJson = stringWriter.ToString();
            }

            var Result = JObject.Parse(geoJson);
            return Result;
        }

        public static Geometry ParseStringToGeoMetry(String CoordinateString)
        {
            String WKT = "MULTIPOLYGON (((" + CoordinateString + ")))";
            // Create a Well Known Text Reader from NetTopologySuite
            WKTReader Reader = new WKTReader();
            // NetTopologySuite passes back a GeoApi IGeometry.  This is a shared interface that can be used by both libraries.
            Geometry Geom = Reader.Read(WKT);
            Geom.SRID = 4326;
            return Geom;
        }
        public static Geometry ParseStringToGeoMetryWithWKT(String WKT)
        {

            // Create a Well Known Text Reader from NetTopologySuite
            WKTReader Reader = new WKTReader();
            // NetTopologySuite passes back a GeoApi IGeometry.  This is a shared interface that can be used by both libraries.
            Geometry Geom = Reader.Read(WKT);
            Geom.SRID = 4326;
            return Geom;
        }

        public static Geometry ParseStringToPoint(String CoordinateString)
        {
            String WKT = "Point (" + CoordinateString + ")";
            // Create a Well Known Text Reader from NetTopologySuite
            WKTReader Reader = new WKTReader();
            // NetTopologySuite passes back a GeoApi IGeometry.  This is a shared interface that can be used by both libraries.
            Geometry Geom = Reader.Read(WKT);
            Geom.SRID = 4326;
            return Geom;
        }

        public static Geometry CombineGeoCollection(List<Geometry> geos)
        {
            Geometry result = NetTopologySuite.Operation.Union.UnaryUnionOp.Union(geos);
            return result;
        }

        public static Geometry ParseStringToLineString(String CoordinateString)
        {
            String WKT = "LINESTRING  (" + CoordinateString + ")";
            // Create a Well Known Text Reader from NetTopologySuite
            WKTReader Reader = new WKTReader();
            // NetTopologySuite passes back a GeoApi IGeometry.  This is a shared interface that can be used by both libraries.
            Geometry Geom = Reader.Read(WKT);
            Geom.SRID = 4326;
            return Geom;
        }
    }
}