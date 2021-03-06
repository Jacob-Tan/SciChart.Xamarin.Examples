﻿using Xamarin.Examples.Demo.Data;
using SciChart.iOS.Charting;

namespace Xamarin.Examples.Demo.iOS
{
    [ExampleDefinition("Line Chart", description: "Creates a simple line chart", icon: ExampleIcon.LineChart)]
    public class LineChartViewController : SingleChartViewController<SCIChartSurface>
    {
        protected override void InitExample()
        {
            var xAxis = new SCINumericAxis { GrowBy = new SCIDoubleRange(0.1, 0.1), VisibleRange = new SCIDoubleRange(1.1, 2.7) };
            var yAxis = new SCINumericAxis { GrowBy = new SCIDoubleRange(0.1, 0.1) };

            var fourierSeries = DataManager.Instance.GetFourierSeries(1.0, 0.1);
            var dataSeries = new XyDataSeries<double, double>();
            dataSeries.Append(fourierSeries.XData, fourierSeries.YData);

            var rSeries = new SCIFastLineRenderableSeries { DataSeries = dataSeries, StrokeStyle = new SCISolidPenStyle(0xFF279B27, 2f) };

            using (Surface.SuspendUpdates())
            {
                Surface.XAxes.Add(xAxis);
                Surface.YAxes.Add(yAxis);
                Surface.RenderableSeries.Add(rSeries);
                Surface.ChartModifiers.Add(CreateDefaultModifiers());

                SCIAnimations.SweepSeries(rSeries, 3, new SCICubicEase());
            }
        }
    }
}