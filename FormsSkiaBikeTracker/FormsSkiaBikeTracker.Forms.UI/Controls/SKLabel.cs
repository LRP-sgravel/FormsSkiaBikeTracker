﻿// **********************************************************************
// 
//   SKLabel.xaml.cs
//   
//   This file is subject to the terms and conditions defined in
//   file 'LICENSE.txt', which is part of this source code package.
//   
//   Copyright (c) 2017, Le rond-point
// 
// ***********************************************************************

using System;
using System.IO;
using LRPLib.Services.Resources;
using MvvmCross.Platform;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace FormsSkiaBikeTracker.Forms.UI.Controls
{
    public class SKLabel : SKCanvasView
    {
        public static readonly BindableProperty FontResourcePathProperty = BindableProperty.Create(nameof(FontResourcePath), typeof(string), typeof(SKLabel), string.Empty, BindingMode.OneWay, null, FontResourcePathPropertyChanged);
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(SKLabel), string.Empty, BindingMode.OneWay, null, ResizePropertyChanged);
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SKLabel), Color.Black, BindingMode.OneWay, null, InvalidatePropertyChanged);
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SKLabel), 14.0, BindingMode.OneWay, null, ResizePropertyChanged);

        public string FontResourcePath
        {
            get { return (string)GetValue(FontResourcePathProperty); }
            set { SetValue(FontResourcePathProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        private SKPaint _Paint { get; set; }
        private SKTypeface _Typeface { get; set; }

        public SKLabel()
        {
            _Paint = new SKPaint();
        }

        ~SKLabel()
        {
            _Typeface?.Dispose();
            _Paint?.Dispose();
        }

        private static void FontResourcePathPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            SKLabel view = bindable as SKLabel;
            IResourceLocator resLocator = Mvx.Resolve<IResourceLocator>();
            string resourcePath = resLocator.GetResourcePath("Fonts", view.FontResourcePath);
            Stream resStream = resLocator.ResourcesAssembly.GetManifestResourceStream(resourcePath);

            view._Typeface?.Dispose();

            if (resStream != null)
            {
                using (resStream)
                {
                    view._Typeface = SKTypeface.FromStream(resStream);
                }
            }
            else
            {
                view._Typeface = null;
            }

            view.RefreshPaint();

            ResizePropertyChanged(bindable, oldvalue, newvalue);
        }
        
        private static void InvalidatePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SKLabel view = bindable as SKLabel;

            view.InvalidateSurface();
        }
                
        private static void ResizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            SKLabel view = bindable as SKLabel;

            view.InvalidateMeasure();
            view.InvalidateSurface();
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            if (HasSomethingToDraw())
            {
                SKRect textBounds = new SKRect();
                Rectangle requestRect;

                RefreshPaint();

                _Paint.MeasureText(Text, ref textBounds);
                requestRect = textBounds.ToFormsRect();

                requestRect.Height += _Paint.FontMetrics.Descent;
                requestRect.Width = Math.Min(requestRect.Width, widthConstraint);
                requestRect.Height = Math.Min(requestRect.Height, heightConstraint);

                return new SizeRequest(requestRect.Size);
            }

            return new SizeRequest();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            if (HasSomethingToDraw())
            {
                RefreshPaint();

                args.Surface.Canvas.Clear(SKColor.Empty);
                args.Surface.Canvas.Scale(CanvasSize.Width / (float)Width, CanvasSize.Height / (float)Height);
                args.Surface.Canvas.DrawText(Text, 0, (float)Height - (_Paint.FontMetrics.Descent * 0.5f), _Paint);
            }

            base.OnPaintSurface(args);
        }

        private bool HasSomethingToDraw()
        {
            return _Typeface != null && !string.IsNullOrEmpty(Text) && TextColor != Color.Transparent && FontSize > 0;
        }

        private void RefreshPaint()
        {
            _Paint.Typeface = _Typeface;
            _Paint.Color = TextColor.ToSKColor();
            _Paint.TextSize = (float)FontSize;
            _Paint.IsAntialias = true;
        }
    }
}
