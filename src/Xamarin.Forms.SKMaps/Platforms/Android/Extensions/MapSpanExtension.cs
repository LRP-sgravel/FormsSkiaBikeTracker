﻿// **********************************************************************
// 
//   MapSpanExtension.cs
//   
//   This file is subject to the terms and conditions defined in
//   file 'LICENSE.txt', which is part of this source code package.
//   
//   Copyright (c) 2017, Sylvain Gravel
// 
// ***********************************************************************

using Android.Gms.Maps.Model;
using Xamarin.Forms.Maps;
using Xamarin.Forms.SKMaps.Extensions;
using Xamarin.Forms.SKMaps.Models;

namespace Xamarin.Forms.SKMaps.Platforms.Droid.Extensions
{
    internal static class MapSpanExtension
    {
        public static LatLngBounds ToLatLng(this MapSpan self)
        {
            return new SKMapSpan(self).ToLatLng();
        }

        public static LatLngBounds ToLatLng(this SKMapSpan self)
        {
            LatLngBounds.Builder builder = new LatLngBounds.Builder().Include(self.BottomLeft.ToLatLng())
                                                                     .Include(self.TopRight.ToLatLng());
            return builder.Build();
        }

        public static SKMapSpan ToMapSpan(this LatLngBounds self)
        {
            double latSpan = self.Northeast.Latitude - self.Southwest.Latitude;
            double longSpan = self.Northeast.Longitude - self.Southwest.Longitude;

            return new SKMapSpan(self.Center.ToPosition(), latSpan * 0.5, longSpan * 0.5);
        }

        public static Rectangle ToRectangle(this LatLngBounds self)
        {
            return self.ToMapSpan().ToMercator();
        }

        public static LatLngBounds ToLatLng(this Rectangle self)
        {
            SKMapSpan mapSpan = self.ToGps();

            return mapSpan.ToLatLng();
        }
    }
}
