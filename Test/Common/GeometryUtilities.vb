Imports System.Math
Imports PclWCommon

Public Module GeometryUtilities
    Public Sub ClosePolygon(ByVal polygon As Polygon)
        Const Delta As Double = 0.01
        If polygon.Vertices.Count > 0 Then
            Dim firstVertex As Vertex = polygon.Vertices(0)
            Dim lastVertex As Vertex =
                polygon.Vertices(polygon.Vertices.Count - 1)
            If Abs(firstVertex.X - lastVertex.X) < Delta _
                    AndAlso Abs(firstVertex.Y - lastVertex.Y) < Delta Then
                polygon.Vertices(polygon.Vertices.Count - 1) = firstVertex
            Else
                polygon.Vertices.Add(firstVertex)
            End If
        End If
    End Sub

    Public Sub ClosePolygons(ByVal polygonSet As PolygonSet)
        For Each polygon As Polygon In polygonSet.Polygons
            ClosePolygon(polygon)
        Next
        For Each polygon As Polygon In polygonSet.Holes
            ClosePolygon(polygon)
        Next
    End Sub

    Public Sub ClosePolygons(ByVal region As Region)
        For Each polygonSet As PolygonSet In region.PolygonSets
            ClosePolygons(polygonSet)
        Next
    End Sub

    Public Function CopyPolygon(ByVal polygon As Polygon) As Polygon
        Dim newPolygon As New Polygon
        For Each vertex As Vertex In polygon.Vertices
            newPolygon.Vertices.Add(vertex)
        Next
        Return newPolygon
    End Function

    Public Function CopyPolygonSet(ByVal polygonSet As PolygonSet
            ) As PolygonSet
        Dim newPolygonSet As New PolygonSet
        For Each polygon As Polygon In polygonSet.Polygons
            newPolygonSet.Polygons.Add(CopyPolygon(polygon))
        Next
        For Each polygon As Polygon In polygonSet.Holes
            newPolygonSet.Holes.Add(CopyPolygon(polygon))
        Next
        Return newPolygonSet
    End Function

    Public Function CopyRegion(ByVal region As Region)
        Dim newRegion As New Region
        For Each polygonSet As PolygonSet In region.PolygonSets
            newRegion.PolygonSets.Add(CopyPolygonSet(polygonSet))
        Next
        Return newRegion
    End Function

    Public Function GetPolygonsCount(ByVal polygonSet As PolygonSet) As Integer
        Return polygonSet.Polygons.Count + polygonSet.Holes.Count
    End Function

    Public Function GetVerticesCount(ByVal polygonSet As PolygonSet) As Integer
        Dim count As Integer = 0
        For Each polygon As Polygon In polygonSet.Polygons
            count = count + polygon.Vertices.Count
        Next
        For Each polygon As Polygon In polygonSet.Holes
            count = count + polygon.Vertices.Count
        Next
        Return count
    End Function

   Public Sub OpenPolygon(ByVal polygon As Polygon)
        Const Delta As Double = 0.01
        If polygon.Vertices.Count > 0 Then
            Dim firstVertex As Vertex = polygon.Vertices(0)
            Dim lastVertex As Vertex =
                polygon.Vertices(polygon.Vertices.Count - 1)
            If Abs(firstVertex.X - lastVertex.X) < Delta _
                    AndAlso Abs(firstVertex.Y - lastVertex.Y) < Delta Then
                polygon.Vertices.RemoveAt(polygon.Vertices.Count - 1)
            End If
        End If
    End Sub

    Public Sub OpenPolygons(ByVal polygonSet As PolygonSet)
        For Each polygon As Polygon In polygonSet.Polygons
            OpenPolygon(polygon)
        Next
        For Each polygon As Polygon In polygonSet.Holes
            OpenPolygon(polygon)
        Next
    End Sub

    Public Sub OpenPolygons(ByVal region As Region)
        For Each polygonSet As PolygonSet In region.PolygonSets
            OpenPolygons(polygonSet)
        Next
    End Sub

    Public Sub ReverseLastPolygon(ByVal polygonSet As PolygonSet)
        polygonSet.Polygons(polygonSet.Polygons.Count - 1).Reverse()
    End Sub

    Public Sub ReversePolygonSet(ByVal polygonSet As PolygonSet)
        For Each polygon As Polygon In polygonSet.Polygons
            polygon.Reverse()
        Next
        For Each polygon As Polygon In polygonSet.Holes
            polygon.Reverse()
        Next
    End Sub

    Public Sub ReverseRegion(ByVal region As Region)
        For Each polygonSet As PolygonSet In region.PolygonSets
            ReversePolygonSet(polygonSet)
        Next
    End Sub
End Module
