﻿using System;
using UIKit;
using SciChart.iOS.Charting;
using Foundation;

namespace AddingAnnotations
{
    public partial class ViewController : UIViewController
    {
        private SCIChartSurface _surface;
        private XyDataSeries<Double, Double> _lineDataSeries;
        private XyDataSeries<Double, Double> _scatterDataSeries;


        private SCIFastLineRenderableSeries _lineRenderableSeries;
        private SCIXyScatterRenderableSeries _scatterRenderableSeries;

        // timer, used for updating data
        private NSTimer _timer;
        // phase variable used for data slipping
        private double _phase = 0.0;
        // instance variable for counting the data points in data series
        private int _i = 0;

        // Used to store annotations
        private SCIAnnotationCollection _annotationCollection = new SCIAnnotationCollection();

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            _surface = new SCIChartSurface();
            _surface.TranslatesAutoresizingMaskIntoConstraints = true;
            _surface.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            _surface.Frame = this.View.Bounds;

            this.View.AddSubview(_surface);

            _surface.XAxes.Add(new SCINumericAxis() { GrowBy = new SCIDoubleRange(0.1, 0.1) });
            _surface.YAxes.Add(new SCINumericAxis() { GrowBy = new SCIDoubleRange(0.1, 0.1) });

            CreateDataSeries();
            CreateRenderableSeries();
            AddModifiers();

            _surface.Annotations = _annotationCollection;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (_timer == null)
            {
                _timer = NSTimer.CreateRepeatingScheduledTimer(0.01, (timer) =>
                {
                    _i++;

                    _lineDataSeries.Append(_i, Math.Sin(_i * 0.1 + _phase));
                    _scatterDataSeries.Append(_i, Math.Cos(_i * 0.1 + _phase));

                    _phase += 0.01;

                    //if (_i % 100 == 0)
                    //{
                    //    var customAnnotation = new SCICustomAnnotation();
                    //    var customAnnotationContentView = new UILabel(frame: CGRect.init(x: 0, y: 0, width: 10, height: 10));
                    //    customAnnotationContentView.text = "Y";
                    //    customAnnotationContentView.backgroundColor = UIColor.lightGray;


                    //    customAnnotation.contentView = customAnnotationContentView;
                    //    customAnnotation.x1 = SCIGeneric(i);
                    //    customAnnotation.y1 = SCIGeneric(0.5);
                    //    customAnnotation.coordinateMode = .relativeY;

                    //    // adding new custom annotation into the annotationGroup property
                    //    _annotationCollection.addItem(customAnnotation);

                    //    // removing annotations that are out of visible range
                    //    let customAn = _annotationCollection.item(at: 0) as!SCICustomAnnotation;

                    //    if (customAn.x1 < Double(i) - totalCapacity)
                    //    {
                    //        // since the contentView is UIView element - we have to call removeFromSuperView method to remove it from screen
                    //        customAn.contentView.removeFromSuperview();
                    //        annotationGroup.removeItem(customAn)
                    //    }
                    //}

                    _surface.ZoomExtents();
                });
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            _timer.Invalidate();
            _timer = null;
        }

        void CreateDataSeries()
        {
            // Init line data series
            _lineDataSeries = new XyDataSeries<Double, Double>();
            // Setting fifo capacity, once new data values will be added, the old one will be removed
            _lineDataSeries.FifoCapacity = 500;
            // Naming data series, so its name will be shown in LegendModifier
            _lineDataSeries.SeriesName = "LineSeries";
            for (var i = 0; i < 500; i++)
            {
                _lineDataSeries.Append(i, Math.Sin(i * 0.1));
            }

            // Init scatter data series
            _scatterDataSeries = new XyDataSeries<Double, Double>();
            // Setting fifo capacity, once new data values will be added, the old one will be removed
            _scatterDataSeries.FifoCapacity = 500;
            // Naming data series, so its name will be shown in LegendModifier
            _scatterDataSeries.SeriesName = "ScatterSeries";
            for (var i = 0; i < 500; i++)
            {
                _scatterDataSeries.Append(i, Math.Cos(i * 0.1));
            }

            _i = _lineDataSeries.Count;
        }

        void CreateRenderableSeries()
        {
            _lineRenderableSeries = new SCIFastLineRenderableSeries();
            _lineRenderableSeries.DataSeries = _lineDataSeries;

            _scatterRenderableSeries = new SCIXyScatterRenderableSeries();
            _scatterRenderableSeries.DataSeries = _scatterDataSeries;

            _surface.RenderableSeries.Add(_lineRenderableSeries);
            _surface.RenderableSeries.Add(_scatterRenderableSeries);
        }

        void AddModifiers()
        {
            var xAxisDragmodifier = new SCIXAxisDragModifier();
            xAxisDragmodifier.DragMode = SCIAxisDragMode.Pan;
            xAxisDragmodifier.ClipModeX = SCIClipMode.None;

            var yAxisDragmodifier = new SCIYAxisDragModifier();
            yAxisDragmodifier.DragMode = SCIAxisDragMode.Pan;

            var extendZoomModifier = new SCIZoomExtentsModifier();
            var pinchZoomModifier = new SCIPinchZoomModifier();

            // Adding Rollover and Legend modifiers
            var rolloverModifier = new SCIRolloverModifier();
            var legendCollectionModifier = new SCILegendModifier();

            var groupModifier = new SCIChartModifierCollection();
            groupModifier.Add(xAxisDragmodifier);
            groupModifier.Add(yAxisDragmodifier);
            groupModifier.Add(pinchZoomModifier);
            groupModifier.Add(extendZoomModifier);
            groupModifier.Add(rolloverModifier);
            groupModifier.Add(legendCollectionModifier);

            _surface.ChartModifiers = groupModifier;
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.

        }
    }
}