using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlotOnMap.Models;
using Newtonsoft;

namespace PlotOnMap.Controllers
{
    public class MappingController : Controller
    {
        public IActionResult Index(HomeModel model)
        {
            MappingModel mapModel = new MappingModel();
            var circumferencePoints = GetList(model.latitude, model.longitude, model.distance);
            Coordinates coordinates = new Coordinates();
            coordinates.lat = model.latitude;
            coordinates.lng = model.longitude;
            var populatedCoordinates = PopulatedCoordinates(circumferencePoints,false, coordinates);
            mapModel.CordinateList = Newtonsoft.Json.JsonConvert.SerializeObject(populatedCoordinates);
            return View(mapModel);
        }

        public List<Coordinates> GetList(double lati, double longi,int distance)
        {
            List<Coordinates> list = new List<Coordinates>();
            var radiusEarth = 3959.8728;
            var adjustdist = distance / radiusEarth;
            for (double i = 0; i <= 359; )
            {
                Coordinates cord = new Coordinates();
                var lat2 = Math.Asin(Math.Sin(ConvertToRadians(lati)) * Math.Cos(adjustdist) + Math.Cos(ConvertToRadians(lati)) * Math.Sin(adjustdist) * Math.Cos(ConvertToRadians(i)));
                var lon2 = ConvertToRadians(longi) + Math.Atan2(Math.Sin(ConvertToRadians(i)) * Math.Sin(adjustdist) * Math.Cos(ConvertToRadians(lati)), Math.Cos(adjustdist) - Math.Sin(ConvertToRadians(lati)) * Math.Sin(ConvertToRadians(lat2)));
                var dLon = longi - lon2;
                var y = Math.Sin(dLon) * Math.Cos(lati);
                var x = Math.Cos(lat2) * Math.Sin(lati) - Math.Sin(lat2) * Math.Cos(lati) * Math.Cos(dLon);
                var d = Math.Atan2(y, x);
                var finalBrg = d + Math.PI;
                var backBrg = d + 2 * Math.PI;
                cord.lat = ConvertToDegree(lat2);
                cord.lng = ConvertToDegree(lon2);
                i += 1.5;
                list.Add(cord);
            }
            return list;
        }

        public List<Coordinates> PopulatedCoordinates(List<Coordinates> list,bool east,Coordinates center)
        {
            List<Coordinates> populatedList = new List<Coordinates>();
            list.RemoveAt((list.Count) / 2 - 1);
            list.RemoveAt(0);
            var eastList = list.Take(list.Count / 2).ToList();
            var westList = list.Skip(list.Count / 2).Reverse().ToList();
            for(var i = 0; i < eastList.Count; i++)
            {
                var eastLong = eastList[i].lng;
                var westLong = westList[i].lng;
                populatedList.AddRange(GenerateLongList(eastLong, westLong, eastList[i].lat,east,center));
            }
            return populatedList;
        }

        public List<Coordinates> GenerateLongList(double eastLong,double westLong, double lat,bool east,Coordinates center)
        {
            List<Coordinates> longList = new List<Coordinates>();
            var differenceDegree = eastLong - westLong;
            var distance = DistanceBetweenCoordinates(eastLong, westLong, lat);
            var eachDegree = differenceDegree / distance;
            double calculatingLongDegree = eastLong;
            while(calculatingLongDegree > westLong)
            {
                if (east)
                {
                    if (!(calculatingLongDegree > center.lng))
                    {
                        calculatingLongDegree -= 0.2;
                        continue;
                    }
                }
                else
                {
                    if (!(calculatingLongDegree < center.lng))
                    {
                        calculatingLongDegree -= 0.2;
                        continue;
                    }
                }
                Coordinates cor = new Coordinates();
                cor.lat = lat;
                cor.lng = calculatingLongDegree;
                calculatingLongDegree -= 0.2;
                longList.Add(cor);
            }
            return longList;
        }

        public double DistanceBetweenCoordinates(double lon1,double lon2, double lat)
        {
            double RADIUS = 3959.8728;
           
            double dlon = ConvertToRadians(lon2 - lon1);
            double dlat = ConvertToRadians(lat - lat);

            double a = (Math.Sin(dlat / 2) * Math.Sin(dlat / 2)) + Math.Cos(ConvertToRadians(lat)) * Math.Cos(ConvertToRadians(lat)) * (Math.Sin(dlon / 2) * Math.Sin(dlon / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return angle * RADIUS;
        }

        public double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public double ConvertToDegree(double angle)
        {
            return angle * (180 / Math.PI);
        }
    }

    public class Coordinates
    {
        public double lat { get; set; }
        public double lng { get; set; }

    }
}



//var radLati = ConvertToRadians(lati);
//var radLongi = ConvertToRadians(longi);

//Coordinates coordinates = new Coordinates();
//var degreeInRadian = ConvertToRadians(i);
//double distanceInRadian = distance / 3959.00;
//var lat2InRad = Math.Asin(Math.Sin(radLati) * Math.Cos(distanceInRadian) + Math.Cos(radLati) * Math.Sin(distanceInRadian) * Math.Cos(degreeInRadian));
//var long2InRad = radLongi + Math.Atan2(Math.Sin(degreeInRadian) * Math.Sin(distanceInRadian) * Math.Cos(radLati), Math.Cos(distanceInRadian) - Math.Sin(radLati) * Math.Sin(coordinates.lat));
//coordinates.lat = ConvertToDegree(lat2InRad);
//coordinates.lng = ConvertToDegree(long2InRad);
//list.Add(coordinates);