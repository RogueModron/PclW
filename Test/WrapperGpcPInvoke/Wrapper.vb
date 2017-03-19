Imports PclWCommon
Imports PclWTestCommon


Public Class GpcPInvokeWrapper : Inherits TestableObj
    Public Sub New()
        Me.ClosedPolygonsRequired = False
    End Sub

    Public Overrides Function GetAdaptedInputFromPolygonSet(ByVal input As PclWCommon.PolygonSet) As Object
        If Me.ClosedPolygonsRequired Then
            ClosePolygons(input)
        Else
            OpenPolygons(input)
        End If

        Dim gpcWPolygon As New GpcWrapper.Polygon
        For Each polygon As PclWCommon.Polygon In input.Polygons
            Dim verticesCount As Integer = polygon.Vertices.Count
            Dim va(verticesCount - 1) As GpcWrapper.Vertex
            For i As Integer = 0 To verticesCount - 1
                va(i) = New GpcWrapper.Vertex(
                    polygon.Vertices(i).X, polygon.Vertices(i).Y)
            Next
            Dim vl As New GpcWrapper.VertexList
            vl.NofVertices = verticesCount
            vl.Vertex = va
            gpcWPolygon.AddContour(vl, Not polygon.IsClockwise)
        Next
        Return gpcWPolygon
    End Function

    Public Overrides Function GetAdaptedInputFromRegion(ByVal input As PclWCommon.Region) As Object
        'Not implemented.
        Return Nothing
    End Function

    Public Overrides Function GetAdaptedOutputToPolygonSet(ByVal output As Object) As PclWCommon.PolygonSet
        'Output Polygon is not preserved.
        Dim polygonSet As New PclWCommon.PolygonSet
        For i As Integer = 0 To output.NofContours - 1
            Dim polygon As New PclWCommon.Polygon
            For j = 0 To output.Contour(i).NofVertices - 1
                Dim gpcWVertex As GpcWrapper.Vertex =
                    output.Contour(i).Vertex(j)
                polygon.Vertices.Add(
                    New PclWCommon.Vertex(gpcWVertex.X, gpcWVertex.Y))
            Next
            If Not (output.ContourIsHole(i) Xor polygon.IsClockwise) Then
                polygon.Reverse()
            End If
            polygonSet.Polygons.Add(polygon)
        Next
        Return polygonSet
    End Function

    Public Overrides Function GetDifference(ByVal subject As Object, ByVal clip As Object) As Object
        Return subject.Clip(GpcWrapper.GpcOperation.Difference, clip)
    End Function

    Public Overrides Function GetIntersection(ByVal subject As Object, ByVal clip As Object) As Object
        Return subject.Clip(GpcWrapper.GpcOperation.Intersection, clip)
    End Function

    Public Overrides Function GetUnion(ByVal subject As Object, ByVal clip As Object) As Object
        Return subject.Clip(GpcWrapper.GpcOperation.Union, clip)
    End Function

    Public Overrides Function GetXor(ByVal subject As Object, ByVal clip As Object) As Object
        Return subject.Clip(GpcWrapper.GpcOperation.XOr, clip)
    End Function
End Class
