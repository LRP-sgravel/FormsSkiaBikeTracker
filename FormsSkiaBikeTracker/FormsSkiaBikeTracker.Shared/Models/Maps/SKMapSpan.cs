﻿// **********************************************************************
// 
//   SKMapPosition.cs
//   
//   This file is subject to the terms and conditions defined in
//   file 'LICENSE.txt', which is part of this source code package.
//   
//   Copyright (c) 2017, Sylvain Gravel
// 
// ***********************************************************************

using System;
using FormsSkiaBikeTracker.Shared.Helpers;
using Xamarin.Forms.Maps;

namespace FormsSkiaBikeTracker.Shared.Models.Maps
{
    public class SKMapSpan
    {
        // Just like the Xamarin Forms Maps MapSpan, but it doesn't clamp at the 180th meridian ands has a smaller minimum degrees
        public SKMapPosition Center { get; }

        public double LatitudeDegrees { get; }
        public double LongitudeDegrees { get; }

        public SKMapSpan(MapSpan mapSpan) : this(new SKMapPosition(mapSpan.Center),
                                                 mapSpan.LatitudeDegrees,
                                                 mapSpan.LongitudeDegrees)
        {
        }

        public SKMapSpan(Position center, double latitudeDegrees, double longitudeDegrees) : this(new SKMapPosition(center),
                                                                                                  latitudeDegrees,
                                                                                                  longitudeDegrees)
        {
        }

        public SKMapSpan(SKMapPosition center, double latitudeDegrees, double longitudeDegrees)
        {
            Center = center;
            LatitudeDegrees = Math.Min(MapSpanExtensions.MaxLatitude, Math.Abs(latitudeDegrees));
            LongitudeDegrees = Math.Min(MapSpanExtensions.MaxLongitude, Math.Abs(longitudeDegrees));
        }

        public MapSpan ToMapSpan()
        {
            return new MapSpan(Center.ToPosition(), LatitudeDegrees, LongitudeDegrees);
        }

        public bool FastIntersects(SKMapSpan span)
        {
            return Math.Abs(Center.Latitude - span.Center.Latitude) < (LatitudeDegrees + span.LatitudeDegrees) ||
                   Math.Abs(Center.Longitude - span.Center.Longitude) < (LongitudeDegrees + span.LongitudeDegrees);

        }

        public bool FastIntersects(MapSpan span)
        {
            return FastIntersects(new SKMapSpan(span));
        }
    }
}
