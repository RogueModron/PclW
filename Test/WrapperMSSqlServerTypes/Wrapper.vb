Imports Microsoft.SqlServer.Types
Imports System.Data.SqlTypes
Imports PclWCommon
Imports PclWTestCommon


Public Class MSSqlServerTypesWrapper : Inherits PclWTestCommon.TestableObj
    Public Sub New()
        Me.ClosedPolygonsRequired = True
    End Sub

    Public Overrides Function GetAdaptedInputFromPolygonSet(ByVal input As PclWCommon.PolygonSet) As Object
        If Me.ClosedPolygonsRequired Then
            ClosePolygons(input)
        Else
            OpenPolygons(input)
        End If
        Dim adapter As New InputAdapterFromPolygonSetToRegionWithAnticlockwiseOuter
        Dim regionInput As Region = adapter.GetAdaptedInput(input)
        Return FromRegionToSqlGeometry(regionInput)
    End Function

    Public Overrides Function GetAdaptedInputFromRegion(ByVal input As PclWCommon.Region) As Object
        If Me.ClosedPolygonsRequired Then
            ClosePolygons(input)
        Else
            OpenPolygons(input)
        End If
        Dim adapter As New InputAdapterFromRegionToRegionWithAnticlockwiseOuter
        Dim regionInput As Region = adapter.GetAdaptedInput(input)
        Return FromRegionToSqlGeometry(regionInput)
    End Function

    Public Overrides Function GetAdaptedOutputToPolygonSet(ByVal output As Object) As PclWCommon.PolygonSet
        Dim regionOutput As Region = FromSqlGeometryToRegion(output)
        Dim adapter As New OutputAdapterFromRegionToPolygonSet
        Return adapter.GetAdaptedOutput(regionOutput)
    End Function

    Public Overrides Function GetDifference(ByVal subject As Object, ByVal clip As Object) As Object
        Return subject.STDifference(clip)
    End Function

    Public Overrides Function GetIntersection(ByVal subject As Object, ByVal clip As Object) As Object
        Return subject.STIntersection(clip)
    End Function

    Public Overrides Function GetUnion(ByVal subject As Object, ByVal clip As Object) As Object
        Return subject.STUnion(clip)
    End Function

    Public Overrides Function GetXor(ByVal subject As Object, ByVal clip As Object) As Object
        Return subject.STSymDifference(clip)
    End Function


    Private Function FromRegionToSqlGeometry(
            ByVal region As Region) As SqlGeometry
        Dim builder As New SqlGeometryBuilder
        builder.SetSrid(0)
        builder.BeginGeometry(OpenGisGeometryType.MultiPolygon)

        For Each polygonSet As PolygonSet In region.PolygonSets
            builder.BeginGeometry(OpenGisGeometryType.Polygon)

            Select Case polygonSet.Polygons.Count
                Case 1
                    AddPolygonToBuilder(builder, polygonSet.Polygons(0))
                Case 0
                    Return New SqlGeometry
                Case Else
                    Throw New ArgumentException
            End Select

            For Each polygon As Polygon In polygonSet.Holes
                AddPolygonToBuilder(builder, polygon)
            Next

            builder.EndGeometry()
        Next

        builder.EndGeometry()

        Return builder.ConstructedGeometry
    End Function

    Private Sub AddPolygonToBuilder(ByVal builder As SqlGeometryBuilder,
            ByVal polygon As Polygon)
        If polygon.Vertices.Count = 0 Then
            Return
        End If
        Dim vertex As Vertex = polygon.Vertices(0)
        builder.BeginFigure(vertex.X, vertex.Y)
        For i As Integer = 1 To polygon.Vertices.Count - 1
            vertex = polygon.Vertices(i)
            builder.AddLine(vertex.X, vertex.Y)
        Next
        builder.EndFigure()
    End Sub

    Private Function FromSqlGeometryToRegion(
            ByVal sqlGeometry As SqlGeometry) As Region
        Dim region As New Region

        For i As SqlInt32 = 1 To sqlGeometry.STNumGeometries
            Dim polygonSet As New PolygonSet
            Dim geometry As SqlGeometry = sqlGeometry.STGeometryN(i)
            If geometry.STGeometryType <> "POLYGON" Then
                Continue For
            End If
            Dim polygon As Polygon = FromSqlRingToPolygon(
                geometry.STExteriorRing)
            polygonSet.Polygons.Add(polygon)
            For j As SqlInt32 = 1 To geometry.STNumInteriorRing
                polygon = FromSqlRingToPolygon(
                    geometry.STInteriorRingN(j))
                polygonSet.Holes.Add(polygon)
            Next
            region.PolygonSets.Add(polygonSet)
        Next

        Return region
    End Function

    Private Function FromSqlRingToPolygon(
            ByVal sqlRing As SqlGeometry) As Polygon
        Dim polygon As New Polygon
        For i As SqlInt32 = 1 To sqlRing.STNumPoints
            Dim point As SqlGeometry = sqlRing.STPointN(i)
            polygon.Vertices.Add(New Vertex(point.STX, point.STY))
        Next
        Return polygon
    End Function
End Class
