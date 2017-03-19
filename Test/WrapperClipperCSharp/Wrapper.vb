Imports PclWCommon
Imports PclWTestCommon


Public Class ClipperCSharpWrapper : Inherits TestableObj
    Private Const Scale As Integer = 1000

    Public Sub New()
        Me.ClosedPolygonsRequired = False
    End Sub

    Public Overrides Function GetAdaptedInputFromPolygonSet(ByVal input As PclWCommon.PolygonSet) As Object
        If Me.ClosedPolygonsRequired Then
            ClosePolygons(input)
        Else
            OpenPolygons(input)
        End If

        Dim clprPolygons As New List(Of List(Of ClipperLib.IntPoint))
        For Each polygon As PclWCommon.Polygon In input.Polygons
            Dim clprPolygon As New List(Of ClipperLib.IntPoint)
            For Each vertex As PclWCommon.Vertex In polygon.Vertices
                Dim clprPoint As New ClipperLib.IntPoint
                clprPoint.X = vertex.X * Scale
                clprPoint.Y = vertex.Y * Scale
                clprPolygon.Add(clprPoint)
            Next
            clprPolygons.Add(clprPolygon)
        Next
        Return clprPolygons
    End Function

    Public Overrides Function GetAdaptedInputFromRegion(ByVal input As PclWCommon.Region) As Object
        'Not implemented.
        Return Nothing
    End Function

    Public Overrides Function GetAdaptedOutputToPolygonSet(ByVal output As Object) As PclWCommon.PolygonSet
        Dim polygonSet As New PclWCommon.PolygonSet
        For Each clprPolygon As List(Of ClipperLib.IntPoint) In output
            Dim polygon As New PclWCommon.Polygon
            For Each clprPoint As ClipperLib.IntPoint In clprPolygon
                Dim vertex As New PclWCommon.Vertex
                vertex.X = clprPoint.X / Scale
                vertex.Y = clprPoint.Y / Scale
                polygon.Vertices.Add(vertex)
            Next
            polygonSet.Polygons.Add(polygon)
        Next
        Return polygonSet
    End Function

    Public Overrides Function GetDifference(ByVal subject As Object, ByVal clip As Object) As Object
        Dim clipperCs As New ClipperLib.Clipper
        'clipperCs.UseFullCoordinateRange = False
        clipperCs.AddPolygons(subject, ClipperLib.PolyType.ptSubject)
        clipperCs.AddPolygons(clip, ClipperLib.PolyType.ptClip)
        Dim result As New List(Of List(Of ClipperLib.IntPoint))
        clipperCs.Execute(ClipperLib.ClipType.ctDifference, result)
        Return result
    End Function

    Public Overrides Function GetIntersection(ByVal subject As Object, ByVal clip As Object) As Object
        Dim clipperCs As New ClipperLib.Clipper
        'clipperCs.UseFullCoordinateRange = False
        clipperCs.AddPolygons(subject, ClipperLib.PolyType.ptSubject)
        clipperCs.AddPolygons(clip, ClipperLib.PolyType.ptClip)
        Dim result As New List(Of List(Of ClipperLib.IntPoint))
        clipperCs.Execute(ClipperLib.ClipType.ctIntersection, result)
        Return result
    End Function

    Public Overrides Function GetUnion(ByVal subject As Object, ByVal clip As Object) As Object
        Dim clipperCs As New ClipperLib.Clipper
        'clipperCs.UseFullCoordinateRange = False
        clipperCs.AddPolygons(subject, ClipperLib.PolyType.ptSubject)
        clipperCs.AddPolygons(clip, ClipperLib.PolyType.ptClip)
        Dim result As New List(Of List(Of ClipperLib.IntPoint))
        clipperCs.Execute(ClipperLib.ClipType.ctUnion, result)
        Return result
    End Function

    Public Overrides Function GetXor(ByVal subject As Object, ByVal clip As Object) As Object
        Dim clipperCs As New ClipperLib.Clipper
        'clipperCs.UseFullCoordinateRange = False
        clipperCs.AddPolygons(subject, ClipperLib.PolyType.ptSubject)
        clipperCs.AddPolygons(clip, ClipperLib.PolyType.ptClip)
        Dim result As New List(Of List(Of ClipperLib.IntPoint))
        clipperCs.Execute(ClipperLib.ClipType.ctXor, result)
        Return result
    End Function
End Class
