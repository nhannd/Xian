#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.RoiGraphics;
using ClearCanvas.ImageViewer.RoiGraphics.Analyzers;

namespace ClearCanvas.ImageViewer.Oto
{
    //PointF doesn't work with Oto, so we need our own.
    [DataContract(Namespace = OtoNamespace.Value)]
    public class Point
    {
        private PointF _real;

        [DataMember]
        public float X
        {
            get { return _real.X; }
            set { _real.X = value; }
        }
        [DataMember]
        public float Y
        {
            get { return _real.Y; }
            set { _real.Y = value; }
        }

        public static implicit operator Point(PointF pt)
        {
            return new Point { X = pt.X, Y = pt.Y };
        }

        public static implicit operator PointF(Point pt)
        {
            return pt._real;
        }

        public override bool Equals(object obj)
        {
            var pt = (PointF)obj;
            return pt.Equals(_real);
        }

        public override int GetHashCode()
        {
            return _real.GetHashCode();
        }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class AnnotationIdentifier
    {
        [DataMember]
        public RectangularLayout.TileIdentifier TileIdentifier { get; set; }
        [DataMember]
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is AnnotationIdentifier)
            {
                var other = (AnnotationIdentifier)obj;
                return Equals(other.TileIdentifier, TileIdentifier) && (other.Name ?? String.Empty) == (Name ?? String.Empty);
            }
            return false;
        }

        public override int GetHashCode()
        {
            var tileHash = TileIdentifier == null ? 0x0 : 0x675FABC1 ^ TileIdentifier.GetHashCode();
            return tileHash ^ (Name ?? String.Empty).GetHashCode();
        }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public abstract class Annotation
    {
        public virtual void Initialize(IControlGraphic graphic)
        {
            IsTileCoordinates = graphic.CoordinateSystem == CoordinateSystem.Destination;
            var annotationGraphic = graphic as AnnotationGraphic;
            if (annotationGraphic == null)
                return;

            Callout = new CalloutAnnotation();
            Callout.Initialize(annotationGraphic.Callout);
        }

        [DataMember]
        public AnnotationIdentifier Identifier { get; set; }

        [DataMember]
        public bool IsTileCoordinates { get; set; }

        [DataMember]
        public virtual CalloutAnnotation Callout { get; set; }

        public static Annotation CreateFrom(IControlGraphic graphic)
        {
            if (EllipseAnnotation.CanCreateFrom(graphic))
            {
                var annotation = new EllipseAnnotation();
                annotation.Initialize(graphic);
                return annotation;
            }
            if (RectangleAnnotation.CanCreateFrom(graphic))
            {
                var annotation = new RectangleAnnotation();
                annotation.Initialize(graphic);
                return annotation;
            }
            if (RulerAnnotation.CanCreateFrom(graphic))
            {
                var annotation = new RulerAnnotation();
                annotation.Initialize(graphic);
                return annotation;
            }
            if (PolygonAnnotation.CanCreateFrom(graphic))
            {
                var annotation = new PolygonAnnotation();
                annotation.Initialize(graphic);
                return annotation;
            }
            if (ProtractorAnnotation.CanCreateFrom(graphic))
            {
                var annotation = new ProtractorAnnotation();
                annotation.Initialize(graphic);
                return annotation;
            }
            if (CalloutAnnotation.CanCreateFrom(graphic))
            {
                var annotation = new CalloutAnnotation();
                annotation.Initialize(graphic);
                return annotation;
            }
            if (TextAreaAnnotation.CanCreateFrom(graphic))
            {
                var annotation = new TextAreaAnnotation();
                annotation.Initialize(graphic);
                return annotation;
            }

            return null;
        }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public abstract class BoundableAnnotation : Annotation
    {
        public override void Initialize(IControlGraphic graphic)
        {
            var annotationGraphic = (AnnotationGraphic)graphic;
            base.Initialize(annotationGraphic);

            var subject = annotationGraphic.Subject;
            TopLeft = subject.BoundingBox.Location;
            BottomRight = subject.BoundingBox.Location + subject.BoundingBox.Size;
            CentrePoint = subject.BoundingBox.Location + new SizeF(subject.BoundingBox.Width / 2F, subject.BoundingBox.Height / 2F);

            // Note: annotationGraphic.Callout actually creates another instance of the callout 
            // It's ok because we are only interested in the statistics of the roi, not the callout.
            var callout = annotationGraphic.Callout as RoiCalloutGraphic;
            if (callout!=null)
                RoiStatistics = GetStatistics(callout);
        }

        [DataMember]
        public Point TopLeft { get; set; }

        [DataMember]
        public Point BottomRight { get; set; }

        [DataMember]
        public Point CentrePoint { get; set; }

        [DataMember]
        public RoiStatistics RoiStatistics { get; set; }

        protected virtual RoiStatistics GetStatistics(RoiCalloutGraphic calloutGraphic)
        {
            var staticstics = new RoiStatistics();

            var roi = calloutGraphic.ParentGraphic.Roi;
            foreach (var analyzer in calloutGraphic.RoiAnalyzers)
            {
                if (analyzer.SupportsRoi(roi))
                {
                    var analyzerResult = analyzer.Analyze(roi, RoiAnalysisMode.Normal);
                    if (analyzerResult != null)
                    {
                        if (staticstics.Items ==null)
                            staticstics.Items = new List<RoiStatistic>();

                        if (analyzerResult is SingleValueRoiAnalyzerResult)
                        {
                            var result = analyzerResult as SingleValueRoiAnalyzerResult;
                            staticstics.Items.Add(new RoiStatistic()
                            {
                                Name = analyzerResult.Name,
                                Value = result.Value,
                                Units = result.Units,
                                DisplayedValue = analyzerResult.SerializedAsString()
                            });
                        }
                        else if (analyzerResult is RoiAnalyzerResultNoValue)
                        {
                            var result = analyzerResult as RoiAnalyzerResultNoValue;
                            staticstics.Items.Add(new RoiStatistic()
                            {
                                Name = result.Name,
                                DisplayedValue = result.SerializedAsString()
                            });
                        }
                        if (analyzerResult is MultiValueRoiAnalyzerResult)
                        {
                            IEnumerable<IRoiAnalyzerResult> results = (analyzerResult as MultiValueRoiAnalyzerResult).Value as IEnumerable<IRoiAnalyzerResult>;
                            if (results!=null)
                            {
                                foreach (var result in results)
                                {
                                    var units = (result is SingleValueRoiAnalyzerResult)? (result as SingleValueRoiAnalyzerResult).Units: null;
                                    var value = result.Value;
                                    var concatName = String.Format("{0}.{1}", analyzerResult.Name, result.Name);

                                    staticstics.Items.Add(new RoiStatistic()
                                    {
                                        Name = concatName,
                                        Value = value,
                                        Units = units,
                                        DisplayedValue = result.SerializedAsString()
                                    });
                                } 
                            }
                            
                        }else
                        {
                            
                        }

                    }
                }
            }

            return staticstics;
        }
    }


    [DataContract(Namespace = OtoNamespace.Value)]
    public class RoiStatistic
    {
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public object Value { get; set; }
        
        [DataMember]
        public string DisplayedValue { get; set; }

        [DataMember]
        public string Units { get; set; }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class RoiStatistics
    {
        [DataMember]
        public List<RoiStatistic> Items { get; set; }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class EllipseAnnotation : BoundableAnnotation
    {
        public override void Initialize(IControlGraphic graphic)
        {
            var annotationGraphic = (AnnotationGraphic)graphic;
            base.Initialize(graphic);
            var ellipseGraphic = (EllipsePrimitive)annotationGraphic.Subject;
            SemiMinorAxis = ellipseGraphic.BoundingBox.Width / 2F;
            SemiMajorAxis = ellipseGraphic.BoundingBox.Height / 2F;
        }


        [DataMember]
        public double SemiMajorAxis { get; set; }

        [DataMember]
        public double SemiMinorAxis { get; set; }

        public static bool CanCreateFrom(IControlGraphic graphic)
        {
            return graphic.Subject is EllipsePrimitive;
        }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class ProtractorAnnotation : Annotation
    {
        public override void Initialize(IControlGraphic graphic)
        {
            var annotationGraphic = (AnnotationGraphic)graphic;
            base.Initialize(graphic);

            var protractorGraphic = (IPointsGraphic)annotationGraphic.Subject;
            Point1 = protractorGraphic.Points[0];
            Vertex = protractorGraphic.Points[1];
            Point2 = protractorGraphic.Points[2];
            SubtendedAngle = Vector.SubtendedAngle(Point1, Vertex, Point2);
        }

        [DataMember]
        public Point Point1 { get; set; }
        [DataMember]
        public Point Vertex { get; set; }
        [DataMember]
        public Point Point2 { get; set; }

        [DataMember]
        public double SubtendedAngle { get; set; }

        public static bool CanCreateFrom(IControlGraphic graphic)
        {
            /// TODO (CR Dec 2011): The difference between a protractor
            /// and a polygon is very slim here ... maybe need to
            /// move ProtractorGraphic into the core viewer
            /// or give it an interface of its own.
            return graphic.Subject is IPointsGraphic && ((IPointsGraphic)graphic.Subject).Points.Count == 3;
        }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class RulerAnnotation : Annotation
    {
        public override void Initialize(IControlGraphic graphic)
        {
            var annotationGraphic = (AnnotationGraphic)graphic;
            base.Initialize(graphic);
            var rulerGraphic = (LinePrimitive)annotationGraphic.Subject;
            Point1 = rulerGraphic.Point1;
            Point2 = rulerGraphic.Point2;
        }

        [DataMember]
        public Point Point1 { get; set; }
        [DataMember]
        public Point Point2 { get; set; }

        public static bool CanCreateFrom(IControlGraphic graphic)
        {
            return graphic.Subject is ILineSegmentGraphic;
        }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class TextAreaAnnotation : Annotation
    {
        public override void Initialize(IControlGraphic graphic)
        {
            var textGraphic = (ITextGraphic)graphic.Subject;
            base.Initialize(graphic);
            Text = textGraphic.Text;
            Location = textGraphic.Location;
            TopLeft = textGraphic.BoundingBox.Location;
            BottomRight = textGraphic.BoundingBox.Location +
                               new SizeF(textGraphic.BoundingBox.Width, textGraphic.BoundingBox.Height);
        }

        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public Point Location { get; set; }
        [DataMember]
        public Point TopLeft { get; set; }
        [DataMember]
        public Point BottomRight { get; set; }
        [DataMember]
        public override CalloutAnnotation Callout
        {
            get { return base.Callout; }
            set { throw new NotSupportedException(); }
        }

        public static bool CanCreateFrom(IControlGraphic graphic)
        {
            return graphic.Subject is ITextGraphic;
        }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class CalloutAnnotation : Annotation
    {
        public override void Initialize(IControlGraphic graphic)
        {
            base.Initialize(graphic);
            Initialize((ICalloutGraphic)graphic.Subject);
        }

        public void Initialize(ICalloutGraphic graphic)
        {
            TextArea = new TextAreaAnnotation
            {
                Text = graphic.Text,
                Location = graphic.TextLocation,
                TopLeft = graphic.TextBoundingBox.Location,
                BottomRight = graphic.TextBoundingBox.Location +
                    new SizeF(graphic.TextBoundingBox.Width, graphic.TextBoundingBox.Height)
            };

            AnchorPoint = graphic.AnchorPoint;
        }

        [DataMember]
        public TextAreaAnnotation TextArea { get; set; }

        [DataMember]
        public Point AnchorPoint { get; set; }

        public static bool CanCreateFrom(IControlGraphic graphic)
        {
            return graphic.Subject is ICalloutGraphic;
        }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class RectangleAnnotation : BoundableAnnotation
    {
        public static bool CanCreateFrom(IControlGraphic graphic)
        {
            return graphic.Subject is RectanglePrimitive;
        }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class PolygonAnnotation : BoundableAnnotation
    {
        public override void Initialize(IControlGraphic graphic)
        {
            var annotationGraphic = (AnnotationGraphic)graphic;
            base.Initialize(graphic);
            var polygonGraphic = (IPointsGraphic)annotationGraphic.Subject;
            Vertices = polygonGraphic.Points.Select(p => (Point)p).ToArray();
        }

        [DataMember]
        public Point[] Vertices { get; set; }

        public static bool CanCreateFrom(IControlGraphic graphic)
        {
            return graphic.Subject is IPointsGraphic && ((IPointsGraphic)graphic.Subject).Points.Count > 3;
        }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class DrawAnnotationRequest<T> where T : Annotation
    {
        [DataMember]
        public T Annotation { get; set; }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class DrawAnnotationResponse<T> where T : Annotation
    {
        [DataMember]
        public T Annotation { get; set; }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class GetAnnotationsRequest
    {
        [DataMember]
        public RectangularLayout.TileIdentifier TileIdentifier { get; set; }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class GetAnnotationsResult
    {
        [DataMember]
        public List<Annotation> Annotations { get; set; }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class GetAnnotationRequest
    {
        [DataMember]
        public AnnotationIdentifier AnnotationIdentifier { get; set; }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class GetAnnotationResult
    {
        [DataMember]
        public Annotation Annotation { get; set; }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class GetSelectedAnnotationRequest
    {
        [DataMember]
        public RectangularLayout.TileIdentifier TileIdentifier { get; set; }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class GetSelectedAnnotationResult
    {
        [DataMember]
        public Annotation Annotation { get; set; }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class SaveAnnotationsRequest
    {
        [DataMember]
        public RectangularLayout.TileIdentifier TileIdentifier { get; set; }

        [DataMember]
        public string FileName;
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class SaveAnnotationsResult
    {
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class SelectAnnotationRequest
    {
        [DataMember]
        public AnnotationIdentifier AnnotationIdentifier { get; set; }
    }

    [DataContract(Namespace = OtoNamespace.Value)]
    public class SelectAnnotationResult
    {
        [DataMember]
        public Annotation Annotation { get; set; }
    }
}